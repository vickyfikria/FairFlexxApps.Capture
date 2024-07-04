
using System.ComponentModel;
using System.Runtime.CompilerServices;

using Microsoft.Maui;

namespace FairFlexxApps.Capture.Controls.RadioButton
{
    public class GlobalSetting
    {
        public GlobalSetting()
        {
        }

        public Color Color { get; set; }
        public Color BorderColor { get; set; }
        public Color TextColor { get; set; }
        public double Size { get; set; }
        public int CornerRadius { get; set; }
        public double FontSize { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;
        void OnPropertyChanged([CallerMemberName]string name = "") => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}