using System;
using Android.Content;
using Android.Runtime;
using Android.Support.V4.View;
using Android.Util;
using Android.Views;
using Java.Lang;

namespace FairFlexxApps.Capture.Droid.FotoScanSdk.Controls
{
    class CustomViewPager : ViewPager
    {
        #region FirstOrDefault
        public CustomViewPager(Context context) : base(context) { }

        public CustomViewPager(Context context, IAttributeSet attrs) : base(context, attrs) { }

        protected CustomViewPager(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer) { }
        #endregion

        public override bool OnTouchEvent(MotionEvent e)
        {
            try
            {
                return base.OnTouchEvent(e);
            }

            catch (IllegalArgumentException ex)
            {
                ex.PrintStackTrace();
            }
            catch (ArrayIndexOutOfBoundsException ex1)
            {
                ex1.PrintStackTrace();
            }
            return false;
        }

        public override bool OnInterceptHoverEvent(MotionEvent e)
        {
            try
            {
                return base.OnInterceptHoverEvent(e);
            }
            catch (IllegalArgumentException ex)
            {
                ex.PrintStackTrace();
            }
            catch (ArrayIndexOutOfBoundsException ex1)
            {
                ex1.PrintStackTrace();
            }
            return false;
        }

        public override bool OnCapturedPointerEvent(MotionEvent e)
        {
            try
            {
                return base.OnCapturedPointerEvent(e);
            }
            catch (IllegalArgumentException ex)
            {
                ex.PrintStackTrace();
            }
            catch (ArrayIndexOutOfBoundsException ex1)
            {
                ex1.PrintStackTrace();
            }
            return false;
        }

        public override bool OnHoverEvent(MotionEvent e)
        {
            try
            {
                return base.OnHoverEvent(e);
            }
            catch (IllegalArgumentException ex)
            {
                ex.PrintStackTrace();
            }
            catch (ArrayIndexOutOfBoundsException ex1)
            {
                ex1.PrintStackTrace();
            }
            return false;
        }

        public override bool OnDragEvent(DragEvent e)
        {
            try
            {
                return base.OnDragEvent(e);
            }
            catch (IllegalArgumentException ex)
            {
                ex.PrintStackTrace();
            }
            catch (ArrayIndexOutOfBoundsException ex1)
            {
                ex1.PrintStackTrace();
            }
            return false;
        }

        public override bool OnInterceptTouchEvent(MotionEvent ev)
        {
            try
            {
                return base.OnInterceptTouchEvent(ev);
            }
            catch (IllegalArgumentException ex)
            {
                ex.PrintStackTrace();
            }
            catch (ArrayIndexOutOfBoundsException ex1)
            {
                ex1.PrintStackTrace();
            }
            return false;
        }
    }
}