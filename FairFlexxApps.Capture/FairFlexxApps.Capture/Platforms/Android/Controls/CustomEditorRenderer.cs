using FairFlexxApps.Capture.Controls;
using FairFlexxApps.Capture.Droid.Controls;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(CustomEditor), typeof(CustomEditorRenderer))]
namespace FairFlexxApps.Capture.Droid.Controls
{
    public class CustomEditorRenderer : EditorRenderer
    {
        public CustomEditorRenderer() : base()
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