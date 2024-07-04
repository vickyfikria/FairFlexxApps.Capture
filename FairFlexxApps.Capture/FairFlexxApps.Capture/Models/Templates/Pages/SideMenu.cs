using System.Xml.Serialization;
using FairFlexxApps.Capture.Enums;

namespace FairFlexxApps.Capture.Models.Templates.Pages
{
    [XmlRoot("sidemenu")]
    public class SideMenu
    {
        [XmlIgnore]
        public string SideMenuID { get; set; }

        [XmlElement("languages")]
        public Language Language { get; set; }

        [XmlIgnore]
        public string ImageSource { get; set; }

        [XmlIgnore]
        public SideMenuItemStatus ItemStatus { get; set; }

        [XmlIgnore]
        public bool Selected { get; set; }

        [XmlIgnore]
        public bool IsMissed { get; set; }

        [XmlIgnore]
        public bool IsVisible { get; set; }

        [XmlElement("short")]
        [XmlIgnore]
        public string Short { get; set; }

        [XmlElement("name")]
        [XmlIgnore]
        public string Name { get; set; }
    }
}
