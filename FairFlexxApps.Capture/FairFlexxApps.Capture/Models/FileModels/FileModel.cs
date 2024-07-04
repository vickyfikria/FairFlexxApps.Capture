using System;
using FairFlexxApps.Capture.Enums;

namespace FairFlexxApps.Capture.Models.FileModels
{
    public class FileModel
    {
        public string LocalPath { get; set; }

        public string FileName { get; set; }

        public bool IsSaved { get; set; }

        public LeadType Type { get; set; }

        public DateTime CreateDate { get; set; }
    }
}
