using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Input;
using FairFlexxApps.Capture.Interfaces;
using FairFlexxApps.Capture.Interfaces.HttpService;
using FairFlexxApps.Capture.Interfaces.LocalDatabase;
using FairFlexxApps.Capture.Localization;
using FairFlexxApps.Capture.Managers;
using FairFlexxApps.Capture.Models;
using FairFlexxApps.Capture.Services.HttpService;
using FairFlexxApps.Capture.Utilities;
using FairFlexxApps.Capture.Views.Popups;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using Prism.Services;
using FairFlexxApps.Capture.Models.LeadModels;
using System.IO;
using Microsoft.AppCenter.Crashes;
using IAppInfo = FairFlexxApps.Capture.Interfaces.IAppInfo;
using NavigationMode = Prism.Navigation.NavigationMode;

namespace FairFlexxApps.Capture.ViewModels.Base
{
    public class ViewModelBase : BindableBase, INavigationAware, IDestructible, INotifyPropertyChanged
    {
        #region Properties Services

        //protected INavigationService NavigationService { get; private set; }
        public INavigationService Navigation { get; private set; }
        public IPageDialogService DialogService { get; private set; }
        public IHttpRequest HttpRequest { get; private set; }
        public ISqLiteService SqLiteService { get; private set; }
        public IFileService FileService { get; private set; }
        public IAppInfo AppInfo { get; private set; }

        #endregion

        #region Properties

        private string _webUrl;
        public string WebUrl
        {
            get => _webUrl;
            set => SetProperty(ref _webUrl, value);
        }
        private string _title;
        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }

        private UserModel _userInfo;
        public UserModel UserInfo
        {
            get => _userInfo;
            set => SetProperty(ref _userInfo, value);
        }

        private IList<EventModel> _listEvents;
        public IList<EventModel> ListEvents
        {
            get => _listEvents;
            set => SetProperty(ref _listEvents, value);
        }

        private EventModel _selectedEvent;
        public EventModel SelectedEvent
        {
            get => _selectedEvent;
            set => SetProperty(ref _selectedEvent, value);
        }

        private ObservableCollection<EventModel> _leadEventList = new ObservableCollection<EventModel>();
        public ObservableCollection<EventModel> LeadEventList
        {
            get => _leadEventList;
            set => SetProperty(ref _leadEventList, value);
        }

        #endregion

        #region Constructor

        public ViewModelBase(INavigationService navigationService = null, IPageDialogService dialogService = null,
            IHttpRequest httpRequest = null, ISqLiteService sqliteService = null,
            IFileService fileService = null, IAppInfo appInfo = null)
        {
            //PageTitle = TranslateExtension.Get("FairflexxFotoScan");
            if (navigationService != null) Navigation = navigationService;
            if (dialogService != null) DialogService = dialogService;
            if (httpRequest != null) HttpRequest = httpRequest;
            if (sqliteService != null) SqLiteService = sqliteService;
            if (fileService != null) FileService = fileService;
            if (appInfo != null) AppInfo = appInfo;

            SettingsCommand = new DelegateCommand(async () => await SettingsExe());
            OpenSettingsPageCommand = new DelegateCommand(async () => await OpenSettingsPageExe());
            BackCommand = new DelegateCommand(async () => await BackExecute());
            ZoomImageCommand = new DelegateCommand<object>(ZoomImageExe);
        }

        #endregion

        #region CheckBusy

        protected async Task CheckBusy(Func<Task> function)
        {
            if (App.IsBusy)
            {
                App.IsBusy = false;
                try
                {
                    await function();
                }
                catch (Exception e)
                {
                    WriteErrorLog(message: e.Message, ex: e);

#if DEBUG
                    Debug.WriteLine(e);

                    await MessagePopup.Instance.Show(e.Message);
#endif
                }
                finally
                {
                    App.IsBusy = true;
                }
            }
        }

        #endregion

        #region Check Permission

        protected async void CheckPermission(Action action)
        {
            await CheckBusy(async () =>
            {
                // .NET MAUI Camera permission
                var camera_permission = await Permissions.CheckStatusAsync<Permissions.Camera>();
                if (camera_permission == PermissionStatus.Granted)
                {
                    action();
                }

                if (Permissions.ShouldShowRationale<Permissions.Camera>())
                {
                    await Shell.Current.DisplayAlert("Need permissions", "BECAUSE!!!", "OK");
                }
                camera_permission = await Permissions.RequestAsync<Permissions.Camera>();
                if (camera_permission != PermissionStatus.Granted)
                    await Shell.Current.DisplayAlert("Permission required", "Camera cannot be used", "Cancel");

            });
        }

        #endregion

        #region GetByteImage

        public byte[] GetByteImage(string filePath)
        {
            return File.ReadAllBytes(filePath);
        }

        #endregion

        #region Navigate

        public virtual void OnNavigatedFrom(INavigationParameters parameters)
        {
#if DEBUG
            Debug.WriteLine("Navigated from");
#endif
        }

        public virtual void OnNavigatingTo(INavigationParameters parameters)
        {
#if DEBUG
            Debug.WriteLine("Navigating to");
#endif
        }

        public void OnNavigatedTo(INavigationParameters parameters)
        {
#if DEBUG
            Debug.WriteLine("Navigated to");
#endif 

            if (parameters != null)
            {
                var navMode = parameters.GetNavigationMode();
                switch (navMode)
                {
                    case NavigationMode.New: OnNavigatedNewTo(parameters); break;
                    case NavigationMode.Back: OnNavigatedBackTo(parameters); break;
                }
            }

        }

        public virtual void OnNavigatedNewTo(INavigationParameters parameters)
        {
#if DEBUG
            Debug.WriteLine("Navigate new to");
#endif
        }

        public virtual void OnNavigatedBackTo(INavigationParameters parameters)
        {
#if DEBUG
            Debug.WriteLine("Navigate back to");
#endif
        }

        #endregion

        #region OnAppear / Disappear

        public virtual void OnAppear()
        {

        }

        public virtual void OnFirstTimeAppear()
        {

        }

        public virtual void OnDisappear()
        {

        }

        #endregion

        #region BackCommand

        public ICommand BackCommand { get; }

        protected virtual async Task BackExecute()
        {
            await CheckBusy(async () =>
            {
                await Navigation.GoBackAsync();
            });
        }

        #endregion

        #region BackButtonPress

        /// <summary>
        /// //false is default value when system call back press
        /// </summary>
        /// <returns></returns>
        public virtual bool OnBackButtonPressed()
        {
            //false is default value when system call back press
            //return false;
            //BackExecute();

            return false;

        }

        /// <summary>
        /// called when page need override soft back button
        /// </summary>
        public virtual void OnSoftBackButtonPressed() { }

        #endregion

        #region Zoom

        public ICommand ZoomImageCommand { get; }
        protected virtual void ZoomImageExe(object obj)
        {

        }

        #endregion

        #region SettingsCommand

        public ICommand SettingsCommand { get; set; }

        public virtual async Task SettingsExe()
        {
            await CheckBusy(async () => await Navigation.NavigateAsync(PageManager.SettingsPage));
        }

        #endregion

        #region AddHeaderToken

        public async void AddHeaderToken(string token)
        {
            var checkHeader = HttpRequest.DefaultRequestHeaders.Authorization;

            if (checkHeader != null)
            {
                HttpRequest.DefaultRequestHeaders.Remove("Authorization");
            }

            //var tokenExpired =
            //    "bearer SmAvxJyV4O3mSxnc1ScUFSQP6xYJvdBLgf6nsB-b7AJDz_xqh_4PDozu6lr8OkFQ_rrQTqp31GsPiUNl-kxyIaVQFlmwZqWbq1xzpzCiqkavN6AwNoxI2bk983XLo4l3tERounXWl6-haJhbIknicJsafLTqzopAd0WMnEVodb94XpOh6Ss53AQ02GFjkKwNRcM8AQlrvdBLxiM774_fi1wRbNEf0Fcp_CIsOs0AlmkfkLMd-bpvwitDAVwXoFpOIJ5XnwULsoe4cHbVUgwsA";
            await SecureStorage.Default.SetAsync("AccessToken", token); // .NET MAUI SetValue
            //CrossSecureStorage.Current.SetValue("AccessToken", token); // Old Xamarin source
            HttpRequest.DefaultRequestHeaders.Add("Authorization", token);

            IsTokenExpire = false;
        }

        #endregion

        #region GetTemplateModel

        public Template GetTemplateModel(string innerXml)
        {
            Serializer ser = new Serializer();
            var template = new Template();
            template = ser.Deserialize<Template>(innerXml);
            return template;
        }

        #endregion

        #region Token Expired

        public bool IsTokenExpire { get; set; } = false;

        #region Properties

        private string _username;
        public string Username
        {
            get => _username;
            set => SetProperty(ref _username, value);
        }

        private string _password;
        public string Password
        {
            get => _password;
            set => SetProperty(ref _password, value);
        }

        #endregion

        public async Task LogOutAsync(string pageViewModel = null)
        {
            await Task.Run(async () =>
            {
                // todo: Call API Logout
                SqLiteService.DeleteAll<UserModel>();
                //SqLiteService.DeleteAll<EventModel>();

                ////todo: Delete token header
                HttpRequest.DefaultRequestHeaders.Remove("Authorization");
                await SecureStorage.Default.SetAsync("AccessToken", string.Empty); // .NET MAUI SetValue
                //CrossSecureStorage.Current.SetValue("AccessToken", string.Empty); // Old Xamarin source

                SelectedEvent = null;
                UserInfo = null;

                Username = Password = string.Empty;
                App.Settings.IsAutoLogin = false;
                SqLiteService.Insert(App.Settings);

                // Navigate to Login page
                //if (!(pageViewModel.Contains(PageManager.SettingsPage)))
                //{
                //    await DeviceExtension.BeginInvokeOnMainThreadAsync(async () =>
                //    {
                //        await OpenSettingsPageExe();
                //    });
                //}

            });
        }

        #region OpenSettingsPageCommand

        public ICommand OpenSettingsPageCommand { get; set; }

        public virtual async Task OpenSettingsPageExe()
        {
            await Navigation.NavigateAsync(PageManager.SettingsPage);
        }

        #endregion

        #endregion

        #region Reset Event List

        public void ResetEventList()
        {
            foreach (var item in ListEvents)
            {
                item.IsDeleted = true;
            }
            
            SqLiteService.DeleteAll<EventModel>();
            SqLiteService.DeleteAll<LeadModel>();
            SqLiteService.InsertAll(ListEvents);
        }

        #endregion

        #region WriteErrorLog

        public void WriteErrorLog(string message = null, Exception ex = null)
        {
            App.Settings.LastError = (message == null) ? string.Empty : message;
            SqLiteService.Insert(App.Settings);

            if (ex != null)
                Crashes.TrackError(ex);
        }

        #endregion

        #region Destroy

        public virtual void Destroy()
        {

        }

        #endregion
    }
}
