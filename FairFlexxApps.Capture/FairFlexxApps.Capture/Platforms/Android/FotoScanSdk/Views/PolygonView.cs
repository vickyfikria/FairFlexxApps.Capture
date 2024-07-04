using System;
using System.Collections.Generic;
using System.Linq;
using Android.Content;
using Android.Graphics;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using FairFlexxApps.Capture.Droid.FotoScanSdk.Activities;
using FairFlexxApps.Capture.Droid.FotoScanSdk.Controls;
using FairFlexxApps.Capture.Droid.FotoScanSdk.Utils;
using static Android.Views.View;

namespace FairFlexxApps.Capture.Droid.FotoScanSdk.Views
{
    public class PolygonView : FrameLayout, IOnTouchListener
    {
        private Context context;
        private static Paint paint;
        private static ImageView pointer1;
        private static ImageView pointer2;
        private static ImageView pointer3;
        private static ImageView pointer4;
        private ImageView midPointer13;
        private ImageView midPointer12;
        private ImageView midPointer34;
        private ImageView midPointer24;
        private static PolygonView polygonView;
        private Paint circleFillPaint;

        public PolygonView(Context context) : base(context)
        {
            this.context = context;
            Init();
        }

        public PolygonView(Context context, IAttributeSet attrs) : base(context, attrs)
        {
            this.context = context;
            Init();
        }

        public PolygonView(Context context, IAttributeSet attrs, int defStyleAttr) : base(context, attrs, defStyleAttr)
        {
            this.context = context;
            Init();
        }

        private void Init()
        {
            polygonView = this;
            pointer1 = GetImageView(0, 0);
            pointer2 = GetImageView(this.Width, 0);
            pointer3 = GetImageView(0, this.Height);
            pointer4 = GetImageView(this.Width, this.Height);

            midPointer13 = GetImageViewTransparent(0, this.Height / 2);
            midPointer13.SetOnTouchListener(new MidPointTouchListenerImpl(pointer1, pointer3));

            midPointer12 = GetImageViewTransparent(0, this.Width / 2);
            midPointer12.SetOnTouchListener(new MidPointTouchListenerImpl(pointer1, pointer2));

            midPointer34 = GetImageViewTransparent(0, this.Width / 2);
            midPointer34.SetOnTouchListener(new MidPointTouchListenerImpl(pointer3, pointer4));

            midPointer24 = GetImageViewTransparent(0, this.Height / 2);
            midPointer24.SetOnTouchListener(new MidPointTouchListenerImpl(pointer2, pointer4));

            AddView(pointer1);
            AddView(pointer2); 
            AddView(midPointer13);
            AddView(midPointer12);
            AddView(midPointer34);
            AddView(midPointer24);
            AddView(pointer3);
            AddView(pointer4);

            InitPaint();
        }

        protected override void AttachViewToParent(View child, int index, ViewGroup.LayoutParams @params)
        {
            base.AttachViewToParent(child, index, @params);
        }

        private void InitPaint()
        {
            paint = new Paint();
            paint.Color = Resources.GetColor(Resource.Color.crop_color);
            paint.StrokeWidth = 7;
            paint.AntiAlias = true;

            circleFillPaint = new Paint();
            circleFillPaint.SetStyle(Paint.Style.Fill);
            circleFillPaint.Color = Resources.GetColor(Resource.Color.crop_color);
            circleFillPaint.AntiAlias = true;
        }

        public Dictionary<int, PointF> GetPoints()
        {
            List<PointF> points = new List<PointF>();
            points.Add(new PointF(pointer1.GetX(), pointer1.GetY()));
            points.Add(new PointF(pointer2.GetX(), pointer2.GetY()));
            points.Add(new PointF(pointer3.GetX(), pointer3.GetY()));
            points.Add(new PointF(pointer4.GetX(), pointer4.GetY()));

            return GetOrderedPoints(points);
        }

        public JavaList<PointF> GetPointsV2()
        {
            List<PointF> points = new List<PointF>();
            points.Add(new PointF(pointer1.GetX(), pointer1.GetY()));
            points.Add(new PointF(pointer2.GetX(), pointer2.GetY()));
            points.Add(new PointF(pointer3.GetX(), pointer3.GetY()));
            points.Add(new PointF(pointer4.GetX(), pointer4.GetY()));

            return GetOrderedPointsV2(points);
        }

        private static Dictionary<int, PointF> GetOrderedPoints(List<PointF> points)
        {
            PointF centerPoint = new PointF();
            int size = points.Count;
            foreach (var pointF in points)
            {
                centerPoint.X += pointF.X / size;
                centerPoint.Y += pointF.Y / size;
            }
            var orderedPoints = new Dictionary<int, PointF>();
            foreach (var pointF in points)
            {
                int index = -1;
                if (pointF.X < centerPoint.X && pointF.Y < centerPoint.Y)
                {
                    index = 0;
                }
                else if (pointF.X > centerPoint.X && pointF.Y < centerPoint.Y)
                {
                    index = 1;
                }
                else if (pointF.X < centerPoint.X && pointF.Y > centerPoint.Y)
                {
                    index = 2;
                }
                else if (pointF.X > centerPoint.X && pointF.X > centerPoint.X)
                {
                    index = 3;
                }

                orderedPoints.Add(index, pointF);
            }
            return orderedPoints;
        }

        private static JavaList<PointF> GetOrderedPointsV2(List<PointF> points)
        {
            PointF centerPoint = new PointF();
            int size = points.Count;
            foreach (var pointF in points)
            {
                centerPoint.X += pointF.X / size;
                centerPoint.Y += pointF.Y / size;
            }
            var orderedPoints = new JavaList<PointF>();
            foreach (var pointF in points)
            {
                if (pointF.X < centerPoint.X && pointF.Y < centerPoint.Y)
                {
                    orderedPoints.Add(pointF);
                }
                else if (pointF.X > centerPoint.X && pointF.Y < centerPoint.Y)
                {
                    orderedPoints.Add(pointF);
                }
                else if (pointF.X < centerPoint.X && pointF.Y > centerPoint.Y)
                {
                    orderedPoints.Add(pointF);
                }
                else if (pointF.X > centerPoint.X && pointF.X > centerPoint.X)
                {
                    orderedPoints.Add(pointF);
                }
            }
            return orderedPoints;
        }

        public void SetPoints(Dictionary<int, PointF> pointFMap)
        {
            if (pointFMap.Count() == 4)
            {
                SetPointsCoordinates(pointFMap);
            }
        }

        public void SetPointsV2(JavaList<PointF> pointFMap)
        {
            if (pointFMap.Count() == 4)
            {
                SetPointsCoordinatesV2(pointFMap);
            }
        }

        private void SetPointsCoordinatesV2(JavaList<PointF> pointFMap)
        {
            pointer1.SetX(pointFMap[0].X);
            pointer1.SetY(pointFMap[0].Y);

            pointer2.SetX(pointFMap[1].X);
            pointer2.SetY(pointFMap[1].Y);

            pointer3.SetX(pointFMap[3].X);
            pointer3.SetY(pointFMap[3].Y);

            pointer4.SetX(pointFMap[2].X);
            pointer4.SetY(pointFMap[2].Y);

            midPointer13.SetX(pointer3.GetX() - ((pointer3.GetX() - pointer1.GetX()) / 2));
            midPointer13.SetY(pointer3.GetY() - ((pointer3.GetY() - pointer1.GetY()) / 2));
            midPointer24.SetX(pointer4.GetX() - ((pointer4.GetX() - pointer2.GetX()) / 2));
            midPointer24.SetY(pointer4.GetY() - ((pointer4.GetY() - pointer2.GetY()) / 2));
            midPointer34.SetX(pointer4.GetX() - ((pointer4.GetX() - pointer3.GetX()) / 2));
            midPointer34.SetY(pointer4.GetY() - ((pointer4.GetY() - pointer3.GetY()) / 2));
            midPointer12.SetX(pointer2.GetX() - ((pointer2.GetX() - pointer1.GetX()) / 2));
            midPointer12.SetY(pointer2.GetY() - ((pointer2.GetY() - pointer1.GetY()) / 2));
        }

        private void SetPointsCoordinates(Dictionary<int, PointF> pointFMap)
        {
            pointer1.SetX(pointFMap[0].X);
            pointer1.SetY(pointFMap[0].Y);

            pointer2.SetX(pointFMap[1].X);
            pointer2.SetY(pointFMap[1].Y);

            pointer3.SetX(pointFMap[2].X);
            pointer3.SetY(pointFMap[2].Y);

            pointer4.SetX(pointFMap[3].X);
            pointer4.SetY(pointFMap[3].Y);

            midPointer13.SetX(pointer3.GetX() - ((pointer3.GetX() - pointer1.GetX()) / 2));
            midPointer13.SetY(pointer3.GetY() - ((pointer3.GetY() - pointer1.GetY()) / 2));
            midPointer24.SetX(pointer4.GetX() - ((pointer4.GetX() - pointer2.GetX()) / 2));
            midPointer24.SetY(pointer4.GetY() - ((pointer4.GetY() - pointer2.GetY()) / 2));
            midPointer34.SetX(pointer4.GetX() - ((pointer4.GetX() - pointer3.GetX()) / 2));
            midPointer34.SetY(pointer4.GetY() - ((pointer4.GetY() - pointer3.GetY()) / 2));
            midPointer12.SetX(pointer2.GetX() - ((pointer2.GetX() - pointer1.GetX()) / 2));
            midPointer12.SetY(pointer2.GetY() - ((pointer2.GetY() - pointer1.GetY()) / 2));
        }

        public void ResetPoints(PolygonPoints polygonPoints)
        {
            pointer1.SetX(polygonPoints.getTopLeftPoint().X);
            pointer1.SetY(polygonPoints.getTopLeftPoint().Y);

            pointer2.SetX(polygonPoints.getTopRightPoint().X);
            pointer2.SetY(polygonPoints.getTopRightPoint().Y);

            pointer3.SetX(polygonPoints.getBottomLeftPoint().X);
            pointer3.SetY(polygonPoints.getBottomLeftPoint().Y);

            pointer4.SetX(polygonPoints.getBottomRightPoint().X);
            pointer4.SetY(polygonPoints.getBottomRightPoint().Y);

            polygonView.Invalidate();
        }

        protected override void DispatchDraw(Canvas canvas)
        {
            base.DispatchDraw(canvas);

            Paint bgPaint = new Paint();
            bgPaint.Color = Resources.GetColor(Resource.Color.colorBlackThirtyFivePercentAlpha);
            bgPaint.AntiAlias = true;

            Path path1 = DrawOutTopRect(canvas);
            canvas.DrawPath(path1, bgPaint);

            Path path4 = DrawOutMiddleLeftRect();
            canvas.DrawPath(path4, bgPaint);

            Path path5 = DrawOutMiddleRightRect(canvas);
            canvas.DrawPath(path5, bgPaint);

            Path path6 = DrawOutBottomRect(canvas);
            canvas.DrawPath(path6, bgPaint);

            canvas.DrawLine(pointer1.GetX() + (pointer1.Width / 2), pointer1.GetY() + (pointer1.Height / 2), pointer3.GetX() + (pointer3.Width / 2), pointer3.GetY() + (pointer3.Height / 2), paint);
            canvas.DrawLine(pointer1.GetX() + (pointer1.Width / 2), pointer1.GetY() + (pointer1.Height / 2), pointer2.GetX() + (pointer2.Width / 2), pointer2.GetY() + (pointer2.Height / 2), paint);
            canvas.DrawLine(pointer2.GetX() + (pointer2.Width / 2), pointer2.GetY() + (pointer2.Height / 2), pointer4.GetX() + (pointer4.Width / 2), pointer4.GetY() + (pointer4.Height / 2), paint);
            canvas.DrawLine(pointer3.GetX() + (pointer3.Width / 2), pointer3.GetY() + (pointer3.Height / 2), pointer4.GetX() + (pointer4.Width / 2), pointer4.GetY() + (pointer4.Height / 2), paint);
            midPointer13.SetX(pointer3.GetX() - ((pointer3.GetX() - pointer1.GetX()) / 2));
            midPointer13.SetY(pointer3.GetY() - ((pointer3.GetY() - pointer1.GetY()) / 2));
            midPointer24.SetX(pointer4.GetX() - ((pointer4.GetX() - pointer2.GetX()) / 2));
            midPointer24.SetY(pointer4.GetY() - ((pointer4.GetY() - pointer2.GetY()) / 2));
            midPointer34.SetX(pointer4.GetX() - ((pointer4.GetX() - pointer3.GetX()) / 2));
            midPointer34.SetY(pointer4.GetY() - ((pointer4.GetY() - pointer3.GetY()) / 2));
            midPointer12.SetX(pointer2.GetX() - ((pointer2.GetX() - pointer1.GetX()) / 2));
            midPointer12.SetY(pointer2.GetY() - ((pointer2.GetY() - pointer1.GetY()) / 2));

            int radius = ScanUtils.Dp2px(context, 11);
            canvas.DrawCircle(pointer1.GetX() + (pointer1.Width / 2), pointer1.GetY() + (pointer1.Height / 2), radius, circleFillPaint);
            canvas.DrawCircle(pointer2.GetX() + (pointer2.Width / 2), pointer2.GetY() + (pointer2.Height / 2), radius, circleFillPaint);
            canvas.DrawCircle(pointer3.GetX() + (pointer3.Width / 2), pointer3.GetY() + (pointer3.Height / 2), radius, circleFillPaint);
            canvas.DrawCircle(pointer4.GetX() + (pointer4.Width / 2), pointer4.GetY() + (pointer4.Height / 2), radius, circleFillPaint);

            canvas.DrawCircle(midPointer13.GetX() + (midPointer13.Width / 2), midPointer13.GetY() + (midPointer13.Height / 2), radius, circleFillPaint);
            canvas.DrawCircle(midPointer24.GetX() + (midPointer24.Width / 2), midPointer24.GetY() + (midPointer24.Height / 2), radius, circleFillPaint);
            canvas.DrawCircle(midPointer34.GetX() + (midPointer34.Width / 2), midPointer34.GetY() + (midPointer34.Height / 2), radius, circleFillPaint);
            canvas.DrawCircle(midPointer12.GetX() + (midPointer12.Width / 2), midPointer12.GetY() + (midPointer12.Height / 2), radius, circleFillPaint);
        }

        private Path DrawOutBottomRect(Canvas canvas)
        {
            Path path = new Path();
            path.MoveTo(0, canvas.Height);
            path.LineTo(canvas.Width, canvas.Height);
            path.LineTo(canvas.Width, pointer4.GetY() + (pointer4.Height / 2));
            path.LineTo(pointer4.GetX() + (pointer4.Width / 2), pointer4.GetY() + (pointer4.Height / 2));
            path.LineTo(pointer3.GetX() + (pointer3.Width / 2), pointer3.GetY() + (pointer3.Height / 2));
            path.LineTo(0, pointer3.GetY() + (pointer3.Height / 2));
            path.Close();
            return path;
        }

        private Path DrawOutMiddleRightRect(Canvas canvas)
        {
            Path path = new Path();
            path.MoveTo(pointer2.GetX() + (pointer2.Width / 2), pointer2.GetY() + (pointer2.Height / 2));
            path.LineTo(canvas.Width, pointer2.GetY() + (pointer2.Height / 2));
            path.LineTo(canvas.Width, pointer4.GetY() + (pointer4.Height / 2));
            path.LineTo(pointer4.GetX() + (pointer4.Width / 2), pointer4.GetY() + (pointer4.Height / 2));
            path.Close();
            return path;
        }

        private Path DrawOutMiddleLeftRect()
        {
            Path path = new Path();
            path.MoveTo(0, pointer1.GetY() + (pointer1.Height / 2));
            path.LineTo(pointer1.GetX() + (pointer1.Width / 2), pointer1.GetY() + (pointer1.Height / 2));
            path.LineTo(pointer3.GetX() + (pointer3.Width / 2), pointer3.GetY() + (pointer3.Height / 2));
            path.LineTo(0, pointer3.GetY() + (pointer3.Height / 2));
            path.Close();
            return path;
        }

        private Path DrawOutTopRect(Canvas canvas)
        {
            Path path = new Path();
            path.MoveTo(0, 0);
            path.LineTo(canvas.Width, 0);
            path.LineTo(canvas.Width, pointer2.GetY() + (pointer2.Height / 2));
            path.LineTo(pointer2.GetX() + (pointer2.Width / 2), pointer2.GetY() + (pointer2.Height / 2));
            path.LineTo(pointer1.GetX() + (pointer1.Width / 2), pointer1.GetY() + (pointer1.Height / 2));
            path.LineTo(0, pointer1.GetY() + (pointer1.Height / 2));
            path.Close();
            return path;
        }

        private ImageView GetImageView(int x, int y)
        {
            ImageView imageView = new ImageView(context);
            LayoutParams layoutParams = new LayoutParams(ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.WrapContent);
            imageView.LayoutParameters = layoutParams;
            imageView.SetImageResource(Resource.Drawable.circle);
            imageView.SetX(x);
            imageView.SetY(y);
            imageView.SetOnTouchListener(new TouchListenerImpl());
            return imageView;
        }

        private ImageView GetImageViewTransparent(int x, int y)
        {
            ImageView imageView = new ImageView(context);
            LayoutParams layoutParams = new LayoutParams(ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.WrapContent);
            imageView.LayoutParameters = layoutParams;
            imageView.SetImageResource(Resource.Drawable.circle);
            imageView.SetX(x);
            imageView.SetY(y);
            return imageView;
        }

        private static bool IsValidShape(Dictionary<int, PointF> pointFMap)
        {
            return pointFMap.Count == 4;
        }

        private static bool IsValidShapeV2(JavaList<PointF> pointFMap)
        {
            return pointFMap.Count == 4;
        }

        private static bool IsValidPointer4()
        {
            return pointer4.GetY() > pointer2.GetY() && pointer4.GetX() > pointer3.GetX();
        }

        private static bool IsValidPointer3()
        {
            return pointer3.GetY() > pointer1.GetY() && pointer3.GetX() < pointer4.GetX();
        }

        private static bool IsValidPointer2()
        {
            return pointer2.GetY() < pointer4.GetY() && pointer2.GetX() > pointer1.GetX();
        }

        private static bool IsValidPointer1()
        {
            return pointer1.GetY() < pointer3.GetY() && pointer1.GetX() < pointer2.GetX();
        }

        PointF DownPT = new PointF(); // Record Mouse Position When Pressed Down
        PointF StartPT = new PointF(); // Record Start Position of 'img'

        private ImageView mainPointer1;
        private ImageView mainPointer2;
        PointF latestPoint = new PointF();
        PointF latestPoint1 = new PointF();
        PointF latestPoint2 = new PointF();

        public bool OnTouch(View v, MotionEvent e)
        {
            var eid = e.Action;

            switch (eid)
            {
                case MotionEventActions.Move:

                    PointF mv = new PointF(e.GetX() - DownPT.X, e.GetY() - DownPT.Y);

                    if (Math.Abs(mainPointer1.GetX() - mainPointer2.GetX()) > Math.Abs(mainPointer1.GetY() - mainPointer2.GetY()))
                    {
                        if (((mainPointer2.GetY() + mv.Y + v.Height < polygonView.Height) && (mainPointer2.GetY() + mv.Y > 0)))
                        {
                            v.SetX((int)(StartPT.Y + mv.Y));
                            StartPT = new PointF(v.GetX(), v.GetY());
                            mainPointer2.SetY((int)(mainPointer2.GetY() + mv.Y));
                        }
                        if (((mainPointer1.GetY() + mv.Y + v.Height < polygonView.Height) && (mainPointer1.GetY() + mv.Y > 0)))
                        {
                            v.SetX((int)(StartPT.Y + mv.Y));
                            StartPT = new PointF(v.GetX(), v.GetY());
                            mainPointer1.SetY((int)(mainPointer1.GetY() + mv.Y));
                        }
                    }
                    else
                    {
                        if ((mainPointer2.GetX() + mv.X + v.Width < polygonView.Width) && (mainPointer2.GetX() + mv.X > 0))
                        {
                            v.SetX((int)(StartPT.X + mv.X));
                            StartPT = new PointF(v.GetX(), v.GetY());
                            mainPointer2.SetX((int)(mainPointer2.GetX() + mv.X));
                        }
                        if ((mainPointer1.GetX() + mv.X + v.Width < polygonView.Width) && (mainPointer1.GetX() + mv.X > 0))
                        {
                            v.SetX((int)(StartPT.X + mv.X));
                            StartPT = new PointF(v.GetX(), v.GetY());
                            mainPointer1.SetX((int)(mainPointer1.GetX() + mv.X));
                        }
                    }

                    break;

                case MotionEventActions.Down:

                    ScanActivity.allDraggedPointsStack.Push(new PolygonPoints(
                        new PointF(pointer1.GetX(), pointer1.GetY()),
                        new PointF(pointer2.GetX(), pointer2.GetY()),
                        new PointF(pointer3.GetX(), pointer3.GetY()),
                        new PointF(pointer4.GetX(), pointer4.GetY())));

                    DownPT.X = e.GetX();
                    DownPT.Y = e.GetY();
                    StartPT = new PointF(v.GetX(), v.GetY());
                    latestPoint = new PointF(v.GetX(), v.GetY());
                    latestPoint1 = new PointF(mainPointer1.GetX(), mainPointer1.GetY());
                    latestPoint2 = new PointF(mainPointer2.GetX(), mainPointer2.GetY());

                    break;

                case MotionEventActions.Up:

                    int color = 0;
                    if (IsValidShapeV2(GetPointsV2()) && IsValidPointer1() && IsValidPointer2() && IsValidPointer3() && IsValidPointer4())
                    {
                        color = Resource.Color.crop_color;
                        latestPoint.X = v.GetX();
                        latestPoint.Y = v.GetY();
                        latestPoint1.X = mainPointer1.GetX();
                        latestPoint1.Y = mainPointer1.GetY();
                        latestPoint2.X = mainPointer2.GetX();
                        latestPoint2.Y = mainPointer2.GetY();
                    }
                    else
                    {
                        ScanActivity.allDraggedPointsStack.Pop();
                        color = Resource.Color.crop_color;
                        v.SetX(latestPoint.X);
                        v.SetY(latestPoint.Y);
                        mainPointer1.SetX(latestPoint1.X);
                        mainPointer1.SetY(latestPoint1.Y);
                        mainPointer2.SetX(latestPoint2.X);
                        mainPointer2.SetY(latestPoint2.Y);
                    }
                    paint.Color = Resources.GetColor(color);

                    break;

                default:
                    break;
            }

            polygonView.Invalidate();
            return true;
        }

        #region temp

        private class TouchListenerImpl : Java.Lang.Object, IOnTouchListener
        {
            PointF DownPT = new PointF(); // Record Mouse Position When Pressed Down
            PointF StartPT = new PointF(); // Record Start Position of 'img'
            PointF latestPoint = new PointF();

            public bool OnTouch(View v, MotionEvent e)
            {
                var eid = e.Action;

                switch (eid)
                {
                    case MotionEventActions.Move:
                        PointF mv = new PointF(e.GetX() - DownPT.X, e.GetY() - DownPT.Y);
                        if (((StartPT.X + mv.X + v.Width) < polygonView.Width && (StartPT.Y + mv.Y + v.Height < polygonView.Height)) && ((StartPT.X + mv.X) > 0 && StartPT.Y + mv.Y > 0))
                        {
                            v.SetX((int)(StartPT.X + mv.X));
                            v.SetY((int)(StartPT.Y + mv.Y));
                            StartPT = new PointF(v.GetX(), v.GetY());
                        }
                        break;

                    case MotionEventActions.Down:
                        ScanActivity.allDraggedPointsStack.Push(new PolygonPoints(
                            new PointF(pointer1.GetX(), pointer1.GetY()),
                            new PointF(pointer2.GetX(), pointer2.GetY()),
                            new PointF(pointer3.GetX(), pointer3.GetY()),
                            new PointF(pointer4.GetX(), pointer4.GetY())));
                        DownPT.X = e.GetX();
                        DownPT.Y = e.GetY();
                        StartPT = new PointF(v.GetX(), v.GetY());
                        latestPoint = new PointF(v.GetX(), v.GetY());
                        break;

                    case MotionEventActions.Up:
                        int color = 0;

                        List<PointF> points = new List<PointF>();
                        points.Add(new PointF(pointer1.GetX(), pointer1.GetY()));
                        points.Add(new PointF(pointer2.GetX(), pointer2.GetY()));
                        points.Add(new PointF(pointer3.GetX(), pointer3.GetY()));
                        points.Add(new PointF(pointer4.GetX(), pointer4.GetY()));

                        var result = GetOrderedPointsV2(points);

                        if (IsValidShapeV2(result) /*&& IsValidPointer4() && IsValidPointer3() && IsValidPointer2() && IsValidPointer1()*/)
                        {
                            color = Resource.Color.crop_color;
                            latestPoint.X = v.GetX();
                            latestPoint.Y = v.GetY();
                        }
                        else
                        {
                            ScanActivity.allDraggedPointsStack.Pop();
                            color = Resource.Color.crop_color;
                            v.SetX(latestPoint.X);
                            v.SetY(latestPoint.Y);
                        }
                        paint.Color = Color.ParseColor("#00d1fe");
                        break;

                    default:
                        break;
                }

                polygonView.Invalidate();
                return true;
            }
        }

        private class MidPointTouchListenerImpl : Java.Lang.Object, IOnTouchListener
        {
            PointF DownPT = new PointF(); // Record Mouse Position When Pressed Down
            PointF StartPT = new PointF(); // Record Start Position of 'img'

            private ImageView mainPointer1;
            private ImageView mainPointer2;
            PointF latestPoint = new PointF();
            PointF latestPoint1 = new PointF();
            PointF latestPoint2 = new PointF();

            public MidPointTouchListenerImpl(ImageView mainPointer1, ImageView mainPointer2)
            {
                this.mainPointer1 = mainPointer1;
                this.mainPointer2 = mainPointer2;
            }

            public bool OnTouch(View v, MotionEvent e)
            {
                var eid = e.Action;

                switch (eid)
                {
                    case MotionEventActions.Move:

                        PointF mv = new PointF(e.GetX() - DownPT.X, e.GetY() - DownPT.Y);

                        if (Math.Abs(mainPointer1.GetX() - mainPointer2.GetX()) > Math.Abs(mainPointer1.GetY() - mainPointer2.GetY()))
                        {
                            if (((mainPointer2.GetY() + mv.Y + v.Height < polygonView.Height) && (mainPointer2.GetY() + mv.Y > 0)))
                            {
                                v.SetX((int)(StartPT.Y + mv.Y));
                                StartPT = new PointF(v.GetX(), v.GetY());
                                mainPointer2.SetY((int)(mainPointer2.GetY() + mv.Y));
                            }
                            if (((mainPointer1.GetY() + mv.Y + v.Height < polygonView.Height) && (mainPointer1.GetY() + mv.Y > 0)))
                            {
                                v.SetX((int)(StartPT.Y + mv.Y));
                                StartPT = new PointF(v.GetX(), v.GetY());
                                mainPointer1.SetY((int)(mainPointer1.GetY() + mv.Y));
                            }
                        }
                        else
                        {
                            if ((mainPointer2.GetX() + mv.X + v.Width < polygonView.Width) && (mainPointer2.GetX() + mv.X > 0))
                            {
                                v.SetX((int)(StartPT.X + mv.X));
                                StartPT = new PointF(v.GetX(), v.GetY());
                                mainPointer2.SetX((int)(mainPointer2.GetX() + mv.X));
                            }
                            if ((mainPointer1.GetX() + mv.X + v.Width < polygonView.Width) && (mainPointer1.GetX() + mv.X > 0))
                            {
                                v.SetX((int)(StartPT.X + mv.X));
                                StartPT = new PointF(v.GetX(), v.GetY());
                                mainPointer1.SetX((int)(mainPointer1.GetX() + mv.X));
                            }
                        }

                        break;

                    case MotionEventActions.Down:

                        ScanActivity.allDraggedPointsStack.Push(new PolygonPoints(
                            new PointF(pointer1.GetX(), pointer1.GetY()),
                            new PointF(pointer2.GetX(), pointer2.GetY()),
                            new PointF(pointer3.GetX(), pointer3.GetY()),
                            new PointF(pointer4.GetX(), pointer4.GetY())));

                        DownPT.X = e.GetX();
                        DownPT.Y = e.GetY();
                        StartPT = new PointF(v.GetX(), v.GetY());
                        latestPoint = new PointF(v.GetX(), v.GetY());
                        latestPoint1 = new PointF(mainPointer1.GetX(), mainPointer1.GetY());
                        latestPoint2 = new PointF(mainPointer2.GetX(), mainPointer2.GetY());

                        break;

                    case MotionEventActions.Up:

                        int color = 0;

                        List<PointF> points = new List<PointF>();
                        points.Add(new PointF(pointer1.GetX(), pointer1.GetY()));
                        points.Add(new PointF(pointer2.GetX(), pointer2.GetY()));
                        points.Add(new PointF(pointer3.GetX(), pointer3.GetY()));
                        points.Add(new PointF(pointer4.GetX(), pointer4.GetY()));

                        var result = GetOrderedPointsV2(points);

                        if (IsValidShapeV2(result) /*&& IsValidPointer1() && IsValidPointer2() && IsValidPointer3() && IsValidPointer4()*/)
                        {
                            color = Resource.Color.crop_color;
                            latestPoint.X = v.GetX();
                            latestPoint.Y = v.GetY();
                            latestPoint1.X = mainPointer1.GetX();
                            latestPoint1.Y = mainPointer1.GetY();
                            latestPoint2.X = mainPointer2.GetX();
                            latestPoint2.Y = mainPointer2.GetY();
                        }
                        else
                        {
                            ScanActivity.allDraggedPointsStack.Pop();
                            color = Resource.Color.crop_color;
                            v.SetX(latestPoint.X);
                            v.SetY(latestPoint.Y);
                            mainPointer1.SetX(latestPoint1.X);
                            mainPointer1.SetY(latestPoint1.Y);
                            mainPointer2.SetX(latestPoint2.X);
                            mainPointer2.SetY(latestPoint2.Y);
                        }
                        paint.Color = Color.ParseColor("#00d1fe");

                        break;

                    default:
                        break;
                }

                polygonView.Invalidate();
                return true;
            }
        }

        #endregion
    }
}