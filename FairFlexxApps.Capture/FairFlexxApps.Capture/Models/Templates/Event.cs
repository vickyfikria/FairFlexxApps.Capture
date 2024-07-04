using System;
using System.Xml.Serialization;

namespace FairFlexxApps.Capture.Models.Templates
{
    [XmlRoot("event")]
    public class Event
    {
        [XmlElement("name")]
        public string Name { get; set; }

        [XmlElement("location")]
        public string Location { get; set; }

        [XmlElement("country")]
        public string Country { get; set; }

        [XmlElement("timezone")]
        public string TimeZone { get; set; }

        [XmlElement("startdate")]
        public string StartDate { get; set; }

        [XmlElement("enddate")]
        public string EndDate { get; set; }
    }
}
