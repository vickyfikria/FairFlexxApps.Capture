using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using FairFlexxApps.Capture.Controls;
using FairFlexxApps.Capture.Controls.InputKit;
using FairFlexxApps.Capture.Controls.RadioButton;
using FairFlexxApps.Capture.Managers;
using FairFlexxApps.Capture.Models;
using FairFlexxApps.Capture.Models.DataTemplate;
using FairFlexxApps.Capture.Models.Templates.Controls;
using FairFlexxApps.Capture.Services.HttpService;
using Microsoft.Maui;

namespace FairFlexxApps.Capture.Extensions.Common
{
    public class Parse
    {
        public Serializer serializer = new Serializer();


        #region ShowSideMenu
        public ListView ShowSideMenu(Template Template, int LanguageIndex)
        {
            ListView SideMenu = new ListView()
            {
                HorizontalOptions = LayoutOptions.CenterAndExpand,

            };
            var MenuList = new ObservableCollection<string>();
            foreach (var Page in Template.Pages)
            {
                if (Page.SideMenu != null)
                {
                    MenuList.Add(Page.SideMenu.Language.GetLanguage(0));
                }
            }
            SideMenu.ItemsSource = MenuList;
            return SideMenu;
        }
        #endregion
    }
}
