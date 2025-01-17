﻿using System;
using System.Diagnostics;
using System.IO;
using FairFlexxApps.Capture.Droid.Services.SQLiteService;
using FairFlexxApps.Capture.Interfaces.LocalDatabase;
using SQLite;

[assembly: Dependency(typeof(DatabaseConnection))]
namespace FairFlexxApps.Capture.Droid.Services.SQLiteService
{
    public class DatabaseConnection : IDatabaseConnection
    {
        string GetPath(string fileName)
        {
            var path = Path.Combine(GetDatabasePath(), fileName);

            if (!File.Exists(path)) File.Create(path);

            return path;
        }

        public string GetDatabasePath()
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.Personal);
        }

        public SQLiteConnection SqliteConnection(string databaseName)
        {
            return new SQLiteConnection(GetPath(databaseName));
        }

        public long GetSize(string databaseName)
        {
            var fileInfo = new FileInfo(GetPath(databaseName));
            return fileInfo.Length;
        }

        public string SaveFile(byte[] bytes)
        {
            try
            {

                var dir = new Java.IO.File(GetDatabasePath() + "/Images");
                dir.Mkdirs();

                var file = new Java.IO.File(dir, $"com.Fairflexx_{DateTime.Now.Ticks}.png");

                if (!file.Exists())
                {
                    file.CreateNewFile();
                    file.Mkdir();

                    File.WriteAllBytes(file.Path, bytes);

                    return file.Path;
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }

            return null;
        }
    }
}
