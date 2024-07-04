namespace FairFlexxApps.Capture.Utilities
{
    public static class DeviceExtension
    {
        public static Task BeginInvokeOnMainThreadAsync(Action action)
        {
            var tcs = new TaskCompletionSource<bool>();
            // Device is obsolter in .NET MAUI
            // change to MainThread
            MainThread.BeginInvokeOnMainThread(() =>
            {
                try
                {
                    action();
                    tcs.SetResult(true);
                }
                catch (Exception ex)
                {
                    tcs.SetException(ex);
                }
            });
            return tcs.Task;
        }

        /// <summary>
        /// invoke asyn Func<T/> on UI thread
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="a"></param>
        /// <returns></returns>
        public static Task<T> BeginInvokeOnMainThreadAsync<T>(Func<T> a)
        {
            var tcs = new TaskCompletionSource<T>();
            // Device is obsolter in .NET MAUI
            // change to MainThread
            MainThread.BeginInvokeOnMainThread(() =>
            {
                try
                {
                    var result = a();
                    tcs.SetResult(result);
                }
                catch (Exception ex)
                {
                    tcs.SetException(ex);
                }
            });
            return tcs.Task;
        }
    }
}
