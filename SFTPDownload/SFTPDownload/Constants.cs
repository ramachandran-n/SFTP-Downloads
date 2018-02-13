using System.Configuration;

namespace SFTPDownload
{
    /// <summary>
    /// Created by Ramachandran Narayanan
    /// This class consists of the constants and the cryptography activites for connecting to the SFTP site
    /// </summary>
    public static class Constants
    {
        //The key
        public static string passKey = "Hash password goes here";
        public static string Host 
        {
            get { return GetHostName(); } 
        }

        public static string UserName 
        {
            get { return GetUserName(); }
        }

        public static string Password 
        {
            get { return GetPassword(); }
        }

        private static string GetHostName()
        {
            return Utilities.Cryptography.DecryptText(ConfigurationManager.AppSettings["Host"].ToString(), passKey);
        }

        private static string GetUserName()
        {
            return Utilities.Cryptography.DecryptText(ConfigurationManager.AppSettings["Username"].ToString(),passKey);
        }

        private static string GetPassword()
        {
            return Utilities.Cryptography.DecryptText(ConfigurationManager.AppSettings["Password"].ToString(), passKey);
        }
    }
}
