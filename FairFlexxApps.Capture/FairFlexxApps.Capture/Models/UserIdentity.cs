using System;
using System.Security.Cryptography;
using System.Text;

namespace FairFlexxApps.Capture.Models
{
    public class UserIdentity
    {
        public int UserIdentityId { get; set; } = 0;
        public string UserName { get; set; }
        public string Password { get; set; }
        public string DateTimeKey { get; set; }

        public string CryptPassword(string password)
        {
            var key = "FairFlexxUser@2000-01-01";

            var inputArray = Encoding.UTF8.GetBytes(password);
            var tripleDES = new TripleDESCryptoServiceProvider
            {
                Key = Encoding.UTF8.GetBytes(key),
                Mode = CipherMode.ECB,
                Padding = PaddingMode.PKCS7
            };
            var cTransform = tripleDES.CreateEncryptor();
            var resultArray = cTransform.TransformFinalBlock(inputArray, 0, inputArray.Length);
            tripleDES.Clear();

            return Convert.ToBase64String(resultArray, 0, resultArray.Length);
        }

        public string DecryptPassword(string encryptedPass, string dateTimeKey)
        {
            string key = "FairFlexxUser@2000-01-01";

            byte[] inputArray = Convert.FromBase64String(encryptedPass);

            TripleDESCryptoServiceProvider tripleDES = new TripleDESCryptoServiceProvider();
            tripleDES.Key = UTF8Encoding.UTF8.GetBytes(key);
            tripleDES.Mode = CipherMode.ECB;
            tripleDES.Padding = PaddingMode.PKCS7;
            ICryptoTransform cTransform = tripleDES.CreateDecryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(inputArray, 0, inputArray.Length);
            tripleDES.Clear();
            return UTF8Encoding.UTF8.GetString(resultArray);
        }
    }
}
