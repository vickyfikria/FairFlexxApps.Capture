
namespace FairFlexxApps.Capture.Services.HttpService
{
    public static class ApiUrl
    {
        //public static string HttpUrl { get; set; } = CrossSecureStorage.Current.GetValue("HttpUrl");

        public static string Link(string link)
        {
            return $"{App.Settings.HttpUrl}api/{link}";
        }

        #region API URL

        public static string UserLogin()
        {
            return Link("user/login/");
        }

        public static string EventGetByClientId()
        {
            //return Link("event/GetByClientId/");

            return Link("event/GetByClientIdV1/");
        }

        public static string TestServer()
        {
            //return Link("file/Test");
            return Link("filefastupload/test");
        }

        public static string UploadFile()
        {
            return Link("file/Upload");
        }

        /*"http://test-fastupload.fairflexx.net/api/filefastupload/testdb"*/
        public static string FastUploadFile()
        {
            return Link("filefastupload/upload");
        }

        public static string Test()
        {
            return Link("file/test");
        }

        public static string GetClientLogo(int clientId)
        {
            return Link("user/GetClientLogo/" + clientId);
        }

        #endregion
    }
}
