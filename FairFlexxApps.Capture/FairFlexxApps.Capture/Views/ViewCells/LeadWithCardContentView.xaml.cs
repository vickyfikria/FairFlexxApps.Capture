using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using FairFlexxApps.Capture.Controls;
using FairFlexxApps.Capture.Models.LeadModels;
using FairFlexxApps.Capture.ViewModels.NewLeadFlows;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace FairFlexxApps.Capture.Views.ViewCells
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class LeadWithCardContentView : ContentView
	{
		public LeadWithCardContentView ()
		{
			InitializeComponent ();
            continue_button.GestureRecognizers.Add(new TapGestureRecognizer()
            {
                Command = new Command(async () =>
                {
                    await NewLeadTemplatePageViewModel.Instance.ContinueButtonHandler();
                })
            });

        }

        public LeadWithCardContentView(bool isMissedVisitorPage)
        {
            InitializeComponent();
            stBusinessCardMissing.IsVisible = isMissedVisitorPage;
            stBusinessCard.IsVisible = !isMissedVisitorPage;
            continue_button.GestureRecognizers.Add(new TapGestureRecognizer()
            {
                Command = new Command(async () =>
                {
                    await NewLeadTemplatePageViewModel.Instance.ContinueButtonHandler();
                })
            });

        }

        private async void SwitchToNoCard(object sender, EventArgs e)
        {
            await NewLeadTemplatePageViewModel.Instance.SwitchCardButtonHandler();
        }

        private async void TapGestureRecognizer_OnTapped(object sender, EventArgs e)
        {
            await NewLeadTemplatePageViewModel.Instance.SwitchCardButtonHandler();
        }
    }
}