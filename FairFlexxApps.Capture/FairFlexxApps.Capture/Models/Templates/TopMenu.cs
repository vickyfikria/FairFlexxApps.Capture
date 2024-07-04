using System.Xml.Serialization;

namespace FairFlexxApps.Capture.Models.Templates
{
    [XmlRoot("topmenu")]
    public class TopMenu
    {
        [XmlElement("notes")]
        public bool Notes { get; set; }

        [XmlElement("sketch")]
        public bool Sketch { get; set; }

        [XmlElement("card")]
        public bool Card { get; set; }

        [XmlElement("attachment")]
        public bool Attachment { get; set; }

        [XmlElement("object")]
        public bool Object { get; set; }
    }
}
