using FairFlexxApps.Capture.Enums;
using FairFlexxApps.Capture.Interfaces;
using FairFlexxApps.Capture.Interfaces.LocalDatabase;
using FairFlexxApps.Capture.Services;
using FairFlexxApps.Capture.ViewModels.Base;

namespace FairFlexxApps.Capture.ViewModels.Commons
{
    public class WebViewPageViewModel : ViewModelBase
    {
        #region WebViewPageViewModel

        //public static NewLeadTemplatePageViewModel Instance { get; private set; }
        public WebViewPageViewModel(INavigationService navigationService, ISqLiteService sqLiteService, IFileService fileService)
            : base(navigationService: navigationService, sqliteService: sqLiteService, fileService: fileService)
        {

        }

        #endregion

        #region Properties

        private string _webViewUrl;
        public string WebViewUrl
        {
            get => _webViewUrl;
            set => SetProperty(ref _webViewUrl, value);
        }

        #endregion

        #region Navigate

        public override void OnNavigatedNewTo(INavigationParameters parameters)
        {
            if (parameters != null)
            {
                if (parameters.ContainsKey(ParamKey.WebView.ToString()))
                {
                    WebViewUrl = (string)parameters[ParamKey.WebView.ToString()];
                }
            }
        }

        #endregion
    }
}
