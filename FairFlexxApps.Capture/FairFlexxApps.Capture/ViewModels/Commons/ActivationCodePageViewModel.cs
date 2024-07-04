using System;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using FairFlexxApps.Capture.Interfaces.LocalDatabase;
using FairFlexxApps.Capture.Localization;
using FairFlexxApps.Capture.Managers;
using FairFlexxApps.Capture.Services;
using FairFlexxApps.Capture.ViewModels.Base;
using FairFlexxApps.Capture.Views.Popups;
using Prism.Commands;
using Prism.Navigation;

namespace FairFlexxApps.Capture.ViewModels.Commons
{
    public partial class ActivationCodePageViewModel : ViewModelBase
    {
        #region Properties

        private string _activationCode;
        public string ActivationCode
        {
            get => _activationCode;
            set => SetProperty(ref _activationCode, value);
        }

        #endregion

        #region Constructor

        public ActivationCodePageViewModel(INavigationService navigationService, ISqLiteService sqLiteService)
            : base(navigationService: navigationService, sqliteService: sqLiteService)
        {
            ActivationCodeCommand = new DelegateCommand(ActivationCodeExecute);
        }

        #endregion

        #region ActivationCodeCommand 

        public ICommand ActivationCodeCommand { get; set; }
        private async void ActivationCodeExecute()
        {
            await CheckBusy(async () =>
            {
                if (!string.IsNullOrEmpty(ActivationCode) && ActivationCode.Equals("54862"))
                {
                    await LoadingPopup.Instance.Show();

                    App.Settings.IsActivated = true;
                    SqLiteService.Insert(App.Settings);

                    // Navigate to Home page
                    await Navigation.NavigateAsync(new Uri(
                        $"{PageManager.NavigationHomeUri}/{PageManager.NavigationPage}/{PageManager.HomePage}"));

                    await LoadingPopup.Instance.Hide();

                }
                else
                {
                    await MessagePopup.Instance.Show(TranslateExtension.Get("InvalidVerificationCode"));
                }
            });
        }
        #endregion


        #region BackButtonPress

        /// <summary>
        /// //false is default value when system call back press
        /// </summary>
        /// <returns></returns>
        public override bool OnBackButtonPressed()
        {
            Task.Run(async () => { await BackExecute(); });

            return false;
        }

        #endregion

    }
}
