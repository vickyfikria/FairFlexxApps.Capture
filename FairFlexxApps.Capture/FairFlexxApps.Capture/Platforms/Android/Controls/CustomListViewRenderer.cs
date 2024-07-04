using Android.Content;
using FairFlexxApps.Capture.Droid.Controls;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(ListView), typeof(CustomListViewRenderer))]

namespace FairFlexxApps.Capture.Droid.Controls
{
    public class CustomListViewRenderer : ListViewRenderer
    {
        public CustomListViewRenderer(Context context) : base(context)
        {

        }

        protected override void OnElementChanged(ElementChangedEventArgs<ListView> e)
        {
            base.OnElementChanged(e);

            if (e.NewElement == null)
                return;

            if (Control != null)
            {
                Control.NestedScrollingEnabled = true;
            }
        }

    }
}