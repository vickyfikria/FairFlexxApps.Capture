using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FairFlexxApps.Capture.ViewModels.Commons;
using FairFlexxApps.Capture.Views.Base;
using Microsoft.Maui;

namespace FairFlexxApps.Capture.Views.Commons
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class WebViewPage : BasePage
    {
        public WebViewPage(WebViewPageViewModel vm)
        {
            InitializeComponent();
            BindingContext = vm;
        }

        private void webviewNavigating(object sender, WebNavigatingEventArgs e)
        {
            labelLoading.IsVisible = true;
            //UpdateBackAndForwardButton();
        }

        private void webviewNavigated(object sender, WebNavigatedEventArgs e)
        {
            labelLoading.IsVisible = false;
            //UpdateBackAndForwardButton();
        }

        private void OnBackButtonClicked(object sender, EventArgs e)
        {
            //UpdateBackAndForwardButton();
            if (webView.CanGoBack)
            {
                webView.GoBack();
            }

        }

        private void OnForwardButtonClicked(object sender, EventArgs e)
        {
            //UpdateBackAndForwardButton();
            if (webView.CanGoForward)
            {
                webView.GoForward();
            }
        }

        //private void UpdateBackAndForwardButton()
        //{
        //    BackButton.IsEnabled = webView.CanGoBack;
        //    ForwardButton.IsEnabled = webView.CanGoForward;
        //}
    }
}