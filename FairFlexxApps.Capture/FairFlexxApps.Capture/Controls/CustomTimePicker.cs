using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Maui;

namespace FairFlexxApps.Capture.Controls
{
    public class CustomTimePicker : TimePicker
    {
        private string _format = null;
        public static readonly BindableProperty NullableTimeProperty =
        BindableProperty.Create<CustomTimePicker, TimeSpan?>(p => p.NullableTime, null);

        public TimeSpan? NullableTime
        {
            get { return (TimeSpan?)GetValue(NullableTimeProperty); }
            set { SetValue(NullableTimeProperty, value); UpdateTime(); }
        }

        private void UpdateTime()
        {
            if (NullableTime.HasValue) { if (null != _format) Format = _format; Time = NullableTime.Value; }
            else { _format = Format; Format = ""; }
        }
        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();
            UpdateTime();
        }

        protected override void OnPropertyChanged(string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);
            if (propertyName == "Time") NullableTime = Time;
        }
    }
}
