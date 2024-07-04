using System.Collections.ObjectModel;
using System.Xml.Serialization;
using FairFlexxApps.Capture.Enums;
using FairFlexxApps.Capture.Models.Templates.Controls;

namespace FairFlexxApps.Capture.Models.Templates.Pages
{
    [XmlRoot("box")]
    public class Box
    {
        [XmlAttribute("id")]
        public string Id { get; set; }

        [XmlAttribute("type")]
        public string BoxType { get; set; }

        [XmlElement("size")]
        public string Size { get; set; }

        [XmlElement("height")]
        public double Height { get; set; }
        
        [XmlElement("mandatory")]
        public bool Mandatory { get; set; }

        [XmlElement("mandatory-all-fields")]
        public bool MandatoryAllFields { get; set; }

        [XmlElement("visible")]
        public bool Visible { get; set; } = true;

        [XmlElement("headline")]
        public HeadLine HeadLine { get; set; }

        [XmlElement("body")]
        public Body Body { get; set; }

        [XmlElement(ElementName = "data")]
        public ObservableCollection<Data> Data { get; set; }

        [XmlAttribute(AttributeName = "column")]
        public int Column { get; set; }

        // Not use

        [XmlElement("short")]
        public string Short { get; set; }

        [XmlElement("name")]
        public string Name { get; set; }

        [XmlIgnore]
        public SideMenuItemStatus BoxStatus { get; set; }

    }
}
