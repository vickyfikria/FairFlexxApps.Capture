using System.Collections.ObjectModel;
using System.Threading.Tasks;
using FairFlexxApps.Capture.Enums;
using FairFlexxApps.Capture.Models.FileModels;
using FairFlexxApps.Capture.Models.LeadModels;
using Microsoft.Maui;

namespace FairFlexxApps.Capture.Interfaces
{
    public interface IFileService
    {
        string FilePath { get; }

        bool DeleteFile(string filePath);

        byte[] GetCompressedBitmap(string imagePath);

        Task<ImageSource> SaveImageCompressed(string localPrivateFilePath, byte[] compressedImage, LeadType type = LeadType.Form,
            string name = "");

        Task<FileModel> SaveFile(string localPrivateFilePath, string eventName, LeadType type = LeadType.Form);

        Task<FileModel> SaveTypeFile(string eventName, LeadType type = LeadType.Form,
            LeadTypeModel leadTypeModel = null, string content = null, byte[] byteImage = null, ObservableCollection<byte[]> bytesPdf = null);
       
        Task<byte[]> ResizeImage(byte[] imageData, string imagePath, int ratioResized);

        bool OverrideSaveFileImageSource(string localPrivateFilePath, byte[] byteImage);
    }
}