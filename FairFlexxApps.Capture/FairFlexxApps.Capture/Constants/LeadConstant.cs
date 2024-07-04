using FairFlexxApps.Capture.Enums;
using FairFlexxApps.Capture.Interfaces;
using System;
using Microsoft.Maui;

namespace FairFlexxApps.Capture.Constants
{
    public class LeadConstant
    {
        private static readonly string FilePath = DependencyService.Get<IFileService>().FilePath;
        //public static string FilePathForm = $"{FilePath}/FotoScan/Form/";

        //public static string FilePathBusinessCard = $"{FilePath}/FotoScan/BusinessCard/";

        //public static string FilePathAttachment = $"{FilePath}/FotoScan/Attachment/";

        //public static string FilePathObject = $"{FilePath}/FotoScan/Object/";


        public static string FotoScanPath(string eventName, LeadType type)
        {

            return $"{FilePath}/Fairflexx Capture/{eventName.ToString()}/{eventName.ToString()}-{DateTime.Now.ToString("yyyy.MM.dd")}/{type.ToString()}/";
        }
    }
}
