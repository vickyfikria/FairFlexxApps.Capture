using FairFlexxApps.Capture.Enums;
using FairFlexxApps.Capture.Interfaces.HttpService;
using FairFlexxApps.Capture.Interfaces.LocalDatabase;
using FairFlexxApps.Capture.Models;
using FairFlexxApps.Capture.ViewModels.Base;
using Prism.Navigation;
using System;
using System.Diagnostics;
using System.Linq;
using VCardReader;

namespace FairFlexxApps.Capture.ViewModels.NewLeadFlows
{
    public class ScanQrCodePageViewModel : ViewModelBase
    {
        public ScanQrCodePageViewModel(INavigationService navigationService, IHttpRequest httpRequest, ISqLiteService sqLiteService)
            : base(navigationService: navigationService, httpRequest: httpRequest, sqliteService: sqLiteService)
        {
            Title = "Scan Qr Code Page";
        }

        public async void GoBackToLeadPage(VCard vcardResult)
        {
            try
            {
                var countryList = CountryService.CountryList();
                for (int i = 0; i < countryList.Count; i++)
                {
                    if (vcardResult?.DeliveryAddresses.ElementAt(0).Country?.ToLower() == countryList.ElementAt(i).GermanyName.ToLower())
                    {
                        vcardResult.DeliveryAddresses.ElementAt(0).Country = countryList.ElementAt(i).EnglishName;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }

            var param = new NavigationParameters();
            param.Add(ParamKey.VCardResult.ToString(), vcardResult);
            await Navigation.GoBackAsync(param);
        }
    }
}
