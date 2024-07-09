using System;
using System.IO;
using FairFlexxApps.Capture.Controls;
using FairFlexxApps.Capture.Models.LeadModels;
using FairFlexxApps.Capture.ViewModels.NewLeadFlows;
using FairFlexxApps.Capture.Views.Popups;
using Rg.Plugins.Popup.Services;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace FairFlexxApps.Capture.Views.ViewCells
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class BusinessCardViewCell : ViewCell
    {
        public BusinessCardViewCell()
        {
            InitializeComponent();
           
        }

        //protected override void OnBindingContextChanged()
        //{
        //    base.OnBindingContextChanged();

        //    base.OnBindingContextChanged();

        //    if (BindingContext == null)
        //        return;

        //    var item = (LeadTypeModel)BindingContext;

        //    UpdateImageSource(item);
        //}
        protected override void OnAppearing()
        {
            base.OnBindingContextChanged();

            var item = (LeadTypeModel)BindingContext;
            UpdateImageSource(item);
        }

        private void UpdateImageSource(LeadTypeModel leadTypeModel)
        {
            var byteImage = leadTypeModel.ScannerResult[leadTypeModel.Position].ByteImage;
            var stream = new MemoryStream(byteImage);
            var imageSource = Xamarin.Forms.ImageSource.FromStream(() => stream);
            SourceCard.Source = imageSource;
        }

        private void FlipBusinessCardOnTapped(object sender, EventArgs e)
        {
            var items = (LeadTypeModel)BindingContext;

            var pos = items.Position;

            var totalPages = items.ScannerResult.Count;

            pos = (pos == totalPages - 1) ? 0 : pos + 1;

            items.Position = pos;

            UpdateImageSource(items);
        }

        private void DeleteBusinessCardOnTapped(object sender, EventArgs e)
        {
            var item = (LeadTypeModel)BindingContext;
            var leadTypeId = (string)item.StringId;

            //var vm = (NewLeadTemplatePageViewModel)BindingContext;
            //vm?.DeleteBusinessCardExecute(leadTypeId: leadTypeId);
            NewLeadTemplatePageViewModel.Instance.DeleteBusinessCardExecute(leadTypeId);
        }

        private async void ZoomImageCard(object sender, EventArgs e)
        {
            var item = (LeadTypeModel)BindingContext;
            var byteImage = item.ScannerResult[item.Position].ByteImage;
            NewLeadTemplatePageViewModel.Instance.ZoomImageCard(obj: byteImage);

            //await PopupNavigation.Instance.PushAsync(new ZoomImagePopUp(byteImage));
        }
    }
}