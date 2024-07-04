using Android.Content;
using Android.Graphics;
using Android.OS;
using Java.Lang;
using Org.Opencv.Core;
using Camera = Android.Hardware.Camera;
using IPreviewCallback = Android.Hardware.Camera.IPreviewCallback;
using Android.Util;
using Debug = System.Diagnostics.Debug;
using Exception = System.Exception;
using static Android.App.ActionBar;
using System.Threading.Tasks;
using Org.Opencv.Imgproc;
using System.Runtime.CompilerServices;
using Java.Lang.Reflect;
using static Android.Hardware.Camera;
using static Android.Hardware.Camera.Parameters;
using FairFlexxApps.Capture.Droid.FotoScanSdk.Activities;
using System.Collections.Generic;
using System.Linq;

namespace FairFlexxApps.Capture.Droid.FotoScanSdk.OpenCVCustom
{
    public class JavaCameraViewCustom : CameraBridgeViewBaseCustom, IPreviewCallback
    {
        private static int MAGIC_TEXTURE_ID = 10;

        private byte[] mBuffer;
        private static Mat[] mFrameChain;
        private static int mChainIdx = 0;
        private Thread mThread;
        private static bool mStopThread;

        protected Camera mCamera;
        private static JavaCameraFrame[] mCameraFrame;
        private SurfaceTexture mSurfaceTexture;
        private static int mPreviewFormat = (int)ImageFormatType.Nv21;
        private static bool _isFlashOn = false;

        public JavaCameraViewCustom(Context context, int cameraId) : base(context, cameraId)
        {
        }

        public JavaCameraViewCustom(Context context, IAttributeSet attrs) : base(context, attrs)
        {
        }

        protected bool InitializeCamera(int width, int height)
        {
            Debug.WriteLine("Initialize camera");
            bool result = true;

            mCamera = null;

            if (mCameraIndex == CAMERA_ID_ANY)
            {
                try
                {
                    mCamera = Open();
                    mCamera.SetDisplayOrientation(90);
                    mCamera.SetPreviewDisplay(Holder);
                }
                catch (Exception e) { }
                if (mCamera == null && Build.VERSION.SdkInt >= Build.VERSION_CODES.Gingerbread)
                {
                    bool connected = false;
                    for (int camIdx = 0; camIdx < NumberOfCameras; ++camIdx)
                    {
                        try
                        {
                            mCamera = Open(camIdx);
                            connected = true;
                            mCamera.SetDisplayOrientation(90);
                            mCamera.SetPreviewDisplay(Holder);
                        }
                        catch (RuntimeException e) { }
                        if (connected) break;
                    }
                }
            }
            else
            {
                if (Build.VERSION.SdkInt >= Build.VERSION_CODES.Gingerbread)
                {
                    int localCameraIndex = mCameraIndex;
                    if (mCameraIndex == CAMERA_ID_BACK)
                    {
                        CameraInfo cameraInfo = new CameraInfo();
                        for (int camIdx = 0; camIdx < NumberOfCameras; ++camIdx)
                        {
                            GetCameraInfo(camIdx, cameraInfo);
                            if (cameraInfo.Facing == CameraInfo.CameraFacingBack)
                            {
                                localCameraIndex = camIdx;
                                break;
                            }
                        }
                    }

                    if (localCameraIndex != CAMERA_ID_BACK)
                    {
                        try
                        {
                            mCamera = Open(localCameraIndex);
                        }
                        catch (RuntimeException e) { }
                    }
                }
            }

            if (mCamera == null)
                return false;

            try
            {
                Parameters param = mCamera.GetParameters();
                var defaultCameraRatio = (float)param.PictureSize.Width / (float)param.PictureSize.Height;
                var sizes = param.SupportedPreviewSizes.OrderBy(s => s.Width).ToList();
                var pictures = param.SupportedPictureSizes.OrderBy(p => p.Height).ToList();
                Org.Opencv.Core.Size picture = GetPreferredPictureSize(pictures, defaultCameraRatio);

                if (sizes != null)
                {
                    //var list = new List<object>(sizes);
                    var frameSize = CalculateCameraFrameSize(sizes);

                    if (Build.Fingerprint.StartsWith("generic")
                            || Build.Fingerprint.StartsWith("unknown")
                            || Build.Model.Contains("google_sdk")
                            || Build.Model.Contains("Emulator")
                            || Build.Model.Contains("Android SDK built for x86")
                            || Build.Manufacturer.Contains("Genymotion")
                            || (Build.Brand.StartsWith("generic") && Build.Device.StartsWith("generic"))
                            || "google_sdk".Equals(Build.Product))
                        param.PreviewFormat = ImageFormatType.Yv12;
                    else
                        param.PreviewFormat = ImageFormatType.Nv21;

                    mPreviewFormat = (int)param.PreviewFormat;

                    double ratio = frameSize.Height / frameSize.Width;
                    param.SetPreviewSize((int)frameSize.Width, (int)frameSize.Height);
                    param.SetPictureSize((int)picture.Width, (int)picture.Height);
                    param.PreviewFrameRate = 30;
                    param.SetPreviewFpsRange(30000, 30000);
                    CameraActivity.size = new Org.Opencv.Core.Size(param.PreviewSize.Width, param.PreviewSize.Height);
                    var FocusModes = param.SupportedFocusModes;
                    if (FocusModes != null && FocusModes.Contains(FocusModeContinuousVideo))
                    {
                        param.FocusMode = FocusModeContinuousVideo;
                    }

                    mCamera.SetParameters(param);
                    param = mCamera.GetParameters();

                    mFrameWidth = param.PreviewSize.Width;
                    mFrameHeight = param.PreviewSize.Height;

                    if ((LayoutParameters.Width == LayoutParams.MatchParent) && (LayoutParameters.Height == LayoutParams.MatchParent))
                        mScale = Java.Lang.Math.Max(((float)height) / mFrameHeight, ((float)width) / mFrameWidth);
                    else
                        mScale = 0;

                    if (mFpsMeter != null)
                    {
                        mFpsMeter.SetResolution(mFrameWidth, mFrameHeight);
                    }

                    int size = mFrameWidth * mFrameHeight;
                    size = size * ImageFormat.GetBitsPerPixel(param.PreviewFormat) / 8;
                    mBuffer = new byte[size];

                    mCamera.AddCallbackBuffer(mBuffer);
                    mCamera.SetPreviewCallbackWithBuffer(this);

                    mFrameChain = new Mat[2];
                    mFrameChain[0] = new Mat(mFrameHeight + (mFrameHeight / 2), mFrameWidth, CvType.Cv8uc1);
                    mFrameChain[1] = new Mat(mFrameHeight + (mFrameHeight / 2), mFrameWidth, CvType.Cv8uc1);

                    AllocateCache();

                    mCameraFrame = new JavaCameraFrame[2];
                    mCameraFrame[0] = new JavaCameraFrame(mFrameChain[0], mFrameWidth, mFrameHeight);
                    mCameraFrame[1] = new JavaCameraFrame(mFrameChain[1], mFrameWidth, mFrameHeight);

                    if (Build.VERSION.SdkInt >= Build.VERSION_CODES.Honeycomb)
                    {
                        mCamera.SetPreviewDisplay(Holder);
                        mSurfaceTexture = new SurfaceTexture(MAGIC_TEXTURE_ID);
                        mCamera.SetPreviewTexture(mSurfaceTexture);
                    }
                    else
                        mCamera.SetPreviewDisplay(null);

                    mCamera.StartPreview();
                }
                else
                    result = false;
            }
            catch (Exception e)
            {
                result = false;
            }

            return result;
        }

        public void TurnOffTheFlash()
        {
            if (_isFlashOn)
            {
                if (mCamera != null)
                {
                    mCamera.Reconnect();
                    var cameraParams = mCamera.GetParameters();

                    var flashModes = cameraParams.SupportedFlashModes;
                    if (flashModes != null/* && flashModes.Contains(Camera.Parameters.FlashModeAuto)*/)
                    {
                        cameraParams.FlashMode = FlashModeOff;
                    }

                    mCamera.SetParameters(cameraParams);
                    try
                    {
                        mCamera.StartPreview();
                    }
                    catch
                    {

                    }
                    //mCamera.StopPreview();
                    _isFlashOn = false;
                }
                else
                {
                    _isFlashOn = false;
                    return;
                }
            }
        }

        private Org.Opencv.Core.Size CalculateCameraFrameSize(List<Camera.Size> sizes)
        {
            int widthScreen = CameraActivity.widthScreen;
            int heightScreen = CameraActivity.heightScreen;
            int position = sizes.Count - 1;
            int min = Math.Abs(sizes[sizes.Count - 1].Height - widthScreen);
            for (int i = sizes.Count - 2; i >= 0; i--)
            {
                if (Math.Abs(sizes[i].Height - widthScreen) < min && sizes[i].Height != sizes[i].Width)
                {
                    min = sizes[i].Height - widthScreen;
                    position = i;
                }
                if (Math.Abs(sizes[i].Width - heightScreen) < min && sizes[i].Height != sizes[i].Width)
                {
                    min = sizes[i].Width - heightScreen;
                    position = i;
                }

            }
            float ratio = (float)sizes[position].Width / sizes[position].Height;
            Org.Opencv.Core.Size size = new Org.Opencv.Core.Size(sizes[position].Width, sizes[position].Width / ratio);
            return size;
        }
        public void TurnOnTheFlash()
        {
            if (!_isFlashOn)
            {
                mCamera.Reconnect();
                var cameraParams = mCamera.GetParameters();

                var flashModes = cameraParams.SupportedFlashModes;
                if (flashModes != null/* && flashModes.Contains(Camera.Parameters.FlashModeAuto)*/)
                {
                    cameraParams.FlashMode = FlashModeTorch;
                }

                mCamera.SetParameters(cameraParams);
                try
                {
                    mCamera.StartPreview();
                }
                catch
                {

                }
                _isFlashOn = true;
            }
        }

        protected void setDisplayOrientation(Camera camera, int angle)
        {
            Method downPolymorphic;
            try
            {
                downPolymorphic = camera.Class.GetMethod("setDisplayOrientation", new Class[] { });
                if (downPolymorphic != null)
                    downPolymorphic.Invoke(camera, new Object[] { angle });
            }
            catch (Exception ex) { }
        }

        protected void ReleaseCamera()
        {
            Task.Run(() =>
            {
                if (mCamera != null)
                {
                    mCamera.StopPreview();
                    mCamera.SetPreviewCallback(null);

                    mCamera.Release();
                }
                mCamera = null;
                if (mFrameChain != null)
                {
                    mFrameChain[0].Release();
                    mFrameChain[1].Release();
                }
                if (mCameraFrame != null)
                {
                    mCameraFrame[0].Release();
                    mCameraFrame[1].Release();
                }
            });
        }

        public static bool mCameraFrameReady = false;

        public void OnPreviewFrame(byte[] data, Camera camera)
        {
            Task.Run(() =>
            {
                mFrameChain[mChainIdx].Put(0, 0, data);
                mCameraFrameReady = true;
                //lock (this)
                //{
                //    this.Notify();
                //}

                if (mCamera != null)
                    mCamera.AddCallbackBuffer(mBuffer);
            });
        }

        protected override bool ConnectCamera(int width, int height)
        {
            if (!InitializeCamera(width, height))
                return false;

            mCameraFrameReady = false;

            mStopThread = false;

            mThread = new Thread(async () =>
            {
                await CameraRun();
            });
            mThread.Start();

            //mThread = new Thread(new CameraWorker(this));
            //mThread.Start();

            return true;
        }

        //[MethodImpl(MethodImplOptions.Synchronized)]
        private async Task CameraRun()
        {
            do
            {
                bool hasFrame = false;

                //try
                //{
                //    while (!mCameraFrameReady && !mStopThread)
                //    {
                //        //Context.Wait();
                //        //this.Wait();
                //    }
                //}
                //catch (InterruptedException e)
                //{
                //}

                if (mCameraFrameReady)
                {
                    mChainIdx = 1 - mChainIdx;
                    mCameraFrameReady = false;
                    hasFrame = true;
                }

                if (!mStopThread && hasFrame)
                {
                    if (!mFrameChain[1 - mChainIdx].Empty())
                        DeliverAndDrawFrame(mCameraFrame[1 - mChainIdx]);
                }
            } while (!mStopThread);
        }

        protected override void DisconnectCamera()
        {
            try
            {
                mStopThread = true;
                //this.Notify();

                if (mThread != null)
                    mThread.Join();
            }
            catch (InterruptedException e)
            {
            }
            finally
            {
                mThread = null;
            }

            ReleaseCamera();

            mCameraFrameReady = false;
        }

        public class JavaCameraSizeAccessor : Java.Lang.Object, ListItemAccessor
        {
            public int GetHeight(Java.Lang.Object obj)
            {
                Camera.Size size = (Camera.Size)obj;
                return size.Height;
            }

            public int GetWidth(Java.Lang.Object obj)
            {
                Camera.Size size = (Camera.Size)obj;
                return size.Width;
            }
        }

        private Org.Opencv.Core.Size  GetPreferredPictureSize(List<Camera.Size> pictures, float defaultCameraRatio)
        {
            Org.Opencv.Core.Size res = null;

            foreach (var p in pictures)
            {
                float ratio = (float)p.Width / (float)p.Height;
                if (ratio == defaultCameraRatio && p.Height <= 1920)
                {
                    res = new Org.Opencv.Core.Size(p.Width, p.Height);
                    break;
                }
            }
            return res;
        }

        private class JavaCameraFrame : Java.Lang.Object, CvCameraViewFrame
        {
            public Mat Gray()
            {
                return mYuvFrameData.Submat(0, mHeight, 0, mWidth);
            }

            public Mat Rgba()
            {
                if (mPreviewFormat == (int)ImageFormatType.Nv21)
                    Imgproc.CvtColor(mYuvFrameData, mRgba, Imgproc.ColorYuv2rgbaNv21, 4);
                else if (mPreviewFormat == (int)ImageFormatType.Yv12)
                    Imgproc.CvtColor(mYuvFrameData, mRgba, Imgproc.ColorYuv2rgbI420, 4);

                return mRgba;
            }

            public JavaCameraFrame(Mat Yuv420sp, int width, int height) : base()
            {
                mWidth = width;
                mHeight = height;
                mYuvFrameData = Yuv420sp;
                mRgba = new Mat();
            }

            public void Release()
            {
                mRgba.Release();
            }

            private Mat mYuvFrameData;
            private Mat mRgba;
            private int mWidth;
            private int mHeight;
        }

        private class CameraWorker : Object, IRunnable
        {
            private JavaCameraViewCustom context;
            private bool isLock = true;

            public CameraWorker(JavaCameraViewCustom context)
            {
                this.context = context;
            }

            [MethodImpl(MethodImplOptions.Synchronized)]
            public void Run()
            {
                do
                {
                    bool hasFrame = false;

                    try
                    {
                        while (!mCameraFrameReady && !mStopThread)
                        {
                            if (isLock)
                            {
                                isLock = false;
                                context.Wait();
                                isLock = true;
                            }
                        }
                    }
                    catch (InterruptedException e)
                    {
                    }
                    if (mCameraFrameReady)
                    {
                        mChainIdx = 1 - mChainIdx;
                        mCameraFrameReady = false;
                        hasFrame = true;
                    }

                    if (!mStopThread && hasFrame)
                    {
                        if (!mFrameChain[1 - mChainIdx].Empty())
                            context.DeliverAndDrawFrame(mCameraFrame[1 - mChainIdx]);
                    }
                } while (!mStopThread);
            }
        }
    }
}