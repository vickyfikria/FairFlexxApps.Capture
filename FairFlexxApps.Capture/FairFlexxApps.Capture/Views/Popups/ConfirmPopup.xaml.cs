using System;
using System.Threading.Tasks;
using System.Windows.Input;
using FairFlexxApps.Capture.Utilities;
using FairFlexxApps.Capture.Views.Base;
using Mopups.Hosting;
using Mopups.Pages;
using Mopups.Services;
using Microsoft.Maui;
using Mopups.PreBaked.AbstractClasses;
using Mopups.PreBaked.Interfaces;

namespace FairFlexxApps.Capture.Views.Popups
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ConfirmPopup : PopupBasePage
    {
        #region Constructors

        public ConfirmPopup()
        {
            InitializeComponent();
        }

        #endregion

        #region Instance

        private static ConfirmPopup _instance;

        public static ConfirmPopup Instance => _instance ?? (_instance = new ConfirmPopup() { IsClosed = true });

        public async Task<ConfirmPopup> Show(string title = null, string message = null, string closeButtonText = null,
            ICommand closeCommand = null, object closeCommandParameter = null,
            string acceptButtonText = null, ICommand acceptCommand = null, object acceptCommandParameter = null,
            bool isAutoClose = false, uint duration = 2000)
        {
            // Close Loading Popup if it is showing
            await LoadingPopup.Instance.Hide();
            IsClosed = true;
            await DeviceExtension.BeginInvokeOnMainThreadAsync( () =>
            {
                if (title != null)
                    LabelConfirmTitle.Text = title;

                if (message != null)
                    LabelConfirmMessage.Text = message;

                if (closeButtonText != null)
                    ButtonConfirmClose.Text = closeButtonText;

                ClosedPopupCommand = closeCommand;
                ClosedPopupCommandParameter = closeCommandParameter;

                if (acceptButtonText != null)
                    ButtonConfirmAccept.Text = acceptButtonText;

                AcceptCommand = acceptCommand;
                AcceptCommandParameter = acceptCommandParameter;

                IsAutoClose = isAutoClose;
                Duration = duration;

            });

            if (IsClosed)
            {
                IsClosed = false;

                if (isAutoClose && duration > 0)
                    AutoClosedPopupAfter(duration);

                await DeviceExtension.BeginInvokeOnMainThreadAsync(async () =>
                {
                    await MopupService.Instance.PushAsync(this);
                    //await Application.Current.MainPage.Navigation.PushPopupAsync(this);
                });
            }

            return this;
        }

        #endregion

        #region Events

        private async void AcceptPopupEvent(object sender, EventArgs e)
        {
            await DeviceExtension.BeginInvokeOnMainThreadAsync(async () =>
            {
                await Navigation.PopPopupAsync();
            });

            // waiting for close animation finished
            await Task.Delay(300);

            AcceptCommand?.Execute(AcceptCommandParameter);

            //_popupId++;
            IsClosed = true;
        }

        #endregion

        #region AcceptCommand

        public static readonly BindableProperty AcceptCommandProperty =
            BindableProperty.Create(nameof(AcceptCommand),
                typeof(ICommand),
                typeof(ConfirmPopup),
                null,
                BindingMode.TwoWay);

        public ICommand AcceptCommand
        {
            get => (ICommand)GetValue(AcceptCommandProperty);
            set => SetValue(AcceptCommandProperty, value);
        }

        public static readonly BindableProperty AcceptCommandParameterProperty =
            BindableProperty.Create(nameof(AcceptCommandParameter),
                typeof(object),
                typeof(ConfirmPopup),
                null,
                BindingMode.TwoWay);

        public object AcceptCommandParameter
        {
            get => GetValue(AcceptCommandParameterProperty);
            set => SetValue(AcceptCommandParameterProperty, value);
        }

        #endregion

        #region RefreshUI

        //public void RefreshUI()
        //{
        //    InitializeComponent();
        //}

        #endregion
    }
}