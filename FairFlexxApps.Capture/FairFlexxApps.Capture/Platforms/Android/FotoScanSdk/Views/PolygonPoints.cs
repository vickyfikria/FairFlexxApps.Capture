using Android.Graphics;

namespace FairFlexxApps.Capture.Droid.FotoScanSdk.Views
{
    public class PolygonPoints
    {
        private PointF topLeftPoint;
        private PointF topRightPoint;
        private PointF bottomLeftPoint;
        private PointF bottomRightPoint;

        public PolygonPoints(PointF topLeftPoint, PointF topRightPoint, PointF bottomLeftPoint, PointF bottomRightPoint)
        {
            this.topLeftPoint = topLeftPoint;
            this.topRightPoint = topRightPoint;
            this.bottomLeftPoint = bottomLeftPoint;
            this.bottomRightPoint = bottomRightPoint;
        }

        public PointF getTopLeftPoint()
        {
            return topLeftPoint;
        }

        public PointF getTopRightPoint()
        {
            return topRightPoint;
        }

        public PointF getBottomLeftPoint()
        {
            return bottomLeftPoint;
        }

        public PointF getBottomRightPoint()
        {
            return bottomRightPoint;
        }
    }
}