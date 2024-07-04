using System;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables.Shapes;
using Android.Graphics.Pdf;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Org.Opencv.Core;
using Org.Opencv.Imgproc;
using Path = Android.Graphics.Path;
using Paint = Android.Graphics.Paint;
using Android.Media;
using Android.Util;
using Bitmap = Android.Graphics.Bitmap;
using Matrix = Android.Graphics.Matrix;
using Color = Android.Graphics.Color;
using Org.Opencv.Android;
using FairFlexxApps.Capture.Droid.FotoScanSdk.Interfaces;
using FairFlexxApps.Capture.Droid.FotoScanSdk.Utils;
using FairFlexxApps.Capture.Droid.FotoScanSdk.Enums;
using FairFlexxApps.Capture.Droid.FotoScanSdk.Constants;
using Java.IO;
using static Android.Hardware.Camera;
using static Android.Hardware.Camera.Parameters;
using Camera = Android.Hardware.Camera;
using File = System.IO.File;
using IOException = System.IO.IOException;
using Point = Org.Opencv.Core.Point;
using Size = Org.Opencv.Core.Size;

namespace FairFlexxApps.Capture.Droid.FotoScanSdk.Views
{
    public class ScanSurfaceView : FrameLayout, ISurfaceHolderCallback,
        IPreviewCallback,
        IShutterCallback,
        IPictureCallback
    {
        SurfaceView mSurfaceView;
        private ScanCanvasView scanCanvasView;
        private int vWidth = 0;
        private int vHeight = 0;

        private Context context;
        private Camera camera;
        private static bool safeToTakePicture;
        private IScanner iScanner;
        private CountDownTimer autoCaptureTimer;
        private static int secondsLeft;
        private bool isAutoCaptureScheduled;
        public bool isAutoMode = true;
        private Camera.Size previewSize;
        private static bool isCapturing = false;
        private bool _isFlashOn;

        public static Mat yuv, mat;
        public static BaseLoaderCallback mLoaderCallback;



        public ScanSurfaceView(Context context, IScanner iScanner) : base(context)
        {
            mSurfaceView = new SurfaceView(context);
            AddView(mSurfaceView);
            this.context = context;
            this.scanCanvasView = new ScanCanvasView(context);
            AddView(scanCanvasView);
            var surfaceHolder = mSurfaceView.Holder;
            surfaceHolder.AddCallback(this);
            this.iScanner = iScanner;
            safeToTakePicture = false;
            mLoaderCallback = new Callback(this, context);
        }

        public void SurfaceCreated(ISurfaceHolder holder)
        {
            try
            {
                RequestLayout();
                OpenCamera();
                camera.SetPreviewDisplay(holder);
                SetPreviewCallback();
            }
            catch (IOException e)
            {
            }
        }

        public void SurfaceChanged(ISurfaceHolder holder, [GeneratedEnum] Android.Graphics.Format format, int width, int height)
        {
            if (vWidth == vHeight)
            {
                return;
            }
            if (previewSize == null)
            {
                //previewSize = ScanUtils.getOptimalPreviewSize(camera, vWidth, vHeight);
                previewSize = camera.GetParameters().PreviewSize;
            }

            Parameters parameters = camera.GetParameters();
            camera.SetDisplayOrientation(ScanUtils.ConfigureCameraAngle((Activity)context));
            parameters.SetPreviewSize(previewSize.Width, previewSize.Height);
            if (parameters.SupportedFocusModes != null
                    && parameters.SupportedFocusModes.Contains(FocusModeContinuousPicture))
            {
                parameters.FocusMode = FocusModeContinuousPicture;
            }
            else if (parameters.SupportedFocusModes != null
                  && parameters.SupportedFocusModes.Contains(FocusModeAuto))
            {
                parameters.FocusMode = FocusModeAuto;
            }

            Camera.Size size = ScanUtils.DeterminePictureSize(camera, parameters.PreviewSize);
            parameters.SetPictureSize(size.Width, size.Height);
            parameters.PictureFormat = Android.Graphics.ImageFormat.Jpeg;

            camera.SetParameters(parameters);
            RequestLayout();
            SetPreviewCallback();
        }
        public void SurfaceDestroyed(ISurfaceHolder holder)
        {
            StopPreviewAndFreeCamera();
        }

        private void StopPreviewAndFreeCamera()
        {
            if (camera != null)
            {
                // Call stopPreview() to stop updating the preview surface.
                camera.StopPreview();
                camera.SetPreviewCallback(null);
                // Important: Call release() to release the camera for use by other
                // applications. Applications should release the camera immediately
                // during onPause() and re-open() it during onResume()).
                camera.Release();
                camera = null;
            }
        }

        private void OpenCamera()
        {
            if (camera == null)
            {
                CameraInfo info = new CameraInfo();
                int defaultCameraId = 0;
                for (int i = 0; i < NumberOfCameras; i++)
                {
                    GetCameraInfo(i, info);
                    if (info.Facing == CameraInfo.CameraFacingBack)
                    {
                        defaultCameraId = i;
                    }
                }
                camera = Open(defaultCameraId);
                Parameters cameraParams = camera.GetParameters();

                var flashModes = cameraParams.SupportedFlashModes;
                if (flashModes != null && flashModes.Contains(FlashModeAuto))
                {
                    cameraParams.FlashMode = FlashModeAuto;
                }

                camera.SetParameters(cameraParams);
            }
        }

        public void TurnOnFlashlight()
        {
            if (!_isFlashOn)
            {
                camera.Reconnect();
                var cameraParams = camera.GetParameters();

                var flashModes = cameraParams.SupportedFlashModes;
                if (flashModes != null/* && flashModes.Contains(Camera.Parameters.FlashModeAuto)*/)
                {
                    cameraParams.FlashMode = FlashModeTorch;
                }

                camera.SetParameters(cameraParams);
                camera.StartPreview();
                _isFlashOn = true;
            }
        }

        public void TurnOffFlashlight()
        {
            if (_isFlashOn)
            {
                camera.Reconnect();
                var cameraParams = camera.GetParameters();

                var flashModes = cameraParams.SupportedFlashModes;
                if (flashModes != null/* && flashModes.Contains(Camera.Parameters.FlashModeAuto)*/)
                {
                    cameraParams.FlashMode = FlashModeOff;
                }

                camera.SetParameters(cameraParams);
                camera.StartPreview();
                _isFlashOn = false;
            }
        }

        public void SetPreviewCallback()
        {
            camera.StartPreview();
            camera.SetPreviewCallback(this);
        }

        public void StopPreview()
        {
            this.camera.StopPreview();
        }

        public void OnPreviewFrame(byte[] data, Camera camera)
        {
            if (null != camera)
            {
                try
                {
                    Camera.Size pictureSize = camera.GetParameters().PreviewSize;

                    yuv = new Mat(new Size(pictureSize.Width, pictureSize.Height * 1.5), CvType.Cv8uc1);
                    yuv.Put(0, 0, data);

                    mat = new Mat(new Size(pictureSize.Width, pictureSize.Height), CvType.Cv8uc4);
                    Imgproc.CvtColor(yuv, mat, Imgproc.ColorYuv2bgrNv21, 4);
                    yuv.Release();

                    Size originalPreviewSize = mat.Size();
                    int originalPreviewArea = mat.Rows() * mat.Cols();

                    Quadrilateral largestQuad = ScanUtils.DetectLargestQuadrilateral(mat);
                    ClearAndInvalidateCanvas();

                    mat.Release();

                    if (null != largestQuad)
                    {
                        DrawLargestRect(largestQuad.contour, largestQuad.points, originalPreviewSize, originalPreviewArea);
                    }
                    else
                    {
                        ShowFindingReceiptHint();
                    }
                }
                catch (Exception e)
                {
                    ShowFindingReceiptHint();
                }
            }
        }

        private void DrawLargestRect(MatOfPoint2f approx, Point[] points, Size stdSize, int previewArea)
        {
            Path path = new Path();
            // ATTENTION: axis are swapped
            float previewWidth = (float)stdSize.Height;
            float previewHeight = (float)stdSize.Width;

            //Points are drawn in anticlockwise direction
            path.MoveTo(previewWidth - (float)points[0].Y, (float)points[0].X);
            path.LineTo(previewWidth - (float)points[1].Y, (float)points[1].X);
            path.LineTo(previewWidth - (float)points[2].Y, (float)points[2].X);
            path.LineTo(previewWidth - (float)points[3].Y, (float)points[3].X);
            path.Close();

            double area = Math.Abs(Imgproc.ContourArea(approx));

            PathShape newBox = new PathShape(path, previewWidth, previewHeight);
            Paint paint = new Paint();
            Paint border = new Paint();

            //Height calculated on Y axis
            double resultHeight = points[1].X - points[0].X;
            double bottomHeight = points[2].X - points[3].X;
            if (bottomHeight > resultHeight)
                resultHeight = bottomHeight;

            //Width calculated on X axis
            double resultWidth = points[3].Y - points[0].Y;
            double bottomWidth = points[2].Y - points[1].Y;
            if (bottomWidth > resultWidth)
                resultWidth = bottomWidth;

            ImageDetectionProperties imgDetectionPropsObj
                    = new ImageDetectionProperties(previewWidth, previewHeight, resultWidth, resultHeight,
                    previewArea, area, points[0], points[1], points[2], points[3]);

            ScanHint scanHint;

            //if (imgDetectionPropsObj.isDetectedAreaBeyondLimits())
            //{
            //    scanHint = ScanHint.FIND_RECT;
            //    CancelAutoCapture();
            //}
            //else 
            if (imgDetectionPropsObj.isDetectedAreaBelowLimits())
            {
                CancelAutoCapture();
                if (imgDetectionPropsObj.isEdgeTouching())
                {
                    scanHint = ScanHint.MOVE_AWAY;
                }
                else
                {
                    scanHint = ScanHint.MOVE_CLOSER;
                }
            }
            //else if (imgDetectionPropsObj.isDetectedHeightAboveLimit())
            //{
            //    CancelAutoCapture();
            //    scanHint = ScanHint.MOVE_AWAY;
            //}
            //else if (imgDetectionPropsObj.isDetectedWidthAboveLimit() || imgDetectionPropsObj.isDetectedAreaAboveLimit())
            //{
            //    CancelAutoCapture();
            //    scanHint = ScanHint.MOVE_AWAY;
            //}
            else
            {
                //if (imgDetectionPropsObj.isEdgeTouching())
                //{
                //    CancelAutoCapture();
                //    scanHint = ScanHint.MOVE_AWAY;
                //}
                //else if (imgDetectionPropsObj.isAngleNotCorrect(approx))
                //{
                //    CancelAutoCapture();
                //    scanHint = ScanHint.ADJUST_ANGLE;
                //}
                //else
                //{
                scanHint = ScanHint.CAPTURING_IMAGE;
                ClearAndInvalidateCanvas();

                if (!isAutoCaptureScheduled && isAutoMode)
                {
                    ScheduleAutoCapture(scanHint);
                }
                //}
            }

            border.StrokeWidth = 12;
            iScanner.DisplayHint(scanHint);
            SetPaintAndBorder(scanHint, paint, border);
            scanCanvasView.clear();
            scanCanvasView.AddShape(newBox, paint, border);
            InvalidateCanvas();
        }

        private void ScheduleAutoCapture(ScanHint scanHint)
        {
            isAutoCaptureScheduled = true;
            secondsLeft = 0;

            _timer = new System.Timers.Timer();
            _timer.Interval = 1000;
            _timer.Elapsed += (s, e) =>
            {
                _countSeconds--;

                if (_countSeconds == 0)
                {
                    _timer.Stop();
                    isAutoCaptureScheduled = false;
                    AutoCapture(scanHint);
                }
            };
            _countSeconds = 2;
            _timer.Enabled = true;
        }

        private System.Timers.Timer _timer;
        private int _countSeconds;

        public class CountDown : CountDownTimer
        {
            private ScanHint scanHint;
            public CountDown(long totaltime, long interval, ScanHint scanHint)
                : base(totaltime, interval)
            {
                this.scanHint = scanHint;
            }

            public override void OnTick(long millisUntilFinished)
            {
                if (Java.Lang.Math.Round((float)millisUntilFinished / 1000.0f) != secondsLeft)
                {
                    secondsLeft = Java.Lang.Math.Round((float)millisUntilFinished / 1000.0f);
                }

                switch (secondsLeft)
                {
                    case 1:
                        //AutoCapture(scanHint);
                        break;
                    default:
                        break;
                }
            }

            public override void OnFinish()
            {
                //isAutoCaptureScheduled = false;
            }
        }

        private void AutoCapture(ScanHint scanHint)
        {
            if (isCapturing) return;
            if (ScanHint.CAPTURING_IMAGE.Equals(scanHint))
            {
                try
                {
                    
                    isCapturing = true;
                    //iScanner.DisplayHint(ScanHint.CAPTURING_IMAGE);
                    camera.TakePicture(this, null, this);
                    SetPreviewCallback();
                }
                catch (Exception e)
                {
                    string mess = e.Message;
                }
            }
        }

        public void OnShutter()
        {
            if (context != null)
            {
                AudioManager mAudioManager = (AudioManager)context.GetSystemService(Context.AudioService);
                if (null != mAudioManager)
                    mAudioManager.PlaySoundEffect(SoundEffect.Standard);
            }
        }

        public async Task TakePicture()
        {
            await Task.Run(() =>
            {
                SetPreviewCallback();
                camera.TakePicture(this, null, this);
            });
        }

        public void OnPictureTaken(byte[] data, Camera camera)
        {
            camera.StartPreview();
            iScanner.DisplayHint(ScanHint.NO_MESSAGE);
            ClearAndInvalidateCanvas();

            Bitmap bitmap = ScanUtils.DecodeBitmapFromByteArray(data,
                    ScanConstants.HIGHER_SAMPLING_THRESHOLD, ScanConstants.HIGHER_SAMPLING_THRESHOLD);

            Matrix matrix = new Matrix();
            matrix.PostRotate(90);
            bitmap = Bitmap.CreateBitmap(bitmap, 0, 0, bitmap.Width, bitmap.Height, matrix, true);

            iScanner.OnPictureClickedAsync(bitmap);

            Handler h = new Handler();
            Action myAction = () =>
            {
                isCapturing = false;
            };

            h.PostDelayed(myAction, 1000);
        }

        private void CancelAutoCapture()
        {
            if (isAutoCaptureScheduled)
            {
                isAutoCaptureScheduled = false;
                if (null != autoCaptureTimer)
                {
                    autoCaptureTimer.Cancel();
                }

                _timer.Enabled = false;
                _timer.Close();
            }
        }

        private void ShowFindingReceiptHint()
        {
            iScanner.DisplayHint(ScanHint.FIND_RECT);
            ClearAndInvalidateCanvas();
        }

        private void SetPaintAndBorder(ScanHint scanHint, Paint paint, Paint border)
        {
            switch (scanHint)
            {
                case ScanHint.MOVE_CLOSER:
                case ScanHint.MOVE_AWAY:
                case ScanHint.ADJUST_ANGLE:
                    paint.Color = Color.Argb(30, 255, 38, 0);
                    border.Color = Color.Rgb(255, 38, 0);
                    break;
                case ScanHint.FIND_RECT:
                    paint.Color = Color.Argb(0, 0, 0, 0);
                    border.Color = Color.Argb(0, 0, 0, 0);
                    break;
                case ScanHint.CAPTURING_IMAGE:
                    paint.Color = Color.Argb(30, 38, 216, 76);
                    border.Color = Color.Rgb(38, 216, 76);
                    break;
            }
        }

        public void ClearAndInvalidateCanvas()
        {
            scanCanvasView.clear();
            InvalidateCanvas();
        }

        public void InvalidateCanvas()
        {
            scanCanvasView.Invalidate();
        }

        protected override void OnMeasure(int widthMeasureSpec, int heightMeasureSpec)
        {
            vWidth = ResolveSize(SuggestedMinimumWidth, widthMeasureSpec);
            vHeight = ResolveSize(SuggestedMinimumHeight, heightMeasureSpec);
            SetMeasuredDimension(vWidth, vHeight);
            previewSize = ScanUtils.GetOptimalPreviewSize(camera, vWidth, vHeight);
        }

        protected override void OnLayout(bool changed, int l, int t, int r, int b)
        {
            if (ChildCount > 0)
            {
                int width = r - l;
                int height = b - t;

                int previewWidth = width;
                int previewHeight = height;

                if (previewSize != null)
                {
                    previewWidth = previewSize.Width;
                    previewHeight = previewSize.Height;

                    int displayOrientation = ScanUtils.ConfigureCameraAngle((Activity)context);
                    if (displayOrientation == 90 || displayOrientation == 270)
                    {
                        previewWidth = previewSize.Height;
                        previewHeight = previewSize.Width;
                    }
                }

                int nW;
                int nH;
                int top;
                int left;

                float scale = 1.0f;

                // Center the child SurfaceView within the parent.
                if (width * previewHeight < height * previewWidth)
                {
                    int scaledChildWidth = (int)((previewWidth * height / previewHeight) * scale);
                    nW = (width + scaledChildWidth) / 2;
                    nH = (int)(height * scale);
                    top = 0;
                    left = (width - scaledChildWidth) / 2;
                }
                else
                {
                    int scaledChildHeight = (int)((previewHeight * width / previewWidth) * scale);
                    nW = (int)(width * scale);
                    nH = (height + scaledChildHeight) / 2;
                    top = (height - scaledChildHeight) / 2;
                    left = 0;
                }
                mSurfaceView.Layout(left, top, nW, nH);
                scanCanvasView.Layout(left, top, nW, nH);
            }
        }
    }

    class Callback : BaseLoaderCallback
    {
        private readonly ScanSurfaceView _activity;
        public Callback(ScanSurfaceView activity, Context context)
            : base(context)
        {
            _activity = activity;
        }

        public override void OnManagerConnected(int status)
        {
            switch (status)
            {
                case LoaderCallbackInterface.Success:
                    {
                        ScanSurfaceView.yuv = new Mat();
                        ScanSurfaceView.mat = new Mat();
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