using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Maui;

namespace FairFlexxApps.Capture.Controls.RadioButton
{
    /// <summary>
    /// Class BindableRadioGroup.
    /// </summary>
    public class BindableRadioGroup : StackLayout
    {
        ///-----------------------------------------------------------------------------
        /// <summary>
        /// Default constructor of RadioButtonGroupView
        /// </summary>
        public BindableRadioGroup()
        {
            this.Orientation = StackOrientation.Vertical;
            this.ChildAdded += RadioButtonGroupView_ChildAdded;
            this.ChildrenReordered += RadioButtonGroupView_ChildrenReordered;
        }

        public BindableRadioGroup(StackOrientation stackOrientation)
        {
            this.Margin = new Thickness(10, 0);
            this.Orientation = stackOrientation;
            this.ChildAdded += RadioButtonGroupView_ChildAdded;
            this.ChildrenReordered += RadioButtonGroupView_ChildrenReordered;
        }
        ///-----------------------------------------------------------------------------
        /// <summary>
        /// Invokes when tapped on RadioButon
        /// </summary>
        public event EventHandler SelectedItemChanged;
        /// <summary>
        /// Implementation of IValidatable, Triggered when value changed.
        /// </summary>
        public event EventHandler ValidationChanged;

        ///-----------------------------------------------------------------------------
        /// <summary>
        /// Executes when tapped on RadioButton
        /// </summary>
        public ICommand SelectedItemChangedCommand { get; set; }
        /// <summary>
        /// Command Parameter will be sent in SelectedItemChangedCommand
        /// </summary>
        public object CommandParameter { get; set; }
        private void RadioButtonGroupView_ChildrenReordered(object sender, EventArgs e)
        {
            UpdateAllEvent();
        }
        private void UpdateAllEvent()
        {
            foreach (var item in this.Children)
            {
                if (item is CustomRadioButton)
                {
                    (item as CustomRadioButton).Clicked -= UpdateSelected;
                    (item as CustomRadioButton).Clicked += UpdateSelected;
                }
            }
        }
        private void RadioButtonGroupView_ChildAdded(object sender, ElementEventArgs e)
        {
            if (e.Element is CustomRadioButton)
            {
                (e.Element as CustomRadioButton).Clicked -= UpdateSelected;
                (e.Element as CustomRadioButton).Clicked += UpdateSelected;
            }
        }
        void UpdateSelected(object selected, EventArgs e)
        {
            foreach (var item in this.Children)
            {
                if (item is CustomRadioButton)
                    (item as CustomRadioButton).IsChecked = item == selected;
            }

            SetValue(SelectedItemProperty, this.SelectedItem);
            OnPropertyChanged(nameof(SelectedItem));
            SetValue(SelectedIndexProperty, this.SelectedIndex);
            OnPropertyChanged(nameof(SelectedIndex));
            SelectedItemChanged?.Invoke(this, new EventArgs());
            if (SelectedItemChangedCommand?.CanExecute(CommandParameter ?? this) ?? false)
                SelectedItemChangedCommand?.Execute(CommandParameter ?? this);
            ValidationChanged?.Invoke(this, new EventArgs());
        }
        /// <summary>
        /// this will be added later
        /// </summary>
        public async void DisplayValidation()
        {
            this.BackgroundColor = Colors.Red;
            await Task.Delay(500);
            this.BackgroundColor = Colors.Transparent;
        }

        /// <summary>
        /// Returns selected radio button's index from inside of this.
        /// </summary>
        public int SelectedIndex
        {
            get
            {
                int index = 0;
                foreach (var item in this.Children)
                {
                    if (item is CustomRadioButton)
                    {
                        if ((item as CustomRadioButton).IsChecked)
                            return index;
                        index++;
                    }
                }
                return -1;
            }
            set
            {
                int index = 0;
                foreach (var item in this.Children)
                {
                    if (item is CustomRadioButton)
                    {
                        (item as CustomRadioButton).IsChecked = index == value;
                        index++;
                    }
                }
            }
        }
        ///-----------------------------------------------------------------------------
        /// <summary>
        /// Returns selected radio button's Value from inside of this.
        /// You can change the selectedItem too by sending a Value which matches ones of radio button's value
        /// </summary>
        public object SelectedItem
        {
            get
            {
                foreach (var item in this.Children)
                {
                    if (item is CustomRadioButton && (item as CustomRadioButton).IsChecked)
                        return (item as CustomRadioButton).Value;
                }
                return null;
            }
            set
            {
                foreach (var item in this.Children)
                {
                    if (item is CustomRadioButton)
                        (item as CustomRadioButton).IsChecked = (item as CustomRadioButton).Value == value;
                }
            }
        }
        ///-----------------------------------------------------------------------------
        /// <summary>
        /// It will be added later
        /// </summary>
        public bool IsRequired { get; set; }
        ///-----------------------------------------------------------------------------
        /// <summary>
        /// It will be added later
        /// </summary>
        public bool IsValidated { get => !this.IsRequired || this.SelectedIndex >= 0; }
        ///-----------------------------------------------------------------------------
        /// <summary>
        /// It will be added later
        /// </summary>
        public string ValidationMessage { get; set; }
        #region BindableProperties
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public static readonly BindableProperty SelectedItemProperty = BindableProperty.Create(nameof(SelectedItem), typeof(object), typeof(BindableRadioGroup), null, propertyChanged: (bo, ov, nv) => (bo as BindableRadioGroup).SelectedItem = nv);
        public static readonly BindableProperty SelectedIndexProperty = BindableProperty.Create(nameof(SelectedIndex), typeof(int), typeof(BindableRadioGroup), -1, BindingMode.TwoWay, propertyChanged: (bo, ov, nv) => (bo as BindableRadioGroup).SelectedIndex = (int)nv);
        public static readonly BindableProperty SelectedItemChangedCommandProperty = BindableProperty.Create(nameof(SelectedItemChangedCommand), typeof(ICommand), typeof(BindableRadioGroup), null, propertyChanged: (bo, ov, nv) => (bo as BindableRadioGroup).SelectedItemChangedCommand = (ICommand)nv);
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
        #endregion
    }

    #region Comment Code
    /*
    /// <summary>
    /// Class BindableRadioGroup.
    /// </summary>
    public class BindableRadioGroup : StackLayout
    {

        /// <summary>
        /// The items
        /// </summary>
        public ObservableCollection<CustomRadioButton> Items;

        /// <summary>
        /// Initializes a new instance of the <see cref="BindableRadioGroup"/> class.
        /// </summary>
        public BindableRadioGroup()
        {
            Items = new ObservableCollection<CustomRadioButton>();
        }

        /// <summary>
        /// The items source property
        /// </summary>
        public static BindableProperty ItemsSourceProperty =
                    BindableProperty.Create<BindableRadioGroup, IEnumerable>(o => o.ItemsSource, default(IEnumerable), propertyChanged: OnItemsSourceChanged);

        /// <summary>
        /// The selected index property
        /// </summary>
        public static BindableProperty SelectedIndexProperty =
            BindableProperty.Create<BindableRadioGroup, int>(o => o.SelectedIndex, -1, BindingMode.TwoWay,
                propertyChanged: OnSelectedIndexChanged);


        /// <summary>
        /// The text color property
        /// </summary>
        public static readonly BindableProperty TextColorProperty =
            BindableProperty.Create<CheckBox, Color>(
                p => p.TextColor, Color.Black);

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
        /// Gets or sets the items source.
        /// </summary>
        /// <value>The items source.</value>
        public IEnumerable ItemsSource
        {
            get { return (IEnumerable)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        /// <summary>
        /// Gets or sets the index of the selected.
        /// </summary>
        /// <value>The index of the selected.</value>
        public int SelectedIndex
        {
            get { return (int)GetValue(SelectedIndexProperty); }
            set { SetValue(SelectedIndexProperty, value); }
        }

        /// <summary>
        /// Gets or sets the color of the text.
        /// </summary>
        /// <value>The color of the text.</value>
        public Color TextColor
        {
            get { return (Color)GetValue(TextColorProperty); }
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

        /// <summary>
        /// Occurs when [checked changed].
        /// </summary>
        public event EventHandler<int> CheckedChanged;

        private void OnCheckedChanged(object sender, EventArgs<bool> e)
        {
            if (e.Value == false)
            {
                return;
            }

            var selectedItem = sender as CustomRadioButton;

            if (selectedItem == null)
            {
                return;
            }

            foreach (var item in Items)
            {
                if (!selectedItem.Id.Equals(item.Id))
                {
                    item.Checked = false;
                }
                else
                {
                    SelectedIndex = selectedItem.Id;
                    if (CheckedChanged != null)
                    {
                        CheckedChanged.Invoke(sender, item.Id);
                    }
                }
            }
        }

        private static void OnSelectedIndexChanged(BindableObject bindable, int oldvalue, int newvalue)
        {
            if (newvalue == -1)
            {
                return;
            }

            var bindableRadioGroup = bindable as BindableRadioGroup;

            if (bindableRadioGroup == null)
            {
                return;
            }

            foreach (var button in bindableRadioGroup.Items.Where(button => button.Id == bindableRadioGroup.SelectedIndex))
            {
                button.Checked = true;
            }
        }

        private static void OnItemsSourceChanged(BindableObject bindable, IEnumerable oldValue, IEnumerable newValue)
        {
            var radButtons = bindable as BindableRadioGroup;


            foreach (var item in radButtons.Items)
            {
                item.CheckedChanged -= radButtons.OnCheckedChanged;
            }

            radButtons.Children.Clear();

            var radIndex = 0;

            foreach (var item in radButtons.ItemsSource)
            {
                var button = new CustomRadioButton
                {
                    Text = item.ToString(),
                    Id = radIndex++,
                    TextColor = radButtons.TextColor,
                    FontSize = Device.GetNamedSize(NamedSize.Small, radButtons),
                    FontName = radButtons.FontName
                };

                button.CheckedChanged += radButtons.OnCheckedChanged;

                radButtons.Items.Add(button);

                radButtons.Children.Add(button);
            }
        }
    }
    */

    #endregion


}
