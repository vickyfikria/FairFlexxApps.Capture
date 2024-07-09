using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace FairFlexxApps.Capture.Views.ViewCells.TopMenuViews
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class NotesContentView : ContentView
	{
		public NotesContentView ()
		{
			InitializeComponent ();
        }

        /*public NotesContentView(string notes)
        {
            InitializeComponent();

            editorText.Text = notes;
        }*/

        #region TextNotes

        /// <summary>
        /// Tao TextNotesProperty de set title cho page
        /// </summary>
        public string TextNotes
        {
            get { return (string)GetValue(TextNotesProperty); }
            set { SetValue(TextNotesProperty, value); }

        }

        public static readonly BindableProperty TextNotesProperty =
            BindableProperty.Create(nameof(TextNotes),
                typeof(string),
                typeof(NotesContentView),
                string.Empty,
                BindingMode.TwoWay,
                propertyChanged: OnTextNotesChanged);

        private static void OnTextNotesChanged(BindableObject bindable, object oldValue, object newValue)
        {
            ((NotesContentView)bindable).OnTextNotesChanged((string)newValue);
        }

        private void OnTextNotesChanged(string textNotes)
        {
            editorText.Text = textNotes;
        }

        #endregion
    }
}