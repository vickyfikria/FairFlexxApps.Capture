using Android.Content;
using Android.Util;
using System.Threading.Tasks;
using Android.Graphics;
using Camera = Android.Hardware.Camera;
using String = System.String;
using System.IO;
using FairFlexxApps.Capture.Droid.FotoScanSdk.Activities;
using FairFlexxApps.Capture.Droid.FotoScanSdk.OpenCVCustom;
using FairFlexxApps.Capture.Droid.FotoScanSdk.Utils;
using FairFlexxApps.Capture.Droid.FotoScanSdk.Constants;
using Org.Opencv.Core;
using static Android.Hardware.Camera.Parameters;
using Java.Interop;

namespace FairFlexxApps.Capture.Droid.Controls
{
    public class CameraControlView : JavaCameraViewCustom, Camera.IPictureCallback
    {
        private String mPictureFileName;
        private static Camera _mCamera;
        private int _previewWidth;
        private int _previewHeigh;
        private Bitmap copyBitmap;
        private int height, width;
        Context context;
        bool _isBusy = false;


        public CameraControlView(Context context, IAttributeSet attrs) : base(context, attrs)
        {
            this.context = context;
        }

        public async Task TakePicture()
        {
            if (_isBusy)
            {
                return;
            }
            await Task.Run(() =>
            {
                GetPreviewSize();
                
                mCamera.SetPreviewCallback(null);

                mCamera.TakePicture(null, null, this);
            });
            _isBusy = false;
        }

        public void SetPreviewSize()
        {
            Camera.Parameters param = mCamera.GetParameters();
            var sizes = param.SupportedPreviewSizes;
            var mw = sizes[0].Width;
            var mh = sizes[0].Height;

            param.SetPreviewSize(mw, mh);
            mCamera.SetParameters(param);
        }

        public void GetPreviewSize()
        {
            Camera.Parameters param = mCamera.GetParameters();
            _previewWidth = param.PreviewSize.Width;
            _previewHeigh = param.PreviewSize.Height;
        }

        public void OnPictureTaken(byte[] data, Camera camera)
        {
            mCamera.StartPreview();
            mCamera.SetPreviewCallback(this);

            Bitmap bitmap = ScanUtils.DecodeBitmapFromByteArray(data,
                    ScanConstants.HIGHER_SAMPLING_THRESHOLD, ScanConstants.HIGHER_SAMPLING_THRESHOLD);

            Matrix matrix = new Matrix();
            matrix.PostRotate(90);
            bitmap = Bitmap.CreateBitmap(bitmap, 0, 0, bitmap.Width, bitmap.Height, matrix, true);

            CameraActivity.Instance.OnPictureClickedAsync(bitmap);
        }

        public void ChangeWhiteBalanceCameraMode(int wbProgress)
        {
            if (mCamera != null)
            {
                GetPreviewSize();
                mCamera.Reconnect();
                Camera.Parameters param;
                param = mCamera.GetParameters();
                // modify parameter

                switch (wbProgress)
                {
                    case 0:
                        param.WhiteBalance = WhiteBalanceIncandescent;
                        break;
                    case 1:
                        param.WhiteBalance = WhiteBalanceFluorescent;
                        break;
                    case 2:
                        param.WhiteBalance = WhiteBalanceAuto;
                        break;
                    case 3:
                        param.WhiteBalance = WhiteBalanceDaylight;
                        break;
                    case 4:
                        param.WhiteBalance = WhiteBalanceCloudyDaylight;
                        break;
                }

                mCamera.SetParameters(param);
                mCamera.StartPreview();
            }
        }

        private static byte[] BitmapToBytes(Bitmap myBitmapImage)
        {
            using (var stream = new MemoryStream())
            {
                myBitmapImage.Compress(Bitmap.CompressFormat.Png, 100, stream);
                var imageByteArray = stream.ToArray();
                myBitmapImage.Recycle();
                return imageByteArray;
            }
        }

    }
}
