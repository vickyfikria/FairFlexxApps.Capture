using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using FairFlexxApps.Capture.Droid.Controls;
using FairFlexxApps.Capture.Droid.FotoScanSdk.Activities;
using FairFlexxApps.Capture.Droid.FotoScanSdk.Adapters;
using FairFlexxApps.Capture.Droid.FotoScanSdk.Constants;
using FairFlexxApps.Capture.Droid.FotoScanSdk.Enums;
using FairFlexxApps.Capture.Droid.FotoScanSdk.Interfaces;
using FairFlexxApps.Capture.Droid.FotoScanSdk.Models;
using FairFlexxApps.Capture.Droid.FotoScanSdk.OpenCVCustom;
using FairFlexxApps.Capture.Droid.FotoScanSdk.Utils;
using Newtonsoft.Json;
using Org.Opencv.Imgproc;
using Org.Opencv.Core;
using Org.Opencv.Android;
using Point = Org.Opencv.Core.Point;
using Switch = Android.Widget.Switch;
using ImageButton = Android.Widget.ImageButton;

namespace FairFlexxApps.Capture.Droid.FotoScanSdk.Activities
{
    [Activity(Label = "CameraActivity", ScreenOrientation = ScreenOrientation.Portrait)]
    public class CameraActivity : Activity, IScanner, CameraBridgeViewBaseCustom.CvCameraViewListener2
    {
        #region Properties

        public static CameraActivity Instance;

        private bool state;
        private ImageButton btnTakePicture;
        private Switch _switchButtonMultiPages;
        private Switch _switchButtonFlashlight;
        private Switch _switchButtonAutomatic;
        private TextView _pageLabel;
        private TextView captureHintText;
        private LinearLayout captureHintLayout;
        SeekBar seekBarCamWhiteBalance;
        ImageView btnResetCamWhiteBalance;
        Android.Hardware.Camera camera;
        CameraControlView mOpenCvCameraView;
        public static Org.Opencv.Core.Size size;
        public static int widthScreen, heightScreen;
        BaseLoaderCallback mLoaderCallback;
        public static Mat bwIMG, hsvIMG, lrrIMG, urrIMG, dsIMG, usIMG, cIMG, hovIMG;
        public static MatOfPoint2f approxCurve;

        bool _isBusy;
        bool _isFlashOn;
        int threshold = 100;
        int height, width;
        List<ImageInfo> imageTaken;

        private bool isAutoCaptureScheduled;
        private System.Timers.Timer timer;
        private int countSeconds;
        private int secondsLeft;
        private bool isCapturing = false;
        private Bitmap copyBitmap;

        #endregion

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Window.AddFlags(WindowManagerFlags.KeepScreenOn);
            Instance = this;
            SetContentView(Resource.Layout.CameraLayout);//   .CameraLayout);

            Init();
        }


        private void Init()
        {
            state = true;


            DisplayMetrics displayMetrics = new DisplayMetrics();
            WindowManager.DefaultDisplay.GetMetrics(displayMetrics);
            widthScreen = displayMetrics.WidthPixels;
            heightScreen = displayMetrics.HeightPixels;
            mOpenCvCameraView = FindViewById<CameraControlView>(Resource.Id.surfaceView);
            mOpenCvCameraView.SetCvCameraViewListener(this);

            //DisplayMetrics displayMetrics = new DisplayMetrics();
            //WindowManager.DefaultDisplay.GetMetrics(displayMetrics);
            //int height = displayMetrics.HeightPixels;
            //int width = displayMetrics.WidthPixels;
            //float scale = (float)height / (float)width;

            // ratio of camera preview size, need to get this params and use it instead of 'scale'

            mLoaderCallback = new Callback(this, this, mOpenCvCameraView);

            btnTakePicture = FindViewById<ImageButton>(Resource.Id.btnTakePicture);
            btnTakePicture.Click += TakePicture_Click;
            FindViewById<TextView>(Resource.Id.cancelLabel).Click += (s, e) => Finish();
            captureHintLayout = FindViewById<LinearLayout>(Resource.Id.capture_hint_layout);
            captureHintText = FindViewById<TextView>(Resource.Id.capture_hint_text);

            _switchButtonMultiPages = FindViewById<Switch>(Resource.Id.switchMultiPages);
            _switchButtonFlashlight = FindViewById<Switch>(Resource.Id.switchFlashlight);
            _switchButtonAutomatic = FindViewById<Switch>(Resource.Id.switchAutomatic);
            seekBarCamWhiteBalance = FindViewById<SeekBar>(Resource.Id.seekBarCameraWhiteBalance);
            _pageLabel = FindViewById<TextView>(Resource.Id.pageLabel);

            seekBarCamWhiteBalance.Progress = 2;
            seekBarCamWhiteBalance.Max = 4;
            seekBarCamWhiteBalance.ProgressChanged += SeekBarCamWhiteBalanceOnProgressChanged;
            btnResetCamWhiteBalance = FindViewById<ImageView>(Resource.Id.btnResetCamWhiteBalance);
            btnResetCamWhiteBalance.Click += BtnResetCamWhiteBalanceOnClick;

            imageTaken = new List<ImageInfo>();
            _switchButtonFlashlight.Checked = false;
            _switchButtonAutomatic.Checked = true;
            isCapturing = false;
            //ResetParams();

            _pageLabel.Click += Page_Click;
            _switchButtonMultiPages.CheckedChange += MultiPages_CheckedChange;
            _switchButtonFlashlight.CheckedChange += Flashlight_CheckedChange;
        }

        private void BtnResetCamWhiteBalanceOnClick(object sender, EventArgs e)
        {
            seekBarCamWhiteBalance.Progress = 2;
        }

        private void SeekBarCamWhiteBalanceOnProgressChanged(object sender, SeekBar.ProgressChangedEventArgs e)
        {
            mOpenCvCameraView.ChangeWhiteBalanceCameraMode(seekBarCamWhiteBalance.Progress);
        }

        private void Flashlight_CheckedChange(object sender, CompoundButton.CheckedChangeEventArgs e)
        {
            if (e.IsChecked)
            {
                mOpenCvCameraView.TurnOnTheFlash();
            }
            else
            {
                mOpenCvCameraView.TurnOffTheFlash();
            }
        }

        private void MultiPages_CheckedChange(object sender, CompoundButton.CheckedChangeEventArgs e)
        {
            if (e.IsChecked)
            {
                _pageLabel.Visibility = ViewStates.Visible;
                _pageLabel.Text = imageTaken.Count <= 1 ?
                    $"{imageTaken.Count} " + Resources.GetString(Resource.String.page) : $"{imageTaken.Count} " + Resources.GetString(Resource.String.pages);
            }
            else
                _pageLabel.Visibility = ViewStates.Invisible;
        }

        private void Page_Click(object sender, EventArgs e)
        {
            if (imageTaken.Count > 0)
                GotoImagePreviewAcctivity(imageTaken);
            else
                Toast.MakeText(this, "Please take picture", ToastLength.Short).Show();
        }

        private async void TakePicture_Click(object sender, EventArgs e)
        {
            try
            {
                captureHintLayout.Visibility = ViewStates.Invisible;
                captureHintText.Visibility = ViewStates.Invisible;
                btnTakePicture.Enabled = false;
                state = false;

                await mOpenCvCameraView.TakePicture();
            }
            catch
            {
                // ignored
            }
        }

        public async void OnPictureClickedAsync(Bitmap bitmap)
        {
            try
            {
                copyBitmap = bitmap.Copy(Bitmap.Config.Argb8888, true);

                height = Window.FindViewById(Android.Views.Window.IdAndroidContent).Height;
                width = Window.FindViewById(Android.Views.Window.IdAndroidContent).Width;
                //copyBitmap = ScanUtils.resize(copyBitmap, width, height);
                //Mat originalMat = new Mat(copyBitmap.Height, copyBitmap.Width, CvType.Cv8uc1);
                //Org.Opencv.Android.Utils.BitmapToMat(copyBitmap, originalMat);

                // Navigate to preview
                if (!_switchButtonMultiPages.Checked)
                {
                    var progressDialog = ProgressDialog.Show(this, null, "Taking photo...", true);
                    await Task.Delay(3000);
                    new System.Threading.Thread(new ThreadStart(
                        delegate
                        { //LOAD METHOD TO GET ACCOUNT INFO 
                            RunOnUiThread(() =>
                            {
                                var filePath = ScanUtils.SaveToExternalStorage(copyBitmap);
                                var fileOriginalPath = filePath;
                                List<ImageInfo> imageTaken = new List<ImageInfo>();
                                imageTaken.Add(new ImageInfo(filePath, fileOriginalPath));
                                GotoImagePreviewAcctivity(imageTaken);
                            });
                            //HIDE PROGRESS DIALOG 
                            RunOnUiThread(() => progressDialog.Hide());
                        })).Start();
                }
                else
                {
                    if(imageTaken.Count < 5)
                    {
                        var progressDialog = ProgressDialog.Show(this, null, Resources.GetString(Resource.String.taking_photo), true);
                        await Task.Delay(3000);
                        new System.Threading.Thread(new ThreadStart(
                            delegate
                            { //LOAD METHOD TO GET ACCOUNT INFO 
                                RunOnUiThread(() =>
                                {
                                    var filePath = ScanUtils.SaveToExternalStorage(copyBitmap);
                                    var fileOriginalPath = filePath;
                                    imageTaken.Add(new ImageInfo(filePath, fileOriginalPath));
                                    _pageLabel.Text = imageTaken.Count <= 1 ?
                                        $"{imageTaken.Count} " + Resources.GetString(Resource.String.page) : $"{imageTaken.Count} " + Resources.GetString(Resource.String.pages);
                                    if (imageTaken.Count == 1)
                                        Toast.MakeText(this,"1 " + Resources.GetString(Resource.String.single_taken), ToastLength.Short).Show();
                                    else
                                        Toast.MakeText(this, imageTaken.Count + " " + Resources.GetString(Resource.String.multi_taken), ToastLength.Short).Show();
                                    ResetState();
                                });
                                //HIDE PROGRESS DIALOG 
                                RunOnUiThread(() => progressDialog.Hide());
                            })).Start();
                    }
                    else
                    {
                        ShowNotification();
                        ResetState();
                    }
                }
            }
            catch (Exception e) { }
        }

        private void ShowNotification()
        {
            AlertDialog.Builder builder = new AlertDialog.Builder(this);
            builder.SetTitle(Resources.GetString(Resource.String.notification));
            builder.SetMessage(Resources.GetString(Resource.String.cannot_take_more_images));
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

        private void GotoImagePreviewAcctivity(List<ImageInfo> imageInfo)
        {
            var intent = new Intent(this, typeof(ImagePreviewActivity));
            intent.PutExtra("ListImage", JsonConvert.SerializeObject(imageInfo));
            intent.PutExtra("ImageHeight", height);
            intent.PutExtra("ImageWidth", width);

            imageTaken = new List<ImageInfo>();

            StartActivityForResult(intent, ScanConstants.RequestCodeAfterSaving);
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            if (requestCode == ScanConstants.RequestCodeAfterSaving)
            {
                if (resultCode == Result.Ok)
                {
                    Finish();
                }
            }
        }

        public Mat OnCameraFrame(CameraBridgeViewBaseCustom.CvCameraViewFrame inputFrame)
        {
            var mGray = inputFrame.Gray();
            var mRgba = inputFrame.Rgba();

            Imgproc.PyrDown(mGray, dsIMG, new Org.Opencv.Core.Size(mGray.Cols() / 2, mGray.Rows() / 2));
            Imgproc.PyrUp(dsIMG, usIMG, mGray.Size());
            Imgproc.Canny(usIMG, bwIMG, 0, threshold);
            Imgproc.Dilate(bwIMG, bwIMG, new Mat(), new Point(-1, 1), 1);

            var contours = new JavaList<MatOfPoint>();

            cIMG = bwIMG.Clone();

            Imgproc.FindContours(cIMG, contours, hovIMG, Imgproc.RetrExternal, Imgproc.ChainApproxSimple);

            double maxVal = 0;
            int maxValIdx = -1;
            for (int contourIdx = 0; contourIdx < contours.Size(); contourIdx++)
            {
                double contourArea = Imgproc.ContourArea(contours[contourIdx]);
                if (maxVal < contourArea && Math.Abs(contourArea) >= 5000)
                {
                    maxVal = contourArea;
                    maxValIdx = contourIdx;
                }
            }

            ScanHint scanHint = ScanHint.FIND_RECT;

            if (maxValIdx != -1)
            {
                MatOfPoint2f dst = new MatOfPoint2f();
                contours[maxValIdx].ConvertTo(dst, CvType.Cv32f);
                var rectRotate = Imgproc.MinAreaRect(dst);
                Point[] points = new Point[4];
                rectRotate.Points(points);
                for (int i = 0; i < 4; ++i)
                    Imgproc.Line(mRgba, points[i], points[(i + 1) % 4], new Scalar(0, 255, 0, 255), 4);

                int previewArea = mRgba.Rows() * mRgba.Cols();
                double resultArea = Math.Abs(Imgproc.ContourArea(contours[maxValIdx]));

                if (resultArea < previewArea * 0.1)
                {
                    CancelAutoCapture();

                    scanHint = ScanHint.MOVE_CLOSER;
                }
                else
                {
                    scanHint = ScanHint.CAPTURING_IMAGE;

                    if (!isAutoCaptureScheduled && _switchButtonAutomatic.Checked)
                    {
                        ScheduleAutoCapture(scanHint);
                    }
                }
            }

            RunOnUiThread(async () =>
            {
                await DisplayHint(scanHint, state);
            });

            return mRgba;
        }

        public async Task DisplayHint(ScanHint scanHint, bool state)
        {
            if (state)
            {
                captureHintLayout.Visibility = ViewStates.Visible;

                switch (scanHint)
                {
                    case ScanHint.MOVE_CLOSER:
                        captureHintText.Text = Resources.GetString(Resource.String.move_closer);
                        captureHintLayout.Background = Resources.GetDrawable(Resource.Drawable.hint_red);
                        break;
                    case ScanHint.MOVE_AWAY:
                        captureHintText.Text = Resources.GetString(Resource.String.move_away);
                        captureHintLayout.Background = Resources.GetDrawable(Resource.Drawable.hint_red);
                        break;
                    case ScanHint.ADJUST_ANGLE:
                        captureHintText.Text = Resources.GetString(Resource.String.adjust_angle);
                        captureHintLayout.Background = Resources.GetDrawable(Resource.Drawable.hint_red);
                        break;
                    case ScanHint.FIND_RECT:
                        captureHintText.Text = Resources.GetString(Resource.String.finding_object);
                        captureHintLayout.Background = Resources.GetDrawable(Resource.Drawable.hint_white);
                        break;
                    case ScanHint.CAPTURING_IMAGE:
                        captureHintText.Text = Resources.GetString(Resource.String.hold_still);
                        captureHintLayout.Background = Resources.GetDrawable(Resource.Drawable.hint_green);
                        break;
                    case ScanHint.NO_MESSAGE:
                        captureHintLayout.Visibility = ViewStates.Gone;
                        break;
                    default:
                        break;
                }
            }
        }

        private void CancelAutoCapture()
        {
            if (isAutoCaptureScheduled)
            {
                isAutoCaptureScheduled = false;

                timer.Enabled = false;
                timer.Close();
            }

            isCapturing = false;
        }

        private void ScheduleAutoCapture(ScanHint scanHint)
        {
            isAutoCaptureScheduled = true;
            secondsLeft = 0;

            timer = new System.Timers.Timer();
            timer.Interval = 1000;
            timer.Elapsed += (s, e) =>
            {
                countSeconds--;

                if (countSeconds == 0)
                {
                    timer.Stop();
                    isAutoCaptureScheduled = false;
                    AutoCapture(scanHint, state);
                }
            };
            countSeconds = 2;
            timer.Enabled = true;
        }

        private async void AutoCapture(ScanHint scanHint, bool state)
        {
            try
            {
                if (state)
                {
                    if (isCapturing) return;
                    if (ScanHint.CAPTURING_IMAGE.Equals(scanHint))
                    {
                        try
                        {
                            isCapturing = true;

                            await mOpenCvCameraView.TakePicture();
                        }
                        catch (Exception e)
                        {
                            string mess = e.Message;
                        }
                    }
                }
            }
            catch
            {
                // ignored
            }
        }

        public void OnCameraViewStarted(int width, int height)
        { 
            var widthRatio = size.Height > widthScreen ?
                (float)size.Height / widthScreen : widthScreen / (float)size.Height;
            var heightRatio = size.Width > heightScreen ?
                (float)size.Width / heightScreen : heightScreen / (float)size.Width;
            //var scaleCamera = widthRatio < heightRatio ? widthRatio : heightRatio;
            //mOpenCvCameraView.ScaleX = 1.2f;
            //mOpenCvCameraView.ScaleY = 1.2f;

            mOpenCvCameraView.ChangeWhiteBalanceCameraMode(seekBarCamWhiteBalance.Progress);
        }

        public void OnCameraViewStopped()
        {
        }

        protected override void OnPause()
        {
            base.OnPause();
            if (mOpenCvCameraView != null)
            {
                mOpenCvCameraView.DisableView();
            }
        }

        protected override void OnResume()
        {
            base.OnResume();
            if (!OpenCVLoader.InitDebug())
            {
                //Toast.MakeText(ApplicationContext, "There is a problem", ToastLength.Short).Show();
            }
            else
            {
                mLoaderCallback.OnManagerConnected(LoaderCallbackInterface.Success);
            }

            ResetParams();
            ResetState();
        }

        private void ResetState()
        {
            captureHintLayout.Visibility = ViewStates.Visible;
            captureHintText.Visibility = ViewStates.Visible;
            btnTakePicture.Enabled = true;
            state = true;
        }

        private void ResetParams()
        {
            //imageTaken = new List<ImageInfo>();
            //_switchButtonMultiPages.Checked = true;
            _switchButtonFlashlight.Checked = false;
            //_switchButtonAutomatic.Checked = true;
            //seekBarCamWhiteBalance.Progress = 3;
            //mOpenCvCameraView.ChangeWhiteBalanceCameraMode(seekBarCamWhiteBalance.Progress);
            isCapturing = false;

            if (_switchButtonMultiPages.Checked)
            {
                _pageLabel.Text = imageTaken.Count <= 1 ?
                    $"{imageTaken.Count} " + Resources.GetString(Resource.String.page) : $"{imageTaken.Count} " + Resources.GetString(Resource.String.pages);
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            if (mOpenCvCameraView != null)
            {
                mOpenCvCameraView.DisableView();
            }
        }

        void IScanner.DisplayHint(ScanHint scanHint)
        {
            throw new NotImplementedException();
        }
    }

    class Callback : BaseLoaderCallback
    {
        private readonly CameraActivity _activity;
        private readonly CameraBridgeViewBaseCustom mOpenCvCameraView;
        public Callback(CameraActivity activity, Context context, CameraBridgeViewBaseCustom view)
            : base(context)
        {
            _activity = activity;
            mOpenCvCameraView = view;
        }

        public override void OnManagerConnected(int status)
        {
            switch (status)
            {
                case LoaderCallbackInterface.Success:
                    {
                        Log.Info("FaceDetect", "OpenCV loaded successfully");

                        CameraActivity.bwIMG = new Mat();
                        CameraActivity.dsIMG = new Mat();
                        CameraActivity.hsvIMG = new Mat();
                        CameraActivity.lrrIMG = new Mat();
                        CameraActivity.urrIMG = new Mat();
                        CameraActivity.usIMG = new Mat();
                        CameraActivity.cIMG = new Mat();
                        CameraActivity.hovIMG = new Mat();
                        CameraActivity.approxCurve = new MatOfPoint2f();

                        mOpenCvCameraView.EnableView();
                    }
                    break;
                default:
                    {
                        base.OnManagerConnected(status);
                    }
                    break;
            }
        }
    }
}
