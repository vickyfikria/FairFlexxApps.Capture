using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using System.Xml;
using FairFlexxApps.Capture.Models;
using FairFlexxApps.Capture.Models.Templates.Pages;
using FairFlexxApps.Capture.ViewModels;
using FairFlexxApps.Capture.Views.Commons.LayoutTemplates;
using FairFlexxApps.Capture.Services.HttpService;

namespace FairFlexxApps.Capture.Views
{
    public partial class MainPage : ContentPage
    {
        #region Properties

        public ObservableCollection<View> LayoutPages { get; set; } = new ObservableCollection<View>();

        public Grid GridLayout { get; set; }

        public View ViewTemp { get; set; }

        public Grid TopMenu { get; set; }

        public StackLayout SideMenu { get; set; }

        public ObservableCollection<SideMenu> DataSideMenu { get; set; }

        public string InnerXml { get; set; }

        public Template Template { get; set; }

        #endregion

        #region OnAppearing

        private bool _isAppeared = false;
        protected MainPageViewModel ViewModel;
        protected override async void OnAppearing()
        {
            if (_isAppeared)
                return;
            
            _isAppeared = true;

            if (BindingContext != null)
                ViewModel = (MainPageViewModel)BindingContext;
            
            InnerXml = await ReadFileXml();

            Template = GetTemplateModel(innerXml: InnerXml);

            // Get TopMenu
            TopMenu = TopMenuTemplate.GetTopMenu(topMenuTemplate: Template.TopMenu);

            // Get SideMenu
            SideMenu = new StackLayout()
            {
                Spacing = 0,
                BackgroundColor = Colors.White,
            };
            DataSideMenu = SideMenuTemplate.GetDataSideMenu(template: Template, languageIndex: 0);

            // Get List Page
            LayoutPages = PageTemplate.GetAllPageTemplate(languageIndex: 0, template: Template);

            GridLayout = GetLayout(topMenu: TopMenu, sideMenu: SideMenu, pageView: LayoutPages[0], template: Template,
                languageIndex: 0);

            this.Content = GridLayout;

        }

        #endregion

        #region Constructor

        public MainPage()
        {
            var content = new StackLayout()
            {
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                VerticalOptions = LayoutOptions.CenterAndExpand,
                Children =
                {
                    new Label()
                    {
                        Text = "Please check on the internet connection and try again!",
                    }
                },
            };

            this.Content = content;

            return;


            /*var vm = (MainPageViewModel)BindingContext;

            InnerXml = ReadFileXml();

            Template = GetTemplateModel(innerXml: InnerXml);

            // Get TopMenu
            TopMenu = TopMenuTemplate.GetTopMenu(topMenuTemplate: Template.TopMenu);

            // Get SideMenu
            SideMenu = new StackLayout()
            {
                Spacing = 0,
                BackgroundColor = Color.White,
            };
            DataSideMenu = SideMenuTemplate.GetDataSideMenu(template: Template, languageIndex: 0);

            // Get List Page
            LayoutPages = PageTemplate.GetAllPageTemplate(languageIndex: 0, template: Template);

            GridLayout = GetLayout(topMenu: TopMenu, sideMenu: SideMenu, pageView: LayoutPages[0], template: Template,
                languageIndex: 0);

            this.Content = GridLayout;
            var v1 = (MainPageViewModel)BindingContext;*/
        }

        #endregion

        #region ReadFileXML

        private async Task<string> ReadFileXml()
        {
            //var v = (MainPageViewModel)BindingContext;

            //var test = await ViewModel?.GetXMLStructure();


            var assembly = typeof(MainPage).GetTypeInfo().Assembly;

            Stream stream = assembly.GetManifestResourceStream("FairFlexxApps.Capture." + "tablet_template.xml");
            XmlDocument docs = new XmlDocument();
            docs.Load(stream);

            return docs.InnerXml;
        }

        #endregion

        #region GetTemplateModel

        private Template GetTemplateModel(string innerXml)
        {
            Serializer ser = new Serializer();
            var template = new Template();
            template = ser.Deserialize<Template>(innerXml);
            return template;
        }

        #endregion

        #region GetLayout

        private Grid GetLayout(View topMenu, StackLayout sideMenu, View pageView, Template template, int languageIndex)
        {
            var layout = new Grid()
            {
                Margin = new Thickness(0, 0, 0, 0),
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.Start,
                RowSpacing = 0,
                ColumnSpacing = 0,
                //BackgroundColor = Color.DimGray,
            };

            layout.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            layout.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(135, GridUnitType.Absolute) });

            layout.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Auto) });
            layout.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Auto) });
            layout.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });

            layout.Add(topMenu, 0, 0); // in .NET MAUI, Grid.Children.Add no longer exist. Changed to Grid.Add()
            Grid.SetColumnSpan(topMenu, 2);

            var stackHeader = new StackLayout()
            {
                HeightRequest = 50,
                BackgroundColor = Colors.White,
                HorizontalOptions = LayoutOptions.FillAndExpand,
            };

            #region BoxView

            var dash = new BoxView()
            {
                HeightRequest = 1,
                BackgroundColor = Colors.DimGray,
                VerticalOptions = LayoutOptions.StartAndExpand,
            };

            #endregion
            
            stackHeader.Children.Add(dash);

            layout.Add(stackHeader, 0, 1); // in .NET MAUI, Grid.Children.Add no longer exist. Changed to Grid.Add()
            Grid.SetColumnSpan(stackHeader, 2);

            var sideMenuView = SideMenuTemplate.GetSideMenu(template: template, languageIndex: 0);
            sideMenuView.ItemSelected += ItemSelectedInSideMenu;

            //sideMenu.Children.Add(dash);
            sideMenu.Children.Add(sideMenuView);

            ViewTemp = pageView;

            layout.Add(pageView, 0, 2); // in .NET MAUI, Grid.Children.Add no longer exist. Changed to Grid.Add()
            layout.Add(sideMenu, 1, 2); // in .NET MAUI, Grid.Children.Add no longer exist. Changed to Grid.Add()

            return layout;
        }

        #endregion

        #region ItemSelected

        private void ItemSelectedInSideMenu(object sender, SelectedItemChangedEventArgs e)
        {
            var item = (SideMenu)e.SelectedItem;

            var indexPage = GetIndexOfSideMenu(sideMenus: DataSideMenu, sideMenu: item);//DataSideMenu.IndexOf(item);

            //GridLayout.Children.Remove(ViewTemp);
            //GridLayout.Children.Add(LayoutPages[indexPage], 0, 2);

            //ViewTemp = LayoutPages[indexPage];

            ViewTemp.IsVisible = false;
            GridLayout.Add(LayoutPages[indexPage], 0, 2); // in .NET MAUI, Grid.Children.Add no longer exist. Changed to Grid.Add()

            ViewTemp = LayoutPages[indexPage];
            ViewTemp.IsVisible = true;
        }

        private void ItemTappedInSideMenu(object sender, ItemTappedEventArgs e)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Get Index Side menu

        public int GetIndexOfSideMenu(ObservableCollection<SideMenu> sideMenus, SideMenu sideMenu)
        {
            var index = -1;
            var a = sideMenus.IndexOf(sideMenu);
            foreach (var item in sideMenus)
            {
                if (item.SideMenuID == sideMenu.SideMenuID)
                    return sideMenus.IndexOf(item);
            }
            return index;
        }

        #endregion

        public static void SaveAndCloseTappedEvent(object sender, EventArgs e)
        {
            
        }

    }
}
