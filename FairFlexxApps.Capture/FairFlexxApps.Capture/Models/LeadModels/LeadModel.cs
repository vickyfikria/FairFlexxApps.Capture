using System;
using System.Collections.ObjectModel;
using FairFlexxApps.Capture.Enums;
using SkiaSharp;
using SQLite;
using SQLiteNetExtensions.Attributes;

namespace FairFlexxApps.Capture.Models.LeadModels
{
    [Table("LeadTable")]
    public class LeadModel
    {
        public LeadModel()
        {
            LeadTypesList = new ObservableCollection<LeadTypeModel>();
        }

        [PrimaryKey]
        public Guid LeadId { get; set; } = Guid.NewGuid();

        public string LeadName { get; set; }

        public string TimeStamp { get; set; }

        public string TimeStampId { get; set; }

        public string CompanyName { get; set; }

        public string Name { get; set; }

        public string EventLeadName { get; set; }

        [ForeignKey(typeof(EventModel))]
        public int EventId { get; set; }

        public string XmlForm { get; set; }

        public string Notes { get; set; }

        [Ignore]
        public Point[] SketchPoints { get; set; }
        public byte[] SketchBytes { get; set; }

        public string StringSketchPoints { get; set; }

        public bool IsAttachmentCreated { get; set; }

        public bool IsObjectCreated { get; set; }

        public int BusinessCardCountCreated { get; set; }

        public StatusOfLeadModel IconStatus { get; set; }

        public bool IsLeadWithCard { get; set; }

        public string Title { get; set; }
        public int IndexOfLanguage { get; set; }

        [Ignore]
        public ObservableCollection<LeadTypeModel> LeadTypesList { get; set; }
    }
}
