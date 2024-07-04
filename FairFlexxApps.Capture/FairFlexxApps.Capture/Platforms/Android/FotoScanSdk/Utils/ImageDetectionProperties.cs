using System;
using Org.Opencv.Core;

namespace FairFlexxApps.Capture.Droid.FotoScanSdk.Utils
{
    public class ImageDetectionProperties
    {
        private double previewWidth;
        private double previewHeight;
        private double resultWidth;
        private double resultHeight;
        private Point topLeftPoint;
        private Point bottomLeftPoint;
        private Point bottomRightPoint;
        private Point topRightPoint;
        private double previewArea;
        private double resultArea;

        public ImageDetectionProperties(double previewWidth, double previewHeight, double resultWidth,
                                    double resultHeight, double previewArea, double resultArea,
                                    Point topLeftPoint, Point bottomLeftPoint, Point bottomRightPoint,
                                    Point topRightPoint)
        {
            this.previewWidth = previewWidth;
            this.previewHeight = previewHeight;
            this.previewArea = previewArea;
            this.resultWidth = resultWidth;
            this.resultHeight = resultHeight;
            this.resultArea = resultArea;
            this.bottomLeftPoint = bottomLeftPoint;
            this.bottomRightPoint = bottomRightPoint;
            this.topLeftPoint = topLeftPoint;
            this.topRightPoint = topRightPoint;
        }

        public bool isDetectedAreaBeyondLimits()
        {
            return resultArea > previewArea * 0.98 || resultArea < previewArea * 0.10;
        }

        public bool isDetectedWidthAboveLimit()
        {
            return resultWidth / previewWidth > 0.98;
        }

        public bool isDetectedHeightAboveLimit()
        {
            return resultHeight / previewHeight > 0.98;
        }

        public bool isDetectedHeightAboveNinetySeven()
        {
            return resultHeight / previewHeight > 0.97;
        }

        public bool isDetectedHeightAboveEightyFive()
        {
            return resultHeight / previewHeight > 0.85;
        }

        public bool isDetectedAreaAboveLimit()
        {
            return resultArea > previewArea * 0.75;
        }

        public bool isDetectedImageDisProportionate()
        {
            return resultHeight / resultWidth > 4;
        }

        public bool isDetectedAreaBelowLimits()
        {
            return resultArea < previewArea * 0.25;
        }

        public bool isDetectedAreaBelowRatioCheck()
        {
            return resultArea < previewArea * 0.35;
        }

        public bool isAngleNotCorrect(MatOfPoint2f approx)
        {
            return getMaxCosine(approx) || isLeftEdgeDistorted() || isRightEdgeDistorted();
        }

        private bool isRightEdgeDistorted()
        {
            return Math.Abs(topRightPoint.Y - bottomRightPoint.X) > 100;
        }

        private bool isLeftEdgeDistorted()
        {
            return Math.Abs(topLeftPoint.Y - bottomLeftPoint.Y) > 100;
        }

        private bool getMaxCosine(MatOfPoint2f approx)
        {
            double maxCosine = 0;
            Point[] approxPoints = approx.ToArray();
            maxCosine = ScanUtils.GetMaxCosine(maxCosine, approxPoints);
            return maxCosine >= 0.085;
        }

        public bool isEdgeTouching()
        {
            return isTopEdgeTouching() || isBottomEdgeTouching() || isLeftEdgeTouching() || isRightEdgeTouching();
        }

        public bool isReceiptToughingSides()
        {
            return isLeftEdgeTouching() || isRightEdgeTouching();
        }

        public bool isReceiptTouchingTopOrBottom()
        {
            return isTopEdgeTouching() || isBottomEdgeTouching();
        }

        public bool isReceiptTouchingTopAndBottom()
        {
            return isTopEdgeTouchingProper() && isBottomEdgeTouchingProper();
        }

        private bool isBottomEdgeTouchingProper()
        {
            return (bottomLeftPoint.X >= previewHeight - 10 || bottomRightPoint.X >= previewHeight - 10);
        }

        private bool isTopEdgeTouchingProper()
        {
            return (topLeftPoint.X <= 10 || topRightPoint.X <= 10);
        }

        private bool isBottomEdgeTouching()
        {
            return (bottomLeftPoint.X >= previewHeight - 50 || bottomRightPoint.X >= previewHeight - 50);
        }

        private bool isTopEdgeTouching()
        {
            return (topLeftPoint.X <= 50 || topRightPoint.X <= 50);
        }

        private bool isRightEdgeTouching()
        {
            return (topRightPoint.Y >= previewWidth - 50 || bottomRightPoint.Y >= previewWidth - 50);
        }

        private bool isLeftEdgeTouching()
        {
            return (topLeftPoint.Y <= 50 || bottomLeftPoint.Y <= 50);
        }
    }
}