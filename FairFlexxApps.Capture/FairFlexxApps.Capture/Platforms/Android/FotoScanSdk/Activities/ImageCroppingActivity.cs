using System;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;
using Android.Graphics;
using FairFlexxApps.Capture.Droid.FotoScanSdk.Models;
using System.Collections.Generic;
using Newtonsoft.Json;
using FairFlexxApps.Capture.Droid.FotoScanSdk.Utils;
using FairFlexxApps.Capture.Droid.FotoScanSdk.Views;
using Android.Views;
using Android.Runtime;
using Point = Org.Opencv.Core.Point;
using Org.Opencv.Imgproc;
using Org.Opencv.Core;
using Android.Content.PM;

namespace FairFlexxApps.Capture.Droid.FotoScanSdk.Activities
{
    [Activity(Label = "ImageCroppingActivity", ScreenOrientation = ScreenOrientation.Portrait)]
    public class ImageCroppingActivity : Activity
    {
        #region Properties
        
        int scaledRatio;
        Bitmap OriginalImage;
        Bitmap oriBitmap;
        Bitmap CroppedImage;
        string OriginalFilePath;

        public static Stack<PolygonPoints> allDraggedPointsStack = new Stack<PolygonPoints>();
        private PolygonView polygonView;
        private ImageView cropImageView;
        private View cropAcceptBtn;
        private View cropRejectBtn;

        private FrameLayout cropLayout;

        int height;
        int width;
        ImageInfo tappedImage;

        #endregion

        #region OnCreate

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.imagecroppinglayout);

            Init();

            tappedImage = JsonConvert.DeserializeObject<ImageInfo>(Intent.GetStringExtra("TappedImage"));

            OriginalFilePath = tappedImage.ImageOriginalPath;
            OriginalImage = GetBitmap(OriginalFilePath);
            oriBitmap = OriginalImage;

            height = Intent.GetIntExtra("ImageHeight", 0);
            width = Intent.GetIntExtra("ImageWidth", 0);

            DisplayImageWithManualCropRetangle();
        }

        private void Init()
        {
            polygonView = FindViewById<PolygonView>(Resource.Id.polygon_view);
            cropImageView = FindViewById<ImageView>(Resource.Id.crop_image_view);
            cropAcceptBtn = FindViewById<View>(Resource.Id.crop_accept_btn);
            cropRejectBtn = FindViewById<View>(Resource.Id.crop_reject_btn);
            cropLayout = FindViewById<FrameLayout>(Resource.Id.crop_layout);

            cropAcceptBtn.Click += AcceptAcrop;
            cropRejectBtn.Click += RejectCrop;
        }

        #endregion

        #region DisplayImageWithManualCropRetangle

        void DisplayImageWithManualCropRetangle()
        {
            if (OriginalImage != null)
            {
                cropImageView.SetImageBitmap(OriginalImage);
                OriginalImage = ScanUtils.resize(OriginalImage, width, height);
                Mat originalMat = new Mat(OriginalImage.Height, OriginalImage.Width, CvType.Cv8uc1);
                Org.Opencv.Android.Utils.BitmapToMat(OriginalImage, originalMat);
                JavaList<PointF> points;

                try
                {
                    Quadrilateral quad = ScanUtils.DetectLargestQuadrilateral(originalMat);
                    if (null != quad)
                    {
                        double resultArea = Math.Abs(Imgproc.ContourArea(quad.contour));
                        double previewArea = originalMat.Rows() * originalMat.Cols();
                        if (resultArea > previewArea * 0.08 && quad.points[0].X != 0 && quad.points[0].Y != 0)
                        {
                            points = new JavaList<PointF>();
                            points.Add(new PointF((float)quad.points[0].X, (float)quad.points[0].Y));
                            points.Add(new PointF((float)quad.points[1].X, (float)quad.points[1].Y));
                            points.Add(new PointF((float)quad.points[2].X, (float)quad.points[2].Y));
                            points.Add(new PointF((float)quad.points[3].X, (float)quad.points[3].Y));
                        }
                        else
                        {
                            points = ScanUtils.FindPoints(OriginalImage);

                            if (points == null)
                                points = ScanUtils.GetPolygonDefaultPoints(OriginalImage);
                        }
                    }
                    else
                    {
                        points = ScanUtils.FindPoints(OriginalImage);

                        if (points == null)
                            points = ScanUtils.GetPolygonDefaultPoints(OriginalImage);
                    }

                    polygonView.SetPointsV2(points);

                    int padding = (int) Resources.GetDimension(Resource.Dimension.scan_padding);
                    FrameLayout.LayoutParams layoutParams = new FrameLayout.LayoutParams(OriginalImage.Width + 2 * padding, OriginalImage.Height + 2 * padding);
                    layoutParams.Gravity = GravityFlags.Center;
                    polygonView.LayoutParameters = layoutParams;
                }
                catch (Exception e) { }
            }
        }

        #endregion

        #region GetBitmap from file path

        private Bitmap GetBitmap(string imagePath)
        {
            var uri = GetImageUri(imagePath);

            try
            {
                const int imageMaxSize = 1024;
                var ins = ContentResolver.OpenInputStream(uri);

                // Decode image size
                var o = new BitmapFactory.Options { InJustDecodeBounds = true };

                var b1 = BitmapFactory.DecodeStream(ins, null, o);
                ins.Close();

                scaledRatio = 1;
                if (o.OutHeight > imageMaxSize || o.OutWidth > imageMaxSize)
                {
                    //scaledRatio = (int)Math.Pow(2, (int)Math.Round(Math.Log(imageMaxSize / (double)Math.Max(o.OutHeight, o.OutWidth)) / Math.Log(0.5)));
                }

                var o2 = new BitmapFactory.Options { InSampleSize = scaledRatio };
                ins = ContentResolver.OpenInputStream(uri);
                var b = BitmapFactory.DecodeStream(ins, null, o2);
                var h2 = b.Height;
                var w2 = b.Width;
                ins.Close();

                return b;
            }
            catch (Exception e)
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

        #endregion

        #region Save Image

        void SaveImage(object sender, EventArgs args)
        {
            var filePath = ScanUtils.SaveToExternalStorage(CroppedImage);
            var fileOriginalPath = OriginalFilePath;
            ImageInfo resultFilter = new ImageInfo(filePath, fileOriginalPath, null);
            var intent = new Intent();
            intent.PutExtra("ImageCropped", JsonConvert.SerializeObject(resultFilter));
            SetResult(Result.Ok, intent);
            //RecycleBitmap();
            Finish();
        }

        #endregion

        #region Accept crop
        
        public void AcceptAcrop(object sender, EventArgs args)
        {
            try
            {
                var points = polygonView.GetPoints();

                var xRatio = (float)oriBitmap.Width / OriginalImage.Width;
                var yRatio = (float)oriBitmap.Height / OriginalImage.Height;

                if (ScanUtils.IsScanPointsValid(points))
                {
                    Point point1 = new Point(points[0].X * xRatio, points[0].Y * yRatio);
                    Point point2 = new Point(points[1].X * xRatio, points[1].Y * yRatio);
                    Point point3 = new Point(points[2].X * xRatio, points[2].Y * yRatio);
                    Point point4 = new Point(points[3].X * xRatio, points[3].Y * yRatio);
                    CroppedImage = ScanUtils.enhanceReceipt(oriBitmap, point1, point2, point3, point4);
                }
                else
                {
                    CroppedImage = OriginalImage;
                }
            }
            catch(Exception ex)
            {
                var points = polygonView.GetPointsV2();

                if (ScanUtils.IsScanPointsValidV2(points))
                {
                    Point point1 = new Point(points[0].X, points[0].Y);
                    Point point2 = new Point(points[1].X, points[1].Y);
                    Point point3 = new Point(points[2].X, points[2].Y);
                    Point point4 = new Point(points[3].X, points[3].Y);
                    CroppedImage = ScanUtils.enhanceReceipt(OriginalImage, point1, point2, point3, point4);
                }
                else
                {
                    CroppedImage = OriginalImage;
                }
            }

            if (CroppedImage == null)
            {
                ShowNotification();
            }
            else
            {
                var filePath = ScanUtils.SaveToExternalStorage(CroppedImage);
                var fileOriginalPath = OriginalFilePath;
                ImageInfo resultFilter = new ImageInfo(filePath, fileOriginalPath, null);
                var intent = new Intent();
                intent.PutExtra("ImageCropped", JsonConvert.SerializeObject(resultFilter));
                SetResult(Result.Ok, intent);
                RecycleBitmap();
                Finish();
            } 
        }

        #endregion

        #region  Reject crop

        public void RejectCrop(object sender, EventArgs args)
        {
            RecycleBitmap();
            Finish();
        }

        void RecycleBitmap()
        {
            if (CroppedImage != null)
                CroppedImage.Recycle();
            if (OriginalImage != null)
                OriginalImage.Recycle();
        }

        #endregion

        private void ShowNotification()
        {
            AlertDialog.Builder builder = new AlertDialog.Builder(this);
            builder.SetTitle(Resources.GetString(Resource.String.notification));
            builder.SetMessage(Resources.GetString(Resource.String.cannot_crop));
            builder.SetCancelable(false);
            builder.SetPositiveButton(Resources.GetString(Resource.String.ok), new OnClickListener());
            AlertDialog alertDialog = builder.Create();
            alertDialog.Show();
        }
        public class OnClickListener : Java.Lang.Object, IDialogInterfaceOnClickListener
        {
            public void OnClick(IDialogInterface dialog, int which)
            {

            }
        }
    }
}