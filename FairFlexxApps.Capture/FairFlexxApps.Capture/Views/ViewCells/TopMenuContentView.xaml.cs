using System;
using Microsoft.Maui;
using Microsoft.Maui.Controls.Xaml;

namespace FairFlexxApps.Capture.Views.ViewCells
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class TopMenuContentView : ContentView
	{
		public TopMenuContentView ()
		{
			InitializeComponent ();
		}

        #region IsNotesVisible

        /// <summary>
        /// 
        /// </summary>
        public bool IsNotesVisible
        {
            get => (bool)GetValue(IsNotesVisibleProperty);
            set => SetValue(IsNotesVisibleProperty, value);
        }

        public static readonly BindableProperty IsNotesVisibleProperty =
            BindableProperty.Create(nameof(IsNotesVisible),
                typeof(bool),
                typeof(TopMenuContentView),
                true,
                BindingMode.TwoWay,
                propertyChanged: OnIsNotesVisiblePropertyChanged);

        private static void OnIsNotesVisiblePropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            ((TopMenuContentView)bindable).notes.IsVisible = (bool)newValue;
        }

        #endregion

        #region IsNotesVisible

        /// <summary>
        /// 
        /// </summary>
        public bool IsSketchVisible
        {
            get => (bool)GetValue(IsSketchVisibleProperty);
            set => SetValue(IsSketchVisibleProperty, value);
        }

        public static readonly BindableProperty IsSketchVisibleProperty =
            BindableProperty.Create(nameof(IsSketchVisible),
                typeof(bool),
                typeof(TopMenuContentView),
                true,
                BindingMode.TwoWay,
                propertyChanged: OnIsSketchVisiblePropertyChanged);

        private static void OnIsSketchVisiblePropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            ((TopMenuContentView)bindable).sketch.IsVisible = (bool)newValue;
        }

        #endregion

        //#region IsCardVisible

        ///// <summary>
        ///// 
        ///// </summary>
        //public bool IsCardVisible
        //{
        //    get => (bool)GetValue(IsCardVisibleProperty);
        //    set => SetValue(IsCardVisibleProperty, value);
        //}

        //public static readonly BindableProperty IsCardVisibleProperty =
        //    BindableProperty.Create(nameof(IsCardVisible),
        //        typeof(bool),
        //        typeof(TopMenuContentView),
        //        true,
        //        BindingMode.TwoWay,
        //        propertyChanged: OnIsCardVisiblePropertyChanged);

        //private static void OnIsCardVisiblePropertyChanged(BindableObject bindable, object oldValue, object newValue)
        //{
        //    ((TopMenuContentView)bindable).card.IsVisible = (bool)newValue;
        //}

        //#endregion

        #region IsObjectVisible

        /// <summary>
        /// 
        /// </summary>
        public bool IsObjectVisible
        {
            get => (bool)GetValue(IsObjectVisibleProperty);
            set => SetValue(IsObjectVisibleProperty, value);
        }

        public static readonly BindableProperty IsObjectVisibleProperty =
            BindableProperty.Create(nameof(IsObjectVisible),
                typeof(bool),
                typeof(TopMenuContentView),
                true,
                BindingMode.TwoWay,
                propertyChanged: OnIsObjectVisiblePropertyChanged);

        private static void OnIsObjectVisiblePropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            ((TopMenuContentView)bindable).@object.IsVisible = (bool)newValue;
        }

        #endregion

        #region IsAttachmentVisible

        /// <summary>
        /// 
        /// </summary>
        public bool IsAttachmentVisible
        {
            get => (bool)GetValue(IsAttachmentVisibleProperty);
            set => SetValue(IsAttachmentVisibleProperty, value);
        }

        public static readonly BindableProperty IsAttachmentVisibleProperty =
            BindableProperty.Create(nameof(IsAttachmentVisible),
                typeof(bool),
                typeof(TopMenuContentView),
                true,
                BindingMode.TwoWay,
                propertyChanged: OnIsAttachmentVisiblePropertyChanged);

        private static void OnIsAttachmentVisiblePropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            ((TopMenuContentView)bindable).attachment.IsVisible = (bool)newValue;
        }

        #endregion


        #region TopMenuItemSelected
	    private void TopMenuItemSelected(object sender, EventArgs e)
	    {
	        //var tempPage = new TempPage();
	        //tempPage.TopMenuItemSelected(sender: sender, e: e);
	    }

        #endregion
    }
}
