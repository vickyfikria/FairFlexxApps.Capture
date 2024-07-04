using FairFlexxApps.Capture.Enums;
using SQLite;
using FontSize = FairFlexxApps.Capture.Enums.FontSize;
namespace FairFlexxApps.Capture.Models
{
    public class AppSettings
    {
        [PrimaryKey]
        public int Id { get; set; } = 0;

        public string LastError { get; set; } = string.Empty;

        //public bool IsLogin { get; set; }

        public bool IsActivated { get; set; }

        public bool IsAutoLogin { get; set; }

        //public string HttpUrl { get; set; } = "http://test-fastupload.fairflexx.net/";
        //public string HttpUrl { get; set; } = "http://192.168.137.1/";
        public string HttpUrl { get; set; } = "https://fastupload.fairflexx.de/";
        public string UrlProduction { get; set; } = "https://fastupload.fairflexx.de/";
        
        //public string HttpUrl { get; set; } = "http://40.113.103.221:8083/";

        public int ClientId { get; set; }

        public double VersionTemplate { get; set; } = 1.0;

        public byte[] LogoUser { get; set; }

        public string LogoPath { get; set; }

        public FontSize FontSize { get; set; } = FontSize.Small;

        public string DefaultLogo { get; set; } = "splash_logo.png";

        public HorizontalPositions HorizontalIcon { get; set; } = HorizontalPositions.Middle;

        public VerticalPostions VerticalIcon { get; set; } = VerticalPostions.Center;

    }
}
