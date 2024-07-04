using System;
using System.Threading;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Widget;
using FairFlexxApps.Capture.Droid.FotoScanSdk.Models;
using FairFlexxApps.Capture.Droid.FotoScanSdk.Utils;
using Newtonsoft.Json;
using static Android.App.ProgressDialog;
using Android.Content.PM;

namespace FairFlexxApps.Capture.Droid.FotoScanSdk.Activities
{
    [Activity(Label = "SingleImagePreviewActivity" , ScreenOrientation = ScreenOrientation.Portrait)]
    public class SingleImagePreviewActivity : Activity
    {

        #region Properties

        TextView _fileNameTextView;
        ImageView imvRotate;
        LinearLayout btnSave;
        Button btnRotateImage;
        Bitmap sourcebm, rotatedImgBitmap, leftRotatedImgBitmap, rightRotatedImgBitmap, reverseRotatedImgBitmap;
        ImageInfo tappedImage;
        enum State { _default, _left, _right, _reverse };
        State state = State._default;
        int height;
        int width;

        int scaledRatio;
        private float _angle = 0;
        #endregion

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.SingleImagePreviewLayout);

            ReceiveIntentData();
            Init();
        }

        private void Init()
        {
            btnSave = FindViewById<LinearLayout>(Resource.Id.btnSaveSingleImagePreview);
            imvRotate = FindViewById<ImageView>(Resource.Id.imvRotate);
            btnRotateImage = FindViewById<Button>(Resource.Id.btnRotateImage);

            FindViewById<LinearLayout>(Resource.Id.backSingleImagePreviewButton).Click += (s, e) =>
            {
                RecycleBitmap();
                Finish();
            };
            _fileNameTextView = FindViewById<TextView>(Resource.Id.txtFileName);
            _fileNameTextView.Text = tappedImage.ImagePath.Substring(tappedImage.ImagePath.LastIndexOf("/") + 1).Replace(".png", "");

            btnSave.Click += SaveImage;
            btnRotateImage.Click += BtnRotateImageOnClick;
            
            sourcebm = GetBitmap(tappedImage.ImageOriginalPath);
            imvRotate.SetImageBitmap(sourcebm);
            rotatedImgBitmap = sourcebm;
            leftRotatedImgBitmap = Rotate(sourcebm, 270);
            rightRotatedImgBitmap = Rotate(sourcebm, 90);
            reverseRotatedImgBitmap = Rotate(sourcebm, 180);
        }

        private void RecycleBitmap()
        {
            if (sourcebm != null)
                sourcebm.Recycle();
            if (rotatedImgBitmap != null)
                rotatedImgBitmap.Recycle();
            if (leftRotatedImgBitmap != null)
                leftRotatedImgBitmap.Recycle();
            if (reverseRotatedImgBitmap != null)
                reverseRotatedImgBitmap.Recycle();
            if (rightRotatedImgBitmap != null)
                rightRotatedImgBitmap.Recycle();
        }

        private void ReceiveIntentData()
        {
            tappedImage = JsonConvert.DeserializeObject<ImageInfo>(Intent.GetStringExtra("TappedImage"));
        }

        private void BtnRotateImageOnClick(object sender, EventArgs e)
        {
            switch (state)
            {
                case State._default:
                    rotatedImgBitmap = rightRotatedImgBitmap;
                    state = State._right;
                    break;
                case State._right:
                    rotatedImgBitmap = reverseRotatedImgBitmap;
                    state = State._reverse;
                    break;
                case State._reverse:
                    rotatedImgBitmap = leftRotatedImgBitmap;
                    state = State._left;
                    break;
                default:
                    rotatedImgBitmap = sourcebm;
                    state = State._default;
                    break;
            }
            imvRotate.SetImageBitmap(rotatedImgBitmap);
        }

        private Bitmap Rotate(Bitmap bm, int angle)
        {
            return ScanUtils.RotateBitmap(bm, angle);
        }

        private async void SaveImage(object sender, EventArgs e)
        {
            var progressDialog = Show(this, null, Resources.GetString(Resource.String.saving), true);
            await Task.Delay(3000);
            new Thread(new ThreadStart(
                delegate { //LOAD METHOD TO GET ACCOUNT INFO 
                    RunOnUiThread(() =>
                    {
                        var filePath = ScanUtils.SaveToExternalStorage(rotatedImgBitmap);
                        var fileOriginalPath = filePath;
                        ImageInfo resultRotate = new ImageInfo(filePath, fileOriginalPath, null);
                        //height = Window.FindViewById(Window.IdAndroidContent).Height;
                        //width = Window.FindViewById(Window.IdAndroidContent).Width;
                        var intent = new Intent();
                        intent.PutExtra("ImageRotate", JsonConvert.SerializeObject(resultRotate));
                        //intent.PutExtra("ImageHeight", height);
                        //intent.PutExtra("ImageWidth", width);
                        SetResult(Result.Ok, intent);
                        RecycleBitmap();
                        Finish();
                    });
                    //HIDE PROGRESS DIALOG 
                    RunOnUiThread(() => progressDialog.Hide());
                })).Start();
        }

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
    }
}