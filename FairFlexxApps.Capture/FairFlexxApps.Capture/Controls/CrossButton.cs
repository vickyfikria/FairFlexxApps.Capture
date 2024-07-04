using Microsoft.Maui;

namespace FairFlexxApps.Capture.Controls
{ 
    public enum ButtonType
    {
        None,
        DisabledButton,
    }

    public class CrossButton : Button
    {
        #region Upper

        public static readonly BindableProperty UpperProperty = BindableProperty.Create(nameof(Upper),
            typeof(bool), typeof(CrossButton), true);

        public bool Upper
        {
            get => (bool)GetValue(UpperProperty);
            set => SetValue(UpperProperty, value);
        }

        #endregion

        #region Type

        public static readonly BindableProperty TypeProperty = BindableProperty.Create(nameof(Type),
            typeof(ButtonType), typeof(CrossButton), ButtonType.None);

        public ButtonType Type
        {
            get => (ButtonType)GetValue(TypeProperty);
            set => SetValue(TypeProperty, value);
        }

        #endregion

        #region EnableColorProperty

        public static readonly BindableProperty EnableColorProperty =
            BindableProperty.Create(nameof(EnableColor), typeof(Color), typeof(CrossButton), Colors.Transparent);
        public Color EnableColor
        {
            get { return (Color)GetValue(EnableColorProperty); }
            set { SetValue(EnableColorProperty, value); }
        }

        #endregion

        #region TextEnableColorProperty

        public static readonly BindableProperty TextEnableColorProperty =
            BindableProperty.Create(nameof(TextEnableColor), typeof(Color), typeof(CrossButton), Colors.Transparent);
        public Color TextEnableColor
        {
            get { return (Color)GetValue(TextEnableColorProperty); }
            set { SetValue(TextEnableColorProperty, value); }
        }

        #endregion

        #region DisableColorProperty

        public static readonly BindableProperty DisableColorProperty =
            BindableProperty.Create(nameof(DisableColor), typeof(Color), typeof(CrossButton), Colors.Transparent);
        public Color DisableColor
        {
            get { return (Color)GetValue(DisableColorProperty); }
            set { SetValue(DisableColorProperty, value); }
        }

        #endregion

        #region TextDisableColorProperty

        public static readonly BindableProperty TextDisableColorProperty =
            BindableProperty.Create(nameof(TextDisableColor), typeof(Color), typeof(CrossButton), Colors.Transparent);
        public Color TextDisableColor
        {
            get { return (Color)GetValue(TextDisableColorProperty); }
            set { SetValue(TextDisableColorProperty, value); }
        }

        #endregion

        #region IconHeight

        public static readonly BindableProperty IconHeightProperty = BindableProperty.Create(nameof(IconHeight),
            typeof(int), typeof(CrossButton), 30);

        public int IconHeight
        {
            get => (int)GetValue(IconHeightProperty);
            set => SetValue(IconHeightProperty, value);
        }

        #endregion

        #region IconWidth

        public static readonly BindableProperty IconWidthProperty = BindableProperty.Create(nameof(IconWidth),
            typeof(int), typeof(CrossButton), 30);

        public int IconWidth
        {
            get => (int)GetValue(IconWidthProperty);
            set => SetValue(IconWidthProperty, value);
        }

        #endregion

        #region IconSpacing

        public static readonly BindableProperty IconSpacingProperty = BindableProperty.Create(nameof(IconSpacing),
            typeof(int), typeof(CrossButton), 2);

        public int IconSpacing
        {
            get => (int)GetValue(IconSpacingProperty);
            set => SetValue(IconSpacingProperty, value);
        }

        #endregion

        #region MaxLine

        public static readonly BindableProperty MaxLineProperty = BindableProperty.Create(nameof(MaxLine),
            typeof(int), typeof(CrossButton), 0);

        public int MaxLine
        {
            get => (int)GetValue(MaxLineProperty);
            set => SetValue(MaxLineProperty, value);
        }

        #endregion
    }
}