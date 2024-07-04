using System;
using Microsoft.Maui;
//using Xamarin.Forms.Internals;

namespace FotoScan.Tablet.Controls
{
    /*public class CustomRadioButton : View
    {
        //public static readonly BindableProperty CheckedProperty =
        //    BindableProperty.Create<CustomRadioButton, bool>(p => p.Checked, false);

        public static readonly BindableProperty CheckProperty =
            BindableProperty.Create(nameof(Checked),
                typeof(bool),
                typeof(CustomRadioButton),
                false);

        public bool Checked
        {
            get { return (bool)GetValue(CheckProperty); }
            set
            {
                SetValue(CheckProperty, value);
                var eventHandler = CheckedChanged;
                if (eventHandler != null)
                {
                    eventHandler.Invoke(this, new EventArg<bool>(value));
                }
            }
        }

        public static readonly BindableProperty TextProperty =
            BindableProperty.Create(nameof(Text),
                typeof(string),
                typeof(CustomRadioButton),
                string.Empty);

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        public static readonly BindableProperty TextColorProperty =
            BindableProperty.Create(nameof(TextColor),
                typeof(Color),
                typeof(CustomRadioButton),
                Color.Black);

        public Color TextColor
        {
            get { return (Color)GetValue(TextColorProperty); }
            set { SetValue(TextColorProperty, value); }
        }

        public EventHandler<EventArg<bool>> CheckedChanged;

        public int Id { get; set; }
    }*/


    public class CustomRadioButton : View
    {
        #region Checked

        /// <summary>
        ///     Gets or sets a value indicating whether the control is checked.
        /// </summary>
        /// <value>The checked state.</value>
        public bool Checked
        {
            get => (bool)this.GetValue(CheckedProperty);
            set => SetValue(CheckedProperty, value);
        }

        /// <summary>
        /// The checked property
        /// </summary>
        public static readonly BindableProperty CheckedProperty =
            BindableProperty.Create(nameof(Checked), typeof(bool), typeof(CustomRadioButton), defaultValue: false,
                defaultBindingMode: BindingMode.OneWay, validateValue: null, propertyChanged: OnCheckChanged);

        /// <summary>
        ///     The checked changed event.
        /// </summary>
        public event EventHandler<bool> CheckedChanged;
        private static void OnCheckChanged(BindableObject bindable, object oldvalue, object newvalue)
        {
            var control = bindable as CustomRadioButton;
            var eventHandler = control?.CheckedChanged;
            eventHandler?.Invoke(control, (bool)newvalue);
        }

        #endregion

        #region Text

        /// <summary>
        /// Gets or sets the text.
        /// </summary>
        /// <value>The text.</value>
        public string Text
        {
            get => (string)this.GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        /// <summary>
        ///     The default text property.
        /// </summary>
        public static readonly BindableProperty TextProperty =
            BindableProperty.Create(nameof(Text), typeof(string), typeof(CustomRadioButton),
                defaultValue: string.Empty);

        #endregion

        #region TextColor

        /// <summary>
        /// Gets or sets the color of the text.
        /// </summary>
        /// <value>The color of the text.</value>
        public Color TextColor
        {
            get => (Color)this.GetValue(TextColorProperty);

            set => SetValue(TextColorProperty, value);
        }

        /// <summary>
        ///     The default text property.
        /// </summary>
        public static readonly BindableProperty TextColorProperty =
            BindableProperty.Create(nameof(TextColor), typeof(Color), typeof(CustomRadioButton),
                defaultValue: Color.FromRgb(109, 119, 160));

        #endregion

        #region FontSize

        /// <summary>
        /// Gets or sets the size of the font.
        /// </summary>
        /// <value>The size of the font.</value>
        public double FontSize
        {
            get => (double)GetValue(FontSizeProperty);
            set => SetValue(FontSizeProperty, value);
        }

        /// <summary>
        /// The font size property
        /// </summary>
        public static readonly BindableProperty FontSizeProperty =
            BindableProperty.Create(nameof(FontSize), typeof(double), typeof(CustomRadioButton), 16.0f);

        #endregion

        #region FontName

        /// <summary>
        /// Gets or sets the name of the font.
        /// </summary>
        /// <value>The name of the font.</value>
        public string FontName
        {
            get => (string)GetValue(FontNameProperty);
            set => SetValue(FontNameProperty, value);
        }

        /// <summary>
        /// The font name property.
        /// </summary>
        public static readonly BindableProperty FontNameProperty =
            BindableProperty.Create(nameof(FontName), typeof(string), typeof(CustomRadioButton),
                defaultValue: string.Empty);

        #endregion

        #region CheckID

        public int CheckId
        {
            get => (int)this.GetValue(CheckIdProperty);
            set => this.SetValue(CheckIdProperty, value);
        }

        public static readonly BindableProperty CheckIdProperty =
            BindableProperty.Create(nameof(CheckId), typeof(int), typeof(CustomRadioButton), defaultValue: 0);

        #endregion

    }

    /// <inheritdoc cref="StackLayout" />
    /// <summary>
    /// Class BindableRadioGroup.
    /// </summary>
    public class BindableRadioGroup : StackLayout, IDisposable
    {
        protected override void OnChildAdded(Element child)
        {
            base.OnChildAdded(child);

            if (!(child is CustomRadioButton radio)) return;
            radio.CheckedChanged += RadioCheckedChanged;
        }

        private void RadioCheckedChanged(object sender, bool e)
        {
            if (!(sender is CustomRadioButton radio)) return;
            if (radio.CheckId == SelectedIndex) return;
            SelectedIndex = radio.CheckId;
            radio.Checked = true;

            //change UI others radio button
            foreach (var view in Children)
            {
                if (!(view is CustomRadioButton ctl)) return;
                if (ctl.CheckId == radio.CheckId) return;
                ctl.Checked = false;
            }
        }

        #region BindableRadioGroup

        /// <summary>
        /// Initializes a new instance of the <see cref="BindableRadioGroup"/> class.
        /// </summary>
        public BindableRadioGroup()
        {
            Spacing = 10;
            SelectedIndex = 0;
        }

        #endregion

        #region SelectedIndex

        /// <summary>
        /// The selected index property
        /// </summary>
        public static BindableProperty SelectedIndexProperty =
            BindableProperty.Create(nameof(SelectedIndex), typeof(int), typeof(BindableRadioGroup), defaultValue: -1);

        /// <summary>
        /// Gets or sets the index of the selected.
        /// </summary>
        /// <value>The index of the selected.</value>
        public int SelectedIndex
        {
            get => (int)GetValue(SelectedIndexProperty);
            set => SetValue(SelectedIndexProperty, value);
        }

        #endregion



        public void Dispose()
        {
            foreach (var view in Children)
            {
                if (!(view is CustomRadioButton ctl)) return;
                ctl.CheckedChanged -= RadioCheckedChanged;
            }
        }
    }
}
