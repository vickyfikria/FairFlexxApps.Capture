using SQLite;
using SQLiteNetExtensions.Attributes;
using FairFlexxApps.Capture.Models.FotoScanSdk;

namespace FairFlexxApps.Capture.Models.LeadModels
{
    public class ScannerResult
    {
        [PrimaryKey]
        public Guid Id { get; set; } = Guid.NewGuid();

        [ForeignKey(typeof(LeadTypeModel))]
        public Guid LeadId { get; set; }

        public byte[] ByteImage { get; set; }

        [Ignore]
        public ImageSource Source { get; set; }

        [Ignore]
        public ImageModel ImageLink { get; set; }
        //public string ImageLinkBlobbed { get; set; } // serialized
    }
}
