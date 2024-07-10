using FairFlexxApps.Capture.Models.LeadModels;

namespace FairFlexxApps.Capture.Views.ViewCells.TopMenuViews
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ImageViewCell : ViewCell
    {
        public ImageViewCell()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnBindingContextChanged();

            var item = (ScannerResult)BindingContext;
            //UpdateImageSource(item);
        }
    }
}
