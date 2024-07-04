using System;
using System.Threading;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Views;
using Android.Widget;
using FairFlexxApps.Capture.Droid.FotoScanSdk.Models;
using FairFlexxApps.Capture.Droid.FotoScanSdk.Utils;
using Newtonsoft.Json;
using Org.Opencv.Android;
using Org.Opencv.Core;
using static Android.App.ProgressDialog;

namespace FairFlexxApps.Capture.Droid.FotoScanSdk.Activities
{
    [Activity(Label = "FilterActivity", ScreenOrientation = ScreenOrientation.Portrait)]
    public class FilterActivity : Activity
    {
        #region Properties
        ImageView imvFilter, imvContrast, imvBrightness;
        ImageView btnResetBrightness, btnResetContrast, btnResetWhiteBalance, whiteBalance;
        LinearLayout btnSave;
        SeekBar seekBarBrightness, seekBarContrast, seekBarWhiteBalance;
        Bitmap sourcebm, resultBitmap, grayBitmap, blackWhiteBitmap, grayBitmapDefault, bitmap, copyBitmap;
        WeakReference<Bitmap> drawBitmap;
        ImageButton btnGrayScale, btnBlackWhite, btnColor;
        TextView txtTitle;

        private const int whiteBalanceOffset = 2000;
        private const int whiteBalanceMaxProgress = 9000;

        private int _brightness = 0;
        private int _contrast = 100;

        ImageInfo tappedImage;
        int scaledRatio;
        int state = 0;
        #endregion

        #region FirstOrDefault

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.FilterLayout);
            Init();
        }
        private void Init()
        {
            seekBarBrightness = FindViewById<SeekBar>(Resource.Id.seekBarBrightness);
            seekBarContrast = FindViewById<SeekBar>(Resource.Id.seekBarContrast);
            seekBarWhiteBalance = FindViewById<SeekBar>(Resource.Id.seekBarWhiteBalance);
            btnResetBrightness = FindViewById<ImageView>(Resource.Id.btnResetBrightness);
            btnResetContrast = FindViewById<ImageView>(Resource.Id.btnResetContrast);
            btnResetWhiteBalance = FindViewById<ImageView>(Resource.Id.btnResetWhiteBalance);
            btnSave = FindViewById<LinearLayout>(Resource.Id.btnSave);
            btnGrayScale = FindViewById<ImageButton>(Resource.Id.btn_grayscale);
            btnBlackWhite = FindViewById<ImageButton>(Resource.Id.btn_blackWhite);
            btnColor = FindViewById<ImageButton>(Resource.Id.btn_color);
            imvFilter = FindViewById<ImageView>(Resource.Id.imvFilter);
            imvContrast = FindViewById<ImageView>(Resource.Id.imvContrast);
            imvBrightness = FindViewById<ImageView>(Resource.Id.imvBrightness);
            whiteBalance = FindViewById<ImageView>(Resource.Id.whiteBalance);
            txtTitle = FindViewById<TextView>(Resource.Id.txtTitle);

            txtTitle.Text = Resources.GetString(Resource.String.color);

            FindViewById<LinearLayout>(Resource.Id.backImageButton).Click += (s, e) => BackClicked();

            seekBarBrightness.Progress = 50;
            seekBarContrast.Progress = 50;
            seekBarWhiteBalance.Max = whiteBalanceMaxProgress;
            seekBarWhiteBalance.Progress = whiteBalanceMaxProgress / 2;

            //seekBarBrightness.ProgressChanged += Brightness_ProgressChanged;
            //seekBarContrast.ProgressChanged += Contrast_ProgressChanged;
            //seekBarWhiteBalance.ProgressChanged += WhiteBalance_ProgressChanged;

            seekBarBrightness.StopTrackingTouch += Brightness_StopTrackingTouch;
            seekBarContrast.StopTrackingTouch += Contrast_StopTrackingTouch;
            seekBarWhiteBalance.StopTrackingTouch += WhiteBalance_StopTrackingTouch;

            btnResetBrightness.Click += ResetBrightness_Click;
            btnResetContrast.Click += ResetContrast_Click;
            btnResetWhiteBalance.Click += ResetWhiteBalance_Click;
            btnGrayScale.Click += BtnGreyScale_Click;
            btnColor.Click += BtnColor_Click;
            btnBlackWhite.Click += BtnBlackWhite_Click;

            btnSave.Click += SaveImage;

            tappedImage = JsonConvert.DeserializeObject<ImageInfo>(Intent.GetStringExtra("TappedImage"));
            sourcebm = GetBitmap(tappedImage.ImageOriginalPath);
            resultBitmap = sourcebm;
            imvFilter.SetImageBitmap(sourcebm);

            var result = ChangeGrayscale(sourcebm);
            grayBitmap = DrawableToBitmap(result);
            grayBitmapDefault = grayBitmap;
        }

        private void BackClicked()
        {
            RecycleBitmap();
            Finish();
        }

        private void RecycleBitmap()
        {
            if (resultBitmap != null)
                resultBitmap.Recycle();
            if (sourcebm != null)
                sourcebm.Recycle();
            if (grayBitmap != null)
                grayBitmap.Recycle();
            if (blackWhiteBitmap != null)
                blackWhiteBitmap.Recycle();
            if (grayBitmapDefault != null)
                grayBitmapDefault.Recycle();
        }

        #endregion

        #region ButtonOnClick
        private void BtnColor_Click(object sender, EventArgs e)
        {
            txtTitle.Text = Resources.GetString(Resource.String.color);

            imvContrast.SetImageResource(Resource.Drawable.ic_contrast);
            seekBarBrightness.Progress = 50;
            seekBarContrast.Progress = 50;
            seekBarWhiteBalance.Max = whiteBalanceMaxProgress;
            seekBarWhiteBalance.Progress = whiteBalanceMaxProgress / 2;

            if (state != 0)
            {
                resultBitmap = sourcebm;
                imvFilter.SetImageBitmap(resultBitmap);
                state = 0;
                btnColor.SetImageResource(Resource.Drawable.ic_colorFilterEnable);
                btnBlackWhite.SetImageResource(Resource.Drawable.ic_blackWhite);
                btnGrayScale.SetImageResource(Resource.Drawable.ic_grayscale);
                seekBarWhiteBalance.Visibility = ViewStates.Visible;
                whiteBalance.Visibility = ViewStates.Visible;
                btnResetWhiteBalance.Visibility = ViewStates.Visible;
                imvBrightness.Visibility = ViewStates.Visible;
                seekBarBrightness.Visibility = ViewStates.Visible;
                btnResetBrightness.Visibility = ViewStates.Visible;
            }
        }

        private void BtnBlackWhite_Click(object sender, EventArgs e)
        {
            txtTitle.Text = Resources.GetString(Resource.String.black_and_white);

            imvContrast.SetImageResource(Resource.Drawable.ic_contrastBlackWhite);
            //seekBarBrightness.Progress = 50;
            seekBarContrast.Progress = 50;

            blackWhiteBitmap = DrawableToBitmap(ChangeBlackWhite(grayBitmapDefault, seekBarContrast.Progress * 2));
            resultBitmap = blackWhiteBitmap;
            if (state != 1)
            {
                imvFilter.SetImageBitmap(resultBitmap);
                state = 1;
                btnGrayScale.SetImageResource(Resource.Drawable.ic_grayscale);
                btnBlackWhite.SetImageResource(Resource.Drawable.ic_blackWhiteEnable);
                btnColor.SetImageResource(Resource.Drawable.ic_colorFilter);
                seekBarWhiteBalance.Visibility = ViewStates.Invisible;
                whiteBalance.Visibility = ViewStates.Invisible;
                btnResetWhiteBalance.Visibility = ViewStates.Invisible;
                seekBarBrightness.Visibility = ViewStates.Invisible;
                imvBrightness.Visibility = ViewStates.Invisible;
                btnResetBrightness.Visibility = ViewStates.Invisible;
            }
        }

        private void BtnGreyScale_Click(object sender, EventArgs e)
        {
            txtTitle.Text = Resources.GetString(Resource.String.grey_scale);
            imvContrast.SetImageResource(Resource.Drawable.ic_contrast);
            seekBarBrightness.Progress = 50;
            seekBarContrast.Progress = 50;
            seekBarWhiteBalance.Max = whiteBalanceMaxProgress;
            seekBarWhiteBalance.Progress = whiteBalanceMaxProgress / 2;

            if (state != 2)
            {
                resultBitmap = grayBitmap;
                imvFilter.SetImageBitmap(resultBitmap);
                state = 2;
                btnGrayScale.SetImageResource(Resource.Drawable.ic_grayscaleEnable);
                btnBlackWhite.SetImageResource(Resource.Drawable.ic_blackWhite);
                btnColor.SetImageResource(Resource.Drawable.ic_colorFilter);
                seekBarWhiteBalance.Visibility = ViewStates.Invisible;
                whiteBalance.Visibility = ViewStates.Invisible;
                imvBrightness.Visibility = ViewStates.Visible;
                btnResetWhiteBalance.Visibility = ViewStates.Invisible;
                seekBarBrightness.Visibility = ViewStates.Visible;
                btnResetBrightness.Visibility = ViewStates.Visible;
            }
        }
        #endregion

        #region ChangeGrayscale
        private Drawable ChangeGrayscale(Bitmap bmp)
        {
            ColorMatrix cMtrx = new ColorMatrix(new float[]
            {
                0.3f, 0.59f, 0.11f, 0, 0,
                0.3f, 0.59f, 0.11f, 0, 0,
                0.3f, 0.59f, 0.11f, 0, 0,
                0, 0, 0, 1, 0
            });
            
            ColorFilter colorFilter = new ColorMatrixColorFilter(cMtrx);
            var drawable = new BitmapDrawable(bmp);
            drawable.SetColorFilter(colorFilter);

            return drawable;
        }
        #endregion

        #region ChangeBlackWhite
        private Drawable ChangeBlackWhite(Bitmap bmp, int value)
        {
            float m = 205f;
            float t = -255 * value;
            ColorMatrix cMtrx = new ColorMatrix(new float[]
            {
                m, 1, 0, 1, t,
                0, m, 1, 1, t,
                1, 0, m, 1, t,
                0, 1, 0, 1, 0
            });

            ColorFilter colorFilter = new ColorMatrixColorFilter(cMtrx);
            var drawable = new BitmapDrawable(bmp);
            drawable.SetColorFilter(colorFilter);
            return drawable;
        }

        #endregion

        #region SeekbarStopEvent
        private void Brightness_StopTrackingTouch(object sender, SeekBar.StopTrackingTouchEventArgs e)
        {
            _brightness = e.SeekBar.Progress;
            resultBitmap = ImageFilter(_brightness, _contrast);
            imvFilter.SetImageBitmap(resultBitmap);
        }

        private void Contrast_StopTrackingTouch(object sender, SeekBar.StopTrackingTouchEventArgs e)
        {
            if (state != 1)
            {
                _contrast = e.SeekBar.Progress + 50;
                resultBitmap = ImageFilter(_brightness, _contrast);
                imvFilter.SetImageBitmap(resultBitmap);
            }
            else
            {
                _contrast = e.SeekBar.Progress;
                resultBitmap = DrawableToBitmap(ChangeBlackWhite(grayBitmapDefault, _contrast*2));
                imvFilter.SetImageBitmap(resultBitmap);
            }
        }

        private void WhiteBalance_StopTrackingTouch(object sender, SeekBar.StopTrackingTouchEventArgs e)
        {
            var value = e.SeekBar.Progress + whiteBalanceOffset;

            if (state == 0)
            {
                var result = ChangeDrawableWhiteBalance(sourcebm, value);
                resultBitmap = DrawableToBitmap(result);
                imvFilter.SetImageBitmap(resultBitmap);
            }
            else if (state == 1)
            {
                var result = ChangeDrawableWhiteBalance(blackWhiteBitmap, value);
                resultBitmap = DrawableToBitmap(result);
                imvFilter.SetImageBitmap(resultBitmap);
            }
            else
            {
                var result = ChangeDrawableWhiteBalance(grayBitmap, value);
                resultBitmap = DrawableToBitmap(result);
                imvFilter.SetImageBitmap(resultBitmap);
            }
        }
        #endregion

        #region  ResetButtonOnClick
        private void ResetContrast_Click(object sender, EventArgs e)
        {
            if (state == 0)
            {
                seekBarContrast.Progress = 50;
                resultBitmap = sourcebm;
                imvFilter.SetImageBitmap(resultBitmap);
            }
            else if (state == 1)
            {
                seekBarContrast.Progress = 50;
                resultBitmap = blackWhiteBitmap;
                imvFilter.SetImageBitmap(resultBitmap);
            }
            else
            {
                seekBarContrast.Progress = 50;
                resultBitmap = grayBitmap;
                imvFilter.SetImageBitmap(resultBitmap);
            }
        }

        private void ResetBrightness_Click(object sender, EventArgs e)
        {
            if (state == 0)
            {
                seekBarBrightness.Progress = 50;
                resultBitmap = sourcebm;
                imvFilter.SetImageBitmap(resultBitmap);
            }
            else if (state == 1)
            {
                seekBarBrightness.Progress = 50;
                resultBitmap = blackWhiteBitmap;
                imvFilter.SetImageBitmap(resultBitmap);
            }
            else
            {
                seekBarBrightness.Progress = 50;
                resultBitmap = grayBitmap;
                imvFilter.SetImageBitmap(resultBitmap);
            }
        }

        private void ResetWhiteBalance_Click(object sender, EventArgs e)
        {
            if (state == 0)
            {
                seekBarWhiteBalance.Progress = whiteBalanceMaxProgress / 2;
                resultBitmap = sourcebm;
                imvFilter.SetImageBitmap(resultBitmap);
            }
        }
        #endregion
            
        #region ProgressChanged
        private void Brightness_ProgressChanged(object sender, SeekBar.ProgressChangedEventArgs e)
        {
            //var result = ChangeBrightness(sourcebm, e.Progress);
            //resultBitmap = DrawableToBitmap(result);
            //imvFilter.SetImageBitmap(resultBitmap);

            _brightness = e.Progress;
            resultBitmap = ImageFilter(_brightness, _contrast);
            imvFilter.SetImageBitmap(resultBitmap);
        }

        private void Contrast_ProgressChanged(object sender, SeekBar.ProgressChangedEventArgs e)
        {
            //var result = ChangeContrast(sourcebm, e.Progress);
            //resultBitmap = DrawableToBitmap(result);
            //imvFilter.SetImageBitmap(resultBitmap);

            _contrast = e.Progress + 50;
            resultBitmap = ImageFilter(_brightness, _contrast);
            imvFilter.SetImageBitmap(resultBitmap);
        }

        private void WhiteBalance_ProgressChanged(object sender, SeekBar.ProgressChangedEventArgs e)
        {
            var value = e.Progress + whiteBalanceOffset;

            var result = ChangeDrawableWhiteBalance(sourcebm, value);
            resultBitmap = DrawableToBitmap(result);
            imvFilter.SetImageBitmap(resultBitmap);
        }
        #endregion

        # region ImageFilter
        private Bitmap ImageFilter(int brightness, int contrast)
        {
            Bitmap result;
            
            if (state == 0)
            {
                System.Diagnostics.Debug.WriteLine($"brightness {brightness - 50}, contrast {(double)contrast / 100}");

                Mat src = new Mat(sourcebm.Height, sourcebm.Width, CvType.Cv8uc1);
                Org.Opencv.Android.Utils.BitmapToMat(sourcebm, src);
                src.ConvertTo(src, -1, (double)contrast / 100, brightness - 50);
                try
                {
                    result = Bitmap.CreateBitmap(src.Cols(), src.Rows(), Bitmap.Config.Argb8888);
                }
                catch
                {
                    src.Release();
                    return resultBitmap;
                }
                Org.Opencv.Android.Utils.MatToBitmap(src, result);
                src.Release();

                return result;
            }
            else if (state == 1)
            {
                System.Diagnostics.Debug.WriteLine($"brightness {brightness - 50}, contrast {(double)contrast / 100}");

                Mat src = new Mat(blackWhiteBitmap.Height, blackWhiteBitmap.Width, CvType.Cv8uc1);
                Org.Opencv.Android.Utils.BitmapToMat(blackWhiteBitmap, src);
                src.ConvertTo(src, -1, (double)contrast / 100, brightness - 50);
                try
                {
                    result = Bitmap.CreateBitmap(src.Cols(), src.Rows(), Bitmap.Config.Argb8888);
                }
                catch
                {
                    result = resultBitmap;
                }
                Org.Opencv.Android.Utils.MatToBitmap(src, result);
                src.Release();

                return result;
            }
            else
            {
                System.Diagnostics.Debug.WriteLine($"brightness {brightness - 50}, contrast {(double)contrast / 100}");

                Mat src = new Mat(grayBitmap.Height, grayBitmap.Width, CvType.Cv8uc1);
                Org.Opencv.Android.Utils.BitmapToMat(grayBitmap, src);
                src.ConvertTo(src, -1, (double)contrast / 100, brightness - 50);
                try
                {
                    result = Bitmap.CreateBitmap(src.Cols(), src.Rows(), Bitmap.Config.Argb8888);
                }
                catch
                {
                    result = resultBitmap;
                }
                Org.Opencv.Android.Utils.MatToBitmap(src, result);
                src.Release();

                return result;
            }
        }
        #endregion

        #region OnResume
        protected override void OnResume()
        {
            base.OnResume();
            if (!OpenCVLoader.InitDebug())
            {
                System.Diagnostics.Debug.WriteLine("There is a problem");
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("OpenCV Loaded");
            }
        }
        #endregion

        #region  DrawableToBitmap
        private Bitmap DrawableToBitmap(Drawable drawable)
        {
            bitmap = null;

            if (drawable.IntrinsicWidth <= 0 || drawable.IntrinsicHeight <= 0)
            {
                bitmap = Bitmap.CreateBitmap(1, 1, Bitmap.Config.Argb8888);
                copyBitmap = bitmap.Copy(Bitmap.Config.Argb8888, true);
                //drawBitmap = new WeakReference<Bitmap>(Bitmap.CreateBitmap(1, 1, Bitmap.Config.Argb8888));
            }
            else
            {
                try
                {
                    bitmap = Bitmap.CreateBitmap(sourcebm.Width, sourcebm.Height, Bitmap.Config.Argb8888);
                    copyBitmap = bitmap.Copy(Bitmap.Config.Argb8888, true);
                }
                catch
                {
                    bitmap = resultBitmap;
                    //copyBitmap = bitmap.Copy(Bitmap.Config.Argb8888, true);
                }
            }
            Canvas canvas = new Canvas(copyBitmap);
            drawable.SetBounds(0, 0, canvas.Width, canvas.Height);
            drawable.Draw(canvas);
            return copyBitmap;
        }
        #endregion

        #region ChangeValue
        private Drawable ChangeBrightness(Bitmap bmp, int value)
        {
            ColorMatrix cMtrx = new ColorMatrix(new float[]
            {
                1, 0, 0, 0, value - 50,
                0, 1, 0, 0, value - 50,
                0, 0, 1, 0, value - 50,
                0, 0, 0, 1, 0,
                0, 0, 0, 0, 1
            });

            ColorFilter colorFilter = new ColorMatrixColorFilter(cMtrx);
            var drawable = new BitmapDrawable(bmp);
            drawable.SetColorFilter(colorFilter);

            return drawable;
        }

        private Drawable ChangeContrast(Bitmap bmp, float contrast)
        {
            float value = (contrast - 50) / 100;
            float scale = value + 1f;
            float translate = (-.5f * scale + .5f) * 255f;

            System.Diagnostics.Debug.WriteLine("*** progress = " + value + " scale = " + scale + ", translate = " + translate);

            ColorMatrix cMtrx = new ColorMatrix(new float[]
            {
                scale, 0, 0, 0, translate,
                0, scale, 0, 0, translate,
                0, 0, scale, 0, translate,
                0, 0, 0, 1, 0
            });

            ColorFilter colorFilter = new ColorMatrixColorFilter(cMtrx);
            var drawable = new BitmapDrawable(bmp);
            drawable.SetColorFilter(colorFilter);

            return drawable;
        }

        private Drawable ChangeDrawableWhiteBalance(Bitmap bmp, int whiteBalance)
        {
            float temperature = whiteBalance / 100;
            float red;
            float green;
            float blue;

            // Calculate red
            if (temperature <= 66)
                red = 255;
            else
            {
                red = temperature - 60;
                red = (float)(329.698727446 * (Math.Pow((double)red, -0.1332047592)));
                if (red < 0)
                    red = 0;
                if (red > 255)
                    red = 255;
            }

            // Calculate green
            if (temperature <= 66)
            {
                green = temperature;
                green = (float)(99.4708025861 * Math.Log(green) - 161.1195681661);
                if (green < 0)
                    green = 0;
                if (green > 255)
                    green = 255;
            }
            else
            {
                green = temperature - 60;
                green = (float)(288.1221695283 * (Math.Pow((double)green, -0.0755148492)));
                if (green < 0)
                    green = 0;
                if (green > 255)
                    green = 255;
            }

            // Calculate blue
            if (temperature >= 66)
                blue = 255;
            else if (temperature <= 19)
                blue = 0;
            else
            {
                blue = temperature - 10;
                blue = (float)(138.5177312231 * Math.Log(blue) - 305.0447927307);
                if (blue < 0)
                    blue = 0;
                if (blue > 255)
                    blue = 255;
            }

            //System.Diagnostics.Debug.WriteLine("red=" + red + ", green=" + green + ", blue=" + blue);

            ColorMatrix cMtrx = new ColorMatrix(new float[]
            {
                red / 255, 0, 0, 0, 0,
                0, green / 255, 0, 0, 0,
                0, 0, blue / 255, 0, 0,
                0, 0, 0, 1, 0
            });

            ColorFilter colorFilter = new ColorMatrixColorFilter(cMtrx);
            var drawable = new BitmapDrawable(bmp);
            drawable.SetColorFilter(colorFilter);

            return drawable;
        }
        #endregion

        #region GetBitMap
        private Bitmap GetBitmap(string imagePath)
        {
            var uri = GetImageUri(imagePath);

            try
            {
                const int imageMaxSize = 1024;
                var ins = ContentResolver.OpenInputStream(uri);

                // Decode image size
                var o = new BitmapFactory.Options { InJustDecodeBounds = true };

                var b1 = BitmapFactory.DecodeStream(ins, null, o);
                ins.Close();

                scaledRatio = 1;
                if (o.OutHeight > imageMaxSize || o.OutWidth > imageMaxSize)
                {
                    //scaledRatio = (int)Math.Pow(2, (int)Math.Round(Math.Log(imageMaxSize / (double)Math.Max(o.OutHeight, o.OutWidth)) / Math.Log(0.5)));
                }

                var o2 = new BitmapFactory.Options { InSampleSize = scaledRatio };
                ins = ContentResolver.OpenInputStream(uri);
                var b = BitmapFactory.DecodeStream(ins, null, o2);
                var h2 = b.Height;
                var w2 = b.Width;
                ins.Close();

                return b;
            }
            catch (Exception e) { }

            return null;
        }

        private Android.Net.Uri GetImageUri(string path)
        {
            return Android.Net.Uri.FromFile(new Java.IO.File(path));
        }
        #endregion

        #region SaveImage
        private async void SaveImage(object sender, EventArgs args)
        {
            //ScanUtils.DeleteScanFile(tappedImage.ImagePath);

            var progressDialog = Show(this, null, Resources.GetString(Resource.String.saving), true);
            await Task.Delay(3000);
            new Thread(new ThreadStart(
                delegate { //LOAD METHOD TO GET ACCOUNT INFO 
                    RunOnUiThread(() =>
                    {
                        var filePath = ScanUtils.SaveToExternalStorage(resultBitmap);
                        var fileOriginalPath = filePath;
                        ImageInfo resultFilter = new ImageInfo(filePath, fileOriginalPath, null);
                        var intent = new Intent();
                        intent.PutExtra("ImageFilter", JsonConvert.SerializeObject(resultFilter));
                        SetResult(Result.Ok, intent);
                        RecycleBitmap();
                        Finish();
                    });
                    //HIDE PROGRESS DIALOG 
                    RunOnUiThread(() => progressDialog.Hide());
                })).Start();
        }
        #endregion
    }
}