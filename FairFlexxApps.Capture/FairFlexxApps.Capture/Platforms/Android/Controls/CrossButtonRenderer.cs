using System.ComponentModel;
using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;

using Android.Util;
using Android.Views;
using AndroidX.Core.Content;
using FairFlexxApps.Capture.Controls;
using FairFlexxApps.Capture.Droid.Controls;
using Microsoft.Maui.Controls.Compatibility;
using Microsoft.Maui.Controls.Compatibility.Platform.Android;
using Microsoft.Maui.Controls.Compatibility.Platform.Android.AppCompat;
using Microsoft.Maui.Controls.Platform;

[assembly: ExportRenderer(typeof(CrossButton), typeof(CrossButtonRenderer))]
namespace FairFlexxApps.Capture.Droid.Controls
{
    public class CrossButtonRenderer : ButtonRenderer
    {
        public CrossButtonRenderer(Context context) : base(context)
        {
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Button> e)
        {
            base.OnElementChanged(e);

            if (e.NewElement != null)
            {
                var element = (CrossButton)e.NewElement;

                // Set text in ALL CAPS mode or not.
                this.Control.SetAllCaps(element.Upper);

                // Set max line
                if (element.MaxLine != 0)
                    Control.SetMaxLines(element.MaxLine);

                this.Control.SetPadding(0, 0, 0, 0);

                #region Type

                if (element.Type == ButtonType.DisabledButton)
                {
                    SetColors(element);
                }

                #endregion

                #region IconProperty

                var editText = this.Control;

                if (!string.IsNullOrEmpty(element.Image))
                {
                    editText.SetCompoundDrawablesWithIntrinsicBounds(
                        GetDrawable(imageButton: element.Image, imageHeight: element.IconHeight,
                            imageWidth: element.IconWidth), null,
                        null, null);
                    //editText.Gravity = GravityFlags.CenterVertical;
                    editText.Gravity = GravityFlags.Center;

                    editText.CompoundDrawablePadding = (int)(element.IconSpacing);

                }

                #endregion

            }
        }

        #region OnElementPropertyChanged

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);
            var element = (CrossButton)sender;
            if (e.PropertyName == nameof(Button.IsEnabled) && element.Type == ButtonType.DisabledButton)
                SetColors(element);
            if (e.PropertyName == nameof(Button.Image))
            { 
                Control.SetCompoundDrawablesWithIntrinsicBounds(
                    GetDrawable(imageButton: element.Image, imageHeight: element.IconHeight,imageWidth: element.IconWidth), null,null, null);
            Control.CompoundDrawablePadding = (int)(element.IconSpacing);
            }
        }

        #endregion

        #region Set Color with disabled button

        private void SetColors(CrossButton btn)
        {
            if (Element.IsEnabled)
                Element.BackgroundColor = btn.EnableColor;
            else
                Element.BackgroundColor = btn.DisableColor;
            Control.SetTextColor(Element.IsEnabled ? btn.TextEnableColor.ToAndroid() : btn.TextDisableColor.ToAndroid());

            #endregion
        }

        #region GetDrawable

        private BitmapDrawable GetDrawable(string imageButton, int imageHeight, int imageWidth)
        {
            var resId = (int)typeof(Resource.Drawable)
                .GetField(imageButton.Replace(".jpg", "").Replace(".png", "")).GetValue(null);
            var drawable = ContextCompat.GetDrawable(this.Context, resId);
            var bitmap = ((BitmapDrawable)drawable).Bitmap;
            if (App.DisplayScaleFactor >=1.5)
            {
                imageHeight = (int)(imageHeight * App.DisplayScaleFactor);
                imageWidth = (int)(imageWidth * App.DisplayScaleFactor);
            }
            return new BitmapDrawable(Resources,
                Bitmap.CreateScaledBitmap(bitmap, (int)(imageWidth), (int)(imageHeight), true));
        }
        #endregion
    }
}
