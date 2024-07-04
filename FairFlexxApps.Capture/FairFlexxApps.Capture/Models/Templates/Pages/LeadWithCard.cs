using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace FairFlexxApps.Capture.Models.Templates.Pages
{
    [XmlRoot("LeadWithCard")]
    public class LeadWithCard
    {
        [XmlElement("visible")]
        public bool Visible { get; set; } 
    }
}
