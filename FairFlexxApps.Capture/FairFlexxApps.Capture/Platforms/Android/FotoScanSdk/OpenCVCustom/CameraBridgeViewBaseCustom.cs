using System;
using System.Collections.Generic;
using Android.Content;
using Android.Content.Res;
using Android.Graphics;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Org.Opencv.Android;
using Org.Opencv.Core;
using Size = Org.Opencv.Core.Size;
using Rect = Android.Graphics.Rect;
using System.Threading.Tasks;


namespace FairFlexxApps.Capture.Droid.FotoScanSdk.OpenCVCustom
{
    public abstract class CameraBridgeViewBaseCustom : SurfaceView, ISurfaceHolderCallback
    {
        private static int MAX_UNSPECIFIED = -1;
        private static int STOPPED = 0;
        private static int STARTED = 1;

        private int mState = STOPPED;
        private Bitmap mCacheBitmap;
        private CvCameraViewListener2 mListener;
        private bool mSurfaceExist;

        protected int mFrameWidth;
        protected int mFrameHeight;
        protected int mMaxHeight;
        protected int mMaxWidth;
        protected float mScale = 0;
        protected int mPreviewFormat = RGBA;
        protected int mCameraIndex = CAMERA_ID_ANY;
        protected bool mEnabled;
        protected FpsMeter mFpsMeter = null;

        public static int CAMERA_ID_ANY = -1;
        public static int CAMERA_ID_BACK = 99;
        public static int CAMERA_ID_FRONT = 98;
        public static int RGBA = 1;
        public static int GRAY = 2;

        public CameraBridgeViewBaseCustom(Context context, int cameraId) : base(context)
        {
            mCameraIndex = cameraId;
            Holder.AddCallback(this);
            mMaxWidth = MAX_UNSPECIFIED;
            mMaxHeight = MAX_UNSPECIFIED;
        }

        public CameraBridgeViewBaseCustom(Context context, IAttributeSet attrs) : base(context, attrs)
        {
            TypedArray styledAttrs = Context.ObtainStyledAttributes(attrs, Resource.Styleable.CameraBridgeViewBase);
            if (styledAttrs.GetBoolean(Resource.Styleable.CameraBridgeViewBase_show_fps, false))
                EnableFpsMeter();

            mCameraIndex = styledAttrs.GetInt(Resource.Styleable.CameraBridgeViewBase_camera_id, -1);

            Holder.AddCallback(this);
            mMaxWidth = MAX_UNSPECIFIED;
            mMaxHeight = MAX_UNSPECIFIED;
            styledAttrs.Recycle();
        }

        public interface CvCameraViewListener2
        {
            void OnCameraViewStarted(int width, int height);
            void OnCameraViewStopped();
            Mat OnCameraFrame(CvCameraViewFrame inputFrame);
        }

        public interface CvCameraViewFrame
        {
            Mat Rgba();
            Mat Gray();
        }

        //[MethodImpl(MethodImplOptions.Synchronized)]
        public async void SurfaceChanged(ISurfaceHolder holder, [GeneratedEnum] Format format, int width, int height)
        {
            await Task.Run(() =>
            {
                if (!mSurfaceExist)
                {
                    mSurfaceExist = true;
                    CheckCurrentState();
                }
                else
                {
                    mSurfaceExist = false;
                    CheckCurrentState();
                    mSurfaceExist = true;
                    CheckCurrentState();
                }
            });
        }

        public void SurfaceCreated(ISurfaceHolder holder)
        {

        }

        //[MethodImpl(MethodImplOptions.Synchronized)]
        public async void SurfaceDestroyed(ISurfaceHolder holder)
        {
            await Task.Run(() =>
            {
                mSurfaceExist = false;
                CheckCurrentState();
            });
        }

        //[MethodImpl(MethodImplOptions.Synchronized)]
        public async void EnableView()
        {
            await Task.Run(() =>
            {
                mEnabled = true;
                CheckCurrentState();
            });
        }

        //[MethodImpl(MethodImplOptions.Synchronized)]
        public async void DisableView()
        {
            await Task.Run(() =>
            {
                mEnabled = false;
                CheckCurrentState();
            });
        }

        public void EnableFpsMeter()
        {
            if (mFpsMeter == null)
            {
                mFpsMeter = new FpsMeter();
                mFpsMeter.SetResolution(mFrameWidth, mFrameHeight);
            }
        }

        public void SetCvCameraViewListener(CvCameraViewListener2 listener)
        {
            mListener = listener;
        }

        private void CheckCurrentState()
        {
            int targetState;

            if (mEnabled && mSurfaceExist && Visibility == ViewStates.Visible)
            {
                targetState = STARTED;
            }
            else
            {
                targetState = STOPPED;
            }

            if (targetState != mState)
            {
                ProcessExitState(mState);
                mState = targetState;
                ProcessEnterState(mState);
            }
        }

        private void ProcessEnterState(int state)
        {
            switch (state)
            {
                case 1:
                    OnEnterStartedState();
                    if (mListener != null)
                    {
                        mListener.OnCameraViewStarted(mFrameWidth, mFrameHeight);
                    }
                    break;
                case 0:
                    OnEnterStoppedState();
                    if (mListener != null)
                    {
                        mListener.OnCameraViewStopped();
                    }
                    break;
            }
        }

        private void ProcessExitState(int state)
        {
            switch (state)
            {
                case 1:
                    OnExitStartedState();
                    break;
                case 0:
                    OnExitStoppedState();
                    break;
            }
        }

        private void OnEnterStoppedState()
        {
        }

        private void OnExitStoppedState()
        {
        }

        private void OnEnterStartedState()
        {
            if (!ConnectCamera(Width, Height))
            {
                System.Diagnostics.Debug.WriteLine("It seems that you device does not support camera (or it is locked). Application will be closed.");
            }
        }

        private void OnExitStartedState()
        {
            DisconnectCamera();
            if (mCacheBitmap != null)
            {
                mCacheBitmap.Recycle();
            }
        }

        protected void DeliverAndDrawFrame(CvCameraViewFrame frame)
        {
            Mat modified;

            if (mListener != null)
            {
                modified = mListener.OnCameraFrame(frame);
            }
            else
            {
                modified = frame.Rgba();
            }

            bool bmpValid = true;
            if (modified != null)
            {
                try
                {
                    Org.Opencv.Android.Utils.MatToBitmap(modified, mCacheBitmap);
                }
                catch (Exception e)
                {
                    bmpValid = false;
                }
            }

            if (bmpValid && mCacheBitmap != null)
            {
                Canvas canvas = Holder.LockCanvas();
                if (canvas != null)
                {
                    if (Resources.Configuration.Orientation == Orientation.Portrait && mCameraIndex == -1)
                    {
                        canvas.Save();
                        canvas.Rotate(90, (canvas.Width / 2), (canvas.Height / 2));
                    }

                    if (Resources.Configuration.Orientation == Orientation.Portrait && mCameraIndex == 1)
                    {
                        canvas.Save();
                        canvas.Rotate(270, (canvas.Width / 2), (canvas.Height / 2));
                    }

                    if (mScale != 0)
                    {
                        canvas.DrawBitmap(mCacheBitmap, new Rect(0, 0, mCacheBitmap.Width, mCacheBitmap.Height),
                             new Rect((int)((canvas.Width - mScale * mCacheBitmap.Width) / 2),
                             (int)((canvas.Height - mScale * mCacheBitmap.Height) / 2),
                             (int)((canvas.Width - mScale * mCacheBitmap.Width) / 2 + mScale * mCacheBitmap.Width),
                             (int)((canvas.Height - mScale * mCacheBitmap.Height) / 2 + mScale * mCacheBitmap.Height)), null);
                    }
                    else
                    {
                        canvas.DrawBitmap(mCacheBitmap, new Rect(0, 0, mCacheBitmap.Width, mCacheBitmap.Height),
                            new Rect((canvas.Width - mCacheBitmap.Width) / 2,
                            (canvas.Height - mCacheBitmap.Height) / 2,
                            (canvas.Width - mCacheBitmap.Width) / 2 + mCacheBitmap.Width,
                            (canvas.Height - mCacheBitmap.Height) / 2 + mCacheBitmap.Height), null);
                    }

                    if (mFpsMeter != null)
                    {
                        mFpsMeter.Measure();
                        mFpsMeter.Draw(canvas, 20, 30);
                    }

                    if (Resources.Configuration.Orientation == Orientation.Portrait)
                    {
                        canvas.Restore();
                    }

                    Holder.UnlockCanvasAndPost(canvas);
                }
            }
        }

        protected abstract bool ConnectCamera(int width, int height);

        protected abstract void DisconnectCamera();

        protected void AllocateCache()
        {
            mCacheBitmap = Bitmap.CreateBitmap(mFrameWidth, mFrameHeight, Bitmap.Config.Argb8888);
        }

        public interface ListItemAccessor
        {
            int GetWidth(Java.Lang.Object obj);
            int GetHeight(Java.Lang.Object obj);
        }

        protected Size CalculateCameraFrameSize(IList<Android.Hardware.Camera.Size> supportedSizes, ListItemAccessor accessor, int surfaceWidth, int surfaceHeight)
        {
            int calcWidth = 0;
            int calcHeight = 0;

            int maxAllowedWidth = (mMaxWidth != MAX_UNSPECIFIED && mMaxWidth < surfaceWidth) ? mMaxWidth : surfaceWidth;
            int maxAllowedHeight = (mMaxHeight != MAX_UNSPECIFIED && mMaxHeight < surfaceHeight) ? mMaxHeight : surfaceHeight;

            foreach (var size in supportedSizes)
            {
                int width = accessor.GetWidth(size);
                int height = accessor.GetHeight(size);

                if (width <= maxAllowedWidth && height <= maxAllowedHeight)
                {
                    if (width >= calcWidth && height >= calcHeight)
                    {
                        calcWidth = (int)width;
                        calcHeight = (int)height;
                    }
                }
            }

            return new Size(calcWidth, calcHeight);
        }
    }
}
