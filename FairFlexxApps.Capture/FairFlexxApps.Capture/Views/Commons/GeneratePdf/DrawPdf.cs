using FairFlexxApps.Capture.Enums.Templates;
using FairFlexxApps.Capture.Managers;
using FairFlexxApps.Capture.Models;
using FairFlexxApps.Capture.Models.GeneratePdf;
using FairFlexxApps.Capture.Models.LeadModels;
using FairFlexxApps.Capture.Models.Templates.Controls;
using FairFlexxApps.Capture.Utilities;

using SkiaSharp;
using SkiaSharp.Views.Maui;
using SkiaSharpFormsDemos;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Maui;

namespace FairFlexxApps.Capture.Views.Commons.GeneratePdf
{
    public class DrawPdf
    {
        #region Properties

        private static SKCanvas saveBitmapCanvas;
        private static bool drawnBorder = false;

        public static SKBitmap FormTemplateBitmap;

        #endregion

        #region SaveFormTemplateToPdf

        public async static Task SaveFormTemplateToPdf(ObservableCollection<SKBitmap> formTemplateBitmaps, Template template, LeadModel newLead, int indexOfLanguage)
        {
            #region Properties

            var wA4Size = 1440f;
            var hA4Size = 2030f;
            if (FormTemplateBitmap == null)
            {
                FormTemplateBitmap = new SKBitmap((int)wA4Size, (int)hA4Size);
            }

            var widthForm = ((float)wA4Size * 90 / 100) / 2;
            var paddingForm = (float)wA4Size * 5 / 100;
            var paddingContent = (float)widthForm * 4 / 100;

            var y = paddingForm;
            var boxYY = y;

            var paintBG = new SKPaint()
            {
                TextSize = 28,
                Color = Colors.DimGray.ToSKColor(),
                Style = SKPaintStyle.StrokeAndFill,
                StrokeWidth = 2,
                //Shader = SKShader.CreateColor(Color.Blue.ToSKColor())
            };

            var paintBorder = new SKPaint()
            {
                TextSize = 28,
                Color = Colors.DimGray.ToSKColor(),
                Style = SKPaintStyle.Stroke,
                StrokeWidth = 2,
                //Shader = SKShader.CreateColor(Color.Blue.ToSKColor())
            };

            var paintHeadline = new SKPaint()
            {
                TextSize = 35,
                Color = Colors.White.ToSKColor(),
                Style = SKPaintStyle.StrokeAndFill,
                StrokeWidth = 2,
            };

            var paintText = new SKPaint()
            {
                TextSize = 28,
                Color = Colors.Black.ToSKColor(),
                Style = SKPaintStyle.Fill,
                StrokeWidth = 2,
                //Shader = SKShader.CreateColor(Color.Blue.ToSKColor())
            };

            float lineHeight = paintText.TextSize * 2.3f;
            var columnIndex = 1;

            #endregion

            saveBitmapCanvas = new SKCanvas(FormTemplateBitmap);
            {
                saveBitmapCanvas.Clear(SKColors.White);
                var firstPage = template.Pages.FirstOrDefault();
                foreach (var page in template.Pages)
                {
                    if (newLead.IsLeadWithCard && page == firstPage)
                    {
                        continue;
                    }
                    foreach (var box in page.Boxs)
                    {
                        var controlXX = columnIndex == 1 ? paddingForm + paddingContent : paddingForm + paddingContent + widthForm + 50;

                        #region Header

                        if (box.BoxType != BoxType.HintAddress)
                        {
                            // Painting to header background
                            var totalLineHeadline = InputText.SplitLines(text: box.HeadLine.Language.GetLanguage(indexOfLanguage: indexOfLanguage), paint: paintHeadline, maxWidth: widthForm - 2 * paddingContent);
                            var heightHeadline = totalLineHeadline.Count() * paintHeadline.TextSize * 1.8f + paintHeadline.TextSize * 1.8f;
                            var boundHeadline = SKRect.Create(x: columnIndex == 1 ? paddingForm : paddingForm + widthForm + 50, y: y, width: widthForm, height: heightHeadline);

                            if (boundHeadline.Bottom > hA4Size - 1.5 * paddingForm)
                            {
                                if(columnIndex != 1)
                                {
                                    formTemplateBitmaps.Add(FormTemplateBitmap);
                                    FormTemplateBitmap = new SKBitmap((int)wA4Size, (int)hA4Size);
                                    boxYY = paddingForm;
                                    y = boxYY;
                                    controlXX = paddingForm + paddingContent;
                                    saveBitmapCanvas = new SKCanvas(FormTemplateBitmap);
                                    columnIndex = 1;
                                } else
                                {
                                    boxYY = paddingForm;
                                    y = boxYY;
                                    controlXX = paddingForm + paddingContent + widthForm + 50;
                                    columnIndex = 2;
                                }
                                
                                boundHeadline = SKRect.Create(x: columnIndex == 1 ? paddingForm : paddingForm + widthForm + 50, y: y, width: widthForm, height: heightHeadline);
                            }

                            saveBitmapCanvas.DrawRect(boundHeadline, paintBG);

                            // Title for header
                            var remainHeadlineText = InputText.DrawText(canvas: saveBitmapCanvas, text: box.HeadLine.Language.GetLanguage(indexOfLanguage: indexOfLanguage), paint: paintHeadline, areaX: controlXX, areaY: y, maxWidth: widthForm - 2 * paddingContent);

                            y += heightHeadline;
                            boxYY += heightHeadline;
                        }

                        #endregion

                        // painting to title in a row
                        var heightRow = (float)0;
                        var heightBody = (float)0;

                        foreach (var rowTemplate in box.Body.Rows)
                        {
                            var remainTexts = new ObservableCollection<RemainText>();
                            var widthTitle = widthForm - 2 * paddingContent;
                            var colSpan = widthForm * 2 / 100;
                            var widthColumn = widthForm - 2 * paddingContent; 

                            var totalLine = 0;
                            controlXX = columnIndex == 1 ? paddingContent + paddingForm : paddingForm + paddingContent + widthForm + 50;

                            float Radio_Radius = 20;
                            float checkBoxWidth = 40;

                            #region Label Control

                            if (rowTemplate.Labels != null && rowTemplate.Labels.Count() > 0)
                            {
                                foreach (var label in rowTemplate.Labels)
                                {
                                    if (!label.Visible) continue;
                                    var remainLabel = InputText.DrawText(canvas: saveBitmapCanvas, text: label.Language.GetLanguage(indexOfLanguage), paint: paintText, areaX: controlXX, areaY: y, maxWidth: widthTitle);

                                    drawnBorder = false;//

                                    if (!string.IsNullOrEmpty(remainLabel))
                                    {
                                        remainTexts.Add(new RemainText()
                                        {
                                            Text = remainLabel,
                                            Index = 0,
                                            Type = InputType.label,
                                        });

                                        var lineRemainLabel = InputText.SplitLines(label.Language.GetLanguage(indexOfLanguage).Replace(remainLabel.TrimEnd(), ""), paintText, widthTitle);
                                        var countLblRemainingLines = string.IsNullOrEmpty(label.Language.GetLanguage(indexOfLanguage).Replace(remainLabel.TrimEnd(), "")) ? 0 : lineRemainLabel.Count();
                                        totalLine = (countLblRemainingLines > totalLine) ? countLblRemainingLines : totalLine;
                                    }
                                    else
                                    {
                                        var lines = InputText.SplitLines(label.Language.GetLanguage(indexOfLanguage), paintText, widthTitle);
                                        var countLblLines = string.IsNullOrEmpty(label.Language.GetLanguage(indexOfLanguage)) ? 0 : lines.Count();
                                        totalLine = (countLblLines > totalLine) ? countLblLines : totalLine;
                                    }
                                }
                            }

                            #endregion

                            #region Text Control
                            
                            if (rowTemplate.Texts != null && rowTemplate.Texts.Count() > 0)
                            {
                                foreach (var text in rowTemplate.Texts)
                                {
                                    controlXX = columnIndex == 1 ? paddingForm + paddingContent : paddingForm + paddingContent + widthForm + 50;

                                    var textWidth = widthForm - 2 * paddingContent;

                                    if (box.BoxType == BoxType.HintAddress)
                                    {
                                        textWidth = textWidth - controlXX;

                                        var iconSize = 150 - paddingContent;

                                        if (y + 150 + paddingContent > hA4Size - paddingForm)
                                        {
                                            if(columnIndex != 1)
                                            {
                                                formTemplateBitmaps.Add(FormTemplateBitmap);
                                                FormTemplateBitmap = new SKBitmap((int)wA4Size, (int)hA4Size);
                                                boxYY = paddingForm;
                                                y = boxYY;
                                                controlXX = paddingContent + paddingForm;
                                                saveBitmapCanvas = new SKCanvas(FormTemplateBitmap);
                                                columnIndex = 1;
                                            }
                                            else
                                            {
                                                boxYY = paddingForm;
                                                y = boxYY;
                                                controlXX = paddingForm + paddingContent + widthForm + 50;
                                                columnIndex = 2;
                                            }

                                        }

                                        // Draw hint icon
                                        SKBitmap bitmap = BitmapExtensions.LoadBitmapResource(typeof(NewLeadTemplatePageViewModel), "FairFlexxApps.Capture.Views.Commons.GeneratePdf.ic_hint.png");
                                        SKRect destRect = new SKRect(left: controlXX, top: y + paintText.TextSize * 1.8f, right: controlXX + iconSize, bottom: y + paintText.TextSize * 1.8f + iconSize);
                                        saveBitmapCanvas.DrawBitmap(bitmap, destRect);

                                        controlXX += 150;
                                    }

                                    var remainText = InputText.DrawText(canvas: saveBitmapCanvas, text: text.Languages.GetLanguage(indexOfLanguage), paint: paintText, areaX: controlXX, areaY: y, maxWidth: textWidth);

                                    drawnBorder = false;

                                    if (!string.IsNullOrEmpty(remainText))
                                    {
                                        var linesRow = InputText.SplitLines(text.Languages.GetLanguage(indexOfLanguage).Replace(remainText.TrimEnd(), ""), paintText, textWidth);
                                        var countLblRemainingLines = string.IsNullOrEmpty(text.Languages.GetLanguage(indexOfLanguage).Replace(remainText.TrimEnd(), "")) ? 0 : linesRow.Count();
                                        totalLine = (countLblRemainingLines > totalLine) ? countLblRemainingLines : totalLine;

                                        heightRow = totalLine * lineHeight;
                                        y += heightRow;
                                        heightBody += heightRow + lineHeight * 2 / 3;

                                        if (box.BoxType == BoxType.HintAddress)
                                        {
                                            var hIcon = 150 + 2 * paddingContent;
                                            if (heightBody < hIcon)
                                                heightBody = hIcon - 15;
                                        }

                                        var boundBodyCut = SKRect.Create(x: columnIndex == 1 ? paddingForm : paddingForm + widthForm + 50, y: boxYY, width: widthForm, height: heightBody);
                                        saveBitmapCanvas.DrawRect(boundBodyCut, paintBorder);

                                        //drawnBorder = true;


                                        if (columnIndex != 1)
                                        {
                                            formTemplateBitmaps.Add(FormTemplateBitmap);
                                            FormTemplateBitmap = new SKBitmap((int)wA4Size, (int)hA4Size);
                                            boxYY = paddingForm;
                                            y = boxYY;
                                            controlXX = paddingForm + paddingContent;
                                            saveBitmapCanvas = new SKCanvas(FormTemplateBitmap);
                                            columnIndex = 1;
                                        }
                                        else
                                        {
                                            boxYY = paddingForm;
                                            y = boxYY;
                                            controlXX = paddingForm + paddingContent + widthForm + 50;
                                            columnIndex = 2;
                                        }

                                        textWidth = widthForm - 2 * paddingContent;

                                        totalLine = 0;
                                        heightRow = (float)0;
                                        heightBody = (float)0;

                                        if (box.BoxType == BoxType.HintAddress)
                                        {
                                            controlXX += 150;
                                            textWidth = textWidth - controlXX;
                                        }

                                        InputText.DrawText(canvas: saveBitmapCanvas, text: remainText, paint: paintText, areaX: controlXX, areaY: y, maxWidth: textWidth);

                                        var linesCut = InputText.SplitLines(remainText, paintText, textWidth);
                                        totalLine = (linesCut.Count() > totalLine) ? linesCut.Count() : totalLine;

                                    }
                                    else
                                    {
                                        var lines = InputText.SplitLines(text.Languages.GetLanguage(indexOfLanguage), paintText, textWidth);
                                        totalLine = (lines.Count() > totalLine) ? lines.Count() : totalLine;
                                    }
                                }
                            }

                            #endregion

                            #region Input Control

                            if (rowTemplate.Inputs != null && rowTemplate.Inputs.Count() > 0)
                            {
                                var radiosAny = (rowTemplate.Inputs.Where(item => item.InputType == InputType.radio.ToString())).Any();
                                var checkBoxAny = (rowTemplate.Inputs.Where(item => item.InputType == InputType.checkbox.ToString())).Any();

                                if (radiosAny)
                                {
                                    int NoOfColumn = rowTemplate.Column;
                                    int drawnRadio = 0;
                                    controlXX = columnIndex == 1 ? paddingContent + paddingForm : paddingForm + paddingContent + widthForm + 50;

                                    
                                    widthColumn = (widthForm - 2 * paddingContent) / NoOfColumn - colSpan;

                                    foreach (var input in rowTemplate.Inputs)
                                    {
                                        //if the number of drawn raido equals the number of column then draw radio in the new row
                                        if (drawnRadio % NoOfColumn == 0 && drawnRadio != 0)
                                        {
                                            heightRow = totalLine * lineHeight;
                                            y += heightRow;
                                            heightBody += heightRow;
                                            //reset the total Line after set to create new row
                                            totalLine = 0;
                                            controlXX = columnIndex == 1 ? paddingContent + paddingForm : paddingForm + paddingContent + widthForm + 50;
                                            //drawnRadio = 0;
                                        }

                                        var inputType = (InputType)Enum.Parse(typeof(InputType), input.InputType);
                                        if (inputType == InputType.radio && input.Visible == true)
                                        {
                                            if (y + lineHeight > hA4Size - 2 * paddingForm)
                                            {
                                                //heightBody += 2 * paddingContent;
                                                if ((drawnRadio / NoOfColumn) != 0)
                                                {
                                                    heightBody += lineHeight;
                                                    var boundBodyCut = SKRect.Create(x: columnIndex == 1 ? paddingForm : paddingForm + widthForm + 50, y: boxYY, width: widthForm, height: heightBody);
                                                    saveBitmapCanvas.DrawRect(boundBodyCut, paintBorder);

                                                    //drawnBorder = true;
                                                }

                                                if(columnIndex != 1)
                                                {
                                                    formTemplateBitmaps.Add(FormTemplateBitmap);
                                                    FormTemplateBitmap = new SKBitmap((int)wA4Size, (int)hA4Size);
                                                    boxYY = paddingForm;
                                                    y = boxYY;
                                                    controlXX = paddingForm + paddingContent;
                                                    saveBitmapCanvas = new SKCanvas(FormTemplateBitmap);
                                                    columnIndex = 1;
                                                } else
                                                {
                                                    boxYY = paddingForm;
                                                    y = boxYY;
                                                    controlXX = paddingForm + paddingContent + widthForm + 50;
                                                    columnIndex = 2;
                                                }

                                                heightRow = (float)0;
                                                heightBody = (float)0;

                                                if (remainTexts.Any())
                                                {
                                                    foreach (var itemRemain in remainTexts)
                                                    {
                                                        var remainControlX = controlXX + itemRemain.Index * (colSpan + widthColumn);
                                                        InputText.DrawText(canvas: saveBitmapCanvas, text: itemRemain.Text, paint: paintText, areaX: remainControlX + (float)2.5 * Radio_Radius, areaY: y, maxWidth: widthColumn - (float)2.5 * Radio_Radius);

                                                        var lineRemaintext = InputText.SplitLines(itemRemain.Text, paintText, widthColumn - (float)2.5 * Radio_Radius);
                                                        totalLine = (lineRemaintext.Count() > totalLine) ? lineRemaintext.Count() : totalLine;
                                                    }

                                                    heightRow = totalLine * lineHeight;
                                                    y += heightRow;
                                                    heightBody += heightRow;
                                                    totalLine = 0;
                                                }
                                                remainTexts = new ObservableCollection<RemainText>();
                                            }

                                            var remainText = InputRadio.DrawRadio(canvas: saveBitmapCanvas, text: input.Languages.GetLanguage(indexOfLanguage), isRadioChecked: Boolean.Parse(input.Value), paint: paintText, areaX: controlXX, areaY: y, maxWidth: widthColumn);

                                            if (!string.IsNullOrEmpty(remainText))
                                            {
                                                remainTexts.Add(new RemainText()
                                                {
                                                    Text = remainText,
                                                    Index = drawnRadio,
                                                    Type = InputType.radio,
                                                });

                                                var lines = InputText.SplitLines(input.Languages.GetLanguage(indexOfLanguage).Replace(remainText.TrimEnd(), ""), paintText, widthColumn - (float)2.5 * Radio_Radius);
                                                totalLine = (lines.Count() > totalLine) ? lines.Count() : totalLine;
                                            }
                                            else
                                            {
                                                var lines = InputText.SplitLines(input.Languages.GetLanguage(indexOfLanguage), paintText, widthColumn - (float)2.5 * Radio_Radius);
                                                totalLine = (lines.Count() > totalLine) ? lines.Count() : totalLine;
                                            }

                                            controlXX += colSpan + widthColumn;
                                            drawnRadio++;
                                        }
                                    }
                                }
                                else if (checkBoxAny)
                                {
                                    totalLine = 0;
                                    int NoOfColumn = rowTemplate.Column;
                                    int drawnCheckBox = 0;
                                    controlXX = columnIndex == 1 ? paddingContent + paddingForm : paddingForm + paddingContent + widthForm + 50;

                                    if (rowTemplate.Inputs.Count == 1 && NoOfColumn == 1)
                                    {
                                        string text = rowTemplate.Inputs[0].Languages.GetLanguage(indexOfLanguage);
                                        float textWidth = string.IsNullOrEmpty(text) ? 0 : paintText.MeasureText(text);

                                        var textChild = rowTemplate.Inputs[0].InputChildren?.Value;
                                        float textChildWidth = string.IsNullOrEmpty(textChild) ? 0 : paintText.MeasureText(textChild);

                                        //CheckBox will display in center
                                        float marginleft = (widthForm - 2 * paddingForm - (textWidth > textChildWidth ? textWidth : textChildWidth) - (float)1.8 * checkBoxWidth) / 2;
                                        if (marginleft > 0)
                                        {
                                            controlXX += marginleft;
                                        }

                                        widthColumn = (widthForm - 2 * paddingContent) / NoOfColumn - colSpan;
                                    }
                                    else
                                    {
                                        widthColumn = (widthForm - 2 * paddingContent) / NoOfColumn - colSpan;
                                    }

                                    foreach (var checkbox in rowTemplate.Inputs)
                                    {
                                        //if the number of drawn raido equals the number of column then draw radio in the new row
                                        if (drawnCheckBox % NoOfColumn == 0 && drawnCheckBox != 0)
                                        {
                                            heightRow = totalLine * lineHeight;
                                            y += heightRow;
                                            heightBody += heightRow;
                                            //reset the total Line after set to create new row
                                            totalLine = 0;
                                            controlXX = columnIndex == 1 ? paddingContent + paddingForm : paddingForm + paddingContent + widthForm + 50;
                                            //drawnCheckBox = 0;

                                        }

                                        var inputType = (InputType)Enum.Parse(typeof(InputType), checkbox.InputType);
                                        if (inputType == InputType.checkbox && checkbox.Visible == true)
                                        {
                                            if (y + lineHeight > hA4Size - 2 * paddingForm)
                                            {
                                                if ((drawnCheckBox / NoOfColumn) != 0)
                                                {
                                                    if (boxYY + heightBody < hA4Size - 2 * paddingForm)
                                                        heightBody += lineHeight;
                                                    else
                                                        heightBody += lineHeight / 2;
                                                    var boundBodyCut = SKRect.Create(x: columnIndex == 1 ? paddingForm : paddingForm + widthForm + 50, y: boxYY, width: widthForm, height: heightBody);
                                                    saveBitmapCanvas.DrawRect(boundBodyCut, paintBorder);

                                                    //drawnBorder = true;
                                                }

                                                if (columnIndex != 1)
                                                {
                                                    formTemplateBitmaps.Add(FormTemplateBitmap);
                                                    FormTemplateBitmap = new SKBitmap((int)wA4Size, (int)hA4Size);
                                                    boxYY = paddingForm;
                                                    y = boxYY;
                                                    controlXX = paddingForm + paddingContent;
                                                    saveBitmapCanvas = new SKCanvas(FormTemplateBitmap);
                                                    columnIndex = 1;
                                                }
                                                else
                                                {
                                                    boxYY = paddingForm;
                                                    y = boxYY;
                                                    controlXX = paddingForm + paddingContent + widthForm + 50;
                                                    columnIndex = 2;
                                                }

                                                heightRow = (float)0;
                                                //y = boundTitle.Top + paddingForm * 2 / 3 + heightRow;
                                                heightBody = (float)0;

                                                if (remainTexts.Any())
                                                {
                                                    foreach (var itemRemainText in remainTexts)
                                                    {
                                                        if (itemRemainText.Type == InputType.checkbox)
                                                        {
                                                            var remainControlX = controlXX + itemRemainText.Index * (colSpan + widthColumn);
                                                            if (!string.IsNullOrEmpty(itemRemainText.Text))
                                                            {
                                                                var test = InputText.DrawText(canvas: saveBitmapCanvas, text: itemRemainText.Text, paint: paintText, areaX: remainControlX + (float)1.8 * checkBoxWidth, areaY: y, maxWidth: widthColumn - (float)1.8 * checkBoxWidth);
                                                            }

                                                            if (itemRemainText.InputChildren != null && (itemRemainText.Value?.StringToBool() ?? false))
                                                            {
                                                                var NoOfLine = InputText.SplitLines(itemRemainText.Text, paintText, widthColumn - (float)1.8 * checkBoxWidth);
                                                                InputText.DrawInputText(canvas: saveBitmapCanvas, text: itemRemainText.InputChildren.Value, paint: paintText, areaX: remainControlX, areaY: y + (string.IsNullOrEmpty(itemRemainText.Text) ? 0 : NoOfLine.Count()) * lineHeight, maxWidth: widthColumn);
                                                                var lines = InputText.SplitLines(itemRemainText.InputChildren.Value, paintText, widthColumn);
                                                                totalLine = (lines.Count() + NoOfLine.Count() > totalLine) ? lines.Count() + NoOfLine.Count() : totalLine;
                                                            }
                                                            else
                                                            {
                                                                var lines = InputText.SplitLines(itemRemainText.Text, paintText, widthColumn - (float)1.8 * checkBoxWidth);
                                                                totalLine = (lines.Count() > totalLine) ? lines.Count() : totalLine;
                                                            }

                                                            var lineRemaintext = InputText.SplitLines(itemRemainText.Text, paintText, widthColumn - (float)1.8 * checkBoxWidth);
                                                            totalLine = (lineRemaintext.Count() > totalLine) ? lineRemaintext.Count() : totalLine;
                                                        }
                                                    }

                                                    heightRow = totalLine * lineHeight;
                                                    y += heightRow;
                                                    heightBody += heightRow;
                                                    totalLine = 0;
                                                }
                                                remainTexts = new ObservableCollection<RemainText>();
                                            }

                                            string remainCheckboxText = InputCheckBox.DrawCheckBox(canvas: saveBitmapCanvas, text: checkbox.Languages.GetLanguage(indexOfLanguage), isChecked: Boolean.Parse(checkbox.Value), paint: paintText, areaX: controlXX, areaY: y, maxWidth: widthColumn);

                                            if (!string.IsNullOrEmpty(remainCheckboxText))
                                            {
                                                remainTexts.Add(new RemainText()
                                                {
                                                    Text = remainCheckboxText,
                                                    Value = checkbox.Value,
                                                    Type = InputType.checkbox,
                                                    Index = drawnCheckBox,
                                                    InputChildren = checkbox.InputChildren
                                                });

                                                //var lastCheckBox = rowTemplate.Inputs.LastOrDefault();
                                                var lines = InputText.SplitLines(checkbox.Languages.GetLanguage(indexOfLanguage).Replace(remainCheckboxText.Trim(), ""), paintText, widthColumn - (float)1.8 * checkBoxWidth);
                                                totalLine = (lines.Count() > totalLine) ? lines.Count() : totalLine;
                                            }
                                            else
                                            {
                                                if (checkbox.InputChildren != null && (checkbox.Value?.StringToBool() ?? false))
                                                {
                                                    var NoOfLine = InputText.SplitLines(checkbox.Languages.GetLanguage(indexOfLanguage), paintText, widthColumn - (float)1.8 * checkBoxWidth);
                                                    var remainChildText = InputText.DrawInputText(canvas: saveBitmapCanvas, text: checkbox.InputChildren.Value, paint: paintText, areaX: controlXX, areaY: y + NoOfLine.Count() * lineHeight, maxWidth: widthColumn);

                                                    if (!string.IsNullOrEmpty(remainChildText))
                                                    {
                                                        var child = new RemainText()
                                                        {
                                                            Text = string.Empty,
                                                            Value = checkbox.Value,
                                                            Index = drawnCheckBox,
                                                            Type = InputType.checkbox,
                                                            InputChildren = new Input()
                                                            {
                                                                Value = remainChildText,
                                                            },
                                                        };
                                                        remainTexts.Add(child);
                                                        var lines = InputText.SplitLines(checkbox.InputChildren.Value?.Replace(remainChildText.Trim(), ""), paintText, widthColumn);
                                                        totalLine = (lines.Count() + NoOfLine.Count() > totalLine) ? lines.Count() + NoOfLine.Count() : totalLine;
                                                    }
                                                    else
                                                    {
                                                        var lines = InputText.SplitLines(checkbox.InputChildren.Value, paintText, widthColumn);
                                                        totalLine = (lines.Count() + NoOfLine.Count() > totalLine) ? lines.Count() + NoOfLine.Count() : totalLine;
                                                    }

                                                }
                                                else
                                                {
                                                    var lines = InputText.SplitLines(checkbox.Languages.GetLanguage(indexOfLanguage), paintText, widthColumn - (float)1.8 * checkBoxWidth);
                                                    totalLine = (lines.Count() > totalLine) ? lines.Count() : totalLine;
                                                }
                                            }

                                            controlXX += colSpan + widthColumn;
                                            drawnCheckBox++;
                                        }
                                    }
                                }
                                else
                                {
                                    var drawnInputText = 0;
                                    foreach (var input in rowTemplate.Inputs)
                                    {
                                        if (!input.Visible) continue;
                                        y += lineHeight;
                                        heightBody += lineHeight;

                                        var remainInputText = InputText.DrawInputText(canvas: saveBitmapCanvas, text: input.Value, paint: paintText, areaX: controlXX, areaY: y, maxWidth: widthColumn);

                                        drawnBorder = false;

                                        if (!string.IsNullOrEmpty(remainInputText))
                                        {
                                            if (remainInputText == "stillremainingtext" && string.IsNullOrEmpty(input.Value))
                                            {
                                                remainTexts.Add(new RemainText()
                                                {
                                                    Text = input.Value,
                                                    Index = drawnInputText,
                                                    Type = InputType.other,
                                                    IsHaveTitle = (rowTemplate.Labels.Where(item => item.Visible && !string.IsNullOrEmpty(item.Language.GetLanguage(indexOfLanguage)))).Any(),
                                                });

                                                var lines = InputText.SplitLines(input.Value, paintText, widthColumn);
                                                var countEmptyTxtLines = string.IsNullOrEmpty(input.Value) ? 0 : lines.Count();
                                                totalLine = (countEmptyTxtLines > totalLine) ? countEmptyTxtLines : totalLine;
                                            }
                                            else
                                            {
                                                remainTexts.Add(new RemainText()
                                                {
                                                    Text = remainInputText,
                                                    Index = drawnInputText,
                                                    Type = InputType.other,
                                                    IsHaveTitle = (rowTemplate.Labels.Where(item => item.Visible && !string.IsNullOrEmpty(item.Language.GetLanguage(indexOfLanguage)))).Any(),
                                                });

                                                var lineRemainText = InputText.SplitLines(input.Value.Replace(remainInputText.TrimEnd(), ""), paintText, widthColumn);
                                                var countTxtLines = string.IsNullOrEmpty(input.Value.Replace(remainInputText.TrimEnd(), "")) ? 0 : lineRemainText.Count();
                                                totalLine = (countTxtLines > totalLine) ? countTxtLines : totalLine;
                                            }

                                        }
                                        else
                                        {
                                            var lines = InputText.SplitLines(input.Value, paintText, widthColumn);
                                            totalLine = (lines.Count() > totalLine) ? lines.Count() : totalLine;
                                        }

                                        drawnInputText++;
                                    }
                                }

                            }

                            #endregion

                            heightRow = totalLine * lineHeight;
                            y += heightRow;
                            heightBody += heightRow;

                            if (remainTexts.Any())
                            {
                                heightBody += lineHeight;
                                var boundBodyCut = SKRect.Create(x: columnIndex == 1 ? paddingForm : paddingForm + widthForm + 50, y: boxYY, width: widthForm, height: heightBody);
                                saveBitmapCanvas.DrawRect(boundBodyCut, paintBorder);

                                drawnBorder = true;


                                if (columnIndex != 1)
                                {
                                    formTemplateBitmaps.Add(FormTemplateBitmap);
                                    FormTemplateBitmap = new SKBitmap((int)wA4Size, (int)hA4Size);
                                    boxYY = paddingForm;
                                    y = boxYY;
                                    controlXX = paddingForm + paddingContent;
                                    saveBitmapCanvas = new SKCanvas(FormTemplateBitmap);
                                    columnIndex = 1;
                                }
                                else
                                {
                                    boxYY = paddingForm;
                                    y = boxYY;
                                    controlXX = paddingForm + paddingContent + widthForm + 50;
                                    columnIndex = 2;
                                }
                                totalLine = 0;
                                // painting to title in a row
                                heightRow = (float)0;
                                heightBody = (float)0;

                                #region Label Control

                                var labelRemainAny = (remainTexts.Where(item => item.Type == InputType.label));
                                if (labelRemainAny.Any())
                                {
                                    foreach (var lbl in labelRemainAny)
                                    {
                                        var remainTextControlX = controlXX;
                                        InputText.DrawText(canvas: saveBitmapCanvas, text: lbl.Text, paint: paintText, areaX: remainTextControlX, areaY: y, maxWidth: widthTitle);

                                        var lineRemaintext = InputText.SplitLines(lbl.Text, paintText, widthTitle);
                                        totalLine = (lineRemaintext.Count() > totalLine) ? lineRemaintext.Count() : totalLine;
                                    }
                                }

                                //var isTitle = (remainTexts.Where(item => item.Type == InputType.other && item.IsHaveTitle)).Any();
                                //if (isTitle)
                                //    controlXX += widthTitle + colSpan;

                                #endregion

                                var radiosRemainAny = (remainTexts.Where(item => item.Type == InputType.radio));
                                var checkBoxRemainAny = (remainTexts.Where(item => item.Type == InputType.checkbox));

                                if (radiosRemainAny.Any())
                                {
                                    foreach (var radio in radiosRemainAny)
                                    {
                                        var indexRad = radio.Index % rowTemplate.Column;
                                        var remainRadControlX = controlXX + indexRad * (colSpan + widthColumn);

                                        var testRad = InputText.DrawText(canvas: saveBitmapCanvas, text: radio.Text, paint: paintText, areaX: remainRadControlX + (float)2.5 * Radio_Radius, areaY: y, maxWidth: widthColumn - (float)2.5 * Radio_Radius);

                                        var lineRemaintext = InputText.SplitLines(radio.Text, paintText, widthColumn - (float)2.5 * Radio_Radius);
                                        totalLine = (lineRemaintext.Count() > totalLine) ? lineRemaintext.Count() : totalLine;

                                        drawnBorder = false;
                                    }
                                }
                                else if (checkBoxRemainAny.Any())
                                {
                                    foreach (var ckb in checkBoxRemainAny)
                                    {
                                        var index = ckb.Index % rowTemplate.Column;
                                        var remainCkbControlX = controlXX + index * (colSpan + widthColumn);

                                        if (!string.IsNullOrEmpty(ckb.Text))
                                        {
                                            var test = InputText.DrawText(canvas: saveBitmapCanvas, text: ckb.Text, paint: paintText, areaX: remainCkbControlX + (float)1.8 * checkBoxWidth, areaY: y, maxWidth: widthColumn - (float)1.8 * checkBoxWidth);
                                        }

                                        if (ckb.InputChildren != null && (ckb.Value?.StringToBool() ?? false))
                                        {

                                            if (string.IsNullOrEmpty(ckb.Text))
                                            {
                                                InputText.DrawInputText(canvas: saveBitmapCanvas, text: ckb.InputChildren.Value, paint: paintText, areaX: remainCkbControlX, areaY: y, maxWidth: widthColumn);
                                                var lines = InputText.SplitLines(ckb.InputChildren.Value, paintText, widthColumn);
                                                totalLine = (lines.Count() > totalLine) ? lines.Count() : totalLine;
                                            }
                                            else
                                            {
                                                var CkbLine = InputText.SplitLines(ckb.Text, paintText, widthColumn - (float)1.8 * checkBoxWidth);
                                                InputText.DrawInputText(canvas: saveBitmapCanvas, text: ckb.InputChildren.Value, paint: paintText, areaX: remainCkbControlX, areaY: y + CkbLine.Count() * lineHeight, maxWidth: widthColumn);
                                                var lines = InputText.SplitLines(ckb.InputChildren.Value, paintText, widthColumn);
                                                totalLine = (lines.Count() + CkbLine.Count() > totalLine) ? lines.Count() + CkbLine.Count() : totalLine;
                                            }
                                        }
                                        else
                                        {
                                            var lineCkb = InputText.SplitLines(ckb.Text, paintText, widthColumn - (float)1.8 * checkBoxWidth);
                                            totalLine = (lineCkb.Count() > totalLine) ? lineCkb.Count() : totalLine;
                                        }
                                        drawnBorder = false;
                                    }
                                }
                                else
                                {
                                    var textRemainAny = (remainTexts.Where(item => item.Type == InputType.other));

                                    foreach (var text in textRemainAny)
                                    {
                                        var remainTextControlX = controlXX + text.Index * (colSpan + widthColumn);
                                        InputText.DrawInputText(canvas: saveBitmapCanvas, text: text.Text, paint: paintText, areaX: remainTextControlX, areaY: y, maxWidth: widthColumn);

                                        var lineRemaintext = InputText.SplitLines(text.Text, paintText, widthColumn);
                                        totalLine = (lineRemaintext.Count() > totalLine) ? lineRemaintext.Count() : totalLine;
                                    }
                                }

                                heightRow = totalLine * lineHeight;
                                y += heightRow;
                                heightBody += heightRow;

                                var lastRow = box.Body.Rows.LastOrDefault();

                                if (lastRow == rowTemplate)
                                {
                                    heightBody += lineHeight;
                                    var boundBodyRemain = SKRect.Create(x: columnIndex == 1 ? paddingForm : paddingForm + widthForm + 50, y: boxYY, width: widthForm, height: heightBody);
                                    saveBitmapCanvas.DrawRect(boundBodyRemain, paintBorder);
                                    boxYY = boundBodyRemain.Bottom + 50;
                                    y = boxYY;

                                    drawnBorder = true;

                                    //reset some value
                                    totalLine = 0;
                                    // painting to title in a row
                                    heightRow = (float)0;
                                    heightBody = (float)0;
                                }

                                remainTexts = new ObservableCollection<RemainText>();
                            }
                        }

                        if (!drawnBorder)
                        {
                            heightBody += lineHeight;
                            var boundBody = SKRect.Create(x: columnIndex == 1 ? paddingForm : paddingForm + widthForm + 50, y: boxYY, width: widthForm, height: heightBody);
                            saveBitmapCanvas.DrawRect(boundBody, paintBorder);

                            boxYY = boundBody.Bottom + 50;
                            y = boxYY;
                        }

                        if (boxYY > hA4Size - paddingForm)
                        {
                            if(columnIndex != 1)
                            {
                                formTemplateBitmaps.Add(FormTemplateBitmap);
                                FormTemplateBitmap = new SKBitmap((int)wA4Size, (int)hA4Size);
                                boxYY = paddingForm;
                                y = boxYY;
                                controlXX = paddingForm + paddingContent;
                                saveBitmapCanvas = new SKCanvas(FormTemplateBitmap);
                                columnIndex = 1;
                            }
                            else
                            {
                                boxYY = paddingForm;
                                y = boxYY;
                                controlXX = paddingForm + paddingContent + widthForm + 50;
                                columnIndex = 2;
                            }
                        }

                        drawnBorder = false;
                    }
                }

            }
            formTemplateBitmaps.Add(FormTemplateBitmap);
            //await SavePdf();
        }

        #endregion

    }
}
