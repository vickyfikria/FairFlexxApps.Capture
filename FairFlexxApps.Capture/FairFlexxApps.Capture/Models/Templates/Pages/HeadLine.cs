using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace FairFlexxApps.Capture.Models.Templates.Pages
{
    [XmlRoot("headline")]
    public class HeadLine
    {
        [XmlElement("languages")]
        public Language Language { get; set; }
    }
}
