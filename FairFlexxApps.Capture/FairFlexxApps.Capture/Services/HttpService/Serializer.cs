using System;
using System.Diagnostics;
using System.IO;
using System.Xml.Serialization;
using FairFlexxApps.Capture.Interfaces.HttpService;
using FairFlexxApps.Capture.Utilities;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace FairFlexxApps.Capture.Services.HttpService
{
    public class Serializer : ISerializer
    {
        #region Constructors

        public Serializer() { }

        public string Serialize<T>(T obj)
        {
            return JsonConvert.SerializeObject(obj, new IsoDateTimeConverter
            {
                DateTimeFormat = @"yyyy-MM-dd\THH:mm:ss.FFFFFFF\Z",
                DateTimeStyles = System.Globalization.DateTimeStyles.AdjustToUniversal
            });
        }

        #endregion

        #region Deserialize

        public T Deserialize<T>(string data, string url = null)
        {
#if DEBUG
            if (!string.IsNullOrEmpty(url))
                StringHelper.FormatJsonThenWrite(data, $"Response for: {url}");
#endif
            try
            {
                return JsonConvert.DeserializeObject<T>(data, new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore
                });
            }
            catch (Exception exception)
            {
#if DEBUG
                Debug.WriteLine($"Deserialize Cannot deserialize url res: {url} with error: {exception.Message}");
#endif
                return default(T);
            }
        }

        public T DeserializeFromJsonStream<T>(Stream jsonStream, string url = null)
        {
            try
            {
                if (jsonStream == null)
#if DEBUG
                    throw new ArgumentNullException(nameof(jsonStream));
#else
                    return default(T);
#endif
                if (jsonStream.CanRead == false)
#if DEBUG
                    throw new ArgumentException($"Json stream must be support read {nameof(jsonStream)}");
#else
                    return default(T);
#endif
                T deserializeObject;

                using (var sr = new StreamReader(jsonStream))
                {
                    using (var jsonTextReader = new JsonTextReader(sr))
                    {
                        var serializer = new JsonSerializer { NullValueHandling = NullValueHandling.Ignore };
                        deserializeObject = serializer.Deserialize<T>(jsonTextReader);
#if DEBUG
                        if (!string.IsNullOrEmpty(url))
                            StringHelper.FormatJsonThenWrite(JsonConvert.SerializeObject(deserializeObject), $"Response for: {url}");
#endif                        
                    }
                }
                return deserializeObject;
            }
            catch (Exception exception)
            {
#if DEBUG
                Debug.WriteLine($"DeserializeFromJsonStream Cannot deserialize url res: {url} with error: {exception.Message}");
#endif
                return default(T);
            }
        }

        #endregion

        #region Parse XML

        /// <summary>
        /// populate a class with xml data 
        /// </summary>
        /// <typeparam name="T">Object Type</typeparam>
        /// <param name="input">xml data</param>
        /// <returns>Object Type</returns>
        public T Deserialize<T>(string input) where T : class
        {
            try
            {
                System.Xml.Serialization.XmlSerializer ser = new System.Xml.Serialization.XmlSerializer(typeof(T));

                using (StringReader sr = new StringReader(input))
                {
                    return (T)ser.Deserialize(sr);
                }
            }
            catch (Exception exception)
            {
#if DEBUG
                Debug.WriteLine($"Deserialize Cannot deserialize url res: {input} with error: {exception.Message}");
#endif
                return default(T);
            }
        }

        /// <summary>
        /// convert object to xml string
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="objectToSerialize"></param>
        /// <returns></returns>
        public string ObjectToSerialize<T>(T objectToSerialize)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(objectToSerialize.GetType());

            using (StringWriter textWriter = new StringWriter())
            {
                xmlSerializer.Serialize(textWriter, objectToSerialize);
                return textWriter.ToString();
            }
        }

        #endregion
    }
}
