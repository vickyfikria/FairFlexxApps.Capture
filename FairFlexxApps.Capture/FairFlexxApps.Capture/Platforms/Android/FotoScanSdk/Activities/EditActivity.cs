using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Graphics;
using Android.OS;
using Android.Widget;
using FairFlexxApps.Capture.Droid.FotoScanSdk.Models;
using FairFlexxApps.Capture.Droid.FotoScanSdk.Utils;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using static Android.App.ProgressDialog;

namespace FairFlexxApps.Capture.Droid.FotoScanSdk.Activities
{
    [Activity(Label = "OptimizationActivity", ScreenOrientation = ScreenOrientation.Portrait)]
    public class EditActivity : Activity
    {
        #region Properties
        private LinearLayout btnSaveOptimization;
        private static ImageView imvOptimization;
        private static ImageInfo tappedImage;
        private static Bitmap sourcebm, compressedBitmap, resizedBitmap, bothBitmap;
        private int scaledRatio;
        private Button btnCompress, btnResize, btnWidthHeightResize;
        private static MemoryStream stream;
        private static int quality, compress = 100;
        private static double resize = 100;
        private static bool checkQuality; 
        private static double percentage;
        private static bool checkPercentage = true;
        private static double ratio;
        private static EditText input;
        private static LinearLayout percentage_Layout;
        private static int state = 0;
        static readonly int NONE = 0;
        static readonly int RESIZE = 1;
        static readonly int COMPRESS = 2;
        static readonly int BOTH = 3;
        private static LinearLayout manual_Layout;
        private static RadioButton percentage_radio;
        private static RadioButton manual_radio;
        //private static LinearLayout percentage_Layout;
        //private static LinearLayout manual_Layout;
        private static RadioGroup radioGroup;
        private static Context context;
        #endregion

        #region OnCreate
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.EditLayout);

            Init();
        }
        #endregion

        private void Init()
        {
            context = BaseContext;
            checkQuality = true;
            btnSaveOptimization = FindViewById<LinearLayout>(Resource.Id.btnSaveOptimization);
            imvOptimization = FindViewById<ImageView>(Resource.Id.imvOptimization);
            FindViewById<LinearLayout>(Resource.Id.backImageButtonOptimization).Click += (s, e) =>
            {
                RecycleBitmap();
                Finish();
            };
            btnCompress = FindViewById<Button>(Resource.Id.btnCompress);
            //btnResize = FindViewById<Button>(Resource.Id.btnResize);
            //radioGroup = FindViewById<RadioGroup>(Resource.Id.radioGroup);
            //input = FindViewById<EditText>(Resource.Id.input);
            //manual_radio = FindViewById<RadioButton>(Resource.Id.radio_manual);
            //percentage_radio = FindViewById<RadioButton>(Resource.Id.radio_percentage);

            //radioGroup.CheckedChange += RadioGroup_CheckedChange;
            btnCompress.Click += Compress_Click;
            btnSaveOptimization.Click += SaveOptimization_Click;
            //btnResize.Click += BtnResize_Click;

            tappedImage = JsonConvert.DeserializeObject<ImageInfo>(Intent.GetStringExtra("TappedImage"));
            sourcebm = GetBitmap(tappedImage.ImageOriginalPath);
            imvOptimization.SetImageBitmap(sourcebm);


        }

        private void RadioGroup_CheckedChange(object sender, RadioGroup.CheckedChangeEventArgs e)
        {
            if(percentage_radio.Checked)
            {
                input.Hint = "Enter percentage";
            }
            if (manual_radio.Checked)
            {
                input.Hint = "Enter width";
            }
        }
        
        private async void SaveOptimization_Click(object sender, EventArgs e)
        {
            if (checkQuality)
            {
                var progressDialog = Show(this, null, Resources.GetString(Resource.String.saving), true);
                await Task.Delay(1000);
                new Thread(new ThreadStart(
                    delegate { //LOAD METHOD TO GET ACCOUNT INFO 
                        RunOnUiThread(() =>
                        {
                            //var filePath = ScanUtils.SaveToExternalStorage(sourcebm);
                            //var fileOriginalPath = filePath;
                            //ImageInfo resultOptimization = new ImageInfo(filePath, fileOriginalPath, null);
                            //var intent = new Intent();
                            //intent.PutExtra("ImageOptimization", JsonConvert.SerializeObject(resultOptimization));
                            //SetResult(Result.Ok, intent);
                            RecycleBitmap();
                            Finish();
                        });
                        //HIDE PROGRESS DIALOG 
                        RunOnUiThread(() => progressDialog.Hide());
                    })).Start();;
            }
            else
            {
                var progressDialog = Show(this, null, Resources.GetString(Resource.String.saving), true);
                await Task.Delay(3000);
                new Thread(new ThreadStart(
                    delegate { //LOAD METHOD TO GET ACCOUNT INFO 
                        RunOnUiThread(() =>
                        {
                            
                            var filePath = ScanUtils.SaveToExternalStorage(compressedBitmap);
                            var fileOriginalPath = filePath;
                            ImageInfo resultOptimization = new ImageInfo(filePath, fileOriginalPath, null);
                            var intent = new Intent();
                            intent.PutExtra("ImageOptimization", JsonConvert.SerializeObject(resultOptimization));
                            SetResult(Result.Ok, intent);
                            RecycleBitmap();
                            Finish();
                        });
                        //HIDE PROGRESS DIALOG 
                        RunOnUiThread(() => progressDialog.Hide());
                    })).Start();
            }
        }

        private void RecycleBitmap()
        {
            if (sourcebm != null)
            {
                sourcebm.Recycle();
            }
            if (compressedBitmap != null)
            {
                compressedBitmap.Recycle();
            }
            if (resizedBitmap != null)
            {
                resizedBitmap.Recycle();
            }
            if (bothBitmap != null)
            {
                bothBitmap.Recycle();
            }
        }

        private static void Edit(double percentage, int quality)
        {
            //Resize(percentage);
            Compress(quality);
        }

        private static void Resize(double percentage)
        {
            if (percentage <= 3)
            {
                double newWidth = percentage * sourcebm.Width;
                double newHeight = percentage * sourcebm.Height;
                resizedBitmap = Bitmap.CreateScaledBitmap(sourcebm, (int)newWidth, (int)newHeight, false);
                imvOptimization.SetImageBitmap(resizedBitmap);
                //Toast.MakeText("Resize complete!", ToastLength.Long).Show();
            }
            else
            {
                
            }
        }

        private static void Compress(int quality)
        {
            if (resize < 1)
            {
                byte[] imageData = System.IO.File.ReadAllBytes(tappedImage.ImagePath);
                stream = new MemoryStream(imageData, true);
                resizedBitmap.Compress(Bitmap.CompressFormat.Jpeg, quality, stream);
                byte[] byteArray = new byte[int.MaxValue];
                byteArray = stream.ToArray();
                compressedBitmap = BitmapFactory.DecodeByteArray(byteArray, 0, byteArray.Length);
                imvOptimization.SetImageBitmap(compressedBitmap);
            }
            else
            {
                if (quality != 100)
                {
                    byte[] imageData = System.IO.File.ReadAllBytes(tappedImage.ImagePath);
                    stream = new MemoryStream(imageData, true);
                    sourcebm.Compress(Bitmap.CompressFormat.Jpeg, quality, stream);
                    //byte[] byteArray = new byte[int.MaxValue/2];
                    var byteArray = stream.ToArray();
                    compressedBitmap = BitmapFactory.DecodeByteArray(byteArray, 0, byteArray.Length);
                    imvOptimization.SetImageBitmap(compressedBitmap);
                }
                else
                {
                    imvOptimization.SetImageBitmap(sourcebm);
                }
            }
        }

        private void Compress_Click(object sender, EventArgs e)
        {
            AlertDialog.Builder builder = new AlertDialog.Builder(this);
            builder.SetTitle(Resources.GetString(Resource.String.choose_quality));
            string[] qualities = {
                Resources.GetString(Resource.String.quality_10),
                Resources.GetString(Resource.String.quality_25),
                Resources.GetString(Resource.String.quality_50),
                Resources.GetString(Resource.String.quality_75),
                Resources.GetString(Resource.String.quality_100) };
            builder.SetItems(qualities, new DialogInterface());
            AlertDialog dialog = builder.Create();
            dialog.Show();
        }

        private void BtnResize_Click(object sender, EventArgs e)
        {
            var param = double.Parse(input.Text);
            if (percentage_radio.Checked)
                resize = param / 100;
            else
                resize = param / sourcebm.Width;
            Edit(resize, compress);
        }

        public class DialogInterface : Java.Lang.Object, IDialogInterfaceOnClickListener
        {
            public void OnClick(IDialogInterface dialog, int which)
            {
                if (checkPercentage)
                    compressedBitmap = sourcebm;
                else
                    compressedBitmap = resizedBitmap;
                switch (which)
                {
                    case 0:
                        compress = 10;
                        break;
                    case 1:
                        compress = 25;
                        break;
                    case 2:
                        compress = 50;
                        break;
                    case 3:
                        compress = 75;
                        break;
                    case 4:
                            compress = 100;
                            break;
                }
                checkQuality = false;
                Edit(resize, compress);
                //byte[] imageData = System.IO.File.ReadAllBytes(tappedImage.ImagePath);
                //stream = new MemoryStream(imageData, true);
                //compressedBitmap.Compress(Bitmap.CompressFormat.Jpeg, quality, stream);
                //byte[] byteArray = new byte[int.MaxValue];
                //byteArray = stream.ToArray();
                //compressedBitmap = BitmapFactory.DecodeByteArray(byteArray, 0, byteArray.Length);
                //imvOptimization.SetImageBitmap(compressedBitmap);
            }
        }

        private Bitmap GetBitmap(string imagePath)
        {
            var uri = GetImageUri(imagePath);

            try
            {
                const int imageMaxSize = 1024;

                // Decode image size
                var o = new BitmapFactory.Options { InJustDecodeBounds = true };

                scaledRatio = 1;
                if (o.OutHeight > imageMaxSize || o.OutWidth > imageMaxSize)
                {
                    //scaledRatio = (int)Math.Pow(2, (int)Math.Round(Math.Log(imageMaxSize / (double)Math.Max(o.OutHeight, o.OutWidth)) / Math.Log(0.5)));
                }

                var o2 = new BitmapFactory.Options { InSampleSize = scaledRatio };
                var ins = ContentResolver.OpenInputStream(uri);
                var b = BitmapFactory.DecodeStream(ins, null, o2);
                var h2 = b.Height;
                var w2 = b.Width;
                ins.Close();

                return b;
            }
            catch (Exception e) { }

            return null;
        }

        private Android.Net.Uri GetImageUri(string imagePath)
        {
            return Android.Net.Uri.FromFile(new Java.IO.File(imagePath));
        }
    }
}
