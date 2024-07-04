
namespace FairFlexxApps.Capture.Interfaces
{

    #region Enum Device orientation

    /// <summary>
    /// Device orientations.
    /// </summary>
    public enum DeviceOrientations
    {
        /// <summary>
        /// Undefined
        /// </summary>
        Undefined,

        /// <summary>
        /// The landscape.
        /// </summary>
        Landscape,

        /// <summary>
        /// The portrait.
        /// </summary>
        Portrait
    }

    #endregion

    #region Interface

    public interface IDeviceOrientation
    {
        DeviceOrientations GetOrientation();
    }

    #endregion

}
