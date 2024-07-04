using Android.Content;
using Android.Content.Res;
using Android.Support.V7.Widget;
using Android.Widget;
using FairFlexxApps.Capture.Controls;
using FairFlexxApps.Capture.Droid.Controls;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(CustomCheckBox), typeof(CustomCheckBoxRenderer))]
namespace FairFlexxApps.Capture.Droid.Controls
{
    /// <summary>
    /// Xamarin.Forms custom renderer for the Checkbox control
    /// </summary>
    public class CustomCheckBoxRenderer : ViewRenderer<CustomCheckBox, AppCompatCheckBox>, CompoundButton.IOnCheckedChangeListener
    {
        private const int DEFAULT_SIZE = 32;
        private ColorStateList defaultTextColor;

        public CustomCheckBoxRenderer(Context context) : base(context)
        {
        }

        #region Init

        /// <summary>
        /// Used for registration with dependency service to ensure it isn't linked out
        /// </summary>
        public static void Init()
        {
            // intentionally empty
        }

        #endregion

        #region OnElementChanged

        /// <summary>
        /// Called when the control is created or changed
        /// </summary>
        /// <param name="e">E.</param>
        protected override void OnElementChanged(ElementChangedEventArgs<CustomCheckBox> e)
        {
            base.OnElementChanged(e);
            if (e.NewElement != null)
            {
                if (Control == null)
                {
                    var checkBox = new AppCompatCheckBox(Context);

                    if (Element.OutlineColor != default(Color))
                    {
                        var backgroundColor = GetBackgroundColorStateList(Element.OutlineColor);
                        //checkBox.BackgroundTintList = GetBackgroundColorStateList(Element.InnerColor);
                        //checkBox.ForegroundTintList = GetBackgroundColorStateList(Element.OutlineColor);
                        checkBox.SupportButtonTintList = backgroundColor;
                    }
                    checkBox.TextAlignment = Android.Views.TextAlignment.TextEnd;
                    defaultTextColor = checkBox.TextColors;
                    checkBox.SetOnCheckedChangeListener(this);
                    SetNativeControl(checkBox);
                }
                if (Control.Checked)
                {
                    Control.Text = e.NewElement.CheckedText;
                }
                else
                {
                    Control.Text = e.NewElement.UncheckedText;
                }
                Control.TextSize = e.NewElement.FontSize;
                Control.Text = e.NewElement.Text;
                
                Control.Checked = e.NewElement.IsChecked;
                UpdateTextColor();
            }
        }

        #endregion

        #region OnElementPropertyChanged

        protected override void OnElementPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);
            switch (e.PropertyName)
            {
                case nameof(Element.OutlineColor):
                    var backgroundColor = GetBackgroundColorStateList(Element.CheckColor);
                    Control.SupportButtonTintList = backgroundColor;
                    Control.BackgroundTintList = GetBackgroundColorStateList(Element.InnerColor);
                    Control.ForegroundTintList = GetBackgroundColorStateList(Element.OutlineColor);
                    break;
                case nameof(Element.IsChecked):
                    Control.Text = Element.Text;
                    Control.Checked = Element.IsChecked;
                    
                    break;
                case nameof(Element.TextColor):
                    UpdateTextColor();
                    break;
                case nameof(Element.FontSize):
                    Control.TextSize = (float)Element.FontSize;
                    break;
                case nameof(Element.CheckedText):
                    Control.Text = Element.CheckedText;
                    break;
                case nameof(Element.UncheckedText):
                    Control.Text = Element.UncheckedText;
                    break;
                default:
                    System.Diagnostics.Debug.WriteLine("Property change for {0} has not been implemented.", e.PropertyName);
                    break;
            }
        }

        #endregion

        #region OnCheckedChanged

        /// <summary>
        /// Update element bindable property from event
        /// </summary>
        /// <param name="buttonView">Button view.</param>
        /// <param name="isChecked">If set to <c>true</c> is checked.</param>
        public void OnCheckedChanged(CompoundButton buttonView, bool isChecked)
        {
            buttonView.SetHeight((int)50);
            buttonView.SetWidth((int)50);
            ((IViewController)Element).SetValueFromRenderer(CustomCheckBox.IsCheckedProperty, isChecked);
            Element.CheckedCommand?.Execute(Element.CheckedCommandParameter);

        }

        #endregion

        #region GetDesiredSize

        public override SizeRequest GetDesiredSize(int widthConstraint, int heightConstraint)
        {
            var sizeConstraint = new SizeRequest(new Size(64, 64),
                new Size(64, 64));
            if (sizeConstraint.Request.Width == 0)
            {
                var width = widthConstraint;
                if (widthConstraint <= 0)
                {
                    System.Diagnostics.Debug.WriteLine("Default values");
                    width = DEFAULT_SIZE;
                }
                else if (widthConstraint <= 0)
                {
                    width = DEFAULT_SIZE;
                }

            }

            return sizeConstraint;
        }

        #endregion

        #region GetBackgroundColorStateList

        private ColorStateList GetBackgroundColorStateList(Color color)
        {
            return new ColorStateList(
                new[]
                {
                    new[] {-global::Android.Resource.Attribute.StateEnabled}, // checked
                    new[] {-global::Android.Resource.Attribute.StateChecked}, // unchecked
                    new[] {global::Android.Resource.Attribute.StateChecked} // checked
                },
                new int[]
                {
                    color.WithSaturation(0.1).ToAndroid(),
                    color.ToAndroid(),
                    color.ToAndroid()
                });
        }

        #endregion

        #region UpdateTextColor

        private void UpdateTextColor()
        {
            if (Control == null || Element == null)
                return;

            if (Element.TextColor == Xamarin.Forms.Color.Default)
                Control.SetTextColor(defaultTextColor);
            else
                Control.SetTextColor(Element.TextColor.ToAndroid());
        }

        #endregion

        #region CheckBoxCheckedChange

        /// <summary>
        /// Sync from native control
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="e">E.</param>
        private void CheckBoxCheckedChange(object sender, CompoundButton.CheckedChangeEventArgs e)
        {
            Element.IsChecked = e.IsChecked;
        }

        #endregion
        
    }
}