using Android.Content;
using Android.Graphics;
using Android.OS;
using Java.Lang;
using Org.Opencv.Android;
using Org.Opencv.Core;
using Camera = Android.Hardware.Camera;
using IPreviewCallback = Android.Hardware.Camera.IPreviewCallback;
using Android.Util;
using Debug = System.Diagnostics.Debug;
using Exception = System.Exception;
using static Android.App.ActionBar;
using System.Threading.Tasks;
using Android.Views;
using Org.Opencv.Imgproc;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Java.Lang.Reflect;

namespace FairFlexxApps.Capture.Droid.FotoScanSdk.Controls
{
    public class ScanCameraView : CameraBridgeViewBase, IPreviewCallback
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

        public ScanCameraView(Context context, int cameraId) : base(context, cameraId)
        {
        }

        public ScanCameraView(Context context, IAttributeSet attrs) : base(context, attrs)
        {
        }

        protected bool InitializeCamera(int width, int height)
        {
            Debug.WriteLine("Initialize camera");
            bool result = true;

            mCamera = null;

            if (MCameraIndex == CameraIdAny)
            {
                try
                {
                    mCamera = Camera.Open();
                    mCamera.SetDisplayOrientation(90);
                    //mCamera.SetPreviewDisplay(Holder);
                }
                catch (Exception e) { }
                if (mCamera == null && Build.VERSION.SdkInt >= Build.VERSION_CODES.Gingerbread)
                {
                    bool connected = false;
                    for (int camIdx = 0; camIdx < Camera.NumberOfCameras; ++camIdx)
                    {
                        try
                        {
                            mCamera = Camera.Open(camIdx);
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
                    int localCameraIndex = MCameraIndex;
                    if (MCameraIndex == CameraIdBack)
                    {
                        Camera.CameraInfo cameraInfo = new Camera.CameraInfo();
                        for (int camIdx = 0; camIdx < Camera.NumberOfCameras; ++camIdx)
                        {
                            Camera.GetCameraInfo(camIdx, cameraInfo);
                            if (cameraInfo.Facing == Camera.CameraInfo.CameraFacingBack)
                            {
                                localCameraIndex = camIdx;
                                break;
                            }
                        }
                    }

                    if (localCameraIndex != CameraIdBack)
                    {
                        try
                        {
                            mCamera = Camera.Open(localCameraIndex);
                        }
                        catch (RuntimeException e) { }
                    }
                }
            }

            if (mCamera == null)
                return false;

            try
            {
                Camera.Parameters param = mCamera.GetParameters();
                var sizes = param.SupportedPreviewSizes;

                if (sizes != null)
                {
                    var list = new List<object>(sizes);
                    var frameSize = CalculateCameraFrameSize(list, new JavaCameraSizeAccessor(), width, height);
                    
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

                    param.SetPreviewSize((int)frameSize.Width, (int)frameSize.Height);

                    var FocusModes = param.SupportedFocusModes;
                    if (FocusModes != null && FocusModes.Contains(Camera.Parameters.FocusModeContinuousVideo))
                    {
                        param.FocusMode = Camera.Parameters.FocusModeContinuousVideo;
                    }

                    mCamera.SetParameters(param);
                    param = mCamera.GetParameters();

                    MFrameWidth = param.PreviewSize.Width;
                    MFrameHeight = param.PreviewSize.Height;

                    if ((LayoutParameters.Width == LayoutParams.MatchParent) && (LayoutParameters.Height == LayoutParams.MatchParent))
                        MScale = Java.Lang.Math.Max(((float)height) / MFrameHeight, ((float)width) / MFrameWidth);
                    else
                        MScale = 0;

                    if (MFpsMeter != null)
                    {
                        MFpsMeter.SetResolution(MFrameWidth, MFrameHeight);
                    }

                    int size = MFrameWidth * MFrameHeight;
                    size = size * ImageFormat.GetBitsPerPixel(param.PreviewFormat) / 8;
                    mBuffer = new byte[size];

                    mCamera.AddCallbackBuffer(mBuffer);
                    mCamera.SetPreviewCallbackWithBuffer(this);

                    mFrameChain = new Mat[2];
                    mFrameChain[0] = new Mat(MFrameHeight + (MFrameHeight / 2), MFrameWidth, CvType.Cv8uc1);
                    mFrameChain[1] = new Mat(MFrameHeight + (MFrameHeight / 2), MFrameWidth, CvType.Cv8uc1);

                    AllocateCache();

                    mCameraFrame = new JavaCameraFrame[2];
                    mCameraFrame[0] = new JavaCameraFrame(mFrameChain[0], MFrameWidth, MFrameHeight);
                    mCameraFrame[1] = new JavaCameraFrame(mFrameChain[1], MFrameWidth, MFrameHeight);

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

        protected void setDisplayOrientation(Camera camera, int angle)
        {
            Method downPolymorphic;
            try
            {
                downPolymorphic = camera.Class.GetMethod("setDisplayOrientation", new Class[] { });
                if (downPolymorphic != null)
                    downPolymorphic.Invoke(camera, new Object[] { angle });
            }
            catch(Exception ex) { }
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

            mThread = new Thread(async() =>
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

        public class JavaCameraSizeAccessor : Java.Lang.Object, IListItemAccessor
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

        private class JavaCameraFrame : Java.Lang.Object, ICvCameraViewFrame
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
            private ScanCameraView context;
            private bool isLock = true;

            public CameraWorker(ScanCameraView context)
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