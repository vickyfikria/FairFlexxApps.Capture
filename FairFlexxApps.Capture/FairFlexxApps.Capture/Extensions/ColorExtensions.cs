using Microsoft.Maui;

namespace FairFlexxApps.Capture.Extensions
{
    public static class ColorExtensions
    {
        /// <summary>
        /// Defines a surface color will be black or white.
        /// </summary>
        /// <param name="color">Background color</param>
        /// <returns>Surface color on background color</returns>
        public static Color ToSurfaceColor(this Color color)
        {
            if ((color.R + color.G + color.B) >= 1.8)
                return Colors.Black;
            else
                return Colors.White;
        }
    }
}
