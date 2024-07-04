using System;
using System.Collections.ObjectModel;
using FairFlexxApps.Capture.Enums;
//using ScanbotSDK.Xamarin.Forms;
using SQLite;
using SQLiteNetExtensions.Attributes;


namespace FairFlexxApps.Capture.Models.LeadModels
{
    public class LeadTypeModel
    {
        [PrimaryKey]
        public Guid Id { get; set; } = Guid.NewGuid();

        public string StringId { get; set; } = Guid.NewGuid().ToString();

        [ForeignKey(typeof(LeadModel))]
        public Guid LeadId { get; set; }

        public LeadType Type { get; set; }

        public string FileName { get; set; }

        public string FilePath { get; set; }

        public DateTime CreateDate { get; set; }

        [Ignore] public bool IsFront { get; set; } = true;

        [Ignore]
        public int Position { get; set; }

        [Ignore]
        public byte[] BusinessCardSource { get; set; }

        [Ignore]
        public ObservableCollection<ScannerResult> ScannerResult { get; set; }

        //[Ignore]
        //public ObservableCollection<IScannedPage> ImageLink { get; set; }
        //public string ImageLinkBlobbed { get; set; } // serialized

    }
}
