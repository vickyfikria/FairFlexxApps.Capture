using System.Collections.Generic;
using System.IO;

namespace FairFlexxApps.Capture.Utilities
{
    public class FileChunkRepository
    {
        private readonly int _chunkSize;
        private readonly string _fullFileName;

        public FileChunkRepository(int chunkSizeInKiloBytes, string fullFileName)
        {
            _chunkSize = chunkSizeInKiloBytes * 1024;
            _fullFileName = fullFileName;
        }

        public Dictionary<int, byte[]> GetChunks()
        {
            var chunks = new Dictionary<int, byte[]>();
            var bufferSize = _chunkSize;

            //using (var fileData = File.OpenRead(_fullFileName))
            using (var fileData = File.OpenRead(_fullFileName))
            {
                var index = 0;
                var i = 1;

                while (fileData.Position < fileData.Length)
                {
                    byte[] buffer;
                    if (index + bufferSize > fileData.Length)
                    {
                        buffer = new byte[(int)fileData.Length - index];
                        fileData.Read(buffer, 0, ((int)fileData.Length - index));
                    }
                    else
                    {
                        buffer = new byte[bufferSize];
                        fileData.Read(buffer, 0, bufferSize);
                    }

                    chunks.Add(i, buffer);
                    index = (int)fileData.Position;
                    i++;
                }
            }

            return chunks;
        }
    }
}