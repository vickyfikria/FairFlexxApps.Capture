using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace FairFlexxApps.Capture.Views.Commons.GeneratePdf
{
    public class InputRadio
    {
        public static string DrawRadio(SKCanvas canvas, bool isRadioChecked, string text, SKPaint paint, float areaX, float areaY, float maxWidth)
        {
            float Radio_Radius = 20;
            float lineHeight = paint.TextSize * 2.3f;
            if (isRadioChecked)
            {
                //draw a dot inside radio
                var circleFill = new SKPaint
                {
                    IsAntialias = true,
                    Style = SKPaintStyle.Fill,
                    Color = SKColors.Black
                };
                canvas.DrawCircle(areaX + Radio_Radius, areaY - Radio_Radius + lineHeight, Radio_Radius * 6 / 10, circleFill);
            }

            var circleBorder = new SKPaint
            {
                IsAntialias = true,
                Style = SKPaintStyle.Stroke,
                Color = SKColors.Black,
                StrokeWidth = 5
            };

            // draw the circle border
            canvas.DrawCircle(areaX + Radio_Radius, areaY - Radio_Radius + lineHeight, Radio_Radius, circleBorder);
            //draw text
            paint.Style = SKPaintStyle.Fill;
            return InputText.DrawText(canvas, text, paint, areaX + (float)2.5 * Radio_Radius, areaY - (float)0.25 * Radio_Radius, maxWidth - (float)2.5 * Radio_Radius);
        }
    }
}
