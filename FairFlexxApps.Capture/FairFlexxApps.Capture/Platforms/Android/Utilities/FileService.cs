using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Security.AccessControl;
using System.Threading.Tasks;
using Android.Graphics;
using Android.Media;
using FairFlexxApps.Capture.Constants;
using FairFlexxApps.Capture.Droid.Utilities;
using FairFlexxApps.Capture.Enums;
using FairFlexxApps.Capture.Interfaces;
using FairFlexxApps.Capture.Models.FileModels;
using FairFlexxApps.Capture.Models.LeadModels;
using Microsoft.Maui;
using static Android.Provider.DocumentsContract;
using Environment = Android.OS.Environment;
using Path = System.IO.Path;
using PdfDocument = Android.Graphics.Pdf.PdfDocument;
[assembly: Dependency(typeof(FileService))]
namespace FairFlexxApps.Capture.Droid.Utilities
{
    public class FileService : IFileService
    {
        #region Properties & private functions

        public string FilePath { get; } = Environment.ExternalStorageDirectory.ToString();

        public bool InitStorageUpload(string filePath)
        {
            if (File.Exists(filePath)) return false;

            Directory.CreateDirectory(filePath);
            return true;
        }

        #endregion

        #region Delete File

        public bool DeleteFile(string filePath)
        {
            if (!File.Exists(filePath)) return false;

            File.Delete(filePath);
            return true;
        }

        #endregion

        #region CompressBitmap

        public byte[] GetCompressedBitmap(string imagePath)
        {
            float maxHeight = 1920.0f;
            float maxWidth = 1080.0f;
            Bitmap scaledBitmap = null;
            BitmapFactory.Options options = new BitmapFactory.Options();
            options.InJustDecodeBounds = true;
            Bitmap bmp = BitmapFactory.DecodeFile(imagePath, options);

            int actualHeight = options.OutHeight;
            int actualWidth = options.OutWidth;
            float imgRatio = (float)actualWidth / (float)actualHeight;
            float maxRatio = maxWidth / maxHeight;

            if (actualHeight > maxHeight || actualWidth > maxWidth)
            {
                if (imgRatio < maxRatio)
                {
                    imgRatio = maxHeight / actualHeight;
                    actualWidth = (int)(imgRatio * actualWidth);
                    actualHeight = (int)maxHeight;
                }
                else if (imgRatio > maxRatio)
                {
                    imgRatio = maxWidth / actualWidth;
                    actualHeight = (int)(imgRatio * actualHeight);
                    actualWidth = (int)maxWidth;
                }
                else
                {
                    actualHeight = (int)maxHeight;
                    actualWidth = (int)maxWidth;
                }
            }

            options.InSampleSize = CalculateInSampleSize(options, actualWidth, actualHeight);
            options.InJustDecodeBounds = false;
            options.InDither = false;
            options.InPurgeable = true;
            options.InInputShareable = true;
            options.InTempStorage = new byte[16 * 1024];

            try
            {
                bmp = BitmapFactory.DecodeFile(imagePath, options);
            }
            catch (Java.Lang.OutOfMemoryError exception)
            {
                exception.PrintStackTrace();
            }
            try
            {
                scaledBitmap = Bitmap.CreateBitmap(actualWidth, actualHeight, Bitmap.Config.Argb8888);
            }
            catch (Java.Lang.OutOfMemoryError exception)
            {
                exception.PrintStackTrace();
            }

            float ratioX = actualWidth / (float)options.OutWidth;
            float ratioY = actualHeight / (float)options.OutHeight;
            float middleX = actualWidth / 2.0f;
            float middleY = actualHeight / 2.0f;

            Matrix scaleMatrix = new Matrix();
            scaleMatrix.SetScale(ratioX, ratioY, middleX, middleY);

            Canvas canvas = new Canvas(scaledBitmap);
            canvas.Matrix = scaleMatrix;
            canvas.DrawBitmap(bmp, middleX - bmp.Width / 2, middleY - bmp.Height / 2, new Paint(PaintFlags.FilterBitmap));

            ExifInterface exif = null;
            try
            {
                exif = new ExifInterface(imagePath);
                int orientation = exif.GetAttributeInt(ExifInterface.TagOrientation, 0);
                Matrix matrix = new Matrix();
                if (orientation == 6)
                {
                    matrix.PostRotate(90);
                }
                else if (orientation == 3)
                {
                    matrix.PostRotate(180);
                }
                else if (orientation == 8)
                {
                    matrix.PostRotate(270);
                }
                scaledBitmap = Bitmap.CreateBitmap(scaledBitmap, 0, 0, scaledBitmap.Width, scaledBitmap.Height, matrix, true);
            }
            catch (IOException e)
            {
                e.StackTrace.ToString();
            }

            MemoryStream output = new MemoryStream();
            scaledBitmap.Compress(Bitmap.CompressFormat.Jpeg, 85, output);

            byte[] byteArray = output.ToArray();

            return byteArray;
        }

        private int CalculateInSampleSize(BitmapFactory.Options options, int reqWidth, int reqHeight)
        {
            int height = options.OutHeight;
            int width = options.OutWidth;
            int inSampleSize = 1;

            if (height > reqHeight || width > reqWidth)
            {
                int heightRatio = Java.Lang.Math.Round((float)height / (float)reqHeight);
                int widthRatio = Java.Lang.Math.Round((float)width / (float)reqWidth);
                inSampleSize = heightRatio < widthRatio ? heightRatio : widthRatio;
            }
            float totalPixels = width * height;
            float totalReqPixelsCap = reqWidth * reqHeight * 2;

            while (totalPixels / (inSampleSize * inSampleSize) > totalReqPixelsCap)
            {
                inSampleSize++;
            }
            return inSampleSize;
        }

        #endregion

        #region ResizeImage

        public async Task<byte[]> ResizeImage(byte[] imageData, string imagePath, int ratioResized)
        {
            // Load the bitmap 
            BitmapFactory.Options options = new BitmapFactory.Options();// Create object of bitmapfactory's option method for further option use
            options.InPurgeable = true; // inPurgeable is used to free up memory while required
            options.InSampleSize = ratioResized;
            Bitmap originalImage = BitmapFactory.DecodeFile(imagePath, options);

            using (MemoryStream ms = new MemoryStream())
            {
                originalImage.Compress(format: Bitmap.CompressFormat.Jpeg, quality: 100, stream: ms);

                originalImage.Recycle();

                return ms.ToArray();
            }

        }

        #endregion

        #region SaveFile

        public async Task<FileModel> SaveFile(string localPrivateFilePath, string eventName,
            LeadType type = LeadType.Form)
        {
            var date = DateTime.Now;
            string filename = $"{type.ToString()} {date:yyyy-MM-dd HH.mm.ss}.pdf";
            //var filename = string.IsNullOrWhiteSpace(name) ? $"{type.ToString()} {date:yyyy-MM-dd HH.mm.ss}.pdf" : $"{name}.pdf";

            // create fileService directory
            var filePath = CreateFilePath(eventName, type);
            

            InitStorageUpload(filePath);

            var localPublicFilePath = Path.Combine(filePath, filename);

            // Read all the PDF data from the LOCAL PRIVATE STORAGE where you saved it to
            var bytes = File.ReadAllBytes(localPrivateFilePath);
            //Copy the private fileService's data to the EXTERNAL PUBLIC location

            if (Device.RuntimePlatform == Device.Android && (int)Android.OS.Build.VERSION.SdkInt >= 30)
            {
                var file = new Java.IO.File(localPublicFilePath);
                localPublicFilePath = file.Path;
                using (FileStream fileStream = new FileStream(localPublicFilePath, FileMode.Create, FileAccess.Write))
                {
                    fileStream.Write(bytes, 0, bytes.Length);
                }
            }
            else
            {
                File.WriteAllBytes(localPublicFilePath, bytes);
            }

            return new FileModel()
            {
                FileName = filename,
                LocalPath = localPublicFilePath,
                IsSaved = true,
                Type = type,
                CreateDate = date
            };
        }

        #endregion

        #region Save Type File

        public async Task<FileModel> SaveTypeFile(string eventName, LeadType type = LeadType.Xml, LeadTypeModel leadTypeModel = null,
            string content = "", byte[] byteImage = null, ObservableCollection<byte[]> bytesPdf = null)
        {
            var date = DateTime.Now;
            string filename = $"{type.ToString()} {date:yyyy-MM-dd HH.mm.ss}";
            //var filename = string.IsNullOrWhiteSpace(name) ? $"{type.ToString()} {date:yyyy-MM-dd HH.mm.ss}.pdf" : $"{name}.pdf";

            // insert file format
            switch (type)
            {
                case LeadType.Note:
                    filename += ".txt";
                    break;
                case LeadType.Sketch:
                    filename += ".pdf";
                    break;
                case LeadType.Xml:
                    filename += ".xml";
                    break;
                case LeadType.FormPdf:
                    filename += ".pdf";
                    break;
            }

            // create fileService directory
            string filePath = CreateFilePath(eventName, type);
            InitStorageUpload(filePath);
            var localPublicFilePath = Path.Combine(filePath, filename);
            if (type == LeadType.FormPdf)
            {
                // Load the bitmap 
                BitmapFactory.Options options = new BitmapFactory.Options();// Create object of bitmapfactory's option method for further option use
                options.InPurgeable = true; // inPurgeable is used to free up memory while required
                //Bitmap bitmap = BitmapFactory.DecodeByteArray(byteImage, 0, byteImage.Length, options);

                PdfDocument document = new PdfDocument();

                foreach (var bytes in bytesPdf)
                {
                    Bitmap bitmap = BitmapFactory.DecodeByteArray(bytes, 0, bytes.Length, options);

                    // crate a page description
                    PdfDocument.PageInfo pageInfo = new PdfDocument.PageInfo.Builder(bitmap.Width, bitmap.Height, 1).Create();

                    // start a page
                    PdfDocument.Page page = document.StartPage(pageInfo);

                    Canvas canvas = page.Canvas;

                    bitmap = Bitmap.CreateScaledBitmap(bitmap, bitmap.Width, bitmap.Height, true);

                    canvas.DrawBitmap(bitmap, 0, 0, null);

                    // finish the page
                    document.FinishPage(page);
                }                               
               
                DeleteFile(leadTypeModel?.FilePath ?? localPublicFilePath);
                var stream = new FileStream(leadTypeModel?.FilePath ?? localPublicFilePath, FileMode.OpenOrCreate, FileAccess.ReadWrite, share: FileShare.ReadWrite);

                document.WriteTo(stream);
                stream.Close();

                // close the document
                document.Close();
            }
            else if (type == LeadType.Sketch)
            {
                //File.WriteAllBytes((leadTypeModel?.FilePath ?? localPublicFilePath), byteImage);

                // Load the bitmap 
                BitmapFactory.Options options = new BitmapFactory.Options();// Create object of bitmapfactory's option method for further option use
                options.InPurgeable = true; // inPurgeable is used to free up memory while required
                Bitmap bitmap = BitmapFactory.DecodeByteArray(byteImage, 0, byteImage.Length, options);

                PdfDocument document = new PdfDocument();

                // crate a page description
                PdfDocument.PageInfo pageInfo = new PdfDocument.PageInfo.Builder(bitmap.Width, bitmap.Height, 1).Create();

                // start a page
                PdfDocument.Page page = document.StartPage(pageInfo);

                Canvas canvas = page.Canvas;

                bitmap = Bitmap.CreateScaledBitmap(bitmap, bitmap.Width, bitmap.Height, true);

                canvas.DrawBitmap(bitmap, 0, 0, null);

                // finish the page
                document.FinishPage(page);
                DeleteFile(leadTypeModel?.FilePath ?? localPublicFilePath);
                var stream = new FileStream(leadTypeModel?.FilePath ?? localPublicFilePath, FileMode.OpenOrCreate, FileAccess.ReadWrite, share: FileShare.ReadWrite);

                document.WriteTo(stream);
                stream.Close();

                // close the document
                document.Close();
            }
               // File.WriteAllBytes((leadTypeModel?.FilePath ?? localPublicFilePath), byteImage);
            else
                File.WriteAllText((leadTypeModel?.FilePath ?? localPublicFilePath), content);
            return new FileModel()
            {
                FileName = (leadTypeModel?.FileName ?? filename),
                LocalPath = (leadTypeModel?.FilePath ?? localPublicFilePath),
                IsSaved = true,
                Type = type,
                CreateDate = (leadTypeModel?.CreateDate ?? date),
            };
        }

        #endregion

        #region Save Image Compress

        public async Task<ImageSource> SaveImageCompressed(string localPrivateFilePath, byte[] compressedImage, LeadType type = LeadType.Form, string name = "")
        {
            //Read all the PDF data from the LOCAL PRIVATE STORAGE where you saved it to
            // create fileService directory
            //Copy the private fileService's data to the EXTERNAL PUBLIC location
            File.WriteAllBytes(localPrivateFilePath + "resize", compressedImage);

            var image = ImageSource.FromFile(localPrivateFilePath + "resize");

            return image;
        }

        #endregion


        #region GetFileImageSource

        public bool OverrideSaveFileImageSource(string localPrivateFilePath, byte[] byteImage)
        {
            try
            {
                //Read all the PDF data from the LOCAL PRIVATE STORAGE where you saved it to
                // create fileService directory
                //Copy the private fileService's data to the EXTERNAL PUBLIC location

                File.WriteAllBytes(localPrivateFilePath, byteImage);

                var image = ImageSource.FromFile(localPrivateFilePath);

                return true;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                return false;
            }

        }
        #endregion


        private string CreateFilePath(string eventName, LeadType type)
        {
            if (Device.RuntimePlatform == Device.Android && (int)Android.OS.Build.VERSION.SdkInt >= 30)
            {
                var dir = Environment.GetExternalStoragePublicDirectory(Environment.DirectoryDocuments);
                var file = new Java.IO.File(dir + $"/Fairflexx Capture/{eventName.ToString()}/{eventName.ToString()}-{DateTime.Now:yyyy.MM.dd)}/{type.ToString()}/");
                return file.Path;
            }
            return LeadConstant.FotoScanPath(eventName, type);
        }

        // Check if the fileService has the same name or not 
        private bool CheckFileNameExists()
        {
            return false;
        }
    }
}
