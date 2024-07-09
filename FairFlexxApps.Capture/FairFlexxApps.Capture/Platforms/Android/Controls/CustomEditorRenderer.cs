using Android.Content;
using FairFlexxApps.Capture.Controls;
using FairFlexxApps.Capture.Droid.Controls;
using Microsoft.Maui.Controls.Compatibility;
using Microsoft.Maui.Controls.Compatibility.Platform.Android;
using Microsoft.Maui.Controls.Platform;


[assembly: ExportRenderer(typeof(CustomEditor), typeof(CustomEditorRenderer))]
namespace FairFlexxApps.Capture.Droid.Controls
{
    public class CustomEditorRenderer : EditorRenderer
    {
        public CustomEditorRenderer(Context context) : base(context)
        {
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Editor> e)
        {
            base.OnElementChanged(e);

            if (e.NewElement != null)
            {
                var element = (CustomEditor)e.NewElement;

                #region IsBorderlessUnderline

                // Set background is borderless underline
                if (element.IsBorderlessUnderline)
                    this.Control.Background = null;

                #endregion

            }

            if (e.OldElement != null)
            {

            }

        }
    }
}
