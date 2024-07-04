using System;
using System.Diagnostics;
using System.Linq;
using FairFlexxApps.Capture.ViewModels.Base;
using Microsoft.Maui.Controls;

namespace FairFlexxApps.Capture.Managers
{
    public class PageManager
    {
        #region Properties

        public static readonly string TestDemoPage = "TestDemoPage";
        public static readonly string TestPage = "TestPage";
        public static readonly string TempPage = "TempPage";
        public static readonly string DemoPage = "DemoPage";

        public const string NavigationHomeUri = "https://fairflexx.fotoscan.com/";
        public static readonly string NavigationPage = "NavigationPage";

        // Main Page
        public static readonly string MainPage = "MainPage";
        public static readonly string HomePage = "HomePage";

        public static readonly string NewContactPage = "NewContactPage";
        public static readonly string NewLeadTemplatePage = "NewLeadTemplatePage"; 
        public static readonly string NewLeadTemplatePhonePage = "NewLeadTemplatePhonePage"; 
        public static readonly string ScanQrCodePage = "ScanQrCodePage";
        public static readonly string ScanQrCodeiOSPage = "ScanQrCodeiOSPage";

        public static readonly string ImageViewerPage = "ImageViewerPage";
        public static readonly string FilterPage = "FilterPage";

        public static readonly string LoginPage = "LoginPage";
        public static readonly string ActivationCodePage = "ActivationCodePage";
        public static readonly string SelectFontSizePage = "SelectFontSizePage";
        public static readonly string SettingsPage = "SettingsPage";
        public static readonly string WebViewPage = "WebViewPage";
        public static readonly string LeadListPage = "LeadListPage";
        public static readonly string UploadLeadList = "UploadLeadList";
        public static readonly string LeadListDetail = "LeadListDetail";
        public static readonly string ErrorLogPage = "ErrorLogPage";

        // Upload Lead Event 
        public static readonly string TheEventListPage = "TheEventListPage";
        public static readonly string TheLeadListPage = "TheLeadListPage";
        public static readonly string UploadLeadListPage = "UploadLeadListPage";

        // Lead Counter
        public static readonly string LeadCounterPage = "LeadCounterPage"; 

        #endregion

        #region MultiplePages

        public static string MultiplePages(string[] pages)
        {
            var path = "";
            if (pages == null) return "";
            if (pages.Length < 1) return "";
            for (var i = 0; i < pages.Length; i++)
            {
                path += i == 0 ? pages[i] : "/" + pages[i];
            }
            return path;
        }

        #endregion

        #region GetCurrentPage

        public static Page GetCurrentPage()
        {
            var mainPage = Application.Current.MainPage;
            var navStack = mainPage.Navigation.NavigationStack;

            if (navStack == null)
                return mainPage;

            if (navStack.Count < 1)
                return mainPage;

            return navStack.Last();
        }

        public static Page GetCurrentPage(bool withModal)
        {

            if (!withModal) return GetCurrentPage();
            try
            {
                var navPage = GetCurrentPage();
                var modalPage = navPage.Navigation.ModalStack.LastOrDefault();
                var foundedPage = modalPage ?? navPage;
                return foundedPage;
            }
            catch (Exception e)
            {
#if DEBUG
                Debug.WriteLine(e.Message);
#endif
                return null;
            }
        }

        public static ViewModelBase GetCurrentPageBaseViewModel()
        {
            return (ViewModelBase)GetCurrentPage(true).BindingContext;
        }

        #endregion

        #region Home

        public static Uri Home(string page = "")
        {
            return new Uri($"{NavigationHomeUri}{NavigationPage}/{HomePage}/{page}");
        }

        #endregion

        #region GoBack

        /// <summary>
        /// 
        /// </summary>
        /// <param name="number"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        public static string GoBack(string page = "", int number = 1)
        {
            var home = "../";

            var mainPage = Application.Current.MainPage;

            var navStack = mainPage.Navigation.NavigationStack;
            if (number < 1 || number >= navStack.Count)
                return "";


            for (; number < navStack.Count; number++)
            {
                home += "../";
            }

            return $"{home}{page}";
        }

        #endregion

    }
}
