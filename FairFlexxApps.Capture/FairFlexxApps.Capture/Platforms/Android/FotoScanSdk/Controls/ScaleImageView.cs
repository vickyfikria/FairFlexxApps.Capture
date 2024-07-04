using System;
using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Util;
using Android.Views;
using Android.Views.Animations;
using Android.Widget;
using FairFlexxApps.Capture.Droid.FotoScanSdk.Utils;
using Java.Lang;
using static Android.Views.GestureDetector;
using static Android.Views.ScaleGestureDetector;
using Math = System.Math;

namespace FairFlexxApps.Capture.Droid.FotoScanSdk.Controls
{
    public class ScaleImageView : ImageView
    {
        //    #region Properties
        //    private static ImageView view;
        //    static readonly int NONE = 0;
        //    static readonly int DRAG = 1;
        //    static readonly int ZOOM = 2;
        //    static readonly int CLICK = 3;
        //    private static int mode = NONE;
        //    private static Matrix matrix = new Matrix();
        //    private static PointF last = new PointF();
        //    private static PointF start = new PointF();
        //    private static float minScale = 1f;
        //    private static float maxScale = 4f;
        //    private static float[] m;
        //    private static int viewWidth, viewHeight, oldMeasuredWidth, oldMeasuredHeight;
        //    private static float saveScale = 1f;
        //    protected static float origWidth, origHeight;
        //    private static ScaleGestureDetector mScaleDetector;
        //    private static GestureDetector mGestureDetector;
        //    private static IOnDoubleTapListener doubleTapListener = null;
        //    private static OnTouchImageViewListener touchImageViewListener = null;
        //    #endregion

        //    #region Contrucstor
        public ScaleImageView(Context context) : base(context)
        {
            //SharedConstructing(context);
        }
        //    public ScaleImageView(Context context, IAttributeSet attrs) : base(context, attrs)
        //    {
        //        SharedConstructing(context);
        //    }
        //    #endregion

        //    #region FirstOrDefautl
        //    private void SharedConstructing(Context context)
        //    {
        //        base.Clickable = true;
        //        view = this;
        //        mScaleDetector = new ScaleGestureDetector(context, new ScaleListener());
        //        mGestureDetector = new GestureDetector(context, new GestureListener());
        //        m = new float[9];
        //        SetScaleType(ScaleType.Matrix);
        //        ImageMatrix = matrix;
        //    }
        //    #endregion

        //    #region Override
        //    protected override void OnMeasure(int widthMeasureSpec, int heightMeasureSpec)
        //    {
        //        base.OnMeasure(widthMeasureSpec, heightMeasureSpec);
        //        viewWidth = MeasureSpec.GetSize(widthMeasureSpec);
        //        viewHeight = MeasureSpec.GetSize(heightMeasureSpec);

        //        if (oldMeasuredWidth == viewWidth && oldMeasuredHeight == viewHeight || viewWidth == 0 || viewHeight == 0)
        //            return;
        //        oldMeasuredHeight = viewHeight;
        //        oldMeasuredWidth = viewWidth;

        //        if (saveScale == 1)
        //        {
        //            float scale;

        //            Drawable drawable = Drawable;
        //            if (drawable == null || drawable.IntrinsicWidth == 0 || drawable.IntrinsicHeight == 0)
        //                return;
        //            int bmWidth = drawable.IntrinsicWidth;
        //            int bmHeight = drawable.IntrinsicHeight;

        //            float scaleX = (float)viewWidth / (float)bmWidth;
        //            float scaleY = (float)viewHeight / (float)bmHeight;
        //            scale = Math.Min(scaleX, scaleY);
        //            matrix.SetScale(scale, scale);

        //            float redundantYSpace = (float)viewHeight
        //                    - (scale * (float)bmHeight);
        //            float redundantXSpace = (float)viewWidth
        //                    - (scale * (float)bmWidth);
        //            redundantYSpace /= (float)2;
        //            redundantXSpace /= (float)2;

        //            matrix.PostTranslate(redundantXSpace, redundantYSpace);

        //            origWidth = viewWidth - 2 * redundantXSpace;
        //            origHeight = viewHeight - 2 * redundantYSpace;
        //            ImageMatrix = matrix;
        //        }
        //    }

        //    public override bool OnTouchEvent(MotionEvent @event)
        //    {
        //        mScaleDetector.OnTouchEvent(@event);
        //        mGestureDetector.OnTouchEvent(@event);

        //        matrix.GetValues(m);
        //        float x = m[Matrix.MtransX];
        //        float y = m[Matrix.MtransY];
        //        PointF curr = new PointF(@event.GetX(), @event.GetY());

        //        switch (@event.Action)
        //        {
        //            case MotionEventActions.Down:
        //                last.Set(curr);
        //                start.Set(last);
        //                mode = DRAG;
        //                break;
        //            case MotionEventActions.PointerDown:
        //                last.Set(curr);
        //                start.Set(last);
        //                mode = ZOOM;
        //                break;
        //            case MotionEventActions.Move:
        //                if (mode == DRAG)
        //                {
        //                    float deltaX = curr.X - last.X;
        //                    float deltaY = curr.Y - last.Y;
        //                    float fixTransX = GetFixDragTrans(deltaX, viewWidth, origWidth * saveScale);
        //                    float fixTransY = GetFixDragTrans(deltaY, viewHeight, origHeight * saveScale);
        //                    matrix.PostTranslate(fixTransX, fixTransY);
        //                    FixTrans();
        //                    last.Set(curr.X, curr.Y);
        //                }
        //                break;

        //            case MotionEventActions.Up:
        //                mode = NONE;
        //                int xDiff = (int)System.Math.Abs(curr.X - start.Y);
        //                int yDiff = (int)System.Math.Abs(curr.Y - start.Y);
        //                if (xDiff < CLICK && yDiff < CLICK)
        //                    PerformClick();
        //                break;
        //            case MotionEventActions.PointerUp:
        //                mode = NONE;
        //                break;
        //        }
        //        ImageMatrix = matrix;
        //        Invalidate();
        //        return true;
        //    }

        //    private void FixTrans()
        //    {
        //        matrix.GetValues(m);
        //        float transX = m[Matrix.MtransX];
        //        float transY = m[Matrix.MtransY];

        //        float fixTransX = GetFixTrans(transX, viewWidth, origWidth * saveScale);
        //        float fixTransY = GetFixTrans(transY, viewHeight, origHeight * saveScale);

        //        if (fixTransX != 0 || fixTransY != 0)
        //            matrix.PostTranslate(fixTransX, fixTransY);
        //    }

        //    float GetFixTrans(float trans, float viewSize, float contentSize)
        //    {
        //        float minTrans, maxTrans;

        //        if (contentSize <= viewSize)
        //        {
        //            minTrans = 0;
        //            maxTrans = viewSize - contentSize;
        //        }
        //        else
        //        {
        //            minTrans = viewSize - contentSize;
        //            maxTrans = 0;
        //        }

        //        if (trans < minTrans)
        //            return -trans + minTrans;
        //        if (trans > maxTrans)
        //            return -trans + maxTrans;
        //        return 0;
        //    }

        //    float GetFixDragTrans(float delta, float viewSize, float contentSize)
        //    {
        //        if (contentSize <= viewSize)
        //        {
        //            return 0;
        //        }
        //        return delta;
        //    }

        //    #endregion

        //    #region ScaleListener Class
        //    private class ScaleListener : SimpleOnScaleGestureListener
        //    {
        //        public override bool OnScaleBegin(ScaleGestureDetector detector)
        //        {
        //            mode = ZOOM;
        //            return true;
        //        }
        //        public override bool OnScale(ScaleGestureDetector detector)
        //        {
        //            float mScaleFactor = detector.ScaleFactor;
        //            float origScale = saveScale;
        //            saveScale *= mScaleFactor;
        //            if (saveScale > maxScale)
        //            {
        //                saveScale = maxScale;
        //                mScaleFactor = maxScale / origScale;
        //            }
        //            else if (saveScale < minScale)
        //            {
        //                saveScale = minScale;
        //                mScaleFactor = minScale / origScale;
        //            }

        //            if (origWidth * saveScale <= viewWidth
        //                    || origHeight * saveScale <= viewHeight)
        //                matrix.PostScale(mScaleFactor, mScaleFactor, viewWidth / 2,
        //                        viewHeight / 2);
        //            else
        //                matrix.PostScale(mScaleFactor, mScaleFactor,
        //                        detector.FocusX, detector.FocusY);

        //            FixTrans();
        //            return true;
        //        }

        //        private void FixTrans()
        //        {
        //            matrix.GetValues(m);
        //            float transX = m[Matrix.MtransX];
        //            float transY = m[Matrix.MtransY];

        //            float fixTransX = GetFixTrans(transX, viewWidth, origWidth * saveScale);
        //            float fixTransY = GetFixTrans(transY, viewHeight, origHeight * saveScale);

        //            if (fixTransX != 0 || fixTransY != 0)
        //                matrix.PostTranslate(fixTransX, fixTransY);
        //        }

        //        float GetFixTrans(float trans, float viewSize, float contentSize)
        //        {
        //            float minTrans, maxTrans;

        //            if (contentSize <= viewSize)
        //            {
        //                minTrans = 0;
        //                maxTrans = viewSize - contentSize;
        //            }
        //            else
        //            {
        //                minTrans = viewSize - contentSize;
        //                maxTrans = 0;
        //            }

        //            if (trans < minTrans)
        //                return -trans + minTrans;
        //            if (trans > maxTrans)
        //                return -trans + maxTrans;
        //            return 0;
        //        }
        //    }
        //    #endregion

        //    public interface OnTouchImageViewListener
        //    {
        //        void onMove();
        //    }

        //    #region GestureListener Class
        //    public class GestureListener : SimpleOnGestureListener
        //    {
        //        //public override bool OnSingleTapConfirmed(MotionEvent e)
        //        //{
        //        //    if (doubleTapListener != null)
        //        //    {
        //        //        return doubleTapListener.OnSingleTapConfirmed(e);
        //        //    }
        //        //    return view.PerformClick();
        //        //}

        //        //public override void OnLongPress(MotionEvent e)
        //        //{
        //        //}

        //        //public override bool OnFling(MotionEvent e1, MotionEvent e2, float velocityX, float velocityY)
        //        //{
        //        //    return false;
        //        //}

        //        public override bool OnDoubleTap(MotionEvent e)
        //        {
        //            //bool consumed = false;
        //            //if (doubleTapListener != null)
        //            //{
        //            //    consumed = doubleTapListener.OnDoubleTap(e);
        //            //}
        //            //if (mode == NONE)
        //            //{
        //            //    float targetZoom = (saveScale == minScale) ? maxScale : minScale;
        //            //    DoubleTapZoom doubleTap = new DoubleTapZoom(targetZoom, e.GetX(), e.GetY(), false);
        //            //    CompatPostOnAnimation(doubleTap);
        //            //    consumed = true;
        //            //}
        //            //return consumed;
        //            float origScale = saveScale;
        //            float mScaleFactor;

        //            if (saveScale == maxScale)
        //            {
        //                saveScale = minScale;
        //                mScaleFactor = minScale / origScale;
        //            }
        //            else
        //            {
        //                saveScale = maxScale;
        //                mScaleFactor = maxScale / origScale;
        //            }

        //            matrix.PostScale(mScaleFactor, mScaleFactor, viewWidth / 2,
        //                    viewHeight / 2);

        //            FixTrans();
        //            return false;
        //        }

        //        private void CompatPostOnAnimation(IRunnable runnable)
        //        {
        //            if (Build.VERSION.SdkInt >= Build.VERSION_CODES.JellyBean)
        //            {
        //                view.PostOnAnimation(runnable);
        //            }
        //            else
        //            {
        //                view.PostDelayed(runnable, 1000 / 60);
        //            }
        //        }

        //        private void FixTrans()
        //        {
        //            matrix.GetValues(m);
        //            float transX = m[Matrix.MtransX];
        //            float transY = m[Matrix.MtransY];

        //            float fixTransX = GetFixTrans(transX, viewWidth, origWidth * saveScale);
        //            float fixTransY = GetFixTrans(transY, viewHeight, origHeight * saveScale);

        //            if (fixTransX != 0 || fixTransY != 0)
        //                matrix.PostTranslate(fixTransX, fixTransY);
        //        }

        //        float GetFixTrans(float trans, float viewSize, float contentSize)
        //        {
        //            float minTrans, maxTrans;

        //            if (contentSize <= viewSize)
        //            {
        //                minTrans = 0;
        //                maxTrans = viewSize - contentSize;
        //            }
        //            else
        //            {
        //                minTrans = viewSize - contentSize;
        //                maxTrans = 0;
        //            }

        //            if (trans < minTrans)
        //                return -trans + minTrans;
        //            if (trans > maxTrans)
        //                return -trans + maxTrans;
        //            return 0;
        //        }

        //        //public override bool OnDoubleTapEvent(MotionEvent e)
        //        //{
        //        //    return false;
        //        //}
        //    }
        //    #endregion

        //    #region DoubleTapZoomClass
        //    public class DoubleTapZoom : Java.Lang.Object, IRunnable
        //    {
        //        #region Properties
        //        private long startTime;
        //        private static readonly float ZOOM_TIME = 500;
        //        private float startZoom, targetZoom;
        //        private float bitmapX, bitmapY;
        //        private bool stretchImageToSuper;
        //        private AccelerateDecelerateInterpolator interpolator = new AccelerateDecelerateInterpolator();
        //        private PointF startTouch;
        //        private PointF endTouch;
        //        #endregion

        //        public DoubleTapZoom(float targetZoom, float focusX, float focusY, bool stretchImageToSuper)
        //        {
        //            startTime = JavaSystem.CurrentTimeMillis();
        //            startZoom = saveScale;
        //            this.targetZoom = targetZoom;
        //            this.stretchImageToSuper = stretchImageToSuper;
        //            PointF bitmapPoint = TransformCoordTouchToBitmap(focusX, focusY, false);
        //            bitmapX = bitmapPoint.X;
        //            bitmapY = bitmapPoint.Y;

        //            startTouch = TransformCoordBitmapToTouch(bitmapX, bitmapY);
        //            endTouch = new PointF(viewWidth / 2, viewHeight / 2);
        //        }

        //        private PointF TransformCoordBitmapToTouch(float bitmapX, float bitmapY)
        //        {
        //            matrix.GetValues(m);
        //            float px = bitmapX / origWidth;
        //            float py = bitmapY / origHeight;
        //            float finalX = m[Matrix.MtransX] + origWidth * saveScale * px;
        //            float finalY = m[Matrix.MtransY] + origHeight * saveScale * px;
        //            return new PointF(finalX, finalY);
        //        }

        //        private PointF TransformCoordTouchToBitmap(float focusX, float focusY, bool clipToBitmap)
        //        {
        //            matrix.GetValues(m);
        //            float transX = m[Matrix.MtransX];
        //            float transY = m[Matrix.MtransY];
        //            float finalX = ((focusX - transX) * origWidth) / origWidth * saveScale;
        //            float finalY = ((focusY - transY) * origHeight) / origHeight * saveScale;

        //            if (clipToBitmap)
        //            {
        //                finalX = Math.Min(Math.Max(finalX, 0), origWidth);
        //                finalY = Math.Min(Math.Max(finalY, 0), origHeight);
        //            }

        //            return new PointF(finalX, finalY);
        //        }

        //        private void translateImageToCenterTouchPosition(float t)
        //        {
        //            float targetX = startTouch.X + t * (endTouch.X - startTouch.X);
        //            float targetY = startTouch.Y + t * (endTouch.Y - startTouch.Y);
        //            PointF curr = TransformCoordBitmapToTouch(bitmapX, bitmapY);
        //            matrix.PostTranslate(targetX - curr.X, targetY - curr.Y);
        //        }

        //        private float interpolate()
        //        {
        //            long currTime = Java.Lang.JavaSystem.CurrentTimeMillis();
        //            float elapsed = (currTime - startTime) / ZOOM_TIME;
        //            elapsed = Math.Min(1f, elapsed);
        //            return interpolator.GetInterpolation(elapsed);
        //        }

        //        private double calculateDeltaScale(float t)
        //        {
        //            double zoom = startZoom + t * (targetZoom - startZoom);
        //            return zoom / saveScale;
        //        }

        //        public void Run()
        //        {
        //            float t = interpolate();
        //            double deltaScale = calculateDeltaScale(t);
        //            ScaleImage(deltaScale, bitmapX, bitmapY, stretchImageToSuper);
        //            translateImageToCenterTouchPosition(t);
        //            FixScaleTrans();
        //            view.ImageMatrix = matrix;

        //            if (touchImageViewListener != null)
        //            {
        //                touchImageViewListener.onMove();
        //            }

        //            if (t < 1f)
        //            {
        //                CompatPostOnAnimation(this);

        //            }
        //            else
        //            {
        //                mode = NONE;
        //            }
        //        }

        //        private void CompatPostOnAnimation(IRunnable runnable)
        //        {
        //            if (Build.VERSION.SdkInt >= Build.VERSION_CODES.JellyBean)
        //            {
        //                view.PostOnAnimation(runnable);
        //            }
        //            else
        //            {
        //                view.PostDelayed(runnable, 1000 / 60);
        //            }
        //        }

        //        private void FixScaleTrans()
        //        {
        //            FixTrans();
        //            matrix.GetValues(m);
        //            if (origWidth*saveScale < viewWidth)
        //            {
        //                m[Matrix.MtransX] = (viewWidth - origWidth * saveScale) / 2;
        //            }

        //            if (origHeight * saveScale < viewHeight)
        //            {
        //                m[Matrix.MtransY] = (viewHeight - origHeight * saveScale) / 2;
        //            }
        //            matrix.SetValues(m);
        //        }

        //        private void FixTrans()
        //        {

        //        }

        //        private void ScaleImage(double deltaScale, float bitmapX, float bitmapY, bool stretchImageToSuper)
        //        {
        //            float lowerScale, upperScale;
        //            if (stretchImageToSuper)
        //            {
        //                lowerScale = minScale;
        //                upperScale = maxScale;

        //            }
        //            else
        //            {
        //                lowerScale = minScale;
        //                upperScale = maxScale;
        //            }

        //            float origScale = saveScale;
        //            saveScale *= (float)deltaScale;
        //            if (saveScale > upperScale)
        //            {
        //                saveScale = upperScale;
        //                deltaScale = upperScale / origScale;
        //            }
        //            else if (saveScale < lowerScale)
        //            {
        //                saveScale = lowerScale;
        //                deltaScale = lowerScale / origScale;
        //            }

        //            matrix.PostScale((float)deltaScale, (float)deltaScale, bitmapX, bitmapY);
        //            FixScaleTrans();
        //        }

        //    }
        //    #endregion
    }
}