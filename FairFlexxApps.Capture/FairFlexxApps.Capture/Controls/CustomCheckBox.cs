using System;
using System.Windows.Input;
using Microsoft.Maui;

namespace FairFlexxApps.Capture.Controls
{
    public class CustomCheckBox : View
    {
        public event EventHandler OnCheckChanged;

        #region OutlineColor

        public Color OutlineColor
        {
            get => (Color) GetValue(OutlineColorProperty);
            set => SetValue(OutlineColorProperty, value);
        }

        public static BindableProperty OutlineColorProperty =
            BindableProperty.Create(nameof(OutlineColor), typeof(Color), typeof(CustomCheckBox), Colors.Blue);

        #endregion

        #region InnerColor

        public Color InnerColor
        {
            get => (Color)GetValue(InnerColorProperty);
            set => SetValue(InnerColorProperty, value);
        }

        public static BindableProperty InnerColorProperty =
            BindableProperty.Create(nameof(InnerColor), typeof(Color), typeof(CustomCheckBox), Colors.Black);

        #endregion

        #region CheckColor

        public Color CheckColor
        {
            get => (Color)GetValue(CheckColorProperty);
            set => SetValue(CheckColorProperty, value);
        }

        public static BindableProperty CheckColorProperty =
            BindableProperty.Create(nameof(CheckColor), typeof(Color), typeof(CustomCheckBox), Colors.White);

        #endregion

        #region CheckedOutlineColor

        public Color CheckedOutlineColor
        {
            get => (Color)GetValue(CheckedOutlineColorProperty);
            set => SetValue(CheckedOutlineColorProperty, value);
        }

        public static BindableProperty CheckedOutlineColorProperty =
            BindableProperty.Create(nameof(CheckedOutlineColor), typeof(Color), typeof(CustomCheckBox), Colors.Black);

        #endregion

        #region CheckedInnerColor

        public Color CheckedInnerColor
        {
            get => (Color)GetValue(CheckedInnerColorProperty);
            set => SetValue(CheckedInnerColorProperty, value);
        }

        public static BindableProperty CheckedInnerColorProperty = BindableProperty.Create(nameof(CheckedInnerColor),
            typeof(Color), typeof(CustomCheckBox), Colors.White);

        #endregion

        #region IsChecked

        public bool IsChecked
        {
            get => (bool)GetValue(IsCheckedProperty);
            set => SetValue(IsCheckedProperty, value);
        }

        public static BindableProperty IsCheckedProperty = BindableProperty.Create(nameof(IsChecked), typeof(bool),
            typeof(CustomCheckBox), false, BindingMode.TwoWay);

        #endregion

        #region Text

        public string Text => this.IsChecked
            ? (string.IsNullOrEmpty(this.CheckedText) ? this.DefaultText : this.CheckedText)
            : (string.IsNullOrEmpty(this.UncheckedText) ? this.DefaultText : this.UncheckedText);

        
        #region DefaultText

        public string DefaultText
        {
            get => (string)GetValue(DefaultTextProperty);
            set => SetValue(DefaultTextProperty, value);
        }
        
        public static readonly BindableProperty DefaultTextProperty =
            BindableProperty.Create(nameof(Text), typeof(string), typeof(CustomCheckBox), string.Empty);

        #endregion

        #region CheckedText

        public string CheckedText
        {
            get => (string)GetValue(CheckedTextProperty);
            set => SetValue(CheckedTextProperty, value);
        }

        public static readonly BindableProperty CheckedTextProperty =
            BindableProperty.Create(nameof(CheckedText), typeof(string), typeof(CustomCheckBox), string.Empty);
        
        #endregion

        #region UncheckedText

        public string UncheckedText
        {
            get => (string)GetValue(UncheckedTextProperty);
            set => SetValue(UncheckedTextProperty, value);
        }
        
        public static readonly BindableProperty UncheckedTextProperty =
            BindableProperty.Create(nameof(UncheckedText), typeof(string), typeof(CustomCheckBox), string.Empty);

        #endregion

        #endregion

        #region TextColor

        public Color TextColor
        {
            get => (Color)GetValue(TextColorProperty);
            set => SetValue(TextColorProperty, value);
        }

        public static readonly BindableProperty TextColorProperty =
            BindableProperty.Create(nameof(TextColor), typeof(Color), typeof(CustomCheckBox), Colors.Black);

        #endregion

        #region FontSize

        public float FontSize
        {
            get => (float)GetValue(FontSizeProperty);
            set => SetValue(FontSizeProperty, value);
        }

        public static BindableProperty FontSizeProperty =
            BindableProperty.Create(nameof(FontSize), typeof(float), typeof(CustomCheckBox), (float) 18);

        #endregion

        #region CheckedCommand

        public ICommand CheckedCommand
        {
            get => (ICommand)GetValue(CheckedCommandProperty);
            set => SetValue(CheckedCommandProperty, value);
        }

        public static BindableProperty CheckedCommandProperty =
            BindableProperty.Create(nameof(CheckedCommand), typeof(ICommand), typeof(CustomCheckBox), null);

        #endregion

        #region CheckedCommandParameter

        public object CheckedCommandParameter
        {
            get => GetValue(CheckedCommandParameterProperty);
            set => SetValue(CheckedCommandParameterProperty, value);
        }

        public static BindableProperty CheckedCommandParameterProperty =
            BindableProperty.Create(nameof(CheckedCommandParameter), typeof(object), typeof(CustomCheckBox), null);

        #endregion

        public void FireCheckChange()
        {
            OnCheckChanged?.Invoke(this, new CheckChangedArgs
            {
                IsChecked = IsChecked
            });
        }

        public class CheckChangedArgs : EventArgs
        {
            public bool IsChecked { get; set; }
        }
    }
}