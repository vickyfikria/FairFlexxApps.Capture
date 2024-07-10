using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;

using Microsoft.Maui;
using Microsoft.Maui.Controls.Xaml;


using FairFlexxApps.Capture.ViewModels;
using SkiaSharp;

using System.Reflection;
using FairFlexxApps.Capture.ViewModels.NewLeadFlows;
using FairFlexxApps.Capture.Views.ViewCells.TopMenuViews.TouchTracking;
using FairFlexxApps.Capture.Enums.Templates;
using SkiaSharp.Views.Maui;

namespace FairFlexxApps.Capture.Views.ViewCells.TopMenuViews
{               
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class SketchSkiaSharpContentView : ContentView
	{
		public SketchSkiaSharpContentView ()
		{
			InitializeComponent ();

            


            colorPicker.ItemsSource = colors;
            colorPicker.SelectedIndex = 3;

            widthPicker.ItemsSource = widthStyles;
            widthPicker.SelectedIndex = 2;
        }

        #region OnBidingContextChanged

        protected override void OnBindingContextChanged()
        {
            //base.OnBindingContextChanged();
        }

        #endregion

        #region Properties

        Dictionary<long, FingerPaintPolyline> InProgressPolylines = new Dictionary<long, FingerPaintPolyline>();
        List<FingerPaintPolyline> CompletedPolylines = new List<FingerPaintPolyline>();
        Dictionary<long, SKPath> inProgressPaths = new Dictionary<long, SKPath>();
        List<FingerPaintPolyline> completedPolylinesRedo = new List<FingerPaintPolyline>();

        SKPaint paint = new SKPaint
        {
            Style = SKPaintStyle.Stroke,
            StrokeCap = SKStrokeCap.Round,
            StrokeJoin = SKStrokeJoin.Round,
        };

        ObservableCollection<string> colors = new ObservableCollection<string>(Enum.GetNames(typeof(ColorSketch)));

        ObservableCollection<string> widthStyles = new ObservableCollection<string>(Enum.GetNames(typeof(WidthSketch)));

        //SKBitmap saveBitmap;

        #endregion

        #region OnTouchEffectAction

        private void OnTouchEffectAction(object sender, TouchActionEventArgs args)
        {
            var vm = (NewLeadTemplatePageViewModel)BindingContext;
            if (vm == null)
                return;

            vm.DrawOnSketch = true;
            switch (args.Type)
            {
                case TouchActionType.Pressed:
                    if (!InProgressPolylines.ContainsKey(args.Id))
                    {
                        Color strokeColor = (Color)typeof(Color).GetRuntimeField(colors[colorPicker.SelectedIndex]).GetValue(null);
                        float strokeWidth = ConvertToPixel(new float[] { 1, 2, 5, 10, 20 }[widthPicker.SelectedIndex]);

                        FingerPaintPolyline polyline = new FingerPaintPolyline
                        {
                            StrokeColor = strokeColor,
                            StrokeWidth = strokeWidth
                        };
                        polyline.Path.MoveTo(ConvertToPixel(args.Location));
                        InProgressPolylines.Add(args.Id, polyline);
                        canvasView.InvalidateSurface();
                        inProgressPaths.Add(args.Id, polyline.Path);
                        UpdateBitmap();
                    }
                    break;

                case TouchActionType.Moved:
                    if (InProgressPolylines.ContainsKey(args.Id))
                    {
                        FingerPaintPolyline polyline = InProgressPolylines[args.Id];
                        polyline.Path.LineTo(ConvertToPixel(args.Location));
                        canvasView.InvalidateSurface();
                        UpdateBitmap();
                    }
                    break;

                case TouchActionType.Released:
                    if (InProgressPolylines.ContainsKey(args.Id))
                    {
                        CompletedPolylines.Add(InProgressPolylines[args.Id]);
                        InProgressPolylines.Remove(args.Id);
                        canvasView.InvalidateSurface();
                        inProgressPaths.Remove(args.Id);
                        UpdateBitmap();
                    }
                    break;

                case TouchActionType.Cancelled:
                    if (InProgressPolylines.ContainsKey(args.Id))
                    {
                        InProgressPolylines.Remove(args.Id);
                        inProgressPaths.Remove(args.Id);
                        canvasView.InvalidateSurface();
                        UpdateBitmap();
                    }
                    break;
            }

            // remove Temp list to undo & redo btn
            completedPolylinesRedo = new List<FingerPaintPolyline>();
            btnUndo.IsEnabled = true;
            btnRedo.IsEnabled = false;
        }

        #endregion

        #region OnCanvasViewPaintSurface

        private void OnCanvasViewPaintSurface(object sender, SKPaintSurfaceEventArgs args)
        {
            var vm = (NewLeadTemplatePageViewModel)BindingContext;
            if (vm == null)
                return;
            SKCanvas canvas = args.Surface.Canvas;
            SKImageInfo info = args.Info;
            canvas.Clear(SKColors.White);
            if (vm.SaveBitmap == null)
            {
                vm.SaveBitmap = new SKBitmap(info.Width, info.Height);
            }
            else if (vm.SaveBitmap.Width < info.Width || vm.SaveBitmap.Height < info.Height)
            {
                SKBitmap newBitmap = new SKBitmap(Math.Max(vm.SaveBitmap.Width, info.Width),
                    Math.Max(vm.SaveBitmap.Height, info.Height));
                using (SKCanvas newCanvas = new SKCanvas(newBitmap))
                {
                    newCanvas.Clear();
                    newCanvas.DrawBitmap(vm.SaveBitmap, 0, 0);
                }
                vm.SaveBitmap = newBitmap;
            }

            // Render the bitmap
            canvas.Clear();
            canvas.DrawBitmap(vm.SaveBitmap, 0, 0);
        }

        SKPoint ConvertToPixel(Point pt)
        {
            return new SKPoint((float)(canvasView.CanvasSize.Width * pt.X / canvasView.Width),
                (float)(canvasView.CanvasSize.Height * pt.Y / canvasView.Height));
        }

        float ConvertToPixel(float fl)
        {
            return (float)(canvasView.CanvasSize.Width * fl / canvasView.Width);
        }

        #endregion

        #region Update Bitmap

        private void UpdateBitmap()
        {
            var vm = (NewLeadTemplatePageViewModel)BindingContext;
            if (vm == null)
                return;

            using (SKCanvas saveBitmapCanvas = new SKCanvas(vm.SaveBitmap))
            {
                saveBitmapCanvas.Clear(SKColors.White);
                if (vm?.NewLead.SketchBytes != null)
                {
                    var saveBitmap = SKBitmap.Decode(vm?.NewLead.SketchBytes);
                    saveBitmapCanvas.DrawBitmap(saveBitmap, 0, 0);
                }

                foreach (var path in CompletedPolylines)
                {
                    paint.Color = path.StrokeColor.ToSKColor();
                    paint.StrokeWidth = path.StrokeWidth;
                    saveBitmapCanvas.DrawPath(path.Path, paint);
                }

                foreach (var path in InProgressPolylines.Values)
                {
                    paint.Color = path.StrokeColor.ToSKColor();
                    paint.StrokeWidth = path.StrokeWidth;
                    saveBitmapCanvas.DrawPath(path.Path, paint);
                }
            }

            canvasView.InvalidateSurface();
        }

        #endregion

        #region Clear Button

        private void OnClearButtonClicked(object sender, EventArgs args)
        {
            var vm = (NewLeadTemplatePageViewModel)BindingContext;
            if (vm == null)
                return;

            CompletedPolylines.Clear();
            inProgressPaths.Clear();

            // Disable two btns
            btnUndo.IsEnabled = false;
            btnRedo.IsEnabled = false;

            UpdateBitmap();
            canvasView.InvalidateSurface();
        }

        #endregion

        #region Undo Button

        private void OnUndoButtonClicked(object sender, EventArgs e)
        {
            var vm = (NewLeadTemplatePageViewModel)BindingContext;
            if (vm == null)
                return;

            if (CompletedPolylines.Count > 0)
            {
                completedPolylinesRedo.Add(CompletedPolylines.Last());

                CompletedPolylines.Remove(CompletedPolylines.Last());

                //if (completedPolylinesRedo.Count > 5)
                //{
                //    completedPolylinesRedo.Remove(completedPolylinesRedo.First());
                //}

                // Limit to only remember 5 steps
                btnUndo.IsEnabled = (!(completedPolylinesRedo.Count == 5 || CompletedPolylines.Count == 0));
                btnRedo.IsEnabled = true;

                UpdateBitmap();
                canvasView.InvalidateSurface();
            }

        }

        #endregion

        #region Redo Button

        private void OnRedoButtonClicked(object sender, EventArgs e)
        {
            var vm = (NewLeadTemplatePageViewModel)BindingContext;
            if (vm == null)
                return;

            if (completedPolylinesRedo.Count > 0)
            {
                CompletedPolylines.Add(completedPolylinesRedo.Last());
                completedPolylinesRedo.Remove(CompletedPolylines.Last());

                btnUndo.IsEnabled = true;
                btnRedo.IsEnabled = completedPolylinesRedo.Count > 0;

                UpdateBitmap();
                canvasView.InvalidateSurface();
            }
        }

        #endregion
        
    }
}
