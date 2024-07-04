using System.Xml.Serialization;

namespace FairFlexxApps.Capture.Models.Templates.Controls
{
    [XmlRoot("input")]
    public class Input
    {
        [XmlAttribute(AttributeName = "type")]
        public string InputType { get; set; }

        [XmlAttribute(AttributeName = "id")]
        public string InputId { get; set; }

        [XmlAttribute(AttributeName = "name")]
        public string InputName { get; set; } // for Radio

        [XmlAttribute(AttributeName = "data")]
        public string InputData { get; set; }

        [XmlElement(ElementName = "input")]
        public Input InputChildren { get; set; }

        [XmlElement("languages")]
        public Language Languages { get; set; }

        [XmlElement("visible")]
        public bool Visible { get; set; } = true;

        [XmlElement("mandatory")]
        public bool Mandatory { get; set; }

        [XmlElement("short")]
        public string Short { get; set; }

        [XmlElement("value")]
        public string Value { get; set; }

        [XmlIgnore]
        public string ValueChildren { get; set; }

    }
}
