using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using FairFlexxApps.Capture.Droid.FotoScanSdk.Models;
using Android.Graphics;
using Org.Opencv.Core;
using FairFlexxApps.Capture.Droid.FotoScanSdk.Views;
using FairFlexxApps.Capture.Droid.FotoScanSdk.Utils;
using Org.Opencv.Imgproc;
using Point = Org.Opencv.Core.Point;

namespace FairFlexxApps.Capture.Droid.FotoScanSdk.Adapters
{
    public class ListImageAdapter : BaseAdapter<ImageInfo>
    {
        List<ImageInfo> Images;
        Context Context;
        bool IsAuto;

        int scaledRatio;

        public ListImageAdapter(List<ImageInfo> images, Context context, bool isAuto)
        {
            this.Images = images;
            this.Context = context;
            this.IsAuto = isAuto;
        }

        public override ImageInfo this[int position] => Images[position];

        public override int Count => Images.Count();

        public override long GetItemId(int position)
        {
            return position;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            var view = convertView;

            if (view == null)
            {
                view = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.ImageViewCell, parent, false);

                var photo = view.FindViewById<ImageView>(Resource.Id.imgvImage);

                view.Tag = new ViewHolder() { Photo = photo};
            }

            var holder = (ViewHolder)view.Tag;

            Bitmap originalImage = GetBitmap(Images[position].ImagePath);

            if (originalImage != null)
            {
                if (/*IsAuto*/ !Images[position].isAlreadyCropped)
                {
                    Mat originalMat = new Mat(originalImage.Height, originalImage.Width, CvType.Cv8uc1);
                    Org.Opencv.Android.Utils.BitmapToMat(originalImage, originalMat);
                    JavaList<PointF> points;

                    Dictionary<int, PointF> pointFs = new Dictionary<int, PointF>();

                    try
                    {
                        Quadrilateral quad = ScanUtils.DetectLargestQuadrilateral(originalMat);

                        if (null != quad)
                        {
                            double resultArea = Math.Abs(Imgproc.ContourArea(quad.contour));
                            double previewArea = originalMat.Rows() * originalMat.Cols();

                            if (resultArea > previewArea * 0.08)
                            {
                                Bitmap croppedBitmap = CropImage(originalImage, quad);

                                holder.Photo.SetImageBitmap(croppedBitmap);
                            }
                            else
                            {
                                holder.Photo.SetImageBitmap(originalImage);
                            }

                        }
                        else
                        {
                            holder.Photo.SetImageBitmap(originalImage);
                        }


                    }
                    catch (Exception e)
                    {

                    }

                }
                else
                {
                    holder.Photo.SetImageBitmap(originalImage);
                }
            }

            return view;
        }

        private Bitmap CropImage(Bitmap originalImage, Quadrilateral quad)
        {
            //if (ScanUtils.IsScanPointsValid(points))
            //{
            Point[] detectedpoints = new Point[]
            {
                                    new Point(quad.points[0].X, quad.points[0].Y),
                                    new Point(quad.points[1].X, quad.points[1].Y),
                                    new Point(quad.points[3].X, quad.points[3].Y),
                                    new Point(quad.points[2].X, quad.points[2].Y)
            };

            Point[] sortedpoints = SortPoints(detectedpoints);
            Point topleftpoint = sortedpoints[0];
            Point toprightpoint = sortedpoints[1];
            Point bottomleftpoint = sortedpoints[3];
            Point bottomrightpoint = sortedpoints[2];

            if (IsValidPoints(topleftpoint, toprightpoint, bottomleftpoint, bottomrightpoint))
            {
                return ScanUtils.enhanceReceipt(originalImage, topleftpoint, toprightpoint, bottomleftpoint, bottomrightpoint);
            }
            else
            {
                return originalImage;
            }
        }

        private Point[] SortPoints(Point[] resourcePoints)
        {
            return ScanUtils.SortPoints(resourcePoints);
        }

        private bool IsValidPoints(Point topleftpoint, Point toprightpoint, Point bottomleftpoint, Point bottomrightpoint)
        {
            if (topleftpoint.X == toprightpoint.X && topleftpoint.Y == toprightpoint.Y)
            {
                return false;
            }
            return true;
        }

        #region GetBitmap from file path

        private Bitmap GetBitmap(string imagePath)
        {
            var uri = GetImageUri(imagePath);

            try
            {
                const int imageMaxSize = 1024;
                var ins = Context.ContentResolver.OpenInputStream(uri);

                // Decode image size
                var imagesizeoptions = new BitmapFactory.Options { InJustDecodeBounds = true };
                BitmapFactory.DecodeStream(ins, null, imagesizeoptions);
                ins.Close();

                scaledRatio = 1;
                if (imagesizeoptions.OutHeight > imageMaxSize || imagesizeoptions.OutWidth > imageMaxSize)
                {
                    //scaledRatio = (int)Math.Pow(2, (int)Math.Round(Math.Log(imageMaxSize / (double)Math.Max(imagesizeoptions.OutHeight, imagesizeoptions.OutWidth)) / Math.Log(0.5)));
                }

                var option = new BitmapFactory.Options { InSampleSize = scaledRatio };
                ins = Context.ContentResolver.OpenInputStream(uri);
                var imagebitmap = BitmapFactory.DecodeStream(ins, null, option);
                ins.Close();

                return imagebitmap;
            }
            catch (Exception e)
            {
#if DEBUG
                //Log.Error(GetType().Name, e.Message);
#endif
            }

            return null;
        }

        private static Android.Net.Uri GetImageUri(string path)
        {
            return Android.Net.Uri.FromFile(new Java.IO.File(path));
        }

        private int CalculateInSampleSize(BitmapFactory.Options options, int reqWidth, int reqHeight)
        {
            int height = options.OutHeight;
            int width = options.OutWidth;
            int inSampleSize = 1;

            if (height > reqHeight || width > reqWidth)
            {
                int heightRatio = Java.Lang.Math.Round((float)height / (float)reqHeight);
                int widthRatio = Java.Lang.Math.Round((float)width / (float)reqWidth);
                inSampleSize = heightRatio < widthRatio ? heightRatio : widthRatio;
            }
            float totalPixels = width * height;
            float totalReqPixelsCap = reqWidth * reqHeight * 2;

            while (totalPixels / (inSampleSize * inSampleSize) > totalReqPixelsCap)
            {
                inSampleSize++;
            }
            return inSampleSize;
        }

        #endregion

        #region CropImage

        private Bitmap CropImage(Bitmap source, double[] points)
        {
            Bitmap bitmap = Bitmap.CreateBitmap(source);

            Bitmap resultingImage = Bitmap.CreateBitmap(source.Width, source.Height, bitmap.GetConfig());
            //Bitmap resultingImage = bitmap.Copy(Bitmap.Config.Argb8888, true);
            Canvas canvas = new Canvas(resultingImage);
            Paint paint = new Paint();

            paint.AntiAlias = true;
            Android.Graphics.Path path = new Android.Graphics.Path();

            path.MoveTo((float)points[0] / scaledRatio, (float)points[1] / scaledRatio);
            path.LineTo((float)points[2] / scaledRatio, (float)points[3] / scaledRatio);
            path.LineTo((float)points[4] / scaledRatio, (float)points[5] / scaledRatio);
            path.LineTo((float)points[6] / scaledRatio, (float)points[7] / scaledRatio);

            canvas.DrawPath(path, paint);

            paint.SetXfermode(new PorterDuffXfermode(PorterDuff.Mode.SrcIn));
            canvas.DrawBitmap(source, 0, 0, paint);

            //return resultingImage;
            return BalanceCroppedImage(resultingImage, points);
        }

        private Bitmap BalanceCroppedImage(Bitmap croppedImage, double[] points)
        {
            Matrix matrix = new Matrix();

            Bitmap resultingImage = Bitmap.CreateBitmap(croppedImage.Width, croppedImage.Height, croppedImage.GetConfig());
            Canvas canvas = new Canvas(resultingImage);

            float[] src = new float[] { (float)points[4], (float)points[5], (float)points[6], (float)points[7], (float)points[0], (float)points[1], (float)points[2], (float)points[3] };
            float[] dst = new float[] { croppedImage.Width, croppedImage.Height, 0, croppedImage.Height, 0, 0, croppedImage.Width, 0 };
            matrix.SetPolyToPoly(src, 0, dst, 0, 4);
            canvas.Matrix = matrix;
            canvas.DrawBitmap(croppedImage, 0, 0, null);

            return resultingImage;
        }

        private Bitmap CropImage(Bitmap source, Android.Graphics.Point lefttoppoint, Android.Graphics.Point righttoppoint, Android.Graphics.Point rightbottompoint, Android.Graphics.Point leftbottompoint)
        {
            Bitmap bitmap = Bitmap.CreateBitmap(source);

            Bitmap resultingImage = Bitmap.CreateBitmap(source.Width, source.Height, bitmap.GetConfig());
            //Bitmap resultingImage = bitmap.Copy(Bitmap.Config.Argb8888, true);
            Canvas canvas = new Canvas(resultingImage);
            Paint paint = new Paint();

            paint.AntiAlias = true;
            Android.Graphics.Path path = new Android.Graphics.Path();
            path.MoveTo(lefttoppoint.X, lefttoppoint.Y);
            path.LineTo(righttoppoint.X, righttoppoint.Y);
            path.LineTo(rightbottompoint.X, rightbottompoint.Y);
            path.LineTo(leftbottompoint.X, leftbottompoint.Y);
            path.LineTo(lefttoppoint.X, lefttoppoint.Y);

            canvas.DrawPath(path, paint);

            paint.SetXfermode(new PorterDuffXfermode(PorterDuff.Mode.SrcIn));
            canvas.DrawBitmap(source, 0, 0, paint);
            return resultingImage;
        }

        #endregion

        #region Draw Detectd Rectangle 

        private Bitmap DrawRectangle(Bitmap source, double[] points)
        {
            Bitmap bitmap = Bitmap.CreateBitmap(source);

            Bitmap resultingImage = Bitmap.CreateBitmap(source.Width, source.Height, bitmap.GetConfig());
            //Bitmap resultingImage = bitmap.Copy(Bitmap.Config.Argb8888, true);
            Canvas canvas = new Canvas(resultingImage);
            Paint paint = new Paint();
            paint.Color = Color.Red;
            paint.StrokeWidth = 3;
            paint.SetStyle(Paint.Style.Stroke);
            paint.AntiAlias = true;
            Android.Graphics.Path path = new Android.Graphics.Path();

            path.MoveTo((float)points[0] / scaledRatio, (float)points[1] / scaledRatio);
            path.LineTo((float)points[2] / scaledRatio, (float)points[3] / scaledRatio);
            path.LineTo((float)points[4] / scaledRatio, (float)points[5] / scaledRatio);
            path.LineTo((float)points[6] / scaledRatio, (float)points[7] / scaledRatio);
            path.LineTo((float)points[0] / scaledRatio, (float)points[1] / scaledRatio);

            canvas.DrawPath(path, paint);

            paint.SetXfermode(new PorterDuffXfermode(PorterDuff.Mode.Overlay));
            canvas.DrawBitmap(source, 0, 0, paint);
            return resultingImage;
        }

        #endregion
    }

    public class ViewHolder : Java.Lang.Object
    {
        public ImageView Photo { get; set; }
    }
}