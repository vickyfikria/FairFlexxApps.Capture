using System.Collections.ObjectModel;
using System.Xml.Serialization;
using FairFlexxApps.Capture.Models.Templates.Pages;

namespace FairFlexxApps.Capture.Models.Templates
{
    [XmlRoot("page")]
    public class Page
    {
        [XmlAttribute("id")] 
        public string PageID { get; set; }

        [XmlElement("short")]
        public string Short { get; set; }

        [XmlElement("name")]
        public string Name { get; set; }

        [XmlElement("sidemenu")]
        public SideMenu SideMenu { get; set; }

        [XmlElement("visible")]
        public bool Visible { get; set; } = true;

        [XmlElement("LeadWithCard")]
        public LeadWithCard LeadWithCard { get; set; } = null;

        [XmlElement("box")]
        public ObservableCollection<Box> Boxs { get; set; } = new ObservableCollection<Box>();
    }
}
