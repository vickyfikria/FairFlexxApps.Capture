using Microsoft.Maui;
using Microsoft.Maui.Controls.Xaml;

namespace FairFlexxApps.Capture.Views.ViewCells
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class UploadLeadListContentView : ContentView
	{
		public UploadLeadListContentView ()
		{
			InitializeComponent ();

            FrameUpload.WidthRequest = App.ScreenWidth / 1.5;
        }

        #region EventName

        /// <summary>
        /// Tao EventNameProperty de set title cho page
        /// </summary>
        public string EventName
        {
            get { return (string)GetValue(EventNameProperty); }
            set { SetValue(EventNameProperty, value); }

        }

        public static readonly BindableProperty EventNameProperty =
            BindableProperty.Create(nameof(EventName),
                typeof(string),
                typeof(UploadLeadListContentView),
                string.Empty,
                BindingMode.TwoWay,
                propertyChanged: OnEventNameChanged);

        private static void OnEventNameChanged(BindableObject bindable, object oldValue, object newValue)
        {
            ((UploadLeadListContentView)bindable).OnEventNameChanged((string)newValue);
        }

        private void OnEventNameChanged(string title)
        {
            lblEventName.Text = title;
        }

        #endregion

        #region NumberLeads

        /// <summary>
        /// Tao NumberLeadsProperty de set title cho page
        /// </summary>
        public string NumberLeads
        {
            get { return (string)GetValue(NumberLeadsProperty); }
            set { SetValue(NumberLeadsProperty, value); }

        }

        public static readonly BindableProperty NumberLeadsProperty =
            BindableProperty.Create(nameof(NumberLeads),
                typeof(string),
                typeof(UploadLeadListContentView),
                string.Empty,
                BindingMode.TwoWay,
                propertyChanged: OnNumberLeadsChanged);

        private static void OnNumberLeadsChanged(BindableObject bindable, object oldValue, object newValue)
        {
            ((UploadLeadListContentView)bindable).OnNumberLeadsChanged((string)newValue);
        }

        private void OnNumberLeadsChanged(string numberLeads)
        {
            lblLeadNumbers.Text = numberLeads;
        }

        #endregion
    }
}
