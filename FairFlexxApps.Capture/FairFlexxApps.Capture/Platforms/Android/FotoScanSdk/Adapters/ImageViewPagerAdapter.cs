using System.Collections.Generic;
using Android.Content;
using Android.Runtime;
using Android.Support.V4.View;
using Android.Views;
using FairFlexxApps.Capture.Droid.FotoScanSdk.Models;
using Android.Graphics;
using Org.Opencv.Core;
using FairFlexxApps.Capture.Droid.FotoScanSdk.Utils;
using Org.Opencv.Imgproc;
using Point = Org.Opencv.Core.Point;
using FairFlexxApps.Capture.Droid.FotoScanSdk.Views;
using ImageViews.Photo;
using Android.OS;
using System;
using Java.Lang;

namespace FairFlexxApps.Capture.Droid.FotoScanSdk.Adapters
{
    public class ImageViewPagerAdapter : PagerAdapter
    {
        Context Context;
        private List<ImageInfo> ImageInfos;
        private LayoutInflater inflater;
        public static Bitmap bitmap1, bitmap2, bitmap3, bitmap4, bitmap5;

        public ImageViewPagerAdapter(Context context, List<ImageInfo> imageInfos)
        {
            this.Context = context;
            this.ImageInfos = imageInfos;
            inflater = LayoutInflater.From(context);
        }

        public override int Count => ImageInfos.Count;

        public override bool IsViewFromObject(View view, Java.Lang.Object @object)
        {
            return view == @object;
        }

        public override Java.Lang.ICharSequence GetPageTitleFormatted(int position)
        {
            int imageIndex = position + 1;
            int totalImages = ImageInfos.Count;
            return new Java.Lang.String(imageIndex + "/" + totalImages);
        }
        
        public override Java.Lang.Object InstantiateItem(ViewGroup view, int position)
        {
            var imageView = new PhotoView(Context);

            BitmapWorkerTask task = new BitmapWorkerTask(imageView, Context, position);
            task.Execute(ImageInfos[position]);

            var viewPager = view.JavaCast<ViewPager>();
            viewPager.AddView(imageView);
            return imageView;
        }

        public override void DestroyItem(View container, int position, Java.Lang.Object view)
        {
            var viewPager = container.JavaCast<ViewPager>();
            viewPager.RemoveView(view as View);
        }
    }

    public class BitmapWorkerTask : AsyncTask<ImageInfo, Java.Lang.Void, Bitmap>
    {
        int scaledRatio;
        private PhotoView imageView;
        Context context;
        int position;

        public BitmapWorkerTask(PhotoView imageView,Context context, int position)
        {
            this.imageView = imageView;
            this.context = context;
            this.position = position;
        }

        Bitmap bitmapResult;

        protected override Bitmap RunInBackground(params ImageInfo[] @params)
        {
            var imageInfo = @params[0];

            // ori
            Bitmap originalImage = GetBitmap(imageInfo.ImagePath);
            Bitmap autoCroppedImage = GetAutoCropImage(originalImage, imageInfo.isAlreadyCropped);

            return autoCroppedImage;


            // temp test
            //switch (position)
            //{
            //    case 0:
            //        if (ImageViewPagerAdapter.bitmap1 != null)
            //            bitmapResult = ImageViewPagerAdapter.bitmap1;
            //        else
            //        {
            //            Bitmap originalImage = GetBitmap(imageInfo.ImagePath);
            //            Bitmap autoCroppedImage = GetAutoCropImage(originalImage, imageInfo.isAlreadyCropped);
            //            ImageViewPagerAdapter.bitmap1 = autoCroppedImage;
            //            bitmapResult = autoCroppedImage;
            //        }
            //        break;
            //    case 1:
            //        if (ImageViewPagerAdapter.bitmap2 != null)
            //            bitmapResult = ImageViewPagerAdapter.bitmap2;
            //        else
            //        {
            //            Bitmap originalImage = GetBitmap(imageInfo.ImagePath);
            //            Bitmap autoCroppedImage = GetAutoCropImage(originalImage, imageInfo.isAlreadyCropped);
            //            ImageViewPagerAdapter.bitmap2 = autoCroppedImage;
            //            bitmapResult = autoCroppedImage;
            //        }
            //        break;
            //    case 2:
            //        if (ImageViewPagerAdapter.bitmap3 != null)
            //            bitmapResult = ImageViewPagerAdapter.bitmap3;
            //        else
            //        {
            //            Bitmap originalImage = GetBitmap(imageInfo.ImagePath);
            //            Bitmap autoCroppedImage = GetAutoCropImage(originalImage, imageInfo.isAlreadyCropped);
            //            ImageViewPagerAdapter.bitmap3 = autoCroppedImage;
            //            bitmapResult = autoCroppedImage;
            //        }
            //        break;
            //    case 3:
            //        if (ImageViewPagerAdapter.bitmap4 != null)
            //            bitmapResult = ImageViewPagerAdapter.bitmap4;
            //        else
            //        {
            //            Bitmap originalImage = GetBitmap(imageInfo.ImagePath);
            //            Bitmap autoCroppedImage = GetAutoCropImage(originalImage, imageInfo.isAlreadyCropped);
            //            ImageViewPagerAdapter.bitmap4 = autoCroppedImage;
            //            bitmapResult = autoCroppedImage;
            //        }
            //        break;
            //    case 4:
            //        if (ImageViewPagerAdapter.bitmap5 != null)
            //            bitmapResult = ImageViewPagerAdapter.bitmap5;
            //        else
            //        {
            //            Bitmap originalImage = GetBitmap(imageInfo.ImagePath);
            //            Bitmap autoCroppedImage = GetAutoCropImage(originalImage, imageInfo.isAlreadyCropped);
            //            ImageViewPagerAdapter.bitmap5 = autoCroppedImage;
            //            bitmapResult = autoCroppedImage;
            //        }
            //        break;
            //}

            //return bitmapResult;
        }
        
        protected override void OnPostExecute(Bitmap bitmap)
        {
            if (bitmap != null)
            {
                if (imageView != null)
                {
                    imageView.SetImageBitmap(bitmap);
                    bitmap.Dispose();
                    GC.Collect();
                }
            }
        }

        public Bitmap GetBitmap(string imagePath)
        {
            var uri = GetImageUri(imagePath);

            try
            {
                const int imageMaxSize = 1024;
                var ins = context.ContentResolver.OpenInputStream(uri);

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
                ins = context.ContentResolver.OpenInputStream(uri);
                var imagebitmap = BitmapFactory.DecodeStream(ins, null, option);
                ins.Close();

                return imagebitmap;
            }
            catch (System.Exception e)
            {
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

        public Bitmap GetAutoCropImage(Bitmap originalImage, bool isAlreadyCropped)
        {
            if (originalImage != null)
            {
                if (!isAlreadyCropped)
                {
                    Mat originalMat = new Mat(originalImage.Height, originalImage.Width, CvType.Cv8uc1);
                    Org.Opencv.Android.Utils.BitmapToMat(originalImage, originalMat);
                    JavaList<PointF> points;
                    Bitmap croppedBitmap;
                    Dictionary<int, PointF> pointFs = new Dictionary<int, PointF>();

                    try
                    {
                        Quadrilateral quad = ScanUtils.DetectLargestQuadrilateral(originalMat);

                        if (null != quad)
                        {
                            double resultArea = System.Math.Abs(Imgproc.ContourArea(quad.contour));
                            double previewArea = originalMat.Rows() * originalMat.Cols();

                            if (resultArea > previewArea * 0.08 && quad.points[0].X != 0 && quad.points[0].Y != 0)
                            {
                                croppedBitmap = CropImage(originalImage, quad);
                            }
                            else
                            {
                                croppedBitmap = originalImage;
                            }
                        }
                        else
                        {
                            croppedBitmap = originalImage;
                        }

                        return croppedBitmap;
                    }
                    catch (System.Exception e)
                    {

                    }
                }
            }

            return originalImage;
        }

        private Bitmap CropImage(Bitmap originalImage, Quadrilateral quad)
        {
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
    }
}