using FairFlexxApps.Capture.Models.LeadModels;
using FairFlexxApps.Capture.ViewModels.NewLeadFlows;
using System;
using System.IO;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace FairFlexxApps.Capture.Views.ViewCells.TopMenuViews
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class AttachmentContentView : ContentView
	{
		public AttachmentContentView ()
		{
			InitializeComponent ();
            attachView.HeightRequest = App.ScreenHeight - 125;

			NewLeadTemplatePageViewModel.ImageView += ShowImage;
		}

		private void ShowImage(object sender, EventArgs e)
		{
			imgView.Source = null;
			var item = (ScannerResult)sender;
			var byteImage = item.ByteImage;
			var stream = new MemoryStream(byteImage);
			var imageSource = Xamarin.Forms.ImageSource.FromStream(() => stream);
			imgView.Source = imageSource;

			//NewLeadTemplatePageViewModel.ImageView -= ShowImage;
		}

		#region ZoomImage

		private async void ZoomImage(object sender, EventArgs e)
		{
			await NewLeadTemplatePageViewModel.Instance.ZoomImageWithPos();
		}

		#endregion
	}
}