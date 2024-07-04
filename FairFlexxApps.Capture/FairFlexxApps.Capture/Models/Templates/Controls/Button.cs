using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace FairFlexxApps.Capture.Models.Templates.Controls
{
    [XmlRoot("button")]
    public class Button
    {
        [XmlAttribute("type")]
        public string ButtonType { get; set; }

        [XmlElement("languages")]
        public Language Languages { get; set; }
    }
}
