using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using FairFlexxApps.Capture.Controls;
using FairFlexxApps.Capture.Enums.Templates;
using FairFlexxApps.Capture.Localization;
using FairFlexxApps.Capture.Models;
using FairFlexxApps.Capture.ViewModels.NewLeadFlows;
using Newtonsoft.Json;
using Microsoft.Maui;
using Page = FairFlexxApps.Capture.Models.Templates.Page;

namespace FairFlexxApps.Capture.Views.Commons.LayoutTemplates
{
    public class PageTemplate
    {
        #region GetPageBinding

        public static Entry Get()
        {
            var title = new Entry();
            title.SetBinding(Entry.TextProperty, "Title");

            return title;
        }

        #endregion

        #region GetAllPageTemplate

        public static ObservableCollection<View> GetAllPageTemplate(int languageIndex, Template template)
        {
            var pagesScrollView = new ObservableCollection<View>();

            var pageIndex = 0;
            foreach (var page in template.Pages)
            {
                var pageView = GetPageTemplate(languageIndex: languageIndex, pagesTemplate: page,
                    totalPages: template.Pages.Count, pageIndex: pageIndex);

                if (page.Visible)
                {
                    pagesScrollView.Add(pageView);
                    pageIndex++;
                }

            }

            return pagesScrollView;
        }

        #endregion

        #region GetPage

        public static View GetPageTemplate(int languageIndex, Page pagesTemplate, int totalPages, int pageIndex)
        {
            //ScrollView pageScrollView = new ScrollView()
            //{
            //    ClassId = pagesTemplate.PageID,
            //    BackgroundColor = Color.White,
            //};

            var pageContent = new StackLayout()
            {
                Margin = new Thickness(0),
                Padding = new Thickness(0),
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                VerticalOptions = LayoutOptions.StartAndExpand,
                ClassId = pagesTemplate.PageID,
                BackgroundColor = Colors.White,
            };

            for (int indexItem = 0; indexItem < pagesTemplate.Boxs.Count; indexItem++)
            {
                if (pagesTemplate.Boxs[indexItem].Size == SizeType.Half.ToString().ToLower())
                {
                    var nextBox = (indexItem < (pagesTemplate.Boxs.Count - 1) &&
                                   pagesTemplate.Boxs[indexItem + 1].Size == SizeType.Half.ToString().ToLower())
                        ? pagesTemplate.Boxs[indexItem + 1]
                        : null;

                    var doubleBox = BoxTemplate.GetDoubleBoxTemplate(languageIndex: languageIndex,
                        currentBoxTemplate: pagesTemplate.Boxs[indexItem],
                        nextBoxTemplate: nextBox);

                    pageContent.Children.Add(doubleBox);

                    if (nextBox != null)
                        indexItem++;
                }
                else
                {
                    var contentBox = BoxTemplate.GetBoxTemplate(languageIndex: languageIndex,
                        boxTemplate: pagesTemplate.Boxs[indexItem]);

                    pageContent.Children.Add(contentBox);
                }

            }

            //pageScrollView.Content = pageContent;

            //var btnContinue = new CrossButton()
            //{
            //    Text = TranslateExtension.Get("Continue"),
            //    HorizontalOptions = LayoutOptions.EndAndExpand,
            //    BackgroundColor = (Color) App.Current.Resources["BlueColor"],
            //    FontSize = (double)App.Current.Resources["NormalLabelControlFont"] + (int)App.Settings.FontSize,
            //    TextColor = (Color)App.Current.Resources["WhiteColor"],
            //    Upper = false,
            //    Margin = new Thickness(0,0,0,20),
            //    Padding = new Thickness(5),
            //};
            //btnContinue.Clicked += async (s, e) => { await NewLeadTemplatePageViewModel.Instance.ContinueButtonHandler(); };

            var lblContinue = new Label()
            {
                FontSize = (double)App.Current.Resources["NormalLabelControlFont"] + (int)App.Settings.FontSize,
                TextColor = (Color)App.Current.Resources["WhiteColor"],
                Text = TranslateExtension.Get("Continue"),
            };

            var frmContinue = new Frame()
            {
                BackgroundColor = (Color)App.Current.Resources["BlueColor"],
                HorizontalOptions = LayoutOptions.EndAndExpand,
                Margin = new Thickness(0, 0, 0, 20),
                Padding = new Thickness(5),
                Content = lblContinue,
            };

            frmContinue.GestureRecognizers.Add(new TapGestureRecognizer()
            {
                Command = new Command(async () =>
                {
                    await NewLeadTemplatePageViewModel.Instance.ContinueButtonHandler();
                })
            });
            

            //button finish
            var btnFinish = new CrossButton()
            {
                Text = TranslateExtension.Get("Finish"),
                HorizontalOptions = LayoutOptions.EndAndExpand,
                BackgroundColor = (Color)App.Current.Resources["BlueColor"],
                FontSize = (double)App.Current.Resources["NormalLabelControlFont"] + (int)App.Settings.FontSize,
                TextColor = (Color)App.Current.Resources["WhiteColor"],
                Upper = false,
                Margin = new Thickness(0, 0, 0, 20),
                Padding = new Thickness(5),
            };
            btnFinish.Clicked += async (s, e) => { await NewLeadTemplatePageViewModel.Instance.FinishButtonHandler(); };

            //var btnSwitchCard = new CrossButton()
            //{
            //    Text = "With Card",
            //    BackgroundColor = (Color)App.Current.Resources["WhiteColor"],
            //    FontSize = (double)App.Current.Resources["NormalLabelControlFont"] + (int)App.Settings.FontSize,
            //    TextColor = (Color)App.Current.Resources["BlackColor"],
            //    BorderWidth = 1,
            //    Padding = new Thickness(5),
            //    Margin = Device.Idiom == TargetIdiom.Tablet ? new Thickness(0, 0, 0, 340) : new Thickness(0, 0, 0, 150),
            //    BorderColor = (Color)App.Current.Resources["BlackColor"],
            //    Upper = false,
            //};
            //btnSwitchCard.Clicked += async (s, e) => { await NewLeadTemplatePageViewModel.Instance.SwitchCardButtonHandler(); };

            var lblSwitchCard = new Label()
            {
                Text = "With Card",
                FontSize = (double)App.Current.Resources["NormalLabelControlFont"] + (int)App.Settings.FontSize,
                TextColor = (Color)App.Current.Resources["BlackColor"],
            };

            var frmSwitchCard = new Frame()
            {
                BackgroundColor = (Color)App.Current.Resources["WhiteColor"],
                Padding = new Thickness(5),
                Margin = Device.Idiom == TargetIdiom.Tablet ? new Thickness(0, 0, 0, 340) : new Thickness(0, 0, 0, 150),
                BorderColor = (Color)App.Current.Resources["BlackColor"],
                Content = lblSwitchCard
            };

            frmSwitchCard.GestureRecognizers.Add(new TapGestureRecognizer()
            {
                Command = new Command(async () =>
                {
                    await NewLeadTemplatePageViewModel.Instance.SwitchCardButtonHandler();
                })
            });

            //var btnScanQrCode = new CrossButton()
            //{
            //    Text = "Scan QR code",
            //    BackgroundColor = (Color)App.Current.Resources["WhiteColor"],
            //    FontSize = (double)App.Current.Resources["NormalLabelControlFont"] + (int)App.Settings.FontSize,
            //    TextColor = (Color)App.Current.Resources["BlackColor"],
            //    BorderWidth = 1,
            //    Padding = new Thickness(5),
            //    Margin = Device.Idiom == TargetIdiom.Tablet ? new Thickness(0, 0, 0, 340) : new Thickness(0, 0, 0, 150),
            //    BorderColor = (Color)App.Current.Resources["BlackColor"],
            //    Upper = false,
            //};
            //btnScanQrCode.Clicked += async (s, e) => { await NewLeadTemplatePageViewModel.Instance.ScanQrCodeButtonHandler(); };

            var lblScanQrCode = new Label()
            {
                Text = "Scan QR code",
                FontSize = (double)App.Current.Resources["NormalLabelControlFont"] + (int)App.Settings.FontSize,
                TextColor = (Color)App.Current.Resources["BlackColor"],
            };

            var frmScanQrCode = new Frame()
            {
                BackgroundColor = (Color)App.Current.Resources["WhiteColor"],
                Padding = new Thickness(5),
                Margin = Device.Idiom == TargetIdiom.Tablet ? new Thickness(0, 0, 0, 340) : new Thickness(0, 0, 0, 150),
                BorderColor = (Color)App.Current.Resources["BlackColor"],
                Content = lblScanQrCode
            };

            frmScanQrCode.GestureRecognizers.Add(new TapGestureRecognizer()
            {
                Command = new Command(async () =>
                {
                    await NewLeadTemplatePageViewModel.Instance.ScanQrCodeButtonHandler();
                })
            });

            if (pageIndex != (totalPages - 1))
            {
                pageContent.Children.Add(frmContinue);

            } else
            {
                pageContent.Children.Add(btnFinish);
            }

            if (pagesTemplate.Name == "visitor")
            {
                var container = new StackLayout
                {
                    Orientation = StackOrientation.Horizontal,
                    Spacing = 10,
                    HorizontalOptions = LayoutOptions.Center,
                };
                container.Children.Add(frmSwitchCard);
                container.Children.Add(frmScanQrCode);
                pageContent.Children.Add(container);
            }

            pageContent.IsVisible = pagesTemplate.Visible;
            return pageContent;
        }

        #endregion
    }
}
