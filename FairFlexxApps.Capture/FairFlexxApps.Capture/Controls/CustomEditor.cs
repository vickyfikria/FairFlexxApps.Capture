using Microsoft.Maui;

namespace FairFlexxApps.Capture.Controls
{
    public class CustomEditor : Editor
    {

        #region IsMandatory

        public bool IsMandatory
        {
            get => (bool)GetValue(IsMandatoryProperty);
            set => SetValue(IsMandatoryProperty, value);
        }

        public static BindableProperty IsMandatoryProperty = BindableProperty.Create(nameof(IsMandatory), typeof(bool),
            typeof(CustomEditor), false, BindingMode.TwoWay);

        #endregion

        #region IsBorderlessUnderline

        public static readonly BindableProperty IsBorderlessUnderlineProperty = BindableProperty.Create(nameof(IsBorderlessUnderline),
            typeof(bool), typeof(CustomEditor), false, BindingMode.TwoWay);

        public bool IsBorderlessUnderline
        {
            get => (bool)GetValue(IsBorderlessUnderlineProperty);
            set => SetValue(IsBorderlessUnderlineProperty, value);
        }

        #endregion

    }
}
