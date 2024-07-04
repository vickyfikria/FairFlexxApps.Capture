using System.Xml.Serialization;

namespace FairFlexxApps.Capture.Models.Templates.Controls
{
    [XmlRoot("text")]
    public class Text
    {
        [XmlElement("languages")]
        public Language Languages { get; set; }
    }
}
