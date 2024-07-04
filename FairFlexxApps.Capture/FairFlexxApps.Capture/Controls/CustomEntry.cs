using Microsoft.Maui;

namespace FairFlexxApps.Capture.Controls
{
    public class CustomEntry : Entry
    {
        public CustomEntry()
        {
        }

        #region IsMandatory
        public bool IsMandatory
        {
            get => (bool)GetValue(IsMandatoryProperty);
            set => SetValue(IsMandatoryProperty, value);
        }

        public static BindableProperty IsMandatoryProperty = BindableProperty.Create(nameof(IsMandatory), typeof(bool),
            typeof(CustomEntry), false, BindingMode.TwoWay);
        #endregion

        #region Image
        public static readonly BindableProperty ImageProperty =
                BindableProperty.Create(nameof(Image), typeof(string), typeof(CustomEntry), string.Empty);
        public string Image
        {
            get { return (string)GetValue(ImageProperty); }
            set { SetValue(ImageProperty, value); }
        }
        #endregion

        #region LineColor

        public static readonly BindableProperty LineColorProperty =
            BindableProperty.Create(nameof(LineColor), typeof(Microsoft.Maui.Graphics.Color), typeof(CustomEntry), Colors.Transparent);
        public Color LineColor
        {
            get { return (Color)GetValue(LineColorProperty); }
            set { SetValue(LineColorProperty, value); }
        }

        #endregion

        #region ImageHeight

        public static readonly BindableProperty ImageHeightProperty =
           BindableProperty.Create(nameof(ImageHeight), typeof(int), typeof(CustomEntry), 25);
        public int ImageHeight
        {
            get { return (int)GetValue(ImageHeightProperty); }
            set { SetValue(ImageHeightProperty, value); }
        }
        #endregion

        #region ImageWidth
        public static readonly BindableProperty ImageWidthProperty =
            BindableProperty.Create(nameof(ImageWidth), typeof(int), typeof(CustomEntry), 25);
        public int ImageWidth
        {
            get { return (int)GetValue(ImageWidthProperty); }
            set { SetValue(ImageWidthProperty, value); }
        }
        #endregion

        #region ImageAlignment
        public static readonly BindableProperty ImageAlignmentProperty =
            BindableProperty.Create(nameof(ImageAlignment), typeof(ImageAlignment), typeof(CustomEntry), ImageAlignment.Right);
        public ImageAlignment ImageAlignment
        {
            get { return (ImageAlignment)GetValue(ImageAlignmentProperty); }
            set { SetValue(ImageAlignmentProperty, value); }
        }

        #endregion

        #region HasRoundedCorner

        public static BindableProperty HasRoundedCornerProperty =
            BindableProperty.Create(nameof(HasRoundedCorner), typeof(bool), typeof(CustomEntry), false);
        public bool HasRoundedCorner
        {
            get { return (bool)GetValue(HasRoundedCornerProperty); }
            set { SetValue(HasRoundedCornerProperty, value); }
        }

        #endregion

        #region IsBorderlessUnderline

        public static readonly BindableProperty IsBorderlessUnderlineProperty = BindableProperty.Create(nameof(IsBorderlessUnderline),
            typeof(bool), typeof(CustomEntry), false, BindingMode.TwoWay);

        public bool IsBorderlessUnderline
        {
            get => (bool)GetValue(IsBorderlessUnderlineProperty);
            set => SetValue(IsBorderlessUnderlineProperty, value);
        }

        #endregion

        #region ConnerBorder

        public static readonly BindableProperty ConnerBorderProperty =
           BindableProperty.Create(nameof(ConnerBorder), typeof(int), typeof(CustomEntry), 0);
        public int ConnerBorder
        {
            get { return (int)GetValue(ConnerBorderProperty); }
            set { SetValue(ConnerBorderProperty, value); }
        }

        #endregion

        #region StrokeColor

        public static readonly BindableProperty StrokeColorProperty =
           BindableProperty.Create(nameof(StrokeColor), typeof(Color), typeof(CustomEntry), Colors.Black);
        public Color StrokeColor
        {
            get { return (Color)GetValue(StrokeColorProperty); }
            set { SetValue(ConnerBorderProperty, value); }
        }

        #endregion

        #region BorderWidth

        public static readonly BindableProperty BorderWidthProperty =
            BindableProperty.Create(nameof(BorderWidth), typeof(int), typeof(CustomEntry), 1);
        public int BorderWidth
        {
            get { return (int)GetValue(BorderWidthProperty); }
            set { SetValue(ConnerBorderProperty, value); }
        }

        #endregion
    }

    public enum ImageAlignment
    {
        Left,
        Right
    }

}
