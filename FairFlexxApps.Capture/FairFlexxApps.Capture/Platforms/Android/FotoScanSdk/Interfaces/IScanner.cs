using Android.Graphics;
using FairFlexxApps.Capture.Droid.FotoScanSdk.Enums;

namespace FairFlexxApps.Capture.Droid.FotoScanSdk.Interfaces
{
    public interface IScanner
    {
        void DisplayHint(ScanHint scanHint);
        void OnPictureClickedAsync(Bitmap bitmap);
    }
}