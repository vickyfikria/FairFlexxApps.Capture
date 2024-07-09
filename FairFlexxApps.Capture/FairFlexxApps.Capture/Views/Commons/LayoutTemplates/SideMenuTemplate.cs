using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using FairFlexxApps.Capture.Models;
using FairFlexxApps.Capture.Models.Templates.Pages;
using FairFlexxApps.Capture.Views.ViewCells;
using Microsoft.Maui;

namespace FairFlexxApps.Capture.Views.Commons.LayoutTemplates
{
    public class SideMenuTemplate
    {
        // Todo: Dont use that
        #region Comment code

        public static ListView GetSideMenu(Template template, int languageIndex)
        {
            var sideMenu = new ListView()
            {
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand,
                //BackgroundColor = Color.DimGray,
                SeparatorColor = Colors.DimGray,
            };

            sideMenu.ItemTemplate = new DataTemplate(typeof(SideMenuViewCell));

            var menuList = new ObservableCollection<SideMenu>();
            foreach (var page in template.Pages)
            {
                if (page.SideMenu != null && page.Visible)
                {
                    var item = new SideMenu();
                    item.SideMenuID = page.SideMenu.Language.GetLanguage(indexOfLanguage: languageIndex);
                    menuList.Add(item);
                    //menuList.Add(page.SideMenu.Language.GetLanguage(indexOfLanguage: languageIndex));
                }
            }

            sideMenu.ItemsSource = menuList;

            sideMenu.SelectedItem = menuList[0];

            return sideMenu;
        }

        #endregion

        #region Get data side menu
        public static ObservableCollection<SideMenu> GetDataSideMenu(Template template, int languageIndex)
        {
            var menuList = new ObservableCollection<SideMenu>();
            foreach (var page in template.Pages)
            {
                if (page.SideMenu != null && page.Visible)
                {
                    var item = new SideMenu();
                    item.SideMenuID = page.SideMenu.Language.GetLanguage(languageIndex);
                    item.ImageSource = "ic_circle";
                    item.Short = page.Short;
                    item.Name = page.Name;
                    menuList.Add(item);
                    //menuList.Add(page.SideMenu.Language.GetLanguage(indexOfLanguage: languageIndex));
                }
            }

            return menuList;
        }

        #endregion
    }
}
