using FairFlexxApps.Capture.Droid.Services.FotoScanSdk;
using FairFlexxApps.Capture.Interfaces.FotoScanSdk;
using System.Threading;
using System.Threading.Tasks;
using System;
using Android.Graphics;
using System.IO;
using FairFlexxApps.Capture.Droid.FotoScanSdk.Activities;
using FairFlexxApps.Capture.Models.FotoScanSdk;

[assembly: Dependency(typeof(FotoScanSdkImplementation))]
namespace FairFlexxApps.Capture.Droid.Services.FotoScanSdk
{
    public class FotoScanSdkImplementation : IFotoScanSdk
    {
        public FotoScanSdkImplementation() { }

        private int _requestId;
        private TaskCompletionSource<ImageModel> _completionSource;

        public Task<ImageModel> OpenCamera()
        {
            var id = GetRequestId();

            _completionSource = null;

            var ntcs = new TaskCompletionSource<ImageModel>(id);
            if (Interlocked.CompareExchange(ref _completionSource, ntcs, null) != null)
            {
                return null;
            }

            //event
            void Handler(object s, EventArgs e)
            {
                var tcs = Interlocked.Exchange(ref _completionSource, null);

                ImagePreviewActivity.ImageResult -= Handler;
                try
                {
                    var resultModel = s as ImageModel;
                    tcs.SetResult(resultModel);
                }
                catch (Exception exception)
                {
#if DEBUG
                    Console.WriteLine(exception);
#endif
                    //tcs.SetResult(null);
                }
            }

            ImagePreviewActivity.ImageResult += Handler;

            Console.WriteLine();
            Platform.CurrentActivity.StartActivity(typeof(CameraActivity));

            // Xamarin.Forms
            //CrossCurrentActivity.Current.Activity.StartActivity(typeof(CameraActivity));

            return _completionSource.Task;
        }

        private int GetRequestId()
        {
            var id = _requestId;
            if (_requestId == int.MaxValue)
                _requestId = 0;
            else
                _requestId++;

            return id;
        }
    }
}
