
namespace FairFlexxApps.Capture.Constants
{
    public class SdkKeyConstants
    {
        #region AppCenter

        public static string AppCenterAndroid = "5c988849-5f45-4082-b6c1-a153dc66a2e9";
        public static string AppCenteriOS = "512126ca-37ef-40e2-a5e4-55bdf1c85465";

        #endregion

        #region Scanbot

        public const string Scanbot = "DGLNV6cAzMWOb1wf4skaYglJ0w6wnb" +
                                      "0eb+6eNPx+g5WWvwsKHYe33pz3Hwoz" +
                                      "VLCadzARhv3mWt2ABKxD8nOxJoKuHR" +
                                      "vVLFsJ2XbH+ebB77svQxVh0pfg4L9j" +
                                      "AO/rEyScEqQTf44h/g1O+V7awHkU+e" +
                                      "QPe0J4Ocwd3I804bPAWpPesrXTQD6E" +
                                      "sGf92/emTiiGwat4U7OpRJWUddTrg3" +
                                      "DoXsWfHFo6fXCwYCUObw6h5IOCtZTt" +
                                      "sfFXpzL+yVFVyI3AKnv9lBvvTqOQZh" +
                                      "zbh1lT9tHCNZFPudT9lyQ4rcxFRyNs" +
                                      "ZVH411WgtR2qeX0sWJXwjmuMgM/6AF" +
                                      "c/rp0CMQGgPg==\nU2NhbmJvdFNESw" +
                                      "pjb20uZmFpcmZsZXh4LmNhcHR1cmUK" +
                                      "MTU4MzI3OTk5OQo1OTAKMw==\n";

        //public const string Scanbot =
        //    "M7CNrUfffRpCb39/aUfVOXAB9VPyHa" +
        //      "P/IWRwglNa9ElxY70F+kBmByoWlEHu" +
        //      "UmZLPjsi51tWvmW8t1kZ0ACgS2sYnN" +
        //      "FaOSfMG4cwZRL24+rUP6APWy3AugbR" +
        //      "YIKEIDkbBn5BNZyImPRvRxHIrTrG4J" +
        //      "PW+5lIJ+Ydh6IUb814Lz6BXCA7OGry" +
        //      "T7WaMIiPICH8Gh2m6M8Dit9s8Tn/1W" +
        //      "sSoShNt0jUyR8XH7dbTpqJsFxkgaXM" +
        //      "hr/gHrUhQGPbohtlpad20pxyGUiiD1" +
        //      "DNvYNEXwAfcQUjIOtkaO9348ODoj2g" +
        //      "/47Mn7QAJ89RIDCyiZPxuZiCZXEnLr" +
        //      "CnRB49iqW7bA==\nU2NhbmJvdFNESw" +
        //      "pjb20uZmFpcmZsZXh4LmNhcHR1cmUK" +
        //      "MTU4MzI3OTk5OQo1OTAKMQ==\n";

        public static int AcceptedAngleScore = 60;
        public static int AcceptedSizeScore = 70;
        public static double AutoSnappingSensitivity = 0.8f;
        public static double ImageScale = 0.8f;
        public static int PolygonLineWidth = 10;

        #endregion

        #region Regex emal
        
        public const string EmailRegex = @"^(?("")("".+?(?<!\\)""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" +
                                  @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-\w]*[0-9a-z]*\.)+[a-z0-9][\-a-z0-9]{0,22}[a-z0-9]))$";

        #endregion

        // Navigation Service

    }
}
