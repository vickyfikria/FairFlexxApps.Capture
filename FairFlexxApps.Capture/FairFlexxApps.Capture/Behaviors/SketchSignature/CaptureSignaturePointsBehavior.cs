using System.Collections.Generic;
using Microsoft.Maui;

using SignaturePad.Forms;
using System.Collections.ObjectModel;

namespace FairFlexxApps.Capture.Behaviors.SketchSignature
{
	public class CaptureSignaturePointsBehavior : CaptureSignatureBehaviorBase
	{
		public static readonly BindableProperty PointsProperty = BindableProperty.Create (
			nameof (Points),
			typeof (IEnumerable<Point>),
			typeof (CaptureSignaturePointsBehavior),
			default (IEnumerable<Point>),
			BindingMode.TwoWay,
			propertyChanged: CreatePropertyChanged (PointsProperty));

		public IEnumerable<Point> Points
		{
			get => (IEnumerable<Point>)GetValue (PointsProperty);
			set => SetValue (PointsProperty, value);
		}

		protected override void UpdateSignaturePad (SignaturePadView bindable, BindableProperty property, object oldValue, object newValue)
		{
            if (newValue == null)
            {
                var sketch = new ObservableCollection<Point>();
                bindable.Points = sketch as IEnumerable<Point>;
            }
			else
                bindable.Points = newValue as IEnumerable<Point>;
		}

		protected override void UpdateBehavior (SignaturePadView signaturePad)
		{
			Points = signaturePad.Points;
		}
	}
}
