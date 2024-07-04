using FairFlexxApps.Capture.Models.GeneratePdf;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FairFlexxApps.Capture.Views.Commons.GeneratePdf
{
    public class InputText
    {
        #region Draw Text

        public static string DrawText(SKCanvas canvas, string text, /*SKRect area,*/ SKPaint paint, float areaX, float areaY, float maxWidth)
        {
            float lineHeight = paint.TextSize * 1.8f;
            var lines = SplitLines(text, paint, maxWidth);
            var height = lines.Count() * lineHeight;

            var wA4Size = 1440f;
            var hA4Size = 2030f;
            var paddingForm = (float)wA4Size * 5 / 100;

            var y = areaY + paint.TextSize * 0.5f;

            var contentRemain = string.Empty;

            foreach (var line in lines)
            {
                y += lineHeight;
                //var x = area.MidX - line.Width / 2;
                paint.TextAlign = SKTextAlign.Left;

                if (y + lineHeight * 0.8f > hA4Size - paddingForm)
                {
                    var index = Array.IndexOf(lines, line);

                    for (var i = index; i < lines.Count(); i++)
                    {
                        contentRemain += lines[i].Value;
                    }
                    contentRemain.TrimEnd();
                    return contentRemain;
                }

                canvas.DrawText(line.Value, areaX, y, paint);

            }
            return contentRemain;
        }

        #endregion

        #region Draw Input Text

        public static string DrawInputText(SKCanvas canvas, string text, SKPaint paint, float areaX, float areaY, float maxWidth)
        {
            float lineHeight = paint.TextSize * 2.3f;
            var lines = SplitLines(text, paint, maxWidth);
            var height = lines.Count() * lineHeight;

            var wA4Size = 1440f;
            var hA4Size = 2030f;
            var paddingForm = (float)wA4Size * 5 / 100;

            var y = areaY;

            var contentRemain = string.Empty;

            foreach (var line in lines)
            {
                y += lineHeight;
                //var x = area.MidX - line.Width / 2;
                paint.TextAlign = SKTextAlign.Left;

                if (y > hA4Size - 2 * paddingForm)
                {
                    var index = Array.IndexOf(lines, line);

                    for (var i = index; i < lines.Count(); i++)
                    {
                        contentRemain += lines[i].Value;
                    }
                    contentRemain.TrimEnd();
                    return (string.IsNullOrEmpty(contentRemain) ? "stillremainingtext" : contentRemain);
                }

                canvas.DrawText(line.Value, areaX, y, paint);
                canvas.DrawLine(areaX, y + paint.TextSize / 2, areaX + maxWidth, y + paint.TextSize / 2, paint);

            }
            return contentRemain;
        }

        #endregion

        #region SplitLines

        public static Line[] SplitLines(string text, SKPaint paint, float maxWidth)
        {
            if (string.IsNullOrEmpty(text))
                text = string.Empty;

            var spaceWidth = paint.MeasureText(" ");
            var lines = text.Split('\n');

            return lines.SelectMany((line) =>
            {
                var result = new List<Line>();

                var words = line.Split(new[] { " " }, StringSplitOptions.None);

                var lineResult = new StringBuilder();
                float width = 0;
                foreach (var word in words)
                {
                    if (word == "") 
                        continue;
                    var wordWidth = paint.MeasureText(word);
                    var wordWithSpaceWidth = wordWidth + spaceWidth;
                    var wordWithSpace = word + " ";
                    if(wordWidth > maxWidth)
                    {
                        var lineResultTxt = lineResult.ToString();
                        if (!string.IsNullOrWhiteSpace(lineResultTxt) ||!string.IsNullOrEmpty(lineResultTxt))
                        {
                            result.Add(new Line() { Value = lineResult.ToString(), Width = width });
                            lineResult = new StringBuilder(wordWithSpace);
                        } 
                            var ratio = Math.Ceiling(wordWidth / maxWidth);
                        var characters = Math.Floor(word.Length / ratio);
                        var subStringIndex = 0;
                        for(var i = 1; i <= ratio; i++)
                        {
                           
                            if(i == ratio)
                            {
                                var x = word.Length;
                                lineResult = new StringBuilder(word.Substring(subStringIndex, word.Length - subStringIndex) + " ");
                                width = paint.MeasureText(word.Substring(subStringIndex, word.Length - subStringIndex) + " ");
                                continue;
                            }
                            result.Add(new Line() { Value = word.Substring(subStringIndex, (int)characters), Width = paint.MeasureText(word.Substring(subStringIndex, (int)characters)) });
                            subStringIndex = (int)(i * characters);
                        }
                        continue;
                        
                    }
                    else if (width + wordWidth > maxWidth)
                    {
                        var lineResultTxt = lineResult.ToString();
                        if (string.IsNullOrWhiteSpace(lineResultTxt) || string.IsNullOrEmpty(lineResultTxt))
                        {
                            for (var i = word.Length - 1; i >= 0; i--)
                            {

                                var txt = word.Substring(0, i);
                                if (!string.IsNullOrEmpty(txt))
                                    txt += "-";
                                if (string.IsNullOrEmpty(txt))
                                {
                                    result.Add(new Line() { Value = lineResult.ToString(), Width = width });
                                    lineResult = new StringBuilder(wordWithSpace);
                                    width = wordWithSpaceWidth;
                                    break;
                                }
                                else if (paint.MeasureText(txt) + width <= maxWidth)
                                {
                                    lineResult.Append(txt);
                                    wordWithSpaceWidth = wordWithSpaceWidth - wordWidth - spaceWidth + paint.MeasureText(txt) + spaceWidth;
                                    width += wordWithSpaceWidth;
                                    result.Add(new Line() { Value = lineResult.ToString(), Width = width });
                                    wordWithSpace = word.Substring(i) + " ";
                                    lineResult = new StringBuilder(wordWithSpace);
                                    width = paint.MeasureText(wordWithSpace);
                                    break;
                                }
                            }
                        }
                        else
                        {
                            result.Add(new Line() { Value = lineResult.ToString(), Width = width });
                            lineResult = new StringBuilder(wordWithSpace);
                            width = wordWithSpaceWidth;
                        }

                    }
                    else
                    {
                        lineResult.Append(wordWithSpace);
                        width += wordWithSpaceWidth;
                    }
                }

                result.Add(new Line() { Value = lineResult.ToString(), Width = width });

                return result.ToArray();
            }).ToArray();
        }

        #endregion

    }
}
