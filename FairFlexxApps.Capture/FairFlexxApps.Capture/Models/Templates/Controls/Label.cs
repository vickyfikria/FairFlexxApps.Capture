using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace FairFlexxApps.Capture.Models.Templates.Controls
{
    [XmlRoot("label")]
    public class Label
    {
        [XmlAttribute(AttributeName = "id")]
        public string LabelId { get; set; }

        [XmlElement("languages")]
        public Language Language { get; set; }

        [XmlElement("visible")]
        public bool Visible { get; set; } = true;

    }
}
