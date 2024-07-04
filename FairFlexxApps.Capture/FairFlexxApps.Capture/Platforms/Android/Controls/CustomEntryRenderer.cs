using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Support.V4.Content;
using FairFlexxApps.Capture.Controls;
using FairFlexxApps.Capture.Droid.Controls;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using Color = Xamarin.Forms.Color;

[assembly: ExportRenderer(typeof(CustomEntry), typeof(CustomEntryRenderer))]
namespace FairFlexxApps.Capture.Droid.Controls
{
    public class CustomEntryRenderer : EntryRenderer
    {
        public CustomEntryRenderer(Context context) : base(context)
        {

        }
        protected override void OnElementChanged(ElementChangedEventArgs<Entry> e)
        {
            base.OnElementChanged(e);
            
            if (e.NewElement != null)
            {
                var element = (CustomEntry)this.Element;

                #region IconImage

                var editText = this.Control;

                if (!string.IsNullOrEmpty(element.Image))
                {
                    switch (element.ImageAlignment)
                    {
                        case ImageAlignment.Left:
                            editText.SetCompoundDrawablesWithIntrinsicBounds(
                                GetDrawable(imageEntryImage: element.Image, imageHeight: element.ImageHeight,
                                    imageWidth: element.ImageWidth), null, null, null);
                            break;
                        case ImageAlignment.Right:
                            editText.SetCompoundDrawablesWithIntrinsicBounds(null, null,
                                GetDrawable(imageEntryImage: element.Image, imageHeight: element.ImageHeight,
                                    imageWidth: element.ImageWidth), null);
                            break;
                    }

                }

                #endregion

                #region LineColor

                Control.Background.SetColorFilter(element.LineColor.ToAndroid(), PorterDuff.Mode.SrcAtop);

                #endregion

                #region IsBoderlessUnderline

                // Set background is borderless
                if (element.IsBorderlessUnderline)
                    this.Control.Background = null;

                #endregion

                #region HasRoundedCorner

                if (element.HasRoundedCorner)
                {
                    ApplyBorder(cornerBorder: element.ConnerBorder, borderWidth: element.BorderWidth,
                        strokeColor: element.StrokeColor);
                }

                #endregion
            }

            if (e.OldElement == null)
            {
               
            }
        }

        private BitmapDrawable GetDrawable(string imageEntryImage, int imageHeight, int imageWidth)
        {
            var resId = (int)typeof(Resource.Drawable)
                .GetField(imageEntryImage.Replace(".jpg", "").Replace(".png", "")).GetValue(null);
            var drawable = ContextCompat.GetDrawable(this.Context, resId);
            var bitmap = ((BitmapDrawable)drawable).Bitmap;
            if (App.DisplayScaleFactor >= 1.5)
            {
                imageHeight = (int)(imageHeight * App.DisplayScaleFactor);
                imageWidth = (int)(imageWidth * App.DisplayScaleFactor);
            }
            return new BitmapDrawable(Resources,
                Bitmap.CreateScaledBitmap(bitmap, imageWidth, imageHeight, true));
        }

        private void ApplyBorder(int cornerBorder, int borderWidth, Color strokeColor)
        {
            //var nativeEditText = (EditText)Control;
            //var shape = new ShapeDrawable(new Android.Graphics.Drawables.Shapes.RectShape());
            //shape.Paint.Color = element.StrokeColor.ToAndroid();
            //shape.Paint.SetStyle(Paint.Style.Stroke);
            //nativeEditText.Background = shape;

            GradientDrawable gd = new GradientDrawable();
            gd.SetColor(Android.Graphics.Color.White);
            gd.SetCornerRadius(cornerBorder);
            gd.SetStroke(borderWidth, strokeColor.ToAndroid());
            this.Control.Background = gd;
        }
    }
}