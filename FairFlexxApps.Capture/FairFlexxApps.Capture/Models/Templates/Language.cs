using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Xml.Serialization;

namespace FairFlexxApps.Capture.Models.Templates
{
    [XmlRoot("languages")]
    public class Language
    {
        [XmlElement("default")]
        public string Default { get; set; }

        [XmlElement("lang1")]
        public string Lang1 { get; set; }

        [XmlElement("lang2")]
        public string Lang2 { get; set; }

        [XmlElement("itemslang1")]
        public ObservableCollection<string> ItemsLang1 { get; set; }

        [XmlElement("itemslang2")]
        public ObservableCollection<string> ItemsLang2 { get; set; }

        public ObservableCollection<string> GetLanguages()
        {
            return new ObservableCollection<string>()
            {
                this.Lang1,
                this.Lang2,
            };
        }

        public ObservableCollection<string> GetLanguagesButton()
        {
            return new ObservableCollection<string>()
            {
                this.Lang1.ToUpper(),
                this.Lang2.ToUpper(),
            };
        }

        public ObservableCollection<string> GeDatatLanguages(int indexOfLanguage)
        {
            return new ObservableCollection<string>[]
            {
                this.ItemsLang1,
                this.ItemsLang2,
            }[indexOfLanguage];
        }

        public string GetLanguage(int indexOfLanguage)
        {
            return GetLanguages()[indexOfLanguage];
        }
    }
}
