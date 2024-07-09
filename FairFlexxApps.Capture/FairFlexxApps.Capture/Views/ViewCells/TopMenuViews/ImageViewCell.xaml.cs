using FairFlexxApps.Capture.Models.LeadModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

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