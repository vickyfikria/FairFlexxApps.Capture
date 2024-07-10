using System;
using System.Collections.ObjectModel;
using System.Linq;
using FairFlexxApps.Capture.Controls;
using FairFlexxApps.Capture.Models.Templates.Pages;
using FairFlexxApps.Capture.ViewModels.NewLeadFlows;
using FairFlexxApps.Capture.Views.Base;
using Prism.Navigation;
using Microsoft.Maui;
using Microsoft.Maui.Controls.Xaml;

namespace FairFlexxApps.Capture.Views.NewLeadFlows
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class NewLeadTemplatePhonePage : BasePage
    {
        public static NewLeadTemplatePhonePage Instance { get; private set; }

        public NewLeadTemplatePhonePage()
        {
            InitializeComponent();

            Instance = this;
            NewLeadTemplatePageViewModel.ContinueHandler += ContinueButtonHandler;
            NewLeadTemplatePageViewModel.SwitchCardHandler += SwitchCardButtonHandler;
            NewLeadTemplatePageViewModel.ScanQrCodeHandler += ScanQrCodeButtonHandler;

            //isVisibleSideMenu = sideMenu.IsVisible = Device.Idiom != TargetIdiom.Phone;
        }

        #region ContinueButtonHandler

        private void ContinueButtonHandler(object sender, EventArgs e)
        {
            var selectedItem = ((NewLeadTemplatePageViewModel)HorizontalBar?.BindingContext).Selected;


            var vm = (NewLeadTemplatePageViewModel)BindingContext;

            if (vm != null && selectedItem != null)
            {
                var index = vm.DataSideMenu.IndexOf(selectedItem);

                var item = vm.DataSideMenu[index + 1];
                UpdateUI(vm, item);
            }
        }

        private void UpdateUI(NewLeadTemplatePageViewModel vm, SideMenu item)
        {
            svContentPage.Content = vm?.Update(item);
            svContentPage.ScrollToAsync(0, 0, false);
        }

        #endregion

        #region SwitchCardButtonHandler

        private void SwitchCardButtonHandler(object sender, EventArgs e)
        {
            svContentPage.Content = (View)sender;
        }

        #endregion

        #region ScanQrCodeButtonHandler

        private void ScanQrCodeButtonHandler(object sender, EventArgs e)
        {
            svContentPage.Content = (View)sender;
        }

        #endregion

        private bool isVisibleSideMenu = true;

        #region Override Navigate new to

        public override void OnNavigatedNewTo(INavigationParameters parameters)
        {
            if (parameters != null)
            {

            }

        }

        #endregion


        private void SideMenuListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Device.Idiom == TargetIdiom.Phone)
            {
                
                svContentPage.IsVisible = true;
                svContentView.IsVisible = false;

                var item = (SideMenu)e.CurrentSelection.FirstOrDefault();

                var vm = (NewLeadTemplatePageViewModel)BindingContext;

                UpdateUI(vm, item);
            }
        }

        #region TopMenuItemSelected

        public async void TopMenuItemSelected(object sender, EventArgs e)
        {
            var button = (CrossButton)sender;

            var background = $"{button.ClassId}Selected";
            var vm = (NewLeadTemplatePageViewModel)BindingContext;

            if (button.ClassId == "Sketch" || button.ClassId == "Notes")
            {
                svContentView.IsVisible = true;
                svContentPage.IsVisible = false;
                svContentView.Content = await vm?.TopMenuItemSelectedExecute(isSelected: background);
            }
            else
            {
                if (Device.RuntimePlatform == Device.Android)
                {
                    svContentView.IsVisible = true;
                    svContentPage.IsVisible = false;
                    svContentView.Content = await vm?.TopMenuItemSelectedExecute(isSelected: background);
                }
                else
                {
                    svContentPage.IsVisible = true;
                    svContentView.IsVisible = false;
                    svContentPage.Content = await vm?.TopMenuItemSelectedExecute(isSelected: background);
                }

            }
        }

        #endregion

        #region HiddenTopMenu

        public async void HiddenTopMenu(object sender, EventArgs e)
        {
            gridTopMenu.IsVisible = false;
            stTitle.IsVisible = false;
        }

        #endregion

        #region HandleSideMenuToSmallSize
        /*
        private void ShowSideMenuOnTapped(object sender, EventArgs e)
        {
            HandleSideMenuToSmallSize();
        }

        private async void InVisibleSideMenuOnTapped(object sender, EventArgs e)
        {
            await CheckBusy(async () =>
            {
                if (!isVisibleSideMenu)
                {
                    svBoxViewDoNothing.IsVisible = false;
                    sideMenu.IsVisible = false;
                }
            });
        }

        private async void IsVisibleSideMenuToDoNothingOnSwipe(object sender, SwipedEventArgs e)
        {
            await CheckBusy(async () =>
            {
                if (!isVisibleSideMenu)
                {
                    svBoxViewDoNothing.IsVisible = false;
                    sideMenu.IsVisible = false;
                }
            });
        }

        private async void VisibleSideMenuOnSwipeLeft(object sender, SwipedEventArgs e)
        {
            await CheckBusy(async () =>
            {
                if (!isVisibleSideMenu)
                {
                    svBoxViewDoNothing.IsVisible = true;
                    sideMenu.IsVisible = true;
                }
            });
        }

        private async void InVisibleSideMenuOnSwipeRight(object sender, SwipedEventArgs e)
        {
            await CheckBusy(async () =>
            {
                if (!isVisibleSideMenu)
                {
                    svBoxViewDoNothing.IsVisible = false;
                    sideMenu.IsVisible = false;
                }
            });
        }

        private async void HandleSideMenuToSmallSize()
        {
            await CheckBusy(async () =>
            {
                if (!isVisibleSideMenu)
                {
                    sideMenu.IsVisible = !sideMenu.IsVisible;
                    svBoxViewDoNothing.IsVisible = sideMenu.IsVisible;
                }
            });

        }
        */
        #endregion

        private void SwipeGestureRecognizer_Swiped(object sender, SwipedEventArgs e)
        {
            var vm = (NewLeadTemplatePageViewModel)BindingContext;
            vm.IsShownTopMenu = true;
        }

        private void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            if (Device.Idiom == TargetIdiom.Phone)
            {

                svContentPage.IsVisible = true;
                svContentView.IsVisible = false;

                var item = (SideMenu)((TappedEventArgs)e).Parameter;

                var vm = (NewLeadTemplatePageViewModel)BindingContext;

                UpdateUI(vm, item);

            }
        }
        public void SetDefaultSideMenu(SideMenu item)
        {
            if (Device.Idiom == TargetIdiom.Phone)
            {

                svContentPage.IsVisible = true;
                svContentView.IsVisible = false;

                var vm = (NewLeadTemplatePageViewModel)BindingContext;

                UpdateUI(vm, item);
            }
        }
    }
}
