using FairFlexxApps.Capture.Localization;
using FairFlexxApps.Capture.ViewModels.NewLeadFlows;
using FairFlexxApps.Capture.Views.Base;
using FairFlexxApps.Capture.Views.Popups;
using BarcodeScanner.Mobile;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using VCardReader;


namespace FairFlexxApps.Capture.Views.NewLeadFlows
{
    public partial class ScanQrCodePage : BasePage
    {
        private bool IsAppeared = false;

        public ScanQrCodePage()
        {
            InitializeComponent();
        }

        protected async override void OnAppearing()
        {
            base.OnAppearing();

            if(!IsAppeared)
            {
                IsAppeared = true;


                bool allowed = await Methods.AskForRequiredPermission();

                //bool allowed = await CheckCameraPermission();

                if (!allowed)
                {
                    await MessagePopup.Instance.Show(message: TranslateExtension.Get("GrantPermissionCamera"),
                        closeButtonText: "OK", textBackgroundColor: "#bdbdbd",
                        closeCommand: ((ScanQrCodePageViewModel)this.BindingContext).BackCommand);

                }
                else
                {
                    ScanningLayout.Children.Add(GetScanningView());
                }
            }
        }

       /* private async Task<bool> CheckCameraPermission()
        {
            var cameraStatus = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Camera);
            if (cameraStatus != Plugin.Permissions.Abstractions.PermissionStatus.Granted)
            {
                var results = await CrossPermissions.Current.RequestPermissionsAsync(Permission.Camera);
                cameraStatus = results[Permission.Camera];
            }

            if (cameraStatus == Plugin.Permissions.Abstractions.PermissionStatus.Granted)
            {
                return true;
            }
            else
            {
                await MessagePopup.Instance.Show(TranslateExtension.Get("PermissionsDenied"));
                CrossPermissions.Current.OpenAppSettings();
            }

            return false;
        }*/

        private CameraView GetScanningView()
        {
            var scanningView = new CameraView()
            {
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand,
                VibrationOnDetected = false,
                ScanInterval = 50
            };
            scanningView.OnDetected += CameraView_OnDetected;

            return scanningView;
        }

        private async void CameraView_OnDetected(object sender, OnDetectedEventArg e)
        {
            var result = e.BarcodeResults?.FirstOrDefault();

            if(result != null)
            {
                Device.BeginInvokeOnMainThread(async () =>
                {
                    if (!(ValidateVCard(result.RawValue)))
                    {
                        ShowErrorAndRescan();
                        return;
                    }
                    try
                    {
                        using (TextReader cardText = new StringReader(result.RawValue))
                        {
                            var vcard = new VCard(cardText);
                            var vm = (ScanQrCodePageViewModel)BindingContext;
                            if (vm != null)
                            {
                                vm.GoBackToLeadPage(vcard);
                            }
                        }
                    }
                    catch
                    {
                        ShowErrorAndRescan();
                    }

                });
            }
            else
            {
                ShowErrorAndRescan();
                return;
            }
        }

        private static bool ValidateVCard(string result)
        {
            if (string.IsNullOrEmpty(result))
                return false;

            return result.Contains("BEGIN:VCARD") && result.Contains("END:VCARD");
        }

        private async void ShowErrorAndRescan()
        {
            await MessagePopup.Instance.Show(message: TranslateExtension.Get("QrFormatError"), 
                closeButtonText: "OK", textBackgroundColor: "#bdbdbd",
                closeCommand: new Command(()=> 
                {
                    for(int i = 0; i < ScanningLayout.Children.Count; i++)
                        ScanningLayout.Children?.RemoveAt(i);

                    ScanningLayout.Children.Add(GetScanningView());
                }));

        }
    }
}
