using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Xml.Serialization;

namespace FairFlexxApps.Capture.Models.Templates.Controls
{
    [XmlRoot("data")]
    public class Data
    {
        [XmlElement(ElementName = "languages")]
        public Language LanguageItem { get; set; }

        [XmlAttribute(AttributeName = "id")]
        public string DataId { get; set; }
    }
}
