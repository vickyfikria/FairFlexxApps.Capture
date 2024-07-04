using Android.Content;
using Android.Runtime;
using Android.Views;
using FairFlexxApps.Capture.Droid.Utilities;
using FairFlexxApps.Capture.Interfaces;
using FairFlexxApps.Capture.Utilities;
using Microsoft.Maui;

[assembly: Dependency(typeof(DeviceOrientationImplementation))]
namespace FairFlexxApps.Capture.Droid.Utilities
{
    /// <summary>
    /// DeviceOrientation Implementation
    /// </summary>
    public class DeviceOrientationImplementation : IDeviceOrientation
    {
        #region Constructor

        public DeviceOrientationImplementation() { }

        #endregion

        #region Init

        /// <summary>
        /// Used for registration with dependency service
        /// </summary>
        public static void Init() { }

        #endregion

        #region GetOrientation

        /// <summary>
        /// Gets the orientation.
        /// </summary>
        /// <returns>The orientation.</returns>
        public DeviceOrientations GetOrientation()
        {
            IWindowManager windowManager = Android.App.Application.Context.GetSystemService(Context.WindowService).JavaCast<IWindowManager>();

            var rotation = windowManager.DefaultDisplay.Rotation;
            bool isLandscape = rotation == SurfaceOrientation.Rotation90 || rotation == SurfaceOrientation.Rotation270;
            return isLandscape ? DeviceOrientations.Landscape : DeviceOrientations.Portrait;
        }

        #endregion

        #region NotifyOrientationChange

        /// <summary>
        /// Send orientation change message through MessagingCenter
        /// </summary>
        /// <param name="newConfig">New configuration</param>
        public static void NotifyOrientationChange(global::Android.Content.Res.Configuration newConfig)
        {
            bool isLandscape = newConfig.Orientation == global::Android.Content.Res.Orientation.Landscape;
            var msg = new DeviceOrientationChangeMessage()
            {
                Orientation = isLandscape ? DeviceOrientations.Landscape : DeviceOrientations.Portrait
            };
            MessagingCenter.Send<DeviceOrientationChangeMessage>(msg, DeviceOrientationChangeMessage.MessageId);
        }

        #endregion
    }
}
