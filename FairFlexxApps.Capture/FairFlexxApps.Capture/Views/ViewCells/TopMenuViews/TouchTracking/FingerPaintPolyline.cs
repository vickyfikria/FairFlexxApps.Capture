using SkiaSharp;
using Microsoft.Maui;

namespace FairFlexxApps.Capture.Views.ViewCells.TopMenuViews.TouchTracking
{
    public class FingerPaintPolyline
    {
        public FingerPaintPolyline()
        {
            Path = new SKPath();
        }

        public SKPath Path { set; get; }

        public Color StrokeColor { set; get; }

        public float StrokeWidth { set; get; }
    }
}
