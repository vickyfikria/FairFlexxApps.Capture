using System;
using System.Collections.ObjectModel;
using Newtonsoft.Json;
using SQLite;

namespace FairFlexxApps.Capture.Models
{
    [Table("EventTable")]
    public class EventModel
    {
        [PrimaryKey]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Location { get; set; }
        public DateTime StartDateTime { get; set; }
        public DateTime EndDateTime { get; set; }
        public string EventDateFormated
        {
            get
            {
                if (StartDateTime.Year > 2000 && EndDateTime.Year > 2000)
                    return " (" + StartDateTime.ToString("dd.MM.") + " - " + EndDateTime.ToString("dd.MM.yyyy") + ")";

                return "";
            }
        }

        public bool IsSelection { get; set; } = false;

        [JsonProperty("Xml")]
        public string XmlFormat { get; set; }

        public string Languages { get; set; }

        public int NumberLeads { get; set; }

        public int OpenLeads { get; set; }

        public int TodayLeads { get; set; }

        public int TotalLeads { get; set; }

        public DateTime LatestUpdatedTime { get; set; }

        public bool IsDeleted { get; set; }

        [Ignore]
        public string DisplayString => Name + EventDateFormated;

        [Ignore]
        public ObservableCollection<string> LanguageButtons { get; set; }

        [Ignore]
        public bool HasReleasedXml { get; set; }
    }
}
