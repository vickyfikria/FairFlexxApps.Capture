using FairFlexxApps.Capture.Models.LeadModels;
using FairFlexxApps.Capture.ViewModels.NewLeadFlows;
using System;
using System.IO;
using System.Runtime.CompilerServices;
using Microsoft.Maui;
using Microsoft.Maui.Controls.Xaml;

namespace FairFlexxApps.Capture.Views.ViewCells.TopMenuViews
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class ObjectContentView : ContentView
	{
		public ObjectContentView ()
		{
			InitializeComponent ();
            objectView.HeightRequest = App.ScreenHeight - 125;

			NewLeadTemplatePageViewModel.ImageView += ShowImage;
		}

		private void ShowImage(object sender, EventArgs e)
		{
			imgView.Source = null;
			var item = (ScannerResult)sender;
			var byteImage = item.ByteImage;
			var stream = new MemoryStream(byteImage);
			var imageSource = ImageSource.FromStream(() => stream);
			imgView.Source = imageSource;

			//NewLeadTemplatePageViewModel.ImageView -= ShowImage;
		}

		#region ZoomImage

		private async void ZoomImage(object sender, EventArgs e)
		{
			await NewLeadTemplatePageViewModel.Instance.ZoomImageWithPos();
		}

		#endregion

		protected override void OnBindingContextChanged()
		{
			base.OnBindingContextChanged();
			var a = BindingContext;
		}
	}
}
