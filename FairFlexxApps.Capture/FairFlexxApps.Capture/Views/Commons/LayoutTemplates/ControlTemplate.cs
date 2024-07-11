using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using FairFlexxApps.Capture.AutoSuggestBox;
using FairFlexxApps.Capture.Controls;
using FairFlexxApps.Capture.Controls.InputKit;
using FairFlexxApps.Capture.Controls.RadioButton;
using FairFlexxApps.Capture.Managers;
using FairFlexxApps.Capture.Models.Templates.Controls;
using FairFlexxApps.Capture.Utilities;
using Microsoft.Maui;
using Microsoft.Maui.Controls.PlatformConfiguration;
using Microsoft.Maui.Controls.PlatformConfiguration.iOSSpecific;
using Button = Microsoft.Maui.Controls.Button;
using CheckBox = FairFlexxApps.Capture.Controls.InputKit.CheckBox;
using DatePicker = Microsoft.Maui.Controls.DatePicker;
using Label = Microsoft.Maui.Controls.Label;
using Picker = Microsoft.Maui.Controls.Picker;

namespace FairFlexxApps.Capture.Views.Commons.LayoutTemplates
{
    public class ControlTemplate
    {
        //private static double FontSizeControl = (Math.Max(App.DisplayScreenWidth, App.DisplayScreenHeight) > 1620) ? 16 + 4 : 16;
        private static double FontSizeControl = (double)App.Current.Resources["NormalLabelControlFont"] + (int)App.Settings.FontSize;

        #region CheckBoxControl

        public static CheckBox CustomSingleCheckBoxControl(string id, string text, bool isVisible, string value, string boxType = null)
        {
            var widthRequest = App.ScreenWidth - 350;
            var check = value?.StringToBool() ?? false;
            return new CheckBox()
            {
                ClassId = id,
                IsVisible = isVisible,
                BorderColor = Colors.Green,
                BoxBackgroundColor = Colors.Transparent,
                BackgroundColorSelected = Colors.Green,
                Color = Colors.White,
                IsChecked = check,
                TextFontSize = (double)App.Current.Resources["NormalLabelControlFont"] + (int)App.Settings.FontSize,
                BoxSizeRequest = 22,
                HeightRequest = 64,
                //WidthRequest = widthRequest / App.DisplayScaleFactor,
                //Margin = new Thickness(100, 0, 0, 0),
                VerticalOptions = LayoutOptions.StartAndExpand,
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                Type = CheckBox.CheckType.Check,
                Text = text,
                UnCheckedTextColor = (!string.IsNullOrEmpty(boxType) && boxType.Equals(BoxType.Gdpr))
                    ? (Color)App.Current.Resources["GreenColor"]
                    : Colors.Black,
                CheckedTextColor = (!string.IsNullOrEmpty(boxType) && boxType.Equals(BoxType.Gdpr))
                    ? (Color)App.Current.Resources["RedColor"]
                    : Colors.Black,
            };

        }

        public static CustomCheckBox CustomCheckBoxControl(string id, string text, bool isVisible, int column, string value)
        {
            //var widthRequest = ((App.ScreenWidth - 225) / column);
            var widthScreen = (Math.Max(App.DisplayScreenWidth, App.DisplayScreenHeight));
            int heightRequest = (widthScreen > 1500 && widthScreen < 1700 && App.DisplayScaleFactor >= 1.5) ? 65 : 55;
            var check = (value == null) ? false : value.StringToBool();
            var fontSize = (double)(Device.RuntimePlatform == Device.Android 
                                ? App.Current.Resources["NormalLabelControlFont"] 
                                : ((double)App.Current.Resources["SmallLabelControlFont"] + 1.0)) 
                                + (int)App.Settings.FontSize;

            //return new CheckBox()
            //{
            //    ClassId = id,
            //    IsVisible = isVisible,
            //    BorderColor = Color.Green,
            //    BoxBackgroundColor = Color.Transparent,
            //    BackgroundColorSelected = Color.Green,
            //    Color = Color.White,
            //    IsChecked = check,
            //    TextFontSize = fontSize,
            //    BoxSizeRequest = 22,
            //    //HeightRequest = 164,
            //    //WidthRequest = widthRequest / App.DisplayScaleFactor,
            //    //Margin = new Thickness(100, 0, 0, 0),
            //    VerticalOptions = LayoutOptions.FillAndExpand,
            //    HorizontalOptions = LayoutOptions.FillAndExpand,
            //    Type = CheckBox.CheckType.Check,
            //    Text = text,
            //    UnCheckedTextColor = Color.Black,
            //    CheckedTextColor = Color.Black,
            //};

            return new CustomCheckBox()
            {
                ClassId = id,
                IsVisible = isVisible,
                OutlineColor = Colors.Green,
                IsChecked = check,
                FontSize = (float)(fontSize),
                HeightRequest = Device.Idiom == TargetIdiom.Tablet ? (heightRequest + ((int)(App.Settings.FontSize) * 5)) : heightRequest,
                ////MinimumWidthRequest = 150,
                //WidthRequest = widthRequest,//200,
                ////Margin = new Thickness(20, 0, 20, 0),
                VerticalOptions = LayoutOptions.StartAndExpand,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                DefaultText = text,
            };
        }

        public static SelectionView CheckBoxInputKitControl(ObservableCollection<InputSelectionModel> inputCheckboxesSource, int column, Input itemSelected, int indexLanguage)
        {
            List<int> selectedIndexList = new List<int>(); ;
            if (itemSelected != null)
            {
                foreach (var item in inputCheckboxesSource)
                {
                    if (item.Text == itemSelected.Languages.GetLanguage(indexLanguage))
                    {
                        var i = inputCheckboxesSource.IndexOf(item);
                        selectedIndexList.Add(i);
                    }
                }
            }

            var fontSize = (double)App.Current.Resources["NormalLabelControlFont"] + (int)App.Settings.FontSize;
            return new SelectionView()
            {
                ColumnNumber = column,
                ItemsSource = inputCheckboxesSource,
                ItemDisplayBinding = new Binding("Text"),
                SelectionType = SelectionType.CheckBox,
                RowSpacing = 10,
                ColumnSpacing = 1,
                SelectedIndexes = selectedIndexList,
                FontSizeView = fontSize,
                //HorizontalOptions = LayoutOptions.FillAndExpand,
                //VerticalOptions = LayoutOptions.FillAndExpand,
            };
        }

        public static CheckBox SingleCheckBoxControl(string id, string text, bool isVisible)
        {
            var widthRequest = ((App.ScreenWidth - 225) / 3);
            return new CheckBox()
            {
                ClassId = id,
                IsVisible = isVisible,
                Type = CheckBox.CheckType.Material,
                IsChecked = false,
                Text = text,
                VerticalOptions = LayoutOptions.StartAndExpand,
                HorizontalOptions = LayoutOptions.Start,
                Color = Colors.Blue,
                BackgroundColor = Colors.Bisque,
                BorderColor = Colors.Aqua,
                BoxBackgroundColor = Colors.DarkGoldenrod,
            };
        }

        public static CheckBox CheckBoxControl(string id, string text, bool isVisible)
        {
            var widthRequest = ((App.ScreenWidth - 225) / 3);
            return new CheckBox()
            {
                ClassId = id,
                IsVisible = isVisible,
                Type = CheckBox.CheckType.Material,
                IsChecked = false,
                Text = text,
                VerticalOptions = LayoutOptions.StartAndExpand,
                HorizontalOptions = LayoutOptions.Start,
                Color = Colors.Blue,
                BackgroundColor = Colors.Bisque,
                BorderColor = Colors.Aqua,
                BoxBackgroundColor = Colors.DarkGoldenrod,
            };
        }

        #endregion

        #region RadioButtonsControl

        public static SelectionView RadioButtonsControl(ObservableCollection<InputSelectionModel> inputCheckboxesSource, int column, Input itemSelected, int indexLanguage)
        {
            var i = 0;
            if (itemSelected != null)
            {
                foreach (var item in inputCheckboxesSource)
                {

                    if (item.Text == itemSelected.Languages.GetLanguage(indexLanguage))
                    {
                        i = inputCheckboxesSource.IndexOf(item);
                        break;
                    }

                }
            }

            var fontSize = ((double)App.Current.Resources["NormalLabelControlFont"] + (int)App.Settings.FontSize);
            return new SelectionView()
            {
                ColumnNumber = column,
                ItemsSource = inputCheckboxesSource,
                ItemDisplayBinding = new Binding("Text"),
                SelectionType = SelectionType.RadioButton,
                RowSpacing = 10,
                ColumnSpacing = 15,
                SelectedIndex = i,
                FontSizeView = fontSize,
                //HorizontalOptions = LayoutOptions.FillAndExpand,
                //VerticalOptions = LayoutOptions.FillAndExpand,
            };
        }

        #endregion

        #region RadioButtonControl

        public static CustomRadioButton RadioButtonControl(string id, string text, bool isVisible)
        {
            return new CustomRadioButton()
            {
                Text = text,
                ClassId = id,
                IsVisible = isVisible,
                Margin = new Thickness(2, 5, 10, 5),
            };
        }

        #endregion

        #region EntryControl

        public static CustomEntry EntryControl(string id, /*bool isMandatory,*/ bool isVisible, string value)
        {
            return new CustomEntry()
            {
                ClassId = id,
                FontSize = (double)App.Current.Resources["NormalLabelControlFont"] + (int)App.Settings.FontSize,
                Text = value,
                //IsMandatory = isMandatory,
                IsVisible = isVisible,
                //Margin = new Thickness(10, 0, 20, 0),
                WidthRequest = 5,
                VerticalOptions = LayoutOptions.CenterAndExpand,
                HorizontalOptions = LayoutOptions.FillAndExpand
            };
        }

        #endregion

        #region EntryNumericControl

        public static CustomEntry EntryNumericControl(string id, /*bool isMandatory,*/ bool isVisible, string value)
        {
            return new CustomEntry()
            {
                ClassId = id,
                Keyboard = Keyboard.Numeric,
                FontSize = (double)App.Current.Resources["NormalLabelControlFont"] + (int)App.Settings.FontSize,
                Text = value,
                //IsMandatory = isMandatory,
                IsVisible = isVisible,
                WidthRequest = 5,
                //Margin = new Thickness(20, 0, 20, 0),
                VerticalOptions = LayoutOptions.CenterAndExpand,
                HorizontalOptions = LayoutOptions.FillAndExpand
            };
        }

        #endregion

        #region ButtonControl

        public static Button ButtonControl(string id, string text)
        {
            return new Button()
            {
                ClassId = id,
                Text = text,
                FontSize = (double)App.Current.Resources["NormalLabelControlFont"] + (int)App.Settings.FontSize,
                Margin = new Thickness(0, 0, 0, 10),
                VerticalOptions = LayoutOptions.CenterAndExpand,
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                BackgroundColor = Colors.White,
                MinimumWidthRequest = 80,
                BorderWidth = 1,
                BorderColor = Colors.DimGray,
                CornerRadius = 8,
                Padding = new Thickness(0),
            };
        }


        #endregion

        #region TimePickerControl

        public static CustomTimePicker TimePickerControl(string id, bool isVisible, string value)
        {
            Debug.WriteLine($"time value is [{value}]");
            var timePicker = new CustomTimePicker()
            {
                ClassId = id,
                FontSize = (double)App.Current.Resources["NormalLabelControlFont"] + (int)App.Settings.FontSize,
                IsVisible = isVisible,
                VerticalOptions = LayoutOptions.CenterAndExpand,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                
            };
            if (string.IsNullOrWhiteSpace(value))
            {
                Debug.WriteLine($"NullableTime value is null");
                timePicker.NullableTime = null;
            }
            else
            {
                Debug.WriteLine($"NullableTime value is {value}");
                timePicker.NullableTime = TimeSpan.Parse(value);
            }

            timePicker.On<iOS>().SetUpdateMode(UpdateMode.WhenFinished);

            return timePicker;
        }

        #endregion

        #region DatePickerControl

        public static DatePicker DatePickerControl(string id, bool isVisible, string value)
        {
            return new DatePicker()
            {
                ClassId = id,
                FontSize = (double)App.Current.Resources["NormalLabelControlFont"] + (int)App.Settings.FontSize,
                Date = value == null ? DateTime.Now : DateTime.Parse(value),
                IsVisible = isVisible,
                //Margin = new Thickness(20, 0, 20, 0),
                VerticalOptions = LayoutOptions.CenterAndExpand,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                //WidthRequest = 200 / App.DisplayScaleFactor,
            };
        }

        #endregion

        #region PickerControl

        public static Picker PickerControl(string id, ObservableCollection<string> dataSource, bool isVisible)
        {
            return new Picker()
            {
                ClassId = id,
                FontSize = (double)App.Current.Resources["NormalLabelControlFont"] + (int)App.Settings.FontSize,
                IsVisible = isVisible,
                //Margin = new Thickness(20, 0, 20, 0),
                VerticalOptions = LayoutOptions.CenterAndExpand,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                ItemsSource = dataSource
            };
        }

        #endregion

        #region AutoCompleteControl

        public static AutoSuggestBox.AutoSuggestBox AutoCompleteControl(string id,
            ObservableCollection<string> dataSource, /*bool isMandatory,*/ bool isVisible, string value)
        {
            return new AutoSuggestBox.AutoSuggestBox()
            {
                ClassId = id,
                //IsMandatory = isMandatory,
                IsVisible = isVisible,
                Text = value,
                //FontSizeView = (float) (18 / App.DisplayScaleFactor),
                WidthRequest = 5,
                //Margin = new Thickness(20, 0, 20, 0),
                VerticalOptions = LayoutOptions.CenterAndExpand,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                ItemsSource = dataSource,
                TextColor = (Color)App.Current.Resources["BlackColor"],
            };
        }

        public static AutoCompleteView AutoCompleteViewControl(string id,
            ObservableCollection<string> dataSource, /*bool isMandatory,*/ bool isVisible, string value)
        {
            return new AutoCompleteView()
            {
                ClassId = id,
                IsVisible = isVisible,
                TextContent = value,
                WidthRequest = 5,
                VerticalOptions = LayoutOptions.CenterAndExpand,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                Suggestions = dataSource,
                TextColor = (Color)App.Current.Resources["BlackColor"],
                FonSizeText = (double)App.Current.Resources["NormalLabelControlFont"] + (int)App.Settings.FontSize,
                ShowSearchButton = false,
                //BackgroundColor = Color.White,
                SearchVerticalOptions = LayoutOptions.Start,
                SearchHorizontalOptions = LayoutOptions.StartAndExpand,
            };
        }

        #endregion

        #region LabelControl

        public static Microsoft.Maui.Controls.Label LabelControl(string text, bool isVisible, bool isMandatory = false)
        {
            var labelSpan = new Span { Text = text };
            var isMandatorySpan = new Span { Text = "  *", TextColor = Colors.Red };

            var formattedText = new FormattedString();
            formattedText.Spans.Add(labelSpan);

            if(isMandatory)
            {
                formattedText.Spans.Add(isMandatorySpan);
            }

            return new Microsoft.Maui.Controls.Label()
            {
                FormattedText = formattedText,
                //Margin = new Thickness(20, 0, 20, 0),
                WidthRequest = 100,
                //VerticalOptions = LayoutOptions.CenterAndExpand,
                FontSize = (double)App.Current.Resources["NormalLabelControlFont"] + (int)App.Settings.FontSize,
                //IsVisible = isVisible,
                IsVisible = (!string.IsNullOrEmpty(text)) && isVisible,
                Margin = new Thickness(0, 8),
            };
        }

        #endregion

        #region TextControl

        public static Microsoft.Maui.Controls.Label TextControl(string text)
        {
            return new Microsoft.Maui.Controls.Label()
            {
                Text = text,
                //Margin = new Thickness(0, 0, 0, 10),
                MinimumWidthRequest = 150,
                VerticalOptions = LayoutOptions.CenterAndExpand,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                FontSize = (double)App.Current.Resources["NormalLabelControlFont"] + (int)App.Settings.FontSize,
                IsVisible = !string.IsNullOrEmpty(text),
            };
        }

        #endregion

        #region HintTextControl

        public static Label HintTextControl(string text)
        {
            return new Label()
            {
                Text = text,
                VerticalOptions = LayoutOptions.CenterAndExpand,
                HorizontalOptions = LayoutOptions.StartAndExpand,
                FontSize = (double)App.Current.Resources["NormalLabelControlFont"] + (int)App.Settings.FontSize,
            };
        }

        #endregion

        #region EditorControl

        public static CustomEditor EditorControl(string id, /*bool isMandatory,*/ bool isVisible, string value)
        {
            return new CustomEditor()
            {
                ClassId = id,
                FontSize = (double)App.Current.Resources["NormalLabelControlFont"] + (int)App.Settings.FontSize,
                IsVisible = isVisible,
                Text = value,
                //IsMandatory = isMandatory,
                Margin = new Thickness(0, 8),
                VerticalOptions = LayoutOptions.FillAndExpand,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                HeightRequest = 100,
                BackgroundColor = Colors.White,
                IsTabStop = false,
            };
        }

        #endregion

    }
}
