using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables.Shapes;
using Android.Runtime;
using Android.Util;
using Android.Views;

namespace FairFlexxApps.Capture.Droid.FotoScanSdk.Views
{
    public class ScanCanvasView : View
    {
        private JavaList<ScanShape> shapes = new JavaList<ScanShape>();

        public ScanCanvasView(Context context) : base(context)
        {
        }

        public ScanCanvasView(Context context, IAttributeSet attrs) : base(context, attrs)
        {
        }

        public ScanCanvasView(Context context, IAttributeSet attrs, int defStyle) : base(context, attrs, defStyle)
        {
        }

        public class ScanShape
        {
            private Shape mShape;
            private Paint mPaint;
            private Paint mBorder;

            public ScanShape(Shape shape, Paint paint)
            {
                mShape = shape;
                mPaint = paint;
                mBorder = null;
            }

            public ScanShape(Shape shape, Paint paint, Paint border)
            {
                mShape = shape;
                mPaint = paint;
                mBorder = border;
                mBorder.SetStyle(Paint.Style.Stroke);
            }

            public void Draw(Canvas canvas)
            {
                mShape.Draw(canvas, mPaint);

                if (mBorder != null)
                {
                    mShape.Draw(canvas, mBorder);
                }
            }

            public Shape getShape()
            {
                return mShape;
            }
        }

        protected override void OnDraw(Canvas canvas)
        {
            base.OnDraw(canvas);

            // allocations per draw cycle.
            int paddingLeft = this.PaddingLeft;
            int paddingTop = this.PaddingTop;
            int paddingRight = this.PaddingRight;
            int paddingBottom = this.PaddingBottom;

            int contentWidth = this.Width - paddingLeft - paddingRight;
            int contentHeight = this.Height - paddingTop - paddingBottom;

            foreach (ScanShape s in shapes)
            {
                s.getShape().Resize(contentWidth, contentHeight);
                s.Draw(canvas);
            }
        }

        public ScanShape AddShape(Shape shape, Paint paint)
        {
            ScanShape scanShape = new ScanShape(shape, paint);
            shapes.Add(scanShape);
            return scanShape;
        }

        public void AddShape(Shape shape, Paint paint, Paint border)
        {
            ScanShape scanShape = new ScanShape(shape, paint, border);
            shapes.Add(scanShape);
        }

        public void removeShape(ScanShape shape)
        {
            shapes.Remove(shape);
        }

        public void removeShape(int index)
        {
            shapes.Remove(index);
        }

        public void clear()
        {
            shapes.Clear();
        }
    }
}