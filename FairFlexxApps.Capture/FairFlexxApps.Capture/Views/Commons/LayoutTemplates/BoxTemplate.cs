using System;
using System.Collections.ObjectModel;
using System.Linq;
using FairFlexxApps.Capture.Behaviors;
//using FairFlexxApps.Capture.AutoSuggestBox;
using FairFlexxApps.Capture.Controls;
using FairFlexxApps.Capture.Controls.InputKit;
using FairFlexxApps.Capture.Enums.Templates;
using FairFlexxApps.Capture.Localization;
using FairFlexxApps.Capture.Managers;
using FairFlexxApps.Capture.Models.Templates.Controls;
using FairFlexxApps.Capture.Models.Templates.Pages;
using Microsoft.Maui;
using Label = Microsoft.Maui.Controls.Label;
using CustomRadioButton = FairFlexxApps.Capture.Controls.RadioButton.CustomRadioButton;
using BindableRadioGroup = FairFlexxApps.Capture.Controls.RadioButton.BindableRadioGroup;
using FairFlexxApps.Capture.Utilities;
using CheckBox = FairFlexxApps.Capture.Controls.InputKit.CheckBox;
using FairFlexxApps.Capture.Enums;
using Newtonsoft.Json.Linq;
using FairFlexxApps.Capture.Models;
using FairFlexxApps.Capture.ViewModels.Base;
using FairFlexxApps.Capture.Constants;
using System.Collections.Generic;

namespace FairFlexxApps.Capture.Views.Commons.LayoutTemplates
{
    public class BoxTemplate
    {
        #region Properties

        //public static ObservableCollection<BindableRadioGroup> RadioGroups;// = new ObservableCollection<BindableRadioGroup>();

        #endregion

        #region GetBoxTemplate

        public static View GetBoxTemplate(int languageIndex, Box boxTemplate)
        {
            //RadioGroups = new ObservableCollection<BindableRadioGroup>();

            var box = new Grid()
            {
                Padding = 2,
                BackgroundColor = Colors.DimGray,
                Margin = new Thickness(0, 0, 0, 20),
                RowSpacing = 0,
                ColumnSpacing = 0,
            };

            if (boxTemplate.BoxType == BoxType.HintAddress)
            {
                var hintBox = GetContentBoxHint(bodyTemplate: boxTemplate.Body, languagesIndex: languageIndex);
                box.Children.Add(hintBox);
            }
            else
            {
                var gridLayout = new Grid()
                {
                    ClassId = boxTemplate.BoxType,
                    HorizontalOptions = LayoutOptions.FillAndExpand,
                    VerticalOptions = LayoutOptions.FillAndExpand,
                    //Margin = new Thickness(20, 0, 20, 20),
                    RowSpacing = 0,
                    ColumnSpacing = 0,
                };

                gridLayout.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Auto) });
                gridLayout.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Auto) });

                var headLineLayout = GetHeadLine(headLine: boxTemplate.HeadLine.Language.GetLanguage(languageIndex));
                var contentBox = GetContentBox(bodyTemplate: boxTemplate.Body, boxType: boxTemplate.BoxType,
                    datasTemplate: boxTemplate.Data, languagesIndex: languageIndex, column: boxTemplate.Column);

                gridLayout.Add(headLineLayout, 0, 0);
                gridLayout.Add(contentBox, 0, 1);

                // add to have background
                box.Children.Add(gridLayout);
            }

            box.IsVisible = boxTemplate.Visible || boxTemplate.Mandatory;
            return box;

        }

        #endregion

        #region GetDoubleBoxTemplate

        public static View GetDoubleBoxTemplate(int languageIndex, Box currentBoxTemplate, Box nextBoxTemplate)
        {
            //RadioGroups = new ObservableCollection<BindableRadioGroup>();

            var twoHalfBoxContent = new Grid()
            {
                ClassId = SizeType.Half.ToString().ToLower(),
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.StartAndExpand,
            };

            twoHalfBoxContent.ColumnSpacing = 10;
            twoHalfBoxContent.ColumnDefinitions.Add(new ColumnDefinition
            {
                Width = new GridLength(1, GridUnitType.Star)
            });
            twoHalfBoxContent.ColumnDefinitions.Add(new ColumnDefinition
            {
                Width = new GridLength(1, GridUnitType.Star)
            });

            var column = (nextBoxTemplate != null)
                ? Math.Max(currentBoxTemplate.Column, nextBoxTemplate.Column)
                : currentBoxTemplate.Column;

            var contentCurrentBox = BoxTemplate.GetHalfBoxTemplate(languageIndex: languageIndex,
                boxTemplate: currentBoxTemplate, boxType: currentBoxTemplate.BoxType, column: column);

            twoHalfBoxContent.Add(contentCurrentBox, 0, 0);

            if (nextBoxTemplate != null)
            {
                var contentNextBox = BoxTemplate.GetHalfBoxTemplate(languageIndex: languageIndex,
                boxTemplate: nextBoxTemplate, boxType: nextBoxTemplate.BoxType, column: column);

                twoHalfBoxContent.Add(contentNextBox, 1, 0);
            }

            return twoHalfBoxContent;
        }

        #endregion

        #region GetHalfBoxTemplate

        public static View GetHalfBoxTemplate(int languageIndex, Box boxTemplate, string boxType, int column)
        {
            //RadioGroups = new ObservableCollection<BindableRadioGroup>();

            var gridLayout = new Grid()
            {
                ClassId = boxTemplate.BoxType,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand,
                Margin = new Thickness(0, 0, 0, 20),
                RowSpacing = 0,
                ColumnSpacing = 0,
                Padding = 2,
                BackgroundColor = Colors.DimGray,
            };
            gridLayout.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Auto) });
            gridLayout.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });

            var headLineLayout = GetHeadLine(headLine: boxTemplate.HeadLine.Language.GetLanguage(languageIndex));
            var contentBox = GetContentBox(bodyTemplate: boxTemplate.Body, boxType: boxType,
                datasTemplate: boxTemplate.Data, languagesIndex: languageIndex, column: column);

            gridLayout.Add(headLineLayout, 0, 0);
            gridLayout.Add(contentBox, 0, 1);

            gridLayout.IsVisible = boxTemplate.Visible;
            return gridLayout;
        }

        #endregion

        #region GetContentBoxHint

        public static View GetContentBoxHint(Body bodyTemplate, int languagesIndex)
        {
            var gridContent = new Grid()
            {
                ClassId = "hint",
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand,
                BackgroundColor = Colors.LightGray,
                //Margin = new Thickness(20, 0, 20, 20),
                Padding = new Thickness(20),
                RowSpacing = 0,
                ColumnSpacing = 0,
            };
            gridContent.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Auto) });
            gridContent.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

            var icHint = new Image()
            {
                Source = "ic_hint_info",
                VerticalOptions = LayoutOptions.CenterAndExpand,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                WidthRequest = 40,
                HeightRequest = 40,
                Margin = new Thickness(0, 0, 15, 0),
            };

            var stackContent = new StackLayout();
            foreach (var item in bodyTemplate.Rows)
            {
                if (item.Texts.Any())
                {
                    stackContent.Children.Add(ControlTemplate.HintTextControl(item.Texts.Select(x => x).FirstOrDefault().Languages.GetLanguage(languagesIndex)));
                }
            }

            // add Hint icon
            gridContent.Add(icHint, 0, 0);
            gridContent.Add(stackContent, 1, 0);

            return gridContent;
        }

        #endregion

        #region GetHeadLine

        public static View GetHeadLine(string headLine)
        {
            var labelTitle = new Label()
            {
                Text = headLine,
                TextColor = Colors.White,
                //FontAttributes = FontAttributes.Bold,
                //FontSize = (Math.Max(App.DisplayScreenWidth, App.DisplayScreenHeight) > 1620) ? 18 + 4 : 18,
                FontSize = (double)App.Current.Resources["LargeLabelControlFont"] + (int)App.Settings.FontSize,
            };

            var stackTitle = new StackLayout()
            {
                ClassId = "header",
                HorizontalOptions = LayoutOptions.FillAndExpand,
                BackgroundColor = Colors.DimGray,
                Children = { labelTitle },
                Padding = new Thickness(20, 8),
            };

            return stackTitle;
        }

        #endregion

        #region GetContentBox

        public static View GetContentBox(Body bodyTemplate, string boxType, ObservableCollection<Data> datasTemplate, int languagesIndex, int column)
        {
            var stackContent = new StackLayout()
            {
                ClassId = "body",
                HorizontalOptions = LayoutOptions.FillAndExpand,
                BackgroundColor = Colors.LightGray,
                Padding = new Thickness(20, 10),
                //Children = { labelTitle },
            };

            if (bodyTemplate != null)
            {
                foreach (var rowItem in bodyTemplate.Rows)
                {
                    rowItem.Column = Device.Idiom == TargetIdiom.Tablet || boxType == BoxType.Visitor ? column : 1;

                    var checkboxesAny = (rowItem.Inputs.Where(item => item.InputType == InputType.checkbox.ToString())).Any();

                    if (checkboxesAny)
                    {
                        var contentForRow = GetCheckboxesInRow(rowTemplate: rowItem, boxType: boxType,
                            languagesIndex: languagesIndex);
                        stackContent.Children.Add(contentForRow);
                    }
                    else
                    {
                        var contentForRow = GetContentRow(rowTemplate: rowItem, boxType: boxType, datasTemplate: datasTemplate,
                            languagesIndex: languagesIndex);
                        stackContent.Children.Add(contentForRow);
                    }

                }
            }

            return stackContent;
        }

        #endregion

        #region GetContentRow

        public static View GetContentRow(Row rowTemplate, string boxType, ObservableCollection<Data> datasTemplate, int languagesIndex)
        {
            rowTemplate.Inputs = new ObservableCollection<Input>(rowTemplate.Inputs.Where(x => x.Visible).ToList());
            var numbersOfControlInRow = rowTemplate.Texts.Count + rowTemplate.Labels.Count + rowTemplate.Inputs.Count +
                                       rowTemplate.Buttons.Count;

            var numberOfEntryInRow = (rowTemplate.Inputs.Where(item => item.InputType == InputType.text.ToString())).Count();

            var contentRow = new StackLayout()
            {
                Orientation = StackOrientation.Vertical,
                VerticalOptions = LayoutOptions.FillAndExpand,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                BackgroundColor = Colors.Transparent,
            };

            if (rowTemplate != null)
            {
                #region Label Control

                if (rowTemplate.Labels != null)
                {
                    var isMandatory = false;

                    foreach (var input in rowTemplate.Inputs)
                    {
                        if (input.Mandatory)
                        {
                            isMandatory = true;
                            break;
                        }
                    }

                    foreach (var label in rowTemplate.Labels)
                    {
                        contentRow.Children.Add(ControlTemplate.LabelControl(text: label.Language.GetLanguage(languagesIndex), isVisible: label.Visible, isMandatory: isMandatory));
                    }
                }

                #endregion

                #region Button Control

                if (rowTemplate.Buttons != null)
                {
                    foreach (var button in rowTemplate.Buttons)
                    {
                        contentRow.Children.Add(ControlTemplate.ButtonControl(button.ButtonType, button.Languages.GetLanguage(languagesIndex)));
                    }
                }

                #endregion

                #region Text Control

                if (rowTemplate.Texts != null)
                {
                    foreach (var text in rowTemplate.Texts)
                    {
                        contentRow.Children.Add(ControlTemplate.TextControl(text.Languages.GetLanguage(languagesIndex)));
                    }
                }

                #endregion

                #region Input Control

                if (rowTemplate.Inputs != null)
                {
                    var radiosAny = (rowTemplate.Inputs.Where(item => item.InputType == InputType.radio.ToString())).Any();
                    var checkBoxAny = (rowTemplate.Inputs.Where(item => item.InputType == InputType.checkbox.ToString())).Any();

                    if (radiosAny)
                    {
                        //var itemsSources = from radio in rowTemplate.Inputs
                        //    where (radio.Visible)
                        //    select (new RadioButtonModel()
                        //    {
                        //        RadioButtonId = radio.InputId,
                        //        Text = radio.Languages.GetLanguage(indexOfLanguage: languagesIndex),
                        //    });

                        #region Radio Button

                        var itemsSource = new ObservableCollection<InputSelectionModel>(rowTemplate.Inputs
                            .Where(radio => radio.Visible && radio.InputType == InputType.radio.ToString())
                            .Select(radio => new InputSelectionModel()
                            {
                                InputId = radio.InputId,
                                Text = radio.Languages.GetLanguage(indexOfLanguage: languagesIndex),
                            }));

                        var naRadioButton = new InputSelectionModel
                        {
                            InputId = "empty",
                            Text = "N/A",
                        };

                        if (!boxType.Equals(BoxType.Visitor))
                            itemsSource.Insert(index: 0, item: naRadioButton);

                        var itemsSourceWithoutEmpty = itemsSource.Where(r => !string.IsNullOrEmpty(r.Text));
                        itemsSource = new ObservableCollection<InputSelectionModel>(itemsSourceWithoutEmpty);

                        var indexItem = rowTemplate.Inputs.FirstOrDefault(item => item.InputType == InputType.radio.ToString() && item.Value == true.ToString());

                        var radioButtons = ControlTemplate.RadioButtonsControl(inputCheckboxesSource: itemsSource,
                            column: rowTemplate.Column, itemSelected: indexItem, indexLanguage: languagesIndex);

                        if (boxType.Equals(BoxType.Visitor))
                        {
                            radioButtons.Margin = Device.Idiom == TargetIdiom.Phone ? new Thickness(0) : new Thickness(105, 0, 0, 0);
                            radioButtons.ColumnSpacing = 30;
                        }
                        else
                        {
                            radioButtons.HorizontalOptions = LayoutOptions.FillAndExpand;
                            radioButtons.VerticalOptions = LayoutOptions.Start;
                        }

                        foreach (var control in rowTemplate.Inputs.Where(radio => radio.InputType == InputType.radio.ToString()))
                        {
                            if (control.Value == null)
                            {
                                control.Value = false.ToString();
                            }
                        }
                        //if (boxType.Equals(BoxType.Visitor))
                        //{
                        //    rowTemplate.Inputs[0].Value = true.ToString();
                        //}

                        radioButtons.PropertyChanged += (s, e) =>
                        {
                            if (e.PropertyName == nameof(SelectionView.SelectedIndex))
                            {
                                foreach (var control in rowTemplate.Inputs)
                                {
                                    control.Value =
                                        control.Languages.GetLanguage(indexOfLanguage: languagesIndex) ==
                                        itemsSource[radioButtons.SelectedIndex].Text
                                            ? true.ToString()
                                            : false.ToString();
                                }
                            }
                        };

                        #region If row is Gender then set default gender is the first option

                        if(rowTemplate.Inputs.Where(input => input.InputId.ToLower() == "mr" || input.InputId.ToLower() == "ms").Any())
                        {
                            rowTemplate.Inputs[0].Value = true.ToString();
                        }

                        #endregion

                        // The number of control is less than column in row
                        var buttonWithColumnLayout = new Grid()
                        {
                            HorizontalOptions = LayoutOptions.FillAndExpand,
                            VerticalOptions = LayoutOptions.Center,
                            Padding = 5,
                            ColumnSpacing = 0,
                            RowSpacing = 0,
                        };

                        if (itemsSource.Count < 3)
                        {
                            itemsSource.Add(new InputSelectionModel
                            {
                                InputId = "empty",
                                Text = " ",
                            });
                        }
                        //buttonWithColumnLayout.HeightRequest = 55;

                        for (int col = 0; col < rowTemplate.Column; col++)
                        {
                            buttonWithColumnLayout.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                        }
                        //Grid.SetRow(radioButtons, 0);
                        //Grid.SetColumn(radioButtons, 0);
                        //Grid.SetColumnSpan(radioButtons, 2);
                        //buttonWithColumnLayout.Children.Add(radioButtons);
                        // buttonWithColumnLayout.Children.Add(new StackLayout() { HorizontalOptions = LayoutOptions.FillAndExpand }, rowTemplate.Column-1, 1, 0, 1);
                        buttonWithColumnLayout.Add(radioButtons, 0, 2, 0, 1);

                        contentRow.Children.Add((rowTemplate.Column > itemsSource.Count) ? buttonWithColumnLayout : radioButtons);

                        #endregion
                    }
                    #region CheckBox InputKit
                    //else if (checkBoxAny)
                    //{
                    //    

                    //    var itemsSource = new ObservableCollection<InputSelectionModel>(rowTemplate.Inputs
                    //        .Where(checkbox => checkbox.Visible)
                    //        .Select(checkbox => new InputSelectionModel()
                    //        {
                    //            InputId = checkbox.InputId,
                    //            Text = checkbox.Languages.GetLanguage(indexOfLanguage: languagesIndex),
                    //        }));

                    //    var indexItem = rowTemplate.Inputs.FirstOrDefault(item => item.InputType == InputType.checkbox.ToString() && item.Value == true.ToString());

                    //    var checkboxButtons = ControlTemplate.CheckBoxInputKitControl(inputCheckboxesSource: itemsSource,
                    //        column: rowTemplate.Column, itemSelected: indexItem, indexLanguage: languagesIndex);

                    //    if (boxType.Equals(BoxType.Visitor))
                    //    {
                    //        checkboxButtons.Margin = new Thickness(105, 0, 0, 0);
                    //        checkboxButtons.ColumnSpacing = 30;
                    //    }
                    //    else
                    //    {
                    //        checkboxButtons.HorizontalOptions = LayoutOptions.FillAndExpand;
                    //        checkboxButtons.VerticalOptions = LayoutOptions.Start;
                    //    }

                    //    foreach (var control in rowTemplate.Inputs)
                    //    {
                    //        if (control.Value == null)
                    //        {
                    //            control.Value = false.ToString();
                    //        }
                    //    }
                    //    if (boxType.Equals(BoxType.Visitor))
                    //    {
                    //        rowTemplate.Inputs[0].Value = true.ToString();
                    //    }

                    //    checkboxButtons.PropertyChanged += (s, e) =>
                    //    {
                    //        if (e.PropertyName == nameof(SelectionView.SelectedIndex))
                    //        {
                    //            foreach (var control in rowTemplate.Inputs)
                    //            {
                    //                if (control.Languages.GetLanguage(indexOfLanguage: languagesIndex) == itemsSource[checkboxButtons.SelectedIndex].Text)
                    //                {
                    //                    control.Value = true.ToString();
                    //                }
                    //                else
                    //                {
                    //                    control.Value = false.ToString();
                    //                }
                    //            }
                    //        }
                    //    };

                    //    // The number of control is less than column in row
                    //    var buttonWithColumnLayout = new Grid()
                    //    {
                    //        HorizontalOptions = LayoutOptions.FillAndExpand,
                    //        VerticalOptions = LayoutOptions.CenterAndExpand,
                    //        HeightRequest = 50,
                    //        Padding = 0,
                    //        ColumnSpacing = 0,
                    //        RowSpacing = 0,
                    //    };

                    //    for (int col = 0; col < rowTemplate.Column; col++)
                    //    {
                    //        buttonWithColumnLayout.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                    //    }
                    //    //Grid.SetRow(checkboxButtons, 0);
                    //    //Grid.SetColumn(checkboxButtons, 0);
                    //    //Grid.SetColumnSpan(checkboxButtons, 2);
                    //    //buttonWithColumnLayout.Children.Add(checkboxButtons);
                    //    buttonWithColumnLayout.Children.Add(checkboxButtons, 0, 2, 0, 1);

                    //    contentRow.Children.Add((rowTemplate.Column > itemsSource.Count) ? buttonWithColumnLayout : checkboxButtons);

                    //    
                    //}
                    #endregion
                    else
                    {
                        foreach (var input in rowTemplate.Inputs)
                        {
                            var inputType = (InputType)Enum.Parse(typeof(InputType), input.InputType);

                            switch (inputType)
                            {
                                #region Text

                                case InputType.text:
                                    var entry = ControlTemplate.EntryControl(id: input.InputId, /*isMandatory: input.Mandatory,*/
                                        isVisible: input.Visible, value: input.Value);

                                    if (input.InputId == "email")
                                    {
                                        entry.Behaviors.Add(new EmailValidatorBehavior());
                                        entry.Keyboard = Keyboard.Email;
                                        var hint = new Label()  
                                        {
                                            Text = TranslateExtension.Get("NotCorrect"),
                                            IsVisible = false,
                                            TextColor = Colors.Red,
                                            VerticalOptions = LayoutOptions.Center
                                        };
                                        entry.TextChanged += (s, e) =>
                                        {
                                            if (((Entry)s).PlaceholderColor == Colors.Red && ((Entry)s).Text != "")
                                            {
                                                hint.IsVisible = true;
                                            }
                                            else
                                            {
                                                hint.IsVisible = false;
                                            }
                                            contentRow.Children.Add(hint);
                                        };                                        
                                    }

                                    var fisrtLetterUppercaseFields = new List<String>() {
                                        "firstname", "lastname", "position", "company", "addrline1", "addrline2", "city"
                                    };  
                                    
                                    if(fisrtLetterUppercaseFields.Where(fieldId => fieldId == input.InputId.ToLower()).Any())
                                    {
                                        entry.Keyboard = Keyboard.Create(KeyboardFlags.CapitalizeSentence);
                                    }

                                    //if (numberOfEntryInRow > 1)
                                    //{
                                    //    entry.HorizontalOptions = LayoutOptions.Start;
                                    //    entry.WidthRequest = 150;
                                    //    numberOfEntryInRow--;
                                    //}
                                    entry.TextChanged += (s, e) =>
                                    {
                                        input.Value = entry.Text;
                                    };
                                    contentRow.Children.Add(entry);
                                    break;

                                #endregion

                                #region Checkbox
                                /*
                                                            case InputType.checkbox:
                                                                if (input.InputChildren != null)
                                                                {
                                                                    var checkbox = ControlTemplate.CheckBoxControl(id: input.InputId,
                                                                        text: input.Languages.GetLanguage(languagesIndex), isVisible: input.Visible);

                                                                    //CustomCheckBox checkbox = new CustomCheckBox()
                                                                    //{
                                                                    //    ClassId = input.InputId,
                                                                    //    OutlineColor = Color.White,
                                                                    //    IsChecked = false,
                                                                    //    FontSize = 18,
                                                                    //    Margin = new Thickness(0, 5),
                                                                    //    HeightRequest = 64,
                                                                    //    MinimumWidthRequest = 150,
                                                                    //    WidthRequest = 200,
                                                                    //    VerticalOptions = LayoutOptions.Start,
                                                                    //    HorizontalOptions = LayoutOptions.Start,
                                                                    //    DefaultText = input.Languages.GetLanguage(languagesIndex),
                                                                    //};

                                                                    var entrychildren = new Entry()
                                                                    {
                                                                        ClassId = input.InputId,
                                                                        FontSize = 18,
                                                                        VerticalOptions = LayoutOptions.Start,
                                                                        HorizontalOptions = LayoutOptions.FillAndExpand,
                                                                        Margin = new Thickness(4, -5, 0, 0),
                                                                        IsVisible = false,
                                                                    };

                                                                    checkbox.PropertyChanged += (sender, e) =>
                                                                    {
                                                                        var check = (CustomCheckBox)sender;
                                                                        entrychildren.IsVisible = check.IsChecked;
                                                                    };

                                                                    StackLayout stackVertical = new StackLayout()
                                                                    {
                                                                        Orientation = StackOrientation.Vertical,
                                                                        VerticalOptions = LayoutOptions.StartAndExpand,
                                                                        HorizontalOptions = LayoutOptions.StartAndExpand,
                                                                        Margin = new Thickness(-4, 0, 0, 0),
                                                                        WidthRequest = 196,
                                                                    };
                                                                    stackVertical.Children.Add(checkbox);
                                                                    stackVertical.Children.Add(entrychildren);
                                                                    contentRow.Children.Add(stackVertical);
                                                                }
                                                                else
                                                                {
                                                                    if (numbersOfControlInRow == 1)
                                                                    {
                                                                        var checkBox = ControlTemplate.SingleCheckBoxControl(id: input.InputId,
                                                                            text: input.Languages.GetLanguage(languagesIndex), isVisible: input.Visible);
                                                                        contentRow.Children.Add(checkBox);
                                                                    }
                                                                    else
                                                                    {
                                                                        var checkBox = ControlTemplate.CheckBoxControl(id: input.InputId,
                                                                            text: input.Languages.GetLanguage(languagesIndex), isVisible: input.Visible);
                                                                        contentRow.Children.Add(checkBox);
                                                                    }
                                                                }
                                                                break;
                                */
                                #endregion

                                #region Radio
                                /*
                                                            case InputType.radio:
                                                                bool checkRadio = false;

                                                                var countRadioOfRow = rowTemplate.Inputs.Select(i => i.InputType == InputType.radio.ToString()).Count();

                                                                foreach (var radioGroup in RadioGroups)
                                                                {
                                                                    //int controls = 0;
                                                                    if (radioGroup.ClassId == input.InputName)
                                                                    {
                                                                        var radioButton = ControlTemplate.RadioButtonControl(id: input.InputName,
                                                                            text: input.Languages.GetLanguage(languagesIndex), isVisible: input.Visible);
                                                                        radioButton.WidthRequest = 250;
                                                                        radioButton.MinimumWidthRequest = 200;

                                                                        radioGroup.Children.Add(radioButton);

                                                                        //radioGroup.Children.Add(new CustomRadioButton()
                                                                        //{
                                                                        //    Text = input.Languages.GetLanguage(languagesIndex),
                                                                        //    ClassId = input.InputName,
                                                                        //    WidthRequest = 250,
                                                                        //    MinimumWidthRequest = 200,
                                                                        //    Margin = new Thickness(2, 5, 0, 5),
                                                                        //});
                                                                        checkRadio = true;
                                                                        break;
                                                                    }
                                                                    //controls++;
                                                                }
                                                                if (!checkRadio)
                                                                {
                                                                    var radioNA = ControlTemplate.RadioButtonControl(id: input.InputName,
                                                                            text: "N/A", isVisible: input.Visible);
                                                                    if (countRadioOfRow > 1)
                                                                    {
                                                                        var group = new BindableRadioGroup(StackOrientation.Horizontal)
                                                                        {
                                                                            ClassId = input.InputName,
                                                                            Margin = new Thickness(100, 5, 0, 5),
                                                                            HorizontalOptions = LayoutOptions.StartAndExpand,
                                                                        };
                                                                        var radioButton = ControlTemplate.RadioButtonControl(id: input.InputName,
                                                                            text: input.Languages.GetLanguage(languagesIndex), isVisible: input.Visible);
                                                                        //var radioButton = new CustomRadioButton()
                                                                        //{
                                                                        //    Text = input.Languages.GetLanguage(languagesIndex),
                                                                        //    ClassId = input.InputName,
                                                                        //    Margin = new Thickness(2, 5, 10, 5),
                                                                        //};
                                                                        // set width of radio if the number of radio > 2
                                                                        if (countRadioOfRow > 2) 
                                                                        {
                                                                            group.Margin = new Thickness(2, 5, 0, 5);
                                                                            radioButton.WidthRequest = 250;
                                                                            radioButton.MinimumWidthRequest = 200;
                                                                        }
                                                                        group.Children.Add(radioNA);
                                                                        group.Children.Add(radioButton);
                                                                        contentRow.Children.Add(group);
                                                                        RadioGroups.Add(group);
                                                                    }
                                                                    else
                                                                    {
                                                                        var group = new BindableRadioGroup()
                                                                        {
                                                                            ClassId = input.InputName
                                                                        };
                                                                        var radioButton = ControlTemplate.RadioButtonControl(id: input.InputName,
                                                                            text: input.Languages.GetLanguage(languagesIndex), isVisible: input.Visible);
                                                                        group.Children.Add(radioNA);
                                                                        group.Children.Add(radioButton);
                                                                        //group.Children.Add(new CustomRadioButton()
                                                                        //{
                                                                        //    Text = input.Languages.GetLanguage(languagesIndex),
                                                                        //    ClassId = input.InputName,
                                                                        //    Margin = new Thickness(2, 5, 0, 5),
                                                                        //});
                                                                        contentRow.Children.Add(group);
                                                                        RadioGroups.Add(group);
                                                                    }
                                                                }
                                                                foreach (var radioGroup in RadioGroups)
                                                                {
                                                                    radioGroup.SelectedIndex = 0;
                                                                }
                                                                break;
                                */
                                #endregion

                                #region Number

                                case InputType.number:
                                    var entryNumer = ControlTemplate.EntryNumericControl(id: input.InputId,
                                        /*isMandatory: input.Mandatory,*/ isVisible: input.Visible, value: input.Value);
                                    entryNumer.TextChanged += (s, e) =>
                                    {
                                        input.Value = entryNumer.Text;
                                    };
                                    contentRow.Children.Add(entryNumer);
                                    break;

                                #endregion

                                #region Time

                                case InputType.time:
                                    var time = ControlTemplate.TimePickerControl(id: input.InputId, isVisible: input.Visible, value: input.Value);
                                    time.PropertyChanged += (s, e) =>
                                    {
                                        input.Value = time.NullableTime.ToString().Trim();
                                    };
                                    contentRow.Children.Add(time);
                                    break;

                                #endregion

                                #region Date

                                case InputType.date:
                                    var date = ControlTemplate.DatePickerControl(id: input.InputId, isVisible: input.Visible, value: input.Value);
                                    date.PropertyChanged += (s, e) =>
                                    {
                                        input.Value = date.Date.ToShortDateString();
                                    };
                                    contentRow.Children.Add(date);
                                    break;

                                #endregion

                                #region Text Area

                                case InputType.multitext:
                                    var textArea = ControlTemplate.EditorControl(id: input.InputId,
                                        /*isMandatory: input.Mandatory,*/ isVisible: input.Visible, value: input.Value);
                                    textArea.TextChanged += (s, e) =>
                                    {
                                        input.Value = textArea.Text;
                                    };
                                    contentRow.Children.Add(textArea);
                                    break;

                                #endregion

                                #region Auto

                                case InputType.auto:
                                    var previousContactSelected = Preferences.Get(PreferencesKey.ContactValue.ToString(), null);
                                    if (input.InputId.Contains(XMLConstants.CONTACT_INPUT_ID_CONTAINS))
                                    {
                                        input.Value = previousContactSelected;
                                    }
                                    if (Device.RuntimePlatform == Device.Android)
                                    {
                                        var dataSource = new ObservableCollection<string>();
                                        if (!string.IsNullOrEmpty(input.InputData))
                                        {
                                            foreach (var data in datasTemplate)
                                            {
                                                if (data.DataId == input.InputData && data.LanguageItem != null)
                                                {
                                                    dataSource = data.LanguageItem.GeDatatLanguages(indexOfLanguage: languagesIndex);
                                                    break;
                                                }
                                            }
                                        }
                                        var autoComplete = ControlTemplate.AutoCompleteControl(id: input.InputId,
                                            dataSource: dataSource, /*isMandatory: input.Mandatory,*/ isVisible: input.Visible, value: input.Value);

                                        autoComplete.TextColor = dataSource.IndexOf(autoComplete.Text) != -1 ? Colors.Black : Colors.Red;
                                        autoComplete.TextChanged += (sender, e) =>
                                        {
                                            if (string.IsNullOrWhiteSpace(autoComplete.Text))
                                            {
                                                autoComplete.ItemsSource = null;
                                            } else
                                            {
                                                var coutry = new Country();
                                                var countryList = CountryService.CountryList();
                                                for(int i = 0; i < countryList.Count; i++)
                                                {
                                                    if (autoComplete.Text.ToLower() == countryList.ElementAt(i).GermanyName.ToLower())
                                                    {
                                                        coutry = countryList.ElementAt(i);
                                                    }
                                                }
                                                if(coutry.EnglishName != null && coutry.GermanyName != null)
                                                {
                                                    autoComplete.ItemsSource = dataSource.Where(s => s.ToLower().Contains(coutry.EnglishName.ToLower())).ToList();
                                                }
                                                else
                                                {
                                                    autoComplete.ItemsSource = dataSource.Where(s => s.ToLower().Contains(autoComplete.Text.ToLower())).ToList();
                                                }
                                            }
                                            autoComplete.TextColor = dataSource.IndexOf(autoComplete.Text) != -1 ? Color.Black : Color.Red;
                                            input.Value = autoComplete.Text;
                                            if (input.InputId.Contains(XMLConstants.CONTACT_INPUT_ID_CONTAINS))
                                            {
                                                Preferences.Set(PreferencesKey.ContactValue.ToString(), autoComplete.Text);
                                            }
                                        };

                                        contentRow.Children.Add(autoComplete);
                                    }
                                    else
                                    {
                                        var dataSource = new ObservableCollection<string>();
                                        if (!string.IsNullOrEmpty(input.InputData))
                                        {
                                            foreach (var data in datasTemplate)
                                            {
                                                if (data.DataId == input.InputData && data.LanguageItem != null)
                                                {
                                                    dataSource = data.LanguageItem.GeDatatLanguages(indexOfLanguage: languagesIndex);
                                                    break;
                                                }
                                            }
                                        }
                                        var autoComplete = ControlTemplate.AutoCompleteViewControl(id: input.InputId,
                                            dataSource: dataSource, /*isMandatory: input.Mandatory,*/ isVisible: input.Visible, value: input.Value);

                                        autoComplete.TextColor = dataSource.IndexOf(autoComplete.Text) != -1 ? Colors.Black : Colors.Red;
                                        autoComplete.TextChanged += (sender, e) =>
                                        {
                                            if (string.IsNullOrWhiteSpace(autoComplete.Text))
                                            {
                                                autoComplete.SuggestionsHeightRequest = 0;
                                            }
                                            else
                                            {
                                                var countryList = CountryService.CountryList();
                                                for (int i = 0; i < countryList.Count; i++)
                                                {
                                                    if (autoComplete.Text.ToLower() == countryList.ElementAt(i).GermanyName.ToLower())
                                                    {
                                                        autoComplete.Text = countryList.ElementAt(i).EnglishName;
                                                    }
                                                }
                                                autoComplete.SuggestionsHeightRequest = dataSource.Where(s => s.ToLower().Contains(autoComplete.Text.ToLower()))
                                                    .ToList().Count * 40;
                                            }
                                            autoComplete.TextColor = dataSource.IndexOf(autoComplete.Text) != -1 ? Color.Black : Color.Red;
                                            input.Value = autoComplete.Text;
                                            if (input.InputId.Contains(XMLConstants.CONTACT_INPUT_ID_CONTAINS))
                                            {
                                                Preferences.Set(PreferencesKey.ContactValue.ToString(), autoComplete.Text);
                                            }
                                        };

                                        contentRow.Children.Add(autoComplete);
                                    }

                                    break;

                                #endregion

                                #region List

                                case InputType.list:
                                    if (Device.RuntimePlatform == Device.Android)
                                    {
                                        var dataSource = new ObservableCollection<string>();
                                        if (!string.IsNullOrEmpty(input.InputData))
                                        {
                                            foreach (var data in datasTemplate)
                                            {
                                                if (data.DataId == input.InputData && data.LanguageItem != null)
                                                {
                                                    dataSource = data.LanguageItem.GeDatatLanguages(indexOfLanguage: languagesIndex);
                                                    break;
                                                }
                                            }
                                        }
                                        var autoComplete = ControlTemplate.AutoCompleteControl(id: input.InputId,
                                            dataSource: dataSource, /*isMandatory: input.Mandatory,*/ isVisible: input.Visible, value: input.Value);

                                        autoComplete.TextColor = dataSource.IndexOf(autoComplete.Text) != -1 ? Colors.Black : Colors.Red;
                                        autoComplete.TextChanged += (sender, e) =>
                                        {
                                            autoComplete.ItemsSource = string.IsNullOrWhiteSpace(autoComplete.Text)
                                                ? null
                                                : dataSource.Where(s => s.ToLower().Contains(autoComplete.Text.ToLower()))
                                                    .ToList();
                                            autoComplete.TextColor = dataSource.IndexOf(autoComplete.Text) != -1 ? Color.Black : Color.Red;
                                            input.Value = autoComplete.Text;
                                        };

                                        contentRow.Children.Add(autoComplete);
                                    }
                                    else
                                    {
                                        var dataSource = new ObservableCollection<string>();
                                        if (!string.IsNullOrEmpty(input.InputData))
                                        {
                                            foreach (var data in datasTemplate)
                                            {
                                                if (data.DataId == input.InputData && data.LanguageItem != null)
                                                {
                                                    dataSource = data.LanguageItem.GeDatatLanguages(indexOfLanguage: languagesIndex);
                                                    break;
                                                }
                                            }
                                        }
                                        var autoComplete = ControlTemplate.AutoCompleteViewControl(id: input.InputId,
                                            dataSource: dataSource, /*isMandatory: input.Mandatory,*/ isVisible: input.Visible, value: input.Value);

                                        autoComplete.TextColor = dataSource.IndexOf(autoComplete.Text) != -1 ? Colors.Black : Colors.Red;
                                        autoComplete.TextChanged += (sender, e) =>
                                        {
                                            autoComplete.SuggestionsHeightRequest = string.IsNullOrWhiteSpace(autoComplete.Text)
                                                ? 0
                                                : dataSource.Where(s => s.ToLower().Contains(autoComplete.Text.ToLower()))
                                                    .ToList().Count * 40;
                                            autoComplete.TextColor = dataSource.IndexOf(autoComplete.Text) != -1 ? Color.Black : Color.Red;
                                            input.Value = autoComplete.Text;
                                        };

                                        contentRow.Children.Add(autoComplete);
                                    }

                                    break;

                                #endregion

                                #region Note

                                case InputType.note:
                                    var editor = new Editor
                                    {
                                        FontSize = (double)App.Current.Resources["NormalLabelControlFont"] + (int)App.Settings.FontSize,
                                        TextColor = Colors.Black,
                                        HorizontalOptions = LayoutOptions.Center,
                                        AutoSize = EditorAutoSizeOption.TextChanges,
                                        MinimumHeightRequest = Device.Idiom == TargetIdiom.Tablet ? 800 : 400,
                                        Margin = new Thickness(0),
                                        WidthRequest = Application.Current.MainPage.Width * 0.8,
                                        IsVisible = input.Visible,
                                    };


                                    editor.TextChanged += (s, e) =>
                                    {   
                                        input.Value = editor.Text;
                                    };
                                    contentRow.Orientation = StackOrientation.Vertical;
                                    contentRow.Children.Add(editor);
                                    break;

                                #endregion
                            }
                        }
                    }

                }

                #endregion

            }
            

            return contentRow;
        }

        //private static void AutoSuggestForEntry(object sender, AutoSuggestBoxTextChangedEventArgs e)
        //{
        //    throw new NotImplementedException();
        //}

        #endregion

        #region GetCheckboxesInRow

        public static View GetCheckboxesInRow(Row rowTemplate, string boxType, int languagesIndex)
        {
            var numbersOfControlInRow = rowTemplate.Texts.Count + rowTemplate.Labels.Count + rowTemplate.Inputs.Count +
                                        rowTemplate.Buttons.Count;

            var contentRow = new Grid()
            {
                VerticalOptions = LayoutOptions.FillAndExpand,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                BackgroundColor = Colors.Transparent,   
            };
            
            foreach (var input in rowTemplate.Inputs)
            {
                var inputType = (InputType)Enum.Parse(typeof(InputType), input.InputType);

                switch (inputType)
                {
                    #region Checkbox

                    case InputType.checkbox:
                        if (input.InputChildren != null)
                        {
                            var checkbox = ControlTemplate.CustomCheckBoxControl(id: input.InputId,
                                text: input.Languages.GetLanguage(languagesIndex), isVisible: input.Visible, column: rowTemplate.Column, value: input.Value);

                            var entrychildren = new CustomEntry()
                            {
                                ClassId = input.InputId,
                                //FontSize = (Math.Max(App.DisplayScreenWidth, App.DisplayScreenHeight) > 1620) ? 16 + 4 : 16,
                                FontSize = (double)App.Current.Resources["NormalLabelControlFont"] + (int)App.Settings.FontSize,
                                Text = input.InputChildren.Value,
                                VerticalOptions = LayoutOptions.Start,
                                HorizontalOptions = LayoutOptions.FillAndExpand,
                                Margin = new Thickness(4, -15, 0, 0),
                                IsVisible = false,
                            };

                            entrychildren.TextChanged += (s, e) =>
                            {
                                input.InputChildren.Value = entrychildren.Text;
                                input.ValueChildren = entrychildren.Text;
                            };

                            checkbox.PropertyChanged += (sender, e) =>
                            {
                                var check = (CustomCheckBox)sender;
                                entrychildren.IsVisible = check.IsChecked;
                                input.Value = check.IsChecked.ToString();
                            };

                            StackLayout stackVertical = new StackLayout()
                            {
                                Orientation = StackOrientation.Vertical,
                                VerticalOptions = LayoutOptions.StartAndExpand,
                                HorizontalOptions = LayoutOptions.FillAndExpand,
                                Margin = new Thickness(0),
                                //WidthRequest = 200,
                            };
                            stackVertical.Children.Add(checkbox);
                            stackVertical.Children.Add(entrychildren);
                            contentRow.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Auto) });
                            contentRow.Add(stackVertical, contentRow.Children.Count % rowTemplate.Column,
                                contentRow.Children.Count / rowTemplate.Column);
                        }
                        else
                        {
                            if (numbersOfControlInRow == 1 && !string.IsNullOrEmpty(boxType) && !boxType.Equals(BoxType.Generic))
                            {
                                var checkBoxSingle = ControlTemplate.CustomSingleCheckBoxControl(id: input.InputId,
                                        text: input.Languages.GetLanguage(languagesIndex), isVisible: input.Visible, value: input.Value, boxType: boxType);

                                checkBoxSingle.PropertyChanged += (sender, e) =>
                                {
                                    var check = (CheckBox)sender;
                                    input.Value = check.IsChecked.ToString();
                                };
                                checkBoxSingle.IsChecked = input.Value?.StringToBool() ?? false;
                                contentRow.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Auto) });
                                contentRow.Add(checkBoxSingle, contentRow.Children.Count % rowTemplate.Column,
                                    contentRow.Children.Count / rowTemplate.Column);
                            }
                            else
                            {
                                var checkBox = ControlTemplate.CustomCheckBoxControl(id: input.InputId,
                                    text: input.Languages.GetLanguage(languagesIndex), isVisible: input.Visible, column: rowTemplate.Column, value: input.Value);
                                checkBox.PropertyChanged += (sender, e) =>
                                {
                                    var check = (CustomCheckBox)sender;
                                    input.Value = check.IsChecked.ToString();
                                };
                                contentRow.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Auto) });
                                contentRow.Add(checkBox, contentRow.Children.Count % rowTemplate.Column,
                                    contentRow.Children.Count / rowTemplate.Column);
                            }
                        }
                        break;

                        #endregion

                }
            }

            while (contentRow.Children.Count < rowTemplate.Column && numbersOfControlInRow != 1)
            {
                var checkBox = ControlTemplate.CustomCheckBoxControl(id: contentRow.Children.Count.ToString(),
                                   text: contentRow.Children.Count.ToString(), isVisible: false, column: rowTemplate.Column, value: false.ToString());

                contentRow.Add(checkBox, contentRow.Children.Count % rowTemplate.Column,
                                      contentRow.Children.Count / rowTemplate.Column);
            }

            return contentRow;
        }

        #endregion
    }
}
