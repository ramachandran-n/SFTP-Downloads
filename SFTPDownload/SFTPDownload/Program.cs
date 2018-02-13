using System;
using System.Configuration;
using System.IO;
using Utilities;

namespace SFTPDownload
{
    /// <summary>
    /// Created by Ramachandran Narayanan
    /// Main Calling method of program
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            Logger.LogMessage("Utility Started", true);
            //Get the inbound interface path from where the files needs to be fetched
            string interfacePath = ConfigurationManager.AppSettings["InterfacePath"].ToString();
            //Get the destination path where the files needs to be placed
            string pathSharedDirectory = Path.Combine(ConfigurationManager.AppSettings["SharedDownloadBox"].ToString(), DateTime.Now.ToString("dd-MM-yyyy"));
            //Get the destination path where the single file needs to be placed
            string pathSharedFile = ConfigurationManager.AppSettings["SharedDownloadBox"].ToString();

            Logger.LogMessage("Checking and creating directory", false);
            //Create a new directory for daily updates. If directory exists then new directory will not be created.
            SFTPClient.CreateDirectory();
            Logger.LogMessage("Directory check completed", false);

            //If the Type is File then a single file download is triggered
            //else a batch download is triggered which includes all the files with directories as well
            if (ConfigurationManager.AppSettings["Type"].Equals("File"))
            {
                SFTPClient.DownloadFile(interfacePath, pathSharedFile);
            }
            else
            {
                SFTPClient.DownloadDirectory(interfacePath, pathSharedDirectory, true);
            }
            Mailer.SendStatus();
            Logger.LogMessage("Utility completed", false);
        }

       
    }
}
