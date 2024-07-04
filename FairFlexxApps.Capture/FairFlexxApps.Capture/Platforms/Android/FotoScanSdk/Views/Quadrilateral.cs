using Org.Opencv.Core;

namespace FairFlexxApps.Capture.Droid.FotoScanSdk.Views
{
    public class Quadrilateral
    {
        public MatOfPoint2f contour;
        public Point[] points;

        public Quadrilateral(MatOfPoint2f contour, Point[] points)
        {
            this.contour = contour;
            this.points = points;
        }
    }
}