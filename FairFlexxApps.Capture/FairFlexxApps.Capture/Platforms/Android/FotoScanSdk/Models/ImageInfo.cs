namespace FairFlexxApps.Capture.Droid.FotoScanSdk.Models
{
    public class ImageInfo
    {
        public string ImagePath { get; set; }
        public string ImageOriginalPath { get; set; }
        public double[] PointsArray { get; set; }

        public bool isAlreadyCropped;

        public ImageInfo()
        {
        }

        public ImageInfo(string imagePath, string imageOriginalPath, double[] pointsArray = null, bool isalreadycropped = false)
        {
            ImagePath = imagePath;
            ImageOriginalPath = imageOriginalPath;
            PointsArray = pointsArray;
            isAlreadyCropped = isalreadycropped;
        }
    }
}