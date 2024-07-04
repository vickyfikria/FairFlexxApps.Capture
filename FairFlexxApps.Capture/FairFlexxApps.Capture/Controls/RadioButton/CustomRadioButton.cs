using System;
using System.Windows.Input;
using Microsoft.Maui;

namespace FairFlexxApps.Capture.Controls.RadioButton
{

    /// <summary>
    /// Class CustomRadioButton.
    /// </summary>
    public class CustomRadioButton : StackLayout
    {
        /// <summary>
        /// Default values of RadioButton
        /// </summary>
        public static GlobalSetting globalSetting = new GlobalSetting()
        {
            Color = Colors.Black,
            BorderColor = Colors.Black,
            TextColor = Colors.Black,
            Size = Device.GetNamedSize(Device.RuntimePlatform == Device.iOS ? NamedSize.Medium : NamedSize.Small, typeof(Label)),
            CornerRadius = -1,
            FontSize = Device.GetNamedSize(Device.RuntimePlatform == Device.iOS ? NamedSize.Medium : NamedSize.Small, typeof(Label)),
        };
        //.92
        //1.66 minReq
        Microsoft.Maui.Controls.Label lblEmpty = new Microsoft.Maui.Controls.Label() { TextColor = globalSetting.BorderColor, Text = "◯", VerticalTextAlignment = TextAlignment.Center, HorizontalTextAlignment = TextAlignment.Center, FontSize = globalSetting.Size, };
        Microsoft.Maui.Controls.Label lblFilled = new Microsoft.Maui.Controls.Label { TextColor = globalSetting.Color, Text = "●", IsVisible = false, Scale = 0.9, VerticalTextAlignment = TextAlignment.Center, HorizontalTextAlignment = TextAlignment.Center, FontSize = globalSetting.Size * .92 };
        Microsoft.Maui.Controls.Label lblText = new Microsoft.Maui.Controls.Label { Margin = new Thickness(0, 5, 0, 0), Text = "", VerticalTextAlignment = TextAlignment.Center, VerticalOptions = LayoutOptions.CenterAndExpand, TextColor = globalSetting.TextColor, FontSize = 18 };
        private bool _isDisabled;

        ///-----------------------------------------------------------------------------
        /// <summary>
        /// Default Constructor
        /// </summary>
        public CustomRadioButton()
        {
            Margin = new Thickness(10, 0, 0, 0);
            if (Device.RuntimePlatform != Device.iOS)
                lblText.FontSize = lblText.FontSize *= 1;
            lblEmpty.FontSize = lblText.FontSize * 1.3;
            lblFilled.FontSize = lblText.FontSize * 1.3;
            Orientation = StackOrientation.Horizontal;
            this.Children.Add(new Grid
            {
                VerticalOptions = LayoutOptions.CenterAndExpand,
                Children =
                {
                    lblEmpty,
                    lblFilled
                },
                MinimumWidthRequest = globalSetting.Size * 1.66,
            });
            this.Children.Add(lblText);
            this.GestureRecognizers.Add(new TapGestureRecognizer { Command = new Command(Tapped) });
        }
        ///-----------------------------------------------------------------------------
        /// <summary>
        /// Quick generating constructor.
        /// </summary>
        /// <param name="value">Value to keep in radio button</param>
        /// <param name="displayMember">If you send an ojbect as value. Which property will be displayed. Or override .ToString() inside of your object.</param>
        /// <param name="isChecked"> Checked or not situation</param>
        public CustomRadioButton(object value, string displayMember, bool isChecked = false) : this()
        {
            this.Value = value;
            this.IsChecked = isChecked;
            string text;
            if (!String.IsNullOrEmpty(displayMember))
                text = value.GetType().GetProperty(displayMember)?.GetValue(value).ToString();
            else
                text = value.ToString();
            lblText.Text = text ?? " ";
        }
        ///-----------------------------------------------------------------------------
        /// <summary>
        /// Quick generating constructor.
        /// </summary>
        /// <param name="text">Text to display right of Radio button </param>
        /// <param name="isChecked">IsSelected situation</param>
        public CustomRadioButton(string text, bool isChecked = false) : this()
        {
            Value = text;
            lblText.Text = text;
            this.IsChecked = isChecked;
        }
        ///-----------------------------------------------------------------------------
        /// <summary>
        /// Click event, triggered when clicked
        /// </summary>
        public event EventHandler Clicked;
        ///-----------------------------------------------------------------------------
        /// <summary>
        /// Click command, executed when clicked.  Parameter will be Value property if CommandParameter is not set
        /// </summary>
        public ICommand ClickCommand { get; set; }
        ///-----------------------------------------------------------------------------
        /// <summary>
        /// A command parameter will be sent to commands.
        /// </summary>
        public object CommandParameter { get; set; }
        ///-----------------------------------------------------------------------------
        /// <summary>
        /// Value to keep inside of Radio Button
        /// </summary>
        public object Value { get; set; }
        ///-----------------------------------------------------------------------------
        /// <summary>
        /// Gets or Sets, is that Radio Button selected/choosed/Checked
        /// </summary>
        public bool IsChecked { get => lblFilled.IsVisible; set { lblFilled.IsVisible = value; SetValue(IsCheckedProperty, value); } }
        ///-----------------------------------------------------------------------------
        /// <summary>
        /// this control if is Disabled
        /// </summary>
        public bool IsDisabled { get => _isDisabled; set { _isDisabled = value; this.Opacity = value ? 0.6 : 1; } }
        ///-----------------------------------------------------------------------------
        /// <summary>
        /// Text Description of Radio Button. It will be displayed right of Radio Button
        /// </summary>
        public string Text { get => lblText.Text; set => lblText.Text = value; }
        /// <summary>
        /// Fontsize of Description Text
        /// </summary>
        public double TextFontSize { get => lblText.FontSize; set { lblText.FontSize = value; } }
        ///-----------------------------------------------------------------------------
        /// <summary>
        /// Size of Radio Button
        /// </summary>
        public double CircleSize { get => lblEmpty.FontSize; set => SetCircleSize(value); }
        ///-----------------------------------------------------------------------------
        /// <summary>
        /// To be added.
        /// </summary>
        public string FontFamily { get => lblText.FontFamily; set => lblText.FontFamily = value; }
        ///-----------------------------------------------------------------------------
        /// <summary>
        /// Color of Radio Button's checked.
        /// </summary>
        public Color Color { get => lblFilled.TextColor; set => lblFilled.TextColor = value; }
        ///-----------------------------------------------------------------------------
        /// <summary>
        /// Color of radio button's outline border 
        /// </summary>
        public Color CircleColor { get => lblEmpty.TextColor; set => lblEmpty.TextColor = value; }
        ///-----------------------------------------------------------------------------
        /// <summary>
        /// Color of description text of Radio Button
        /// </summary>
        public Color TextColor { get => lblText.TextColor; set => lblText.TextColor = value; }
        #region BindableProperties
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public static readonly BindableProperty IsCheckedProperty = BindableProperty.Create(nameof(IsChecked), typeof(bool), typeof(CustomRadioButton), false, propertyChanged: (bo, ov, nv) => (bo as CustomRadioButton).IsChecked = (bool)nv);
        public static readonly BindableProperty TextProperty = BindableProperty.Create(nameof(Text), typeof(string), typeof(CustomRadioButton), "", propertyChanged: (bo, ov, nv) => (bo as CustomRadioButton).Text = (string)nv);
        public static readonly BindableProperty TextFontSizeProperty = BindableProperty.Create(nameof(TextFontSize), typeof(double), typeof(CustomRadioButton), 20.0, propertyChanged: (bo, ov, nv) => (bo as CustomRadioButton).TextFontSize = (double)nv);
        public static readonly BindableProperty ColorProperty = BindableProperty.Create(nameof(Color), typeof(Color), typeof(CustomRadioButton), Colors.Transparent, propertyChanged: (bo, ov, nv) => (bo as CustomRadioButton).Color = (Color)nv);
        public static readonly BindableProperty CircleColorProperty = BindableProperty.Create(nameof(CircleColor), typeof(Color), typeof(CustomRadioButton), Colors.Transparent, propertyChanged: (bo, ov, nv) => (bo as CustomRadioButton).CircleColor = (Color)nv);
        public static readonly BindableProperty TextColorProperty = BindableProperty.Create(nameof(TextColor), typeof(Color), typeof(CustomRadioButton), Colors.Transparent, propertyChanged: (bo, ov, nv) => (bo as CustomRadioButton).TextColor = (Color)nv);
        public static readonly BindableProperty ClickCommandProperty = BindableProperty.Create(nameof(ClickCommand), typeof(ICommand), typeof(CustomRadioButton), null, propertyChanged: (bo, ov, nv) => (bo as CustomRadioButton).ClickCommand = (ICommand)nv);
        public static readonly BindableProperty CommandParameterProperty = BindableProperty.Create(nameof(CommandParameter), typeof(object), typeof(CustomRadioButton), propertyChanged: (bo, ov, nv) => (bo as CustomRadioButton).CommandParameter = nv);
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
        #endregion

        ///-----------------------------------------------------------------------------
        /// <summary>
        /// That handles tapps and triggers event, commands etc.
        /// </summary>
        void Tapped()
        {
            if (IsDisabled) return;
            IsChecked = !IsChecked;
            Clicked?.Invoke(this, new EventArgs());
            ClickCommand?.Execute(CommandParameter ?? Value);
        }
        ///-----------------------------------------------------------------------------
        /// <summary>
        /// Sets size of Circle
        /// </summary>
        void SetCircleSize(double value)
        {
            lblEmpty.FontSize = value;
            lblFilled.FontSize = value * .92;
            if (this.Children.Count > 0)
                this.Children[0].MinimumWidthRequest = value * 1.66;
        }
    }

    #region Comment Code

    /*
    /// <summary>
    /// Class CustomRadioButton.
    /// </summary>
    public class CustomRadioButton : View
    {
        /// <summary>
        /// The checked property
        /// </summary>
        public static readonly BindableProperty CheckedProperty =
            BindableProperty.Create<CustomRadioButton, bool>(
                p => p.Checked, false);

        /// <summary>
        ///     The default text property.
        /// </summary>
        public static readonly BindableProperty TextProperty =
            BindableProperty.Create<CustomRadioButton, string>(
                p => p.Text, string.Empty);

        /// <summary>
        ///     The default text property.
        /// </summary>
        public static readonly BindableProperty TextColorProperty =
            BindableProperty.Create<CustomRadioButton, Color>(
                p => p.TextColor, Color.Default);

        /// <summary>
        /// The font size property
        /// </summary>
        public static readonly BindableProperty FontSizeProperty =
            BindableProperty.Create<CheckBox, double>(
                p => p.FontSize, -1);

        /// <summary>
        /// The font name property.
        /// </summary>
        public static readonly BindableProperty FontNameProperty =
            BindableProperty.Create<CheckBox, string>(
                p => p.FontName, string.Empty);

        /// <summary>
        ///     The checked changed event.
        /// </summary>
        public EventHandler<EventArgs<bool>> CheckedChanged;

        /// <summary>
        ///     Gets or sets a value indicating whether the control is checked.
        /// </summary>
        /// <value>The checked state.</value>
        public bool Checked
        {
            get { return this.GetValue<bool>(CheckedProperty); }

            set
            {
                SetValue(CheckedProperty, value);

                var eventHandler = CheckedChanged;

                if (eventHandler != null)
                {
                    eventHandler.Invoke(this, value);
                }
            }
        }

        /// <summary>
        /// Gets or sets the text.
        /// </summary>
        /// <value>The text.</value>
        public string Text
        {
            get { return this.GetValue<string>(TextProperty); }

            set { SetValue(TextProperty, value); }
        }

        /// <summary>
        /// Gets or sets the color of the text.
        /// </summary>
        /// <value>The color of the text.</value>
        public Color TextColor
        {
            get { return this.GetValue<Color>(TextColorProperty); }

            set { SetValue(TextColorProperty, value); }
        }

        /// <summary>
        /// Gets or sets the size of the font.
        /// </summary>
        /// <value>The size of the font.</value>
        public double FontSize
        {
            get
            {
                return (double)GetValue(FontSizeProperty);
            }
            set
            {
                SetValue(FontSizeProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the name of the font.
        /// </summary>
        /// <value>The name of the font.</value>
        public string FontName
        {
            get
            {
                return (string)GetValue(FontNameProperty);
            }
            set
            {
                SetValue(FontNameProperty, value);
            }
        }

#pragma warning disable CS0108 // Member hides inherited member; missing new keyword
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>The identifier.</value>
        public int Id { get; set; }
#pragma warning restore CS0108 // Member hides inherited member; missing new keyword
    }
    */

    #endregion

}