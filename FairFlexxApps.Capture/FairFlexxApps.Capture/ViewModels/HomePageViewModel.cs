using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using FairFlexxApps.Capture.Enums;
using FairFlexxApps.Capture.Interfaces.HttpService;
using FairFlexxApps.Capture.Interfaces.LocalDatabase;
using FairFlexxApps.Capture.Localization;
using FairFlexxApps.Capture.Managers;
using FairFlexxApps.Capture.Models;
using FairFlexxApps.Capture.Models.LeadModels;
using FairFlexxApps.Capture.Services.HttpService;
using FairFlexxApps.Capture.Utilities;
using FairFlexxApps.Capture.ViewModels.Base;
using FairFlexxApps.Capture.Views.Popups;
using Prism.Commands;
using Prism.Navigation;
using Microsoft.Maui;

namespace FairFlexxApps.Capture.ViewModels
{
    public class HomePageViewModel : ViewModelBase
    {
        #region Properties
        private bool _isLeadExist;
        public bool IsLeadExist
        {
            get => _isLeadExist;
            set => SetProperty(ref _isLeadExist, value);
        }
        #endregion
        #region HomePageViewModel

        public HomePageViewModel(INavigationService navigationService, ISqLiteService sqLiteService)
            : base(navigationService: navigationService, sqliteService: sqLiteService)
        {
            Title = "Home Page";

            LeadWithCardCommand = new DelegateCommand(LeadWithCardExecute);
            LeadWithoutCardCommand = new DelegateCommand(LeadWithoutCardExecute);
            ContactListCommand = new DelegateCommand(ContactListExecute);
            LeadCounterCommand = new DelegateCommand(LeadCounterExecute);
            OpenPreferencesPageCommand = new DelegateCommand(OpenPreferencesPageExecute);
        }

        #endregion

        public override void OnAppear()
        {
            base.OnAppear();
            IsLeadExist = SqLiteService.GetList<LeadModel>(x => x.LeadName != null).Any();
        }

        #region Override Navigate back to

        public override void OnNavigatedBackTo(INavigationParameters parameters)
        {
            if (parameters != null)
            {
                if (parameters.ContainsKey(ParamKey.GoBackAndReload.ToString()))
                {
                    var list = (ObservableCollection<EventModel>)parameters[ParamKey.GoBackAndReload.ToString()];
                    int n = 0;
                    foreach (var leadEventList in list)
                    {
                        n += leadEventList.NumberLeads;
                    }
                    IsLeadExist = n > 0 ? true : false;
                }
            }
        }
        #endregion

        #region LeadWithCard

        public ICommand LeadWithCardCommand { get; set; }
        private async void LeadWithCardExecute()
        {
            await CheckBusy(async () =>
            {
                var param = new NavigationParameters
                {
                    { ParamKey.LeadWithCard.ToString(), true }
                };

                await Navigation.NavigateAsync(PageManager.NewContactPage, param);
            });
        }

        #endregion

        #region LeadWithoutCard

        public ICommand LeadWithoutCardCommand { get; set; }
        private async void LeadWithoutCardExecute()
        {
            await CheckBusy(async () =>
            {
                var param = new NavigationParameters();

                param.Add(ParamKey.LeadWithCard.ToString(), false);

                await Navigation.NavigateAsync(PageManager.NewContactPage, param);
            });
        }

        #endregion

        #region ContactListCommand

        public ICommand ContactListCommand { get; set; }
        private async void ContactListExecute()
        {
            await CheckBusy(async () =>
            {
                await Navigation.NavigateAsync(PageManager.TheEventListPage);
            });
        }

        #endregion

        #region LeadCounterCommand

        public ICommand LeadCounterCommand { get; set; }
        private async void LeadCounterExecute()
        {
            await CheckBusy(async () =>
            {
                await Navigation.NavigateAsync(PageManager.LeadCounterPage);
            });
        }

        #endregion

        #region OpenPreferencesPageCommand

        public ICommand OpenPreferencesPageCommand { get; set; }
        private async void OpenPreferencesPageExecute()
        {
            await CheckBusy(async () =>
            {
                await Navigation.NavigateAsync(PageManager.SettingsPage);
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
