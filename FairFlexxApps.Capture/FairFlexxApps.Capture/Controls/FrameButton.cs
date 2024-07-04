using System.Threading.Tasks;
using System.Windows.Input;


namespace FairFlexxApps.Capture.Controls
{
    public class FrameButton : Frame
    {
        #region Contructor

        public FrameButton()
        {
            Initialize();
        }

        #endregion

        #region Initalize

        public void Initialize()
        {
            GestureRecognizers.Add(new TapGestureRecognizer()
            {
                Command = AnimationCommand
            });
            //Content = new Label()
            //{
            //    VerticalOptions = LayoutOptions.Center,
            //    HorizontalOptions = LayoutOptions.Center,
            //    Text = this.Text,
            //    TextColor = this.TextColor,
            //    FontSize = this.FontSize
            //};
        }

        #endregion

        #region AnimationCommand

        private ICommand AnimationCommand
        {
            get
            {
                return new Command(async () =>
                {
                    //var opacity = this.Opacity;
                    this.Opacity = 0.4;

                    this.AnchorX = 0.48;
                    this.AnchorY = 0.48;

                    await this.ScaleTo(0.8, 50, Easing.Linear);

                    await Task.Delay(100);

                    await this.ScaleTo(1, 50, Easing.Linear);
                    this.Opacity = 1;

                    Command?.Execute(CommandParameter);

                });
            }
        }

        #endregion

        #region Command

        public static readonly BindableProperty CommandProperty = BindableProperty.Create(nameof(Command),
            typeof(ICommand), typeof(FrameButton), null, BindingMode.TwoWay);
        public ICommand Command
        {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }

        #endregion

        #region CommandParameter

        public static readonly BindableProperty CommandParameterProperty = BindableProperty.Create(nameof(CommandParameter), typeof(object), typeof(FrameButton));
        public object CommandParameter
        {
            get { return (object)GetValue(CommandParameterProperty); }
            set { SetValue(CommandParameterProperty, value); }
        }

        #endregion

        #region Text

        public static readonly BindableProperty TextProperty =
            BindableProperty.Create(nameof(Text), typeof(string), typeof(FrameButton), string.Empty);
        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        #endregion

        #region TextColor

        public static readonly BindableProperty TextColorProperty =
            BindableProperty.Create(nameof(TextColor), typeof(Color), typeof(FrameButton), Colors.Transparent);
        public Color TextColor
        {
            get { return (Color)GetValue(TextColorProperty); }
            set { SetValue(TextColorProperty, value); }
        }

        #endregion

        #region FontSize

        public static readonly BindableProperty FontSizeProperty =
            BindableProperty.Create(nameof(FontSize), typeof(double), typeof(FrameButton),(double)13);
        public double FontSize
        {
            get { return (double)GetValue(FontSizeProperty); }
            set { SetValue(FontSizeProperty, value); }
        }

        #endregion

        private ICommand TransitionCommand
        {
            get
            {
                return new Command(async () =>
                {
                    this.AnchorX = 0.48;
                    this.AnchorY = 0.48;
                    await this.ScaleTo(0.8, 50, Easing.Linear);
                    await Task.Delay(100);
                    await this.ScaleTo(1, 50, Easing.Linear);
                    if (Command != null)
                    {
                        Command.Execute(CommandParameter);
                    }
                });
            }
        }

    }
}
