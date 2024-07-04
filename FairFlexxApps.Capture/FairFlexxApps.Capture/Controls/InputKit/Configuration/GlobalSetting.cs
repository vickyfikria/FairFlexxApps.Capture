using System.ComponentModel;
using System.Runtime.CompilerServices;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Maui;

namespace FairFlexxApps.Capture.Controls.InputKit.Configuration
{
    /// <summary>
    /// Xamarin to .NET MAUI 
    /// by Vicky - 2024-06-27
    /// </summary>
    public partial class GlobalSetting : ObservableObject

    {
        [ObservableProperty] Color? color;
        [ObservableProperty] Color? backgroundColor;
        [ObservableProperty] Color? borderColor;
        [ObservableProperty] float cornerRadius;
        [ObservableProperty] double fontSize;
        [ObservableProperty] double size;

        ///------------------------------------------------------------------
        /// <summary>
        /// Main color of control
        /// </summary>
        //public Color Color { get; set; }
        ///------------------------------------------------------------------
        /// <summary>
        /// Background color of control
        /// </summary>
        //public Color BackgroundColor { get; set; }
        ///------------------------------------------------------------------
        /// <summary>
        /// Border color of control
        /// </summary>
        //public Color BorderColor { get; set; }
        ///------------------------------------------------------------------
        /// <summary>
        /// If control has a corner radius, this is it.
        /// </summary>
        //public float CornerRadius { get; set; }
        ///------------------------------------------------------------------
        /// <summary>
        /// If control has fontsize, this is it.
        /// </summary>
        //public double FontSize { get; set; }
        ///------------------------------------------------------------------
        /// <summary>
        /// Size of control. ( Like HeightRequest and WidthRequest )
        /// </summary>
        //public double Size { get; set; }
        ///------------------------------------------------------------------
        /// <summary>
        /// Text Color of control.
        /// </summary>
        //public Color TextColor { get; set; }
        [ObservableProperty]
        Color textColor;
        ///------------------------------------------------------------------
        /// <summary>
        /// Font family of control.
        /// </summary>
        [ObservableProperty]
        string? fontFamily;
        //public string FontFamily { get; set; }

        ///------------------------------------------------------------------
        /// <summary>
        /// INotifyPropertyChanged Implementation
        /// </summary>
        //public event PropertyChangedEventHandler PropertyChanged;
        //void OnPropertyChanged([CallerMemberName]string name = "") => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
