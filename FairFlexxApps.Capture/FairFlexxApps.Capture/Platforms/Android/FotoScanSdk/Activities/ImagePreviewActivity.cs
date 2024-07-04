using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.View;
using Android.Views;
using Android.Widget;
using FairFlexxApps.Capture.Droid.FotoScanSdk.Adapters;
using FairFlexxApps.Capture.Droid.FotoScanSdk.Constants;
using FairFlexxApps.Capture.Droid.FotoScanSdk.Controls;
using FairFlexxApps.Capture.Droid.FotoScanSdk.Models;
using FairFlexxApps.Capture.Droid.FotoScanSdk.Utils;
using FairFlexxApps.Capture.Models.FotoScanSdk;
using Newtonsoft.Json;

namespace FairFlexxApps.Capture.Droid.FotoScanSdk.Activities
{
    [Activity(Label = "ImagePreviewActivity", ScreenOrientation = ScreenOrientation.Portrait)]
    public class ImagePreviewActivity : Activity, ListView.IOnItemClickListener
    {
        private TextView _fileNameTextView;

        private LinearLayout _layoutCoverFilterTab;
        private LinearLayout _layoutCoverOptimizationTab;
        private LinearLayout _layoutCoverCropTab;
        private LinearLayout _layoutCoverRotateTab;
        private LinearLayout _layoutCoverDeleteTab;
        
        internal static event EventHandler<EventArgs> ImageResult;
        
        LinearLayout _saveButtonTextView;
        Bitmap sourceBm;

        CustomViewPager imageViewPager;
        ImageViewPagerAdapter imvPagerAdapter;
        private List<ImageInfo> Images;

        ImageInfo image;
        ImageModel imageModel;

        int tappedItemPosition;
        int HeightImage;
        int WidthImage;
        
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.ImagePreviewLayout);
            //OpeningNotification();
            ReceiveIntentData();
            InitControlsAndEvents();
        }

        private void ReceiveIntentData()
        {
            var images = JsonConvert.DeserializeObject<List<ImageInfo>>(Intent.GetStringExtra("ListImage"));
            Images = new List<ImageInfo>(images);
            HeightImage = Intent.GetIntExtra("ImageHeight", 0);
            WidthImage = Intent.GetIntExtra("ImageWidth", 0);
        }

        private void InitControlsAndEvents()
        {
            imageViewPager = FindViewById<CustomViewPager>(Resource.Id.imageViewPager);
            imageViewPager.Adapter = new ImageViewPagerAdapter(this, Images);
            imageViewPager.OffscreenPageLimit = Images.Count - 1;
            //imageViewPager.CurrentItem = 1;
            var position = imageViewPager.CurrentItem;
            _saveButtonTextView = FindViewById<LinearLayout>(Resource.Id.saveLabelButton);
            _saveButtonTextView.Click += SaveImage;
            
            FindViewById<ImageView>(Resource.Id.backImageButton).Click += (s, e) => Finish();

            _layoutCoverOptimizationTab = FindViewById<LinearLayout>(Resource.Id.layoutCoverOptimizationTab);
            _layoutCoverOptimizationTab.Click += OptimizationTab_Click;

            _layoutCoverFilterTab = FindViewById<LinearLayout>(Resource.Id.layoutCoverFilterTab);
            _layoutCoverFilterTab.Click += FilterTab_Click;
            
            _layoutCoverCropTab = FindViewById<LinearLayout>(Resource.Id.layoutCoverCropTab);
            _layoutCoverCropTab.Click += CropTab_Click;
            
            _layoutCoverRotateTab = FindViewById<LinearLayout>(Resource.Id.layoutCoverRotateTab);
            _layoutCoverRotateTab.Click += RotateTab_Click;
            
            _layoutCoverDeleteTab = FindViewById<LinearLayout>(Resource.Id.layoutCoverDeleteTab);
            _layoutCoverDeleteTab.Click += DeleteTab_Click;

            _fileNameTextView = FindViewById<TextView>(Resource.Id.txtFileName);
            _fileNameTextView.Text = Images[0].ImagePath.Substring(Images[0].ImagePath.LastIndexOf("/") + 1).Replace(".png", "");
        }

        private void OpeningNotification()
        {
            AlertDialog.Builder notificationDialog = new AlertDialog.Builder(this);
            AlertDialog alert = notificationDialog.Create();
            alert.SetTitle("Warning");
            alert.SetMessage("Occasionally, unable to identify an object to crop automatically.\nPlease use the crop function to crop manually, thanks.");
            alert.SetButton("OK", (c, ev) =>
            {
                // Ok button click task  
            });
            alert.Show();
        }

        private void OptimizationTab_Click(object sender, EventArgs e)
        {
            if (!S.AllowTap) return;
            S.AllowTap = false;
            int position = imageViewPager.CurrentItem;
            ImageInfo tappeditem = Images[position];
            tappedItemPosition = position;
            var activity = new Intent(this, typeof(EditActivity));
            activity.PutExtra("TappedImage", JsonConvert.SerializeObject(tappeditem));

            StartActivityForResult(activity, ScanConstants.RequestCodeOptimization);
            S.ResumeTap();
        }

        private async void DeleteTab_Click(object sender, EventArgs e)
        {
            if (!S.AllowTap) return;
            S.AllowTap = false;
            var progressDialog = ProgressDialog.Show(this, null, Resources.GetString(Resource.String.deleting), true);
            await Task.Delay(3000);
            new System.Threading.Thread(new ThreadStart(
                delegate
                { //LOAD METHOD TO GET ACCOUNT INFO 
                    RunOnUiThread(() =>
                    {
                        int position = imageViewPager.CurrentItem;
                        ScanUtils.DeleteScanFile(Images[position].ImagePath);
                        if (Images.Count == 1)
                        {
                            Toast.MakeText(this, Resources.GetString(Resource.String.delete), ToastLength.Short).Show();
                            Finish();
                        }
                        else
                        {
                            for (int i = position; i < Images.Count - 1; i++)
                                Images[i] = Images[i + 1];
                            Images.RemoveAt(Images.Count - 1);
                            imageViewPager.Adapter = new ImageViewPagerAdapter(this, Images);
                            imageViewPager.CurrentItem = position;
                            Toast.MakeText(this, Resources.GetString(Resource.String.deleted), ToastLength.Short).Show();
                        }
                    });
                    //HIDE PROGRESS DIALOG 
                    RunOnUiThread(() => progressDialog.Hide());
                })).Start();
            S.ResumeTap();
        }
        public struct S
        {
            // control whether the button events are executed
            public static bool AllowTap = true;

            // wait for 200ms after allowing another button event to be executed
            public static async void ResumeTap()
            {
                await Task.Delay(200);
                AllowTap = true;
            }
        }

        void DisableAllButton()
        {
            _layoutCoverCropTab.Enabled = false;
            _layoutCoverDeleteTab.Enabled = false;
            _layoutCoverFilterTab.Enabled = false;
            _layoutCoverOptimizationTab.Enabled = false;
            _layoutCoverRotateTab.Enabled = false;
        }
        void EnableAllButton()
        {
            _layoutCoverCropTab.Enabled = true;
            _layoutCoverDeleteTab.Enabled = true;
            _layoutCoverFilterTab.Enabled = true;
            _layoutCoverOptimizationTab.Enabled = true;
            _layoutCoverRotateTab.Enabled = true;
        }

        private void RotateTab_Click(object sender, EventArgs e)
        {
            if (!S.AllowTap) return;
            S.AllowTap = false;
            int position = imageViewPager.CurrentItem;
            ImageInfo tappeditem = Images[position];
            tappedItemPosition = position;
            var activity = new Intent(this, typeof(SingleImagePreviewActivity));
            activity.PutExtra("TappedImage", JsonConvert.SerializeObject(tappeditem));

            StartActivityForResult(activity, ScanConstants.RequestCodeRotate);
            S.ResumeTap();
        }

        private void CropTab_Click(object sender, EventArgs e)
        {
            if (!S.AllowTap) return;
            S.AllowTap = false;
            int position = imageViewPager.CurrentItem;
            ImageInfo tappeditem = Images[position];
            tappedItemPosition = position;
            var activity = new Intent(this, typeof(ImageCroppingActivity));
            activity.PutExtra("TappedImage", JsonConvert.SerializeObject(tappeditem));
            activity.PutExtra("ImageHeight", HeightImage);
            activity.PutExtra("ImageWidth", WidthImage);
            StartActivityForResult(activity, ScanConstants.RequestCodeCrop);
            S.ResumeTap();
        }

        private void FilterTab_Click(object sender, EventArgs e)
        {
            if (!S.AllowTap) return;
            S.AllowTap = false;
            int position = imageViewPager.CurrentItem;
            ImageInfo tappeditem = Images[position];
            tappedItemPosition = position;
            var intent = new Intent(this, typeof(FilterActivity));
            intent.PutExtra("TappedImage", JsonConvert.SerializeObject(tappeditem));

            StartActivityForResult(intent, ScanConstants.RequestCodeFilter);
            S.ResumeTap();
        }

        public void OnItemClick(AdapterView parent, View view, int position, long id)
        {
            ImageInfo tappeditem = Images[position];
            tappedItemPosition = position;
            var activity = new Intent(this, typeof(ImageCroppingActivity));
            activity.PutExtra("TappedImage", JsonConvert.SerializeObject(tappeditem));

            StartActivityForResult(activity, ScanConstants.RequestCodeCrop);
        }

        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            if (requestCode == ScanConstants.RequestCodeFilter)
            {
                if (resultCode == Result.Ok)
                {
                    var imageFilter = JsonConvert.DeserializeObject<ImageInfo>(data.GetStringExtra("ImageFilter"));
                    Images[tappedItemPosition] = new ImageInfo(imageFilter.ImagePath, imageFilter.ImageOriginalPath, null);
                    imageViewPager.Adapter = new ImageViewPagerAdapter(this, Images);
                    imageViewPager.SetCurrentItem(tappedItemPosition, true);

                    //bmCropped = GetBitmap(imageFilter.ImagePath);
                }
            }
            if(requestCode == ScanConstants.RequestCodeCrop)
            {
                if (resultCode == Result.Ok)
                {
                    var imageCropped = JsonConvert.DeserializeObject<ImageInfo>(data.GetStringExtra("ImageCropped"));
                    Images[tappedItemPosition] = new ImageInfo(imageCropped.ImagePath, imageCropped.ImageOriginalPath, null, true);
                    imageViewPager.Adapter = new ImageViewPagerAdapter(this, Images);
                    imageViewPager.SetCurrentItem(tappedItemPosition, true);

                    //bmCropped = GetBitmap(imageCropped.ImagePath);
                }
            }
            if (requestCode == ScanConstants.RequestCodeRotate)
            {
                if (resultCode == Result.Ok)
                {
                    var imageRotate = JsonConvert.DeserializeObject<ImageInfo>(data.GetStringExtra("ImageRotate"));
                    Images[tappedItemPosition] = new ImageInfo(imageRotate.ImagePath, imageRotate.ImageOriginalPath,null);
                    imageViewPager.Adapter = new ImageViewPagerAdapter(this, Images);
                    imageViewPager.SetCurrentItem(tappedItemPosition, true);

                    //bmCropped = GetBitmap(imageRotate.ImagePath);
                }
            }

            if (requestCode == ScanConstants.RequestCodeOptimization)
            {
                if (resultCode == Result.Ok)
                {
                    var imageOptimization = JsonConvert.DeserializeObject<ImageInfo>(data.GetStringExtra("ImageOptimization"));
                    Images[tappedItemPosition] = new ImageInfo(imageOptimization.ImagePath, imageOptimization.ImageOriginalPath, null);
                    imageViewPager.Adapter = new ImageViewPagerAdapter(this, Images);
                    imageViewPager.SetCurrentItem(tappedItemPosition, true);

                    //bmCropped = GetBitmap(imageOptimization.ImagePath);
                }
            }
        }

        private Bitmap GetBitmap(string imagePath)
        {
            var uri = GetImageUri(imagePath);

            try
            {
                const int imageMaxSize = 1024;
                var ins = ContentResolver.OpenInputStream(uri);
                
                var o = new BitmapFactory.Options { InJustDecodeBounds = true };

                var b1 = BitmapFactory.DecodeStream(ins, null, o);
                ins.Close();

                if (o.OutHeight > imageMaxSize || o.OutWidth > imageMaxSize)
                {
                }

                var o2 = new BitmapFactory.Options { InSampleSize = 1 };
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

        private static Android.Net.Uri GetImageUri(string path)
        {
            return Android.Net.Uri.FromFile(new Java.IO.File(path));
        }

        async void SaveImage(object sender, EventArgs args)
        {
            var progressDialog = ProgressDialog.Show(this, null, "Saving...", true);
            await Task.Delay(3000);
            new System.Threading.Thread(new ThreadStart(
                delegate
                { //LOAD METHOD TO GET ACCOUNT INFO 
                    RunOnUiThread(() =>
                    {
                        List<Bitmap> listBitmaps = new List<Bitmap>();
                        for (int i = 0; i < Images.Count; i++)
                        {
                            ImageInfo tappeditem = Images[i];
                            sourceBm = GetBitmap(tappeditem.ImagePath);
                            listBitmaps.Add(sourceBm);
                        }
                        var filePath = ScanUtils.SaveMultipleImageAsPdf(listBitmaps);
                        Toast.MakeText(this, Resources.GetString(Resource.String.saved), ToastLength.Short).Show();

                        var intent = new Intent();
                        intent.PutExtra("Status", true);
                        SetResult(Result.Ok, intent);
                        imageModel = new ImageModel
                        {
                            ImagePath = filePath,
                            ImageSource = BitmapToBytes(sourceBm)
                        };
                        ImageResult?.Invoke(imageModel, EventArgs.Empty);
                        RecycleBitmap();
                        Finish();
                        //StartActivityForResult(intent, ScanConstants.RequestCodeAfterSaving);
                    });
                    //HIDE PROGRESS DIALOG 
                    RunOnUiThread(() => progressDialog.Hide());
                })).Start();
           
        }

        private void RecycleBitmap()
        {
            if (sourceBm != null)
            {
                sourceBm.Recycle();
            }
            //if (bmCropped != null)
            //{
            //    bmCropped.Recycle();
            //}
        }

        private static byte[] BitmapToBytes(Bitmap myBitmapImage)
        {
            using (var stream = new MemoryStream())
            {
                // Converting Bitmap image to byte[] array
                //myBitmapImage.Compress(Bitmap.CompressFormat.Png, 0, ms);//bug cho nay
                myBitmapImage.Compress(Bitmap.CompressFormat.Png, 100, stream);//v2
                var imageByteArray = stream.ToArray();
                //myBitmapImage.Recycle();
                return imageByteArray;
            }
        }
    }
}