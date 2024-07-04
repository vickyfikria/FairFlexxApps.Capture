using System.Threading.Tasks;
using FairFlexxApps.Capture.Models.FotoScanSdk;

namespace FairFlexxApps.Capture.Interfaces.FotoScanSdk
{
    public interface IFotoScanSdk
    {
        Task<ImageModel> OpenCamera();
    }
}
