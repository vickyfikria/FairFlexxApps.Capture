
using System;
using System.Collections.Generic;
using System.Text;

namespace FairFlexxApps.Capture.Models.LeadModels
{
    public class LeadListModel
    {
        public string EventList { get; set; }
        public int LeadNmumber { get; set; } 
        public List<LeadModel> LeadList { get; set; }
    }
}
