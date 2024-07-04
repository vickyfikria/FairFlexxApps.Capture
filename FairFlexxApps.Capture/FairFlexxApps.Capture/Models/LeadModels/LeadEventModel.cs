using System;
using SQLite;

namespace FairFlexxApps.Capture.Models.LeadModels
{
    public class LeadEventModel
    {
        [PrimaryKey]
        public Guid LeadEventId { get; set; } = Guid.NewGuid();

        public string EventName { get; set; }

        public string XmlFormat { get; set; }

        public int NumberLeads { get; set; }
    }
}
