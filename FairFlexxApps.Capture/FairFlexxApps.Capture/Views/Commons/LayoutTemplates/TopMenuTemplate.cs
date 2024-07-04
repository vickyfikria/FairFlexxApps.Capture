using System;
using System.Collections.ObjectModel;
using FairFlexxApps.Capture.Models;
using FairFlexxApps.Capture.Models.Templates;
using FairFlexxApps.Capture.Models.Templates.Pages;
using Microsoft.Maui;

namespace FairFlexxApps.Capture.Views.Commons.LayoutTemplates
{
    public class TopMenuTemplate
    {
        public static Grid GetTopMenu(TopMenu topMenuTemplate)
        {
            #region Set up

            var topMenu = new Grid()
            {
                ColumnSpacing = 1,
                //RowSpacing = 1,
                VerticalOptions = LayoutOptions.StartAndExpand,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                BackgroundColor = Color.DimGray,
            };

            #endregion

            #region ColumnsDefinition & RowsDefinition

            topMenu.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Auto) });
            topMenu.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            topMenu.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            topMenu.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            topMenu.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            topMenu.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            topMenu.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Auto) });

            topMenu.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Auto) });
            //topMenu.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Auto) });

            #endregion

            #region Button Back Arrow

            var backArrowLayout = GetButtonBackArrow();

            #endregion

            #region Notes button

            var noteLayout = GetMenuButton(imageSource: "ic_edit.png", backgroundColor: Color.LightGray,
                buttonText: "Notes", isVisible: topMenuTemplate.Notes);

            #endregion

            #region Sketch button

            var sketchLayout = GetMenuButton(imageSource: "ic_edit.png", backgroundColor: Color.LightGray,
                buttonText: "Sketch", isVisible: topMenuTemplate.Sketch);

            #endregion

            #region Card button

            var cardLayout = GetMenuButton(imageSource: "ic_photo_camera.png", backgroundColor: Color.Cyan,
                buttonText: "Card", isVisible: topMenuTemplate.Card);

            #endregion

            #region Attachment button

            var attachmentLayout = GetMenuButton(imageSource: "ic_photo_camera.png", backgroundColor: Color.Cyan,
                buttonText: "Attachment", isVisible: topMenuTemplate.Attachment);

            #endregion

            #region Object button

            var objectLayout = GetMenuButton(imageSource: "ic_photo_camera.png", backgroundColor: Color.Cyan,
                buttonText: "Object", isVisible: topMenuTemplate.Object);

            #endregion

            #region Save & Close Button

            var saveAndCloseLayout = GetSaveAndCloseLayout();
            var saveAndCloseTap = new TapGestureRecognizer();
            saveAndCloseTap.Tapped += MainPage.SaveAndCloseTappedEvent;
            saveAndCloseLayout.GestureRecognizers.Add(saveAndCloseTap);

            #endregion

            #region BoxView

            var dash = new BoxView()
            {
                HeightRequest = 1,
                BackgroundColor = Color.DimGray,
                VerticalOptions = LayoutOptions.StartAndExpand,
            };

            #endregion

            #region Set Layout

            topMenu.Children.Add(backArrowLayout, 0, 0);
            topMenu.Children.Add(noteLayout, 1, 0);
            topMenu.Children.Add(sketchLayout, 2, 0);
            topMenu.Children.Add(cardLayout, 3, 0);
            topMenu.Children.Add(attachmentLayout, 4, 0);
            topMenu.Children.Add(objectLayout, 5, 0);
            topMenu.Children.Add(saveAndCloseLayout, 6, 0);

            //topMenu.Children.Add(dash, 0, 1);
            //Grid.SetColumnSpan(dash, 7);

            #endregion

            return topMenu;
        }
        
        #region GetButtonBackArrow

        public static View GetButtonBackArrow()
        {
            #region IconBack

            var backArrow = new Image()
            {
                WidthRequest = 25,
                HeightRequest = 25,
            };
            backArrow.Source = "ic_back.png";

            #endregion

            #region Layout

            var backArrowLayout = new StackLayout()
            {
                VerticalOptions = LayoutOptions.FillAndExpand,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                BackgroundColor = Color.LightGray,
                Spacing = 0,
                Children =
                {
                    new StackLayout()
                    {
                        VerticalOptions = LayoutOptions.CenterAndExpand,
                        HorizontalOptions = LayoutOptions.CenterAndExpand,
                        Children =
                        {
                            backArrow,
                        },
                        Margin = new Thickness(3,3,10,3),
                    },

                },
            };

            #endregion

            return backArrowLayout;
        }

        #endregion

        #region GetMenuButton

        public static View GetMenuButton(string imageSource, Color backgroundColor, string buttonText, bool isVisible)
        {
            #region Icon

            var buttonIcon = new Image()
            {
                WidthRequest = 20,
                HeightRequest = 20,
                Margin = new Thickness(0,0,5,0),
            };
            buttonIcon.Source = imageSource;

            #endregion

            #region Layout

            var buttonLayout = new StackLayout()
            {
                VerticalOptions = LayoutOptions.FillAndExpand,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                BackgroundColor = backgroundColor,
                IsVisible = isVisible,
                Children =
                {
                    new StackLayout()
                    {
                        Orientation = StackOrientation.Horizontal,
                        VerticalOptions = LayoutOptions.CenterAndExpand,
                        HorizontalOptions = LayoutOptions.StartAndExpand,
                        Spacing = 0,
                        Margin = new Thickness(8,5,5,5),
                        Children =
                        {
                            buttonIcon,
                            new Label()
                            {
                                Text = buttonText,
                                VerticalOptions = LayoutOptions.CenterAndExpand,
                                LineBreakMode = LineBreakMode.WordWrap,
                            }
                        },
                    },
                },
            };

            #endregion

            return buttonLayout;
        }

        #endregion

        #region GetSaveAndCloseLayout

        public static View GetSaveAndCloseLayout()
        {
            var saveAndCloseLayout = new StackLayout()
            {
                VerticalOptions = LayoutOptions.FillAndExpand,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                BackgroundColor = Color.FromHex("#0DAE02"),
                WidthRequest = 135,
                Children =
                {
                    new StackLayout()
                    {
                        Orientation = StackOrientation.Horizontal,
                        VerticalOptions = LayoutOptions.CenterAndExpand,
                        HorizontalOptions = LayoutOptions.CenterAndExpand,
                        Spacing = 0,
                        Margin = new Thickness(5),
                        Children =
                        {
                            new Label()
                            {
                                Text = "Save & Close",
                                TextColor = Color.White,
                                VerticalOptions = LayoutOptions.CenterAndExpand,
                            },
                        },
                    },
                },
            };

            return saveAndCloseLayout;
        }
        #endregion


        public static TopMenu GetDataTopMenu(TopMenu topMenuTemplate)
        {
            return new TopMenu()
            {
                Attachment = topMenuTemplate.Attachment,
                Card = topMenuTemplate.Card,
                Notes = topMenuTemplate.Notes,
                Object = topMenuTemplate.Object,
                Sketch = topMenuTemplate.Sketch,
            };
        }

    }
}
