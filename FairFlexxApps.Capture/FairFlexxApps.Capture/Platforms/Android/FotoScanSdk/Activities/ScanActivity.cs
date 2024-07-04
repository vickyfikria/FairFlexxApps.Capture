using System;
using System.Collections.Generic;
using System.IO;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Graphics;
using Android.Graphics.Pdf;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using FairFlexxApps.Capture.Droid.FotoScanSdk.Enums;
using FairFlexxApps.Capture.Droid.FotoScanSdk.Interfaces;
using FairFlexxApps.Capture.Droid.FotoScanSdk.Models;
using FairFlexxApps.Capture.Droid.FotoScanSdk.Utils;
using FairFlexxApps.Capture.Droid.FotoScanSdk.Views;
using Java.IO;
using Newtonsoft.Json;
using Org.Opencv.Android;
using Org.Opencv.Core;
using File = Java.IO.File;

namespace FairFlexxApps.Capture.Droid.FotoScanSdk.Activities
{
    [Activity(Label = "ScanActivity" , ScreenOrientation = ScreenOrientation.Portrait)]
    public class ScanActivity : Activity, IScanner
    {
        private ViewGroup containerScan;
        private FrameLayout cameraPreviewLayout;
        private ScanSurfaceView mImageSurfaceView;
        private TextView captureHintText;
        private LinearLayout captureHintLayout;

        public static Stack<PolygonPoints> allDraggedPointsStack = new Stack<PolygonPoints>();
        private Bitmap copyBitmap;

        private Switch _switchButtonMultiPages;
        private Switch _switchButtonFlashlight;
        private Switch _switchButtonAutomatic;
        private TextView _pageLabel;

        List<ImageInfo> imageTaken;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_scan);
            RequestedOrientation = Android.Content.PM.ScreenOrientation.Portrait;

            //int uiOptions = (int)Window.DecorView.SystemUiVisibility;
            //uiOptions |= (int)SystemUiFlags.LowProfile;
            //uiOptions |= (int)SystemUiFlags.Fullscreen;
            //uiOptions |= (int)SystemUiFlags.HideNavigation;
            //uiOptions |= (int)SystemUiFlags.ImmersiveSticky;
            //Window.DecorView.SystemUiVisibility = (StatusBarVisibility)uiOptions;

            Init();
        }

        protected override void OnResume()
        {
            base.OnResume();
            if (!OpenCVLoader.InitDebug())
            {
                Toast.MakeText(ApplicationContext, "There is a problem", ToastLength.Short).Show();
            }
            else
            {
                ScanSurfaceView.mLoaderCallback.OnManagerConnected(LoaderCallbackInterface.Success);
            }

            ResetParams();
        }

        private void Init()
        {
            containerScan = FindViewById<ViewGroup>(Resource.Id.container_scan);
            cameraPreviewLayout = FindViewById<FrameLayout>(Resource.Id.camera_preview);
            captureHintLayout = FindViewById<LinearLayout>(Resource.Id.capture_hint_layout);
            captureHintText = FindViewById<TextView>(Resource.Id.capture_hint_text);

            RunOnUiThread(() =>
            {
                mImageSurfaceView = new ScanSurfaceView(this, this);
                cameraPreviewLayout.AddView(mImageSurfaceView);
            });

            FindViewById<ImageButton>(Resource.Id.btnTakePicture).Click += TakePicture_Click;
            FindViewById<TextView>(Resource.Id.cancelLabel).Click += (s, e) => Finish();

            _switchButtonMultiPages = FindViewById<Switch>(Resource.Id.switchMultiPages);
            _switchButtonFlashlight = FindViewById<Switch>(Resource.Id.switchFlashlight);
            _switchButtonAutomatic = FindViewById<Switch>(Resource.Id.switchAutomatic);
            _pageLabel = FindViewById<TextView>(Resource.Id.pageLabel);

            ResetParams();

            _pageLabel.Click += Page_Click;
            _switchButtonMultiPages.CheckedChange += MultiPages_CheckedChange;
            _switchButtonFlashlight.CheckedChange += Flashlight_CheckedChange;
            _switchButtonAutomatic.CheckedChange += Automatic_CheckedChange;
        }

        private void Automatic_CheckedChange(object sender, CompoundButton.CheckedChangeEventArgs e)
        {
            if(e.IsChecked)
            {
                mImageSurfaceView.isAutoMode = true;
            }
            else
            {
                mImageSurfaceView.isAutoMode = false;
            }
        }

        private void ResetParams()
        {
            imageTaken = new List<ImageInfo>();
            _switchButtonMultiPages.Checked = false;
            _switchButtonFlashlight.Checked = false;
            _switchButtonAutomatic.Checked = true;
        }

        private void Flashlight_CheckedChange(object sender, CompoundButton.CheckedChangeEventArgs e)
        {
            if (e.IsChecked)
            {
                mImageSurfaceView.TurnOnFlashlight();
            }
            else
            {
                mImageSurfaceView.TurnOffFlashlight();
            }
        }

        private async void TakePicture_Click(object sender, EventArgs e)
        {
            await mImageSurfaceView.TakePicture();
        }

        private void Page_Click(object sender, EventArgs e)
        {
            if (imageTaken.Count > 0)
                GotoCroppingImageAcctivity(imageTaken);
            else
                Toast.MakeText(this, "Please take picture", ToastLength.Short).Show();
        }

        private void MultiPages_CheckedChange(object sender, CompoundButton.CheckedChangeEventArgs e)
        {
            if (e.IsChecked)
            {
                _pageLabel.Visibility = ViewStates.Visible;
                _pageLabel.Text = imageTaken.Count <= 1 ? $"{imageTaken.Count} Page" : $"{imageTaken.Count} Pages";
            }
            else
                _pageLabel.Visibility = ViewStates.Invisible;
        }

        public void DisplayHint(ScanHint scanHint)
        {
            captureHintLayout.Visibility = ViewStates.Visible;

            switch (scanHint)
            {
                case ScanHint.MOVE_CLOSER:
                    captureHintText.Text = "Move closer";
                    captureHintLayout.Background = Resources.GetDrawable(Resource.Drawable.hint_red);
                    break;
                case ScanHint.MOVE_AWAY:
                    captureHintText.Text = "Move away";
                    captureHintLayout.Background = Resources.GetDrawable(Resource.Drawable.hint_red);
                    break;
                case ScanHint.ADJUST_ANGLE:
                    captureHintText.Text = "Adjust angle";
                    captureHintLayout.Background = Resources.GetDrawable(Resource.Drawable.hint_red);
                    break;
                case ScanHint.FIND_RECT:
                    captureHintText.Text = "Use darker background";
                    captureHintLayout.Background = Resources.GetDrawable(Resource.Drawable.hint_white);
                    break;
                case ScanHint.CAPTURING_IMAGE:
                    captureHintText.Text = "Hold still";
                    captureHintLayout.Background = Resources.GetDrawable(Resource.Drawable.hint_green);
                    break;
                case ScanHint.NO_MESSAGE:
                    captureHintLayout.Visibility = ViewStates.Gone;
                    break;
                default:
                    break;
            }
        }

        private int height;
        private int width;
        public void OnPictureClickedAsync(Bitmap bitmap)
        {
            try
            {
                copyBitmap = bitmap.Copy(Bitmap.Config.Argb8888, true);

                height = Window.FindViewById(Window.IdAndroidContent).Height;
                width = Window.FindViewById(Window.IdAndroidContent).Width; 
                 copyBitmap = ScanUtils.ResizeToScreenContentSize(copyBitmap, width, height);
                Mat originalMat = new Mat(copyBitmap.Height, copyBitmap.Width, CvType.Cv8uc1);
                Org.Opencv.Android.Utils.BitmapToMat(copyBitmap, originalMat);
                
                // Navigate to preview
                if (!_switchButtonMultiPages.Checked)
                {
                    var filePath = ScanUtils.SaveToExternalStorage(copyBitmap);
                    var fileOriginalPath = filePath;
                    List<ImageInfo> imageTaken = new List<ImageInfo>();
                    imageTaken.Add(new ImageInfo(filePath, fileOriginalPath));
                    GotoCroppingImageAcctivity(imageTaken);
                }
                else
                {
                    var filePath = ScanUtils.SaveToExternalStorage(copyBitmap);
                    var fileOriginalPath = filePath;
                    imageTaken.Add(new ImageInfo(filePath, fileOriginalPath));
                    _pageLabel.Text = imageTaken.Count <= 1 ? $"{imageTaken.Count} Page" : $"{imageTaken.Count} Pages";
                    mImageSurfaceView.SetPreviewCallback();
                }
            }
            catch (Exception e) { }
        }

        private void GotoCroppingImageAcctivity(List<ImageInfo> imageInfo)
        {
            var activity = new Intent(this, typeof(ImagePreviewActivity));
            activity.PutExtra("ListImage", JsonConvert.SerializeObject(imageInfo));
            activity.PutExtra("ImageHeight", height);
            activity.PutExtra("ImageWidth", width);
            StartActivity(activity);
        }
    }
}