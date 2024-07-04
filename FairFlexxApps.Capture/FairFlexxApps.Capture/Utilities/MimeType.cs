using System;
using FairFlexxApps.Capture.Constants;

namespace FairFlexxApps.Capture.Utilities
{
    public static class MimeType
    {
        #region GetMimeType

        public static string GetMimeType(string extension)
        {
            if (extension == null)
            {
                throw new ArgumentNullException("extension");
            }

            if (!extension.StartsWith("."))
            {
                extension = "." + extension;
            }

            return Mimetypes.MimeTypes.TryGetValue(extension, out var mime) ? mime : "application/octet-stream";
        }

        #endregion
    }
}
