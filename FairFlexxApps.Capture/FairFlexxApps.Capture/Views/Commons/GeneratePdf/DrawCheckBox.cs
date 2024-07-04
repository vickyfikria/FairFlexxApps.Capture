using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace FairFlexxApps.Capture.Views.Commons.GeneratePdf
{
    public class InputCheckBox
    {
        public static string DrawCheckBox(SKCanvas canvas, bool isChecked, string text, SKPaint paint, float areaX, float areaY, float maxWidth)
        {
            float checkBoxWidth = 40;

            float lineHeight = paint.TextSize * 2.3f;
            if (isChecked)
            {
                //draw a dot inside radio
                //var checkedFill = new SKPaint
                //{
                //    IsAntialias = true,
                //    Style = SKPaintStyle.Fill,
                //    Color = SKColors.Black,
                //};
                //canvas.DrawRect(areaX + checkBoxWidth / 3, areaY + checkBoxWidth / 3, checkBoxWidth / 3, checkBoxWidth / 3, checkedFill);

                SKPaint thinLinePaint = new SKPaint
                {
                    IsAntialias = true,
                    Style = SKPaintStyle.Stroke,
                    Color = SKColors.Black,
                    StrokeWidth = 6
                };

                canvas.DrawLine(new SKPoint(areaX + checkBoxWidth / 4, areaY + checkBoxWidth / 3 + lineHeight * 1.3f / 2.3f), new SKPoint(areaX + checkBoxWidth / 3, areaY + checkBoxWidth * 3 / 4 + lineHeight * 1.3f / 2.3f), thinLinePaint);
                canvas.DrawLine(new SKPoint(areaX + checkBoxWidth / 3, areaY + checkBoxWidth * 3 / 4 + lineHeight * 1.3f / 2.3f), new SKPoint(areaX + checkBoxWidth * 4 / 5, areaY + checkBoxWidth / 5 + lineHeight * 1.3f / 2.3f), thinLinePaint);

                //var checkedFill = new SKPaint
                //{
                //    TextSize = 60,
                //    IsAntialias = true,
                //    Style = SKPaintStyle.StrokeAndFill,
                //    StrokeWidth = 2,
                //    Color = SKColors.Black,
                //    Typeface = SKTypeface.FromFile(Device.RuntimePlatform == Device.iOS ? "Ionicons" : "ionicons.ttf#ionicons")
                //};
                //canvas.DrawText("✓", areaX + checkBoxWidth / 3, areaY + checkBoxWidth * 2 / 3, checkedFill);
            }

            var checkBorder = new SKPaint
            {
                IsAntialias = true,
                Style = SKPaintStyle.Stroke,
                Color = SKColors.Black,
                StrokeWidth = 6
            };

            // draw the circle border
            canvas.DrawRect(areaX, areaY + lineHeight * 1.3f / 2.3f, checkBoxWidth, checkBoxWidth, checkBorder);
            paint.Style = SKPaintStyle.Fill;
            paint.Color = SKColors.Black;
            //draw text
            return InputText.DrawText(canvas, text, paint, areaX + (float)1.8 * checkBoxWidth, areaY - (float)0.15 * checkBoxWidth, maxWidth - (float)1.8 * checkBoxWidth);
        }
    }
}
