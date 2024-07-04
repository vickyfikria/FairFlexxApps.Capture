using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Util;
using Android.Views;
using FairFlexxApps.Capture.Droid.FotoScanSdk.Views;
using Java.IO;
using Java.Text;
using Java.Util;
using Org.Opencv.Core;
using Org.Opencv.Imgproc;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Media;
using Java.Nio;
using Bitmap = Android.Graphics.Bitmap;
using Camera = Android.Hardware.Camera;
using File = Java.IO.File;
using Matrix = Android.Graphics.Matrix;
using PdfDocument = Android.Graphics.Pdf.PdfDocument;
using Point = Org.Opencv.Core.Point;
using PointF = Android.Graphics.PointF;

namespace FairFlexxApps.Capture.Droid.FotoScanSdk.Utils
{
    public class ScanUtils
    {
        public static float MAX_VALUE_FLOAT = 3.4028235E38F;
        public static double MAX_VALUE_DOUBLE = 1.7976931348623157E308D;

        public static bool CompareFloats(double left, double right)
        {
            double epsilon = 0.00000001;
            return Math.Abs(left - right) < epsilon;
        }

        public static Camera.Size DeterminePictureSize(Camera camera, Camera.Size previewSize)
        {
            if (camera == null) return null;

            Camera.Parameters cameraParams = camera.GetParameters();
            var pictureSizeList = cameraParams.SupportedPictureSizes;

            Camera.Size retSize = null;

            // if the preview size is not supported as a picture size
            float reqRatio = ((float)previewSize.Width) / previewSize.Height;
            float curRatio, deltaRatio;
            float deltaRatioMin = MAX_VALUE_FLOAT;
            foreach (var size in pictureSizeList)
            {
                curRatio = ((float)size.Width) / size.Height;
                deltaRatio = Math.Abs(reqRatio - curRatio);
                if (deltaRatio < deltaRatioMin)
                {
                    deltaRatioMin = deltaRatio;
                    retSize = size;
                }
                if (ScanUtils.CompareFloats(deltaRatio, 0))
                {
                    break;
                }
            }

            return retSize;
        }

        public static Camera.Size GetOptimalPreviewSize(Camera camera, int w, int h)
        {
            if (camera == null) return null;
            double targetRatio = (double)h / w;
            Camera.Parameters cameraParams = camera.GetParameters();
            var previewSizeList = cameraParams.SupportedPreviewSizes;
            var index = FindMaxCameraSize(previewSizeList, targetRatio);

            return previewSizeList[index];
        }

        private static int FindMaxCameraSize(IList<Camera.Size> sizes, double targetRatio)
        {
            int maxValIdx = 0;

            for (int contourIdx = 0; contourIdx < sizes.Count; contourIdx++)
            {
                double ratio1 = (double)sizes[maxValIdx].Width / sizes[maxValIdx].Height;
                double ratio2 = (double)sizes[contourIdx].Width / sizes[contourIdx].Height;
                double ratioDiff1 = Math.Abs(ratio1 - targetRatio);
                double ratioDiff2 = Math.Abs(ratio2 - targetRatio);

                if (ratioDiff2 > ratioDiff1)
                {
                    maxValIdx = contourIdx;
                }
            }

            return maxValIdx;
        }

        public static int GetDisplayOrientation(Activity activity, int cameraId)
        {
            Camera.CameraInfo info = new Camera.CameraInfo();
            var rotation = activity.WindowManager.DefaultDisplay.Orientation;
            int degrees = 0;
            var dm = new DisplayMetrics();

            Camera.GetCameraInfo(cameraId, info);
            activity.WindowManager.DefaultDisplay.GetMetrics(dm);

            switch (rotation)
            {
                case (int)SurfaceOrientation.Rotation0:
                    degrees = 0;
                    break;
                case (int)SurfaceOrientation.Rotation90:
                    degrees = 90;
                    break;
                case (int)SurfaceOrientation.Rotation180:
                    degrees = 180;
                    break;
                case (int)SurfaceOrientation.Rotation270:
                    degrees = 270;
                    break;
            }

            int displayOrientation;
            if (info.Facing == Camera.CameraInfo.CameraFacingFront)
            {
                displayOrientation = (info.Orientation + degrees) % 360;
                displayOrientation = (360 - displayOrientation) % 360;
            }
            else
            {
                displayOrientation = (info.Orientation - degrees + 360) % 360;
            }

            return displayOrientation;
        }

        public static Camera.Size GetOptimalPictureSize(Camera camera, int width, int height, Camera.Size previewSize)
        {
            if (camera == null) return null;
            Camera.Parameters cameraParams = camera.GetParameters();
            var supportedSizes = cameraParams.SupportedPictureSizes;

            Camera.Size size = new Camera.Size(camera, width, height);

            // convert to landscape if necessary
            if (size.Width < size.Height)
            {
                int temp = size.Width;
                size.Width = size.Height;
                size.Height = temp;
            }

            Camera.Size requestedSize = new Camera.Size(camera, size.Width, size.Height);

            double previewAspectRatio = (double)previewSize.Width / (double)previewSize.Height;

            if (previewAspectRatio < 1.0)
            {
                // reset ratio to landscape
                previewAspectRatio = 1.0 / previewAspectRatio;
            }

            double aspectTolerance = 0.1;
            double bestDifference = MAX_VALUE_DOUBLE;

            for (int i = 0; i < supportedSizes.Count; i++)
            {
                Camera.Size supportedSize = supportedSizes[i];

                // Perfect match
                if (supportedSize.Equals(requestedSize))
                {
                    return supportedSize;
                }

                double difference = Math.Abs(previewAspectRatio - ((double)supportedSize.Width / (double)supportedSize.Height));

                if (difference < bestDifference - aspectTolerance)
                {
                    // better aspectRatio found
                    if ((width != 0 && height != 0) || (supportedSize.Width * supportedSize.Height < 2048 * 1024))
                    {
                        size.Width = supportedSize.Width;
                        size.Height = supportedSize.Height;
                        bestDifference = difference;
                    }
                }
                else if (difference < bestDifference + aspectTolerance)
                {
                    // same aspectRatio found (within tolerance)
                    if (width == 0 || height == 0)
                    {
                        // set highest supported resolution below 2 Megapixel
                        if ((size.Width < supportedSize.Width) && (supportedSize.Width * supportedSize.Height < 2048 * 1024))
                        {
                            size.Width = supportedSize.Width;
                            size.Height = supportedSize.Height;
                        }
                    }
                    else
                    {
                        // check if this pictureSize closer to requested width and height
                        if (Math.Abs(width * height - supportedSize.Width * supportedSize.Height) < Math.Abs(width * height - size.Width * size.Height))
                        {
                            size.Width = supportedSize.Width;
                            size.Height = supportedSize.Height;
                        }
                    }
                }
            }

            return size;
        }

        public static Camera.Size GetOptimalPreviewSize(int displayOrientation, List<Camera.Size> sizes, int w, int h)
        {
            double ASPECT_TOLERANCE = 0.1;
            double targetRatio = (double)w / h;
            if (displayOrientation == 90 || displayOrientation == 270)
            {
                targetRatio = (double)h / w;
            }

            if (sizes == null)
            {
                return null;
            }

            Camera.Size optimalSize = null;
            double minDiff = MAX_VALUE_DOUBLE;

            int targetHeight = h;

            // Try to find an size match aspect ratio and size
            foreach (Camera.Size size in sizes)
            {
                double ratio = (double)size.Width / size.Height;
                if (Math.Abs(ratio - targetRatio) > ASPECT_TOLERANCE) continue;
                if (Math.Abs(size.Height - targetHeight) < minDiff)
                {
                    optimalSize = size;
                    minDiff = Math.Abs(size.Height - targetHeight);
                }
            }

            // Cannot find the one match the aspect ratio, ignore the requirement
            if (optimalSize == null)
            {
                minDiff = MAX_VALUE_DOUBLE;
                foreach (Camera.Size size in sizes)
                {
                    if (Math.Abs(size.Height - targetHeight) < minDiff)
                    {
                        optimalSize = size;
                        minDiff = Math.Abs(size.Height - targetHeight);
                    }
                }
            }

            return optimalSize;
        }

        public static int ConfigureCameraAngle(Activity activity)
        {
            int angle;

            Display display = activity.WindowManager.DefaultDisplay;
            switch (display.Rotation)
            {
                case SurfaceOrientation.Rotation0: // This is display orientation
                    angle = 90; // This is camera orientation
                    break;
                case SurfaceOrientation.Rotation90:
                    angle = 0;
                    break;
                case SurfaceOrientation.Rotation180:
                    angle = 270;
                    break;
                case SurfaceOrientation.Rotation270:
                    angle = 180;
                    break;
                default:
                    angle = 90;
                    break;
            }

            return angle;
        }

        public static Quadrilateral DetectLargestQuadrilateral(Mat mat)
        {
            Mat mGrayMat = new Mat(mat.Rows(), mat.Cols(), CvType.Cv8uc1);
            Imgproc.CvtColor(mat, mGrayMat, Imgproc.ColorBgr2gray, 4);
            Imgproc.Threshold(mGrayMat, mGrayMat, 150, 255, Imgproc.ThreshBinary + Imgproc.ThreshOtsu);

            var largestContour = FindLargestContour(mGrayMat);
            if (null != largestContour)
            {
                Quadrilateral mLargestRect = FindQuadrilateral(largestContour);
                if (mLargestRect != null)
                    return mLargestRect;
            }
            return null;
        }

        public static double GetMaxCosine(double maxCosine, Point[] approxPoints)
        {
            for (int i = 2; i < 5; i++)
            {
                double cosine = Math.Abs(Angle(approxPoints[i % 4], approxPoints[i - 2], approxPoints[i - 1]));
                maxCosine = Math.Max(cosine, maxCosine);
            }
            return maxCosine;
        }

        private static double Angle(Point p1, Point p2, Point p0)
        {
            double dx1 = p1.X - p0.X;
            double dy1 = p1.Y - p0.Y;
            double dx2 = p2.X - p0.X;
            double dy2 = p2.Y - p0.Y;
            return (dx1 * dx2 + dy1 * dy2) / Math.Sqrt((dx1 * dx1 + dy1 * dy1) * (dx2 * dx2 + dy2 * dy2) + 1e-10);
        }

        public static Point[] SortPoints(Point[] src)
        {
            Point[] result = new Point[4];
            
            result[0] = GetMinSumXY(src); //top - left corner = minimal sum
            result[2] = GetMaxSumXY(src); //bottom - right corner = maximal sum
            result[1] = GetMinDifferenceXY(src); //top - right corner = minimal difference
            result[3] = GetMaxDifferenceXY(src); //bottom - left corner = maximal difference

            return result;
        }

        private static Point GetMaxSumXY(Point[] src)
        {
            double maxSum = 0;
            int maxSumIndex = 0;
            for (int i = 0; i < src.Length; i++)
            {
                double sum = SumXY(src[i]);
                if (maxSum < sum)
                {
                    maxSum = sum;
                    maxSumIndex = i;
                }
            }

            return src[maxSumIndex];
        }

        private static Point GetMinSumXY(Point[] src)
        {
            double minSum = SumXY(src[0]);
            int minSumIndex = 0;
            for (int i = 1; i < src.Length; i++)
            {
                double sum = SumXY(src[i]);
                if (minSum > sum)
                {
                    minSum = sum;
                    minSumIndex = i;
                }
            }

            return src[minSumIndex];
        }

        private static Point GetMaxDifferenceXY(Point[] src)
        {
            double maxDiff = 0;
            int maxDiffIndex = 0;
            for (int i = 0; i < src.Length; i++)
            {
                double sub = SubYX(src[i]);
                if (maxDiff < sub)
                {
                    maxDiff = sub;
                    maxDiffIndex = i;
                }
            }

            return src[maxDiffIndex];
        }

        private static Point GetMinDifferenceXY(Point[] src)
        {
            double minDiff = SubYX(src[0]);
            int minDiffIndex = 0;
            for (int i = 1; i < src.Length; i++)
            {
                double sub = SubYX(src[i]);
                if (minDiff > sub)
                {
                    minDiff = sub;
                    minDiffIndex = i;
                }
            }

            return src[minDiffIndex];
        }

        private static double SumXY(Point p)
        {
            return p.X + p.Y;
        }

        private static double SubYX(Point p)
        {
            return p.Y - p.X;
        }

        private static JavaList<MatOfPoint> FindLargestContour(Mat inputMat)
        {
            Mat mHierarchy = new Mat();
            var mContourList = new JavaList<MatOfPoint>();

            //finding contours
            Imgproc.FindContours(inputMat, mContourList, mHierarchy, Imgproc.RetrExternal, Imgproc.ChainApproxSimple);

            Mat mContoursMat = new Mat();
            mContoursMat.Create(inputMat.Rows(), inputMat.Cols(), CvType.Cv8u);

            if (mContourList.Count != 0)
            {
                double maxVal = 0;
                int maxValIdx = -1;
                for (int contourIdx = 0; contourIdx < mContourList.Size(); contourIdx++)
                {
                    double contourArea = Imgproc.ContourArea(mContourList[contourIdx]);
                    if (maxVal < contourArea && Math.Abs(contourArea) >= 30000)
                    {
                        maxVal = contourArea;
                        maxValIdx = contourIdx;
                    }
                }

                var test = new JavaList<MatOfPoint>();
                test.Add(mContourList[maxValIdx]);

                return test;// mContourList;
                //return mContourList; 
            }

            return null;
        }

        private static Quadrilateral FindQuadrilateral(JavaList<MatOfPoint> mContourList)
        {
            foreach (MatOfPoint c in mContourList)
            {
                MatOfPoint2f c2f = new MatOfPoint2f(c.ToArray());
                double peri = Imgproc.ArcLength(c2f, true);
                MatOfPoint2f approx = new MatOfPoint2f();
                Imgproc.ApproxPolyDP(c2f, approx, 0.02 * peri, true);
                Point[] points = approx.ToArray();
                // select biggest 4 angles polygon
                if (approx.Rows() == 4)
                {
                    Point[] foundPoints = SortPoints(points);
                    return new Quadrilateral(approx, points);
                }
            }
            return null;
        }

        public static Bitmap enhanceReceipt(Bitmap image, Point topLeft, Point topRight, Point bottomLeft, Point bottomRight)
        {
            int resultWidth = (int)Math.Abs(topRight.X - topLeft.X);
            int bottomWidth = (int)Math.Abs(bottomRight.X - bottomLeft.X);
            if (bottomWidth > resultWidth)
                resultWidth = bottomWidth;

            int resultHeight = (int)(bottomLeft.Y - topLeft.Y);
            int bottomHeight = (int)(bottomRight.Y - topRight.Y);
            if (bottomHeight > resultHeight)
                resultHeight = bottomHeight;

            Mat inputMat = new Mat(image.Height, image.Height, CvType.Cv8uc1);
            Org.Opencv.Android.Utils.BitmapToMat(image, inputMat);
            resultHeight = resultHeight < 0 ? Math.Abs(resultHeight) : resultHeight;
            Mat outputMat = new Mat(resultWidth, resultHeight, CvType.Cv8uc1);

            var source = new List<Point>();
            source.Add(topLeft);
            source.Add(topRight);
            source.Add(bottomLeft);
            source.Add(bottomRight);
            Mat startM = Org.Opencv.Utils.Converters.Vector_Point2f_to_Mat(source);

            Point ocvPOut1 = new Point(0, 0);
            Point ocvPOut2 = new Point(resultWidth, 0);
            Point ocvPOut3 = new Point(0, resultHeight);
            Point ocvPOut4 = new Point(resultWidth, resultHeight);
            List<Point> dest = new List<Point>();
            dest.Add(ocvPOut1);
            dest.Add(ocvPOut2);
            dest.Add(ocvPOut3);
            dest.Add(ocvPOut4);
            Mat endM = Org.Opencv.Utils.Converters.Vector_Point2f_to_Mat(dest);

            Mat perspectiveTransform = Imgproc.GetPerspectiveTransform(startM, endM);

            Imgproc.WarpPerspective(inputMat, outputMat, perspectiveTransform, new Org.Opencv.Core.Size(resultWidth, resultHeight));

            if (resultWidth > 0 && resultHeight > 0)
            {
                Bitmap output = Bitmap.CreateBitmap(resultWidth, resultHeight, Bitmap.Config.Argb8888);
                Org.Opencv.Android.Utils.MatToBitmap(outputMat, output);
                return output;
            }
            else
            {
                return null;
            }
        }

        public static Bitmap RotateBitmap(Bitmap bitmap, float angle)
        {
            Matrix matrix = new Matrix();

            matrix.PostRotate(angle);
            return Bitmap.CreateBitmap(bitmap, 0, 0, bitmap.Width, bitmap.Height, matrix, true);
        }

        public static string[] SaveToInternalMemory(Bitmap bitmap, string mFileDirectory, string
            mFileName, Context mContext, int mQuality)
        {
            string[] mReturnParams = new string[2];
            File mDirectory = GetBaseDirectoryFromPathString(mFileDirectory, mContext);
            File mPath = new File(mDirectory, mFileName);
            try
            {
                var mFileOutputStream = new FileOutputStream(mPath);
                //Compress method used on the Bitmap object to write  image to output stream
                //bitmap.Compress(Bitmap.CompressFormat.Jpeg, mQuality, mFileOutputStream);
                mFileOutputStream.Close();
            }
            catch (Exception e)
            {
            }
            mReturnParams[0] = mDirectory.AbsolutePath;
            mReturnParams[1] = mFileName;
            return mReturnParams;
        }

        public static string SaveToExternalStorage(Bitmap bitmap)
        {
            var filePath = CreateFilePath();

            try
            {
                FileOutputStream file = new FileOutputStream(filePath);
                var stream = new System.IO.FileStream(filePath, System.IO.FileMode.Create);
                bitmap.Compress(Bitmap.CompressFormat.Jpeg, 80, stream);
                stream.Close();
            }
            catch (Exception e)
            {
            }

            return filePath;
        }

        private static string CreateFilePath()
        {
            var filePath = $"{Android.OS.Environment.ExternalStorageDirectory.Path}/FotoScanSdk/";

            if (!System.IO.File.Exists(filePath))
                System.IO.Directory.CreateDirectory(filePath);

            SimpleDateFormat sdf = new SimpleDateFormat("yyyy-MM-dd HH.mm.ss");
            string currentDateandTime = sdf.Format(new Date());

            string fileName = "Scan " + currentDateandTime + ".png";

            var localPublicFilePath = System.IO.Path.Combine(filePath, fileName);
            return localPublicFilePath;
        }

        public static string SaveMultipleImageAsPdf(List<Bitmap> listBitmaps)
        {
            PdfDocument document = new PdfDocument();
            foreach (var bitmap in listBitmaps)
            {
                PdfDocument.PageInfo pageInfo = new PdfDocument.PageInfo.Builder(bitmap.Width, bitmap.Height, 1).Create();

                // start a page
                PdfDocument.Page page = document.StartPage(pageInfo);

                Canvas canvas = page.Canvas;

                var scaledBitmap = Bitmap.CreateScaledBitmap(bitmap, bitmap.Width, bitmap.Height, true);

                canvas.DrawBitmap(scaledBitmap, 0, 0, null);

                document.FinishPage(page);
            }

            var sdCardPath = $"{Android.OS.Environment.ExternalStorageDirectory.Path}/FotoScanSdk/";
            SimpleDateFormat sdf = new SimpleDateFormat("yyyy-MM-dd HH.mm.ss");
            string currentDateandTime = sdf.Format(new Date());
            string fileName = "Scan " + currentDateandTime + ".pdf";
            var filePath = System.IO.Path.Combine(sdCardPath, fileName);

            var stream = new FileStream(filePath, FileMode.Create);

            document.WriteTo(stream);
            stream.Flush();

            // close the document
            document.Close();

            return filePath;
        }

        public static void SaveImageAsPdf(Bitmap bitmap)
        {
            PdfDocument document = new PdfDocument();

            // crate a page description
            PdfDocument.PageInfo pageInfo = new PdfDocument.PageInfo.Builder(bitmap.Width, bitmap.Height, 1).Create();

            // start a page
            PdfDocument.Page page = document.StartPage(pageInfo);

            Canvas canvas = page.Canvas;

            bitmap = Bitmap.CreateScaledBitmap(bitmap, bitmap.Width, bitmap.Height, true);

            canvas.DrawBitmap(bitmap, 0, 0, null);

            // finish the page
            document.FinishPage(page);

            var sdCardPath = $"{Android.OS.Environment.ExternalStorageDirectory.Path}/FotoScanSdk/";
            SimpleDateFormat sdf = new SimpleDateFormat("yyyy-MM-dd HH.mm.ss");
            string currentDateandTime = sdf.Format(new Date());
            string fileName = "Scan " + currentDateandTime + ".pdf";
            var filePath = System.IO.Path.Combine(sdCardPath, fileName);

            var stream = new FileStream(filePath, FileMode.Create);

            document.WriteTo(stream);
            stream.Flush();

            // close the document
            document.Close();
        }

        private static File GetBaseDirectoryFromPathString(string mPath, Context mContext)
        {
            ContextWrapper mContextWrapper = new ContextWrapper(mContext);

            return mContextWrapper.GetDir(mPath, FileCreationMode.Private);
        }

        public static Android.Graphics.Bitmap DecodeBitmapFromFile(String path, String imageName)
        {
            // First decode with inJustDecodeBounds=true to check dimensions
            var options = new Android.Graphics.BitmapFactory.Options();
            options.InPreferredConfig = Android.Graphics.Bitmap.Config.Argb8888;

            return Android.Graphics.BitmapFactory.DecodeFile(new File(path, imageName).AbsolutePath,
                    options);
        }

        public static int Dp2px(Context context, float dp)
        {
            float px = TypedValue.ApplyDimension(ComplexUnitType.Dip, dp, context.Resources.DisplayMetrics);
            return Java.Lang.Math.Round(px);
        }

        public static Android.Graphics.Bitmap DecodeBitmapFromByteArray(byte[] data, int reqWidth, int reqHeight)
        {
            // Raw height and width of image
            // First decode with inJustDecodeBounds=true to check dimensions
            Android.Graphics.BitmapFactory.Options options = new Android.Graphics.BitmapFactory.Options();
            options.InJustDecodeBounds = true;
            Android.Graphics.BitmapFactory.DecodeByteArray(data, 0, data.Length, options);

            // Calculate inSampleSize
            int height = options.OutHeight;
            int width = options.OutWidth;
            int inSampleSize = 1;

            if (height > reqHeight || width > reqWidth)
            {
                int halfHeight = height / 2;
                int halfWidth = width / 2;

                // Calculate the largest inSampleSize value that is a power of 2 and keeps both
                // height and width larger than the requested height and width.
                while ((halfHeight / inSampleSize) >= reqHeight && (halfWidth / inSampleSize) >= reqWidth)
                {
                    inSampleSize *= 2;
                }
            }
            options.InSampleSize = inSampleSize;

            // Decode bitmap with inSampleSize set
            options.InJustDecodeBounds = false;
            return Android.Graphics.BitmapFactory.DecodeByteArray(data, 0, data.Length, options);
        }

        public static Android.Graphics.Bitmap LoadEfficientBitmap(byte[] data, int width, int height)
        {
            Android.Graphics.Bitmap bmp;

            // First decode with inJustDecodeBounds=true to check dimensions
            Android.Graphics.BitmapFactory.Options options = new Android.Graphics.BitmapFactory.Options();
            options.InJustDecodeBounds = true;
            Android.Graphics.BitmapFactory.DecodeByteArray(data, 0, data.Length, options);

            // Calculate inSampleSize
            options.InSampleSize = CalculateInSampleSize(options, width, height);

            // Decode bitmap with inSampleSize set
            options.InJustDecodeBounds = false;
            bmp = Android.Graphics.BitmapFactory.DecodeByteArray(data, 0, data.Length, options);
            return bmp;
        }

        private static int CalculateInSampleSize(Android.Graphics.BitmapFactory.Options options, int reqWidth, int reqHeight)
        {
            // Raw height and width of image
            int height = options.OutHeight;
            int width = options.OutWidth;
            int inSampleSize = 1;

            if (height > reqHeight || width > reqWidth)
            {
                int halfHeight = height / 2;
                int halfWidth = width / 2;

                // Calculate the largest inSampleSize value that is a power of 2 and keeps both
                // height and width larger than the requested height and width.
                while ((halfHeight / inSampleSize) >= reqHeight
                        && (halfWidth / inSampleSize) >= reqWidth)
                {
                    inSampleSize *= 2;
                }
            }

            return inSampleSize;
        }

        public static Bitmap resize(Bitmap image, int maxWidth, int maxHeight)
        {
            try
            {
                if (maxHeight > 0 && maxWidth > 0)
                {
                    int width = image.Width;
                    int height = image.Height;
                    float ratioBitmap = (float)width / (float)height;
                    float ratioMax = (float)maxWidth / (float)maxHeight;

                    int finalWidth = maxWidth;
                    int finalHeight = maxHeight;
                    if (ratioMax > 1)
                    {
                        finalWidth = (int)((float)maxHeight * ratioBitmap);
                    }
                    else
                    {
                        finalHeight = (int)((float)maxWidth / ratioBitmap);
                    }

                    image = Bitmap.CreateScaledBitmap(image, finalWidth, finalHeight, true);
                    return image;
                }
                else
                {
                    return image;
                }
            }
            catch(Exception ex)
            {
                return null;
            }
        }

        public static Bitmap ResizeToScreenContentSize(Bitmap bm, int newWidth, int newHeight)
        {
            int width = bm.Width;
            int height = bm.Height;
            float scaleWidth = ((float)newWidth) / width;
            float scaleHeight = ((float)newHeight) / height;
            // CREATE A MATRIX FOR THE MANIPULATION
            Matrix matrix = new Matrix();
            // RESIZE THE BIT MAP
            matrix.PostScale(scaleWidth, scaleHeight);

            // "RECREATE" THE NEW BITMAP
            Bitmap resizedBitmap = Bitmap.CreateBitmap(
                    bm, 0, 0, width, height, matrix, false);
            
            bm.Recycle();
            return resizedBitmap;
        }

        public static JavaList<PointF> GetPolygonDefaultPoints(Bitmap bitmap)
        {
            JavaList<PointF> points;
            points = new JavaList<PointF>();
            points.Add(new PointF(bitmap.Width * (0.14f), (float)bitmap.Height * (0.13f)));
            points.Add(new PointF(bitmap.Width * (0.84f), (float)bitmap.Height * (0.13f)));
            points.Add(new PointF(bitmap.Width * (0.84f), (float)bitmap.Height * (0.83f)));
            points.Add(new PointF(bitmap.Width * (0.14f), (float)bitmap.Height * (0.83f)));
            return points;
        }

        public static bool IsScanPointsValid(Dictionary<int, PointF> points)
        {
            return points.Count == 4;
        }

        public static bool IsScanPointsValidV2(JavaList<PointF> points)
        {
            return points.Count == 4;
        }

        #region Auto crop

        public static JavaList<PointF> FindPoints(Bitmap mBitmap)
        {
            JavaList<PointF> result = null;

            Mat image = new Mat();
            Org.Opencv.Android.Utils.BitmapToMat(mBitmap, image);

            Mat edges = EdgeDetection(image);
            MatOfPoint largest = FindLargestContourSingle(edges);

            if (largest != null)
            {
                var points = SortPoints(largest.ToArray());
                result = new JavaList<PointF>();
                result.Add(new PointF((float)points[0].X, (float)points[0].Y));
                result.Add(new PointF((float)points[1].X, (float)points[1].Y));
                result.Add(new PointF((float)points[2].X, (float)points[2].Y));
                result.Add(new PointF((float)points[3].X, (float)points[3].Y));
                largest.Release();
            }
            else
            {
            }

            edges.Release();
            image.Release();

            return result;
        }

        private static Mat EdgeDetection(Mat src)
        {
            Mat edges = new Mat();
            Imgproc.CvtColor(src, edges, Imgproc.ColorBgr2gray);
            Imgproc.GaussianBlur(edges, edges, new Org.Opencv.Core.Size(5, 5), 0);
            Imgproc.Canny(edges, edges, 0, 100);
            return edges;
        }

        private static MatOfPoint FindLargestContourSingle(Mat src)
        {
            var contours = new JavaList<MatOfPoint>();
            Imgproc.FindContours(src, contours, new Mat(), Imgproc.RetrList, Imgproc.ChainApproxSimple);

            double maxVal = 0;
            int maxValIdx = -1;
            for (int contourIdx = 0; contourIdx < contours.Size(); contourIdx++)
            {
                double contourArea = Imgproc.ContourArea(contours[contourIdx]);
                if (maxVal < contourArea && Math.Abs(contourArea) >= 1000)
                {
                    maxVal = contourArea;
                    maxValIdx = contourIdx;
                }
            }

            if (maxValIdx != -1)
                return contours[maxValIdx];

            return null;
        }

        #endregion

        public static void DeleteScanFile(string filePath)
        {
            File fdelete = new File(filePath);
            if (fdelete.Exists())
            {
                if (fdelete.Delete())
                {
                    System.Diagnostics.Debug.WriteLine("file Deleted :" + filePath);
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("file not Deleted :" + filePath);
                }
            }
        }
    }
}