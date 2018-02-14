using System;
using System.Collections.Generic;
using Renci.SshNet;
using Renci.SshNet.Sftp;
using System.IO;
using System.Configuration;
using System.Linq;
using Utilities;

namespace SFTPDownload
{
    /// <summary>
    /// Created by Ramachandran Narayanan
    /// This class includes the core SFTP functionality like connecting and downloading files
    /// The below was written usign the classes referred from https://ourcodeworld.com/articles/read/369/how-to-access-a-sftp-server-using-ssh-net-sync-and-async-with-c-in-winforms for the DownloadDirectory and DownloadFile alone
    /// </summary>
    public class SFTPClient
    {
        private static SftpClient _client;
        private static string _downloadPath;
        
        /// <summary>
        /// Created by Ramachandran Narayanan
        /// This method initializes a static sftp client along with the host, username and password
        /// </summary>
        public static void Initialize()
        {
            string host = Constants.Host.ToString();
            string username = Constants.UserName.ToString();
            string password = Constants.Password.ToString();
            var methods = new List<AuthenticationMethod>();
            methods.Add(new PasswordAuthenticationMethod(username, password));
            ConnectionInfo con = new ConnectionInfo(host, 22, username, methods.ToArray());
            _client = new SftpClient(con);
        }

        /// <summary>
        /// Created by Ramachandran Narayanan
        /// This method is used to create a new directory in DD-MM-YYYY format 
        /// If folder already exists then new directory is not created
        /// </summary>
        public static void CreateDirectory()
        {
            _downloadPath = Path.Combine(ConfigurationManager.AppSettings["SharedDownloadBox"].ToString(),DateTime.Now.ToString("dd-MM-yyyy"));
            if(!Directory.Exists(_downloadPath))
            {
                Directory.CreateDirectory(_downloadPath);
            }
        }

        /// <summary>
        /// Created by OUR CODE WORLD
        /// This method is used to download a single file
        /// </summary>
        /// <param name="_localPath"></param>
        /// <param name="_destination"></param>
        public static void DownloadFile(string _localPath, string _destination)
        {
            _client.Connect();
            Console.WriteLine("Downloading a single file {0}", _localPath);
            using (Stream _fileStream = File.OpenWrite(_destination))
            {
                _client.DownloadFile(_localPath, _fileStream);
            }
            _client.Disconnect();
        }

        /// <summary>
        /// Created by OUR CODE WORLD
        /// This method is used to download the files in batches including the directories and files with in
        /// ** RECURSIVE METHOD **
        /// </summary>
        /// <param name="source"></param>
        /// <param name="destination"></param>
        /// <param name="recursive"></param>
        public static void DownloadDirectory(string source, string destination, bool recursive = false)
        {
            try
            {
                Logger.LogMessage("Directory download initiated", false);
                Initialize();
                Logger.LogMessage("Connecting to SFTP client", false);
                if (!_client.IsConnected)
                {
                    _client.Connect();
                }
                Logger.LogMessage("SFTP Client Connected", false);
                // List the files and folders of the directory
                var files = _client.ListDirectory(source);

                // Iterate over them
                foreach (SftpFile file in files)
                {
                    // If is a file, download it
                    if (!file.IsDirectory && !file.IsSymbolicLink)
                    {
                        DownloadFile(_client, file, destination);
                    }
                    // If it's a symbolic link, ignore it
                    else if (file.IsSymbolicLink)
                    {
                        Console.WriteLine("Symbolic link ignored: {0}", file.FullName);
                    }
                    // If its a directory, create it locally (and ignore the .. and .=) 
                    //. is the current folder
                    //.. is the folder above the current folder -the folder that contains the current folder.
                    else if (file.Name != "." && file.Name != "..")
                    {
                        var dir = Directory.CreateDirectory(Path.Combine(destination, file.Name));
                        // and start downloading it's content recursively :) in case it's required
                        if (recursive)
                        {
                            DownloadDirectory(file.FullName, dir.FullName);
                        }
                    }
                }
                GetFileCountByRegion();
            }
            catch(Exception ex)
            {
                Logger.LogException(ex);
            }
        }

        /// <summary>
        /// Created by OUR CODE WORLD
        /// This method is a sub module of the DownloadDirectory method where the current file information is passed and downloaded
        /// </summary>
        /// <param name="client"></param>
        /// <param name="file"></param>
        /// <param name="directory"></param>
        public static void DownloadFile(SftpClient client, SftpFile file, string directory)
        {
            //Console.WriteLine("Downloading {0}", file.FullName);
            try
            {
                Logger.LogMessage("Downloading file " + file.FullName.ToString(), false);
                //Check for a particular file and then rename the old - copy the new
                if (file.Name.ToString().Equals("File name goes here"))
                {
                    CheckIfFileExists(client, file, directory);
                }
                using (Stream fileStream = File.OpenWrite(Path.Combine(directory, file.Name)))
                {
                    client.DownloadFile(file.FullName, fileStream);
                }
            }
            catch(Exception ex)
            {
                Logger.LogException(ex);
            }
        }


        /// <summary>
        /// Created by Ramachandran Narayanan
        /// This method is used to check if the file exists and rename the particular file
        /// </summary>
        /// <param name="_client"></param>
        /// <param name="_file"></param>
        /// <param name="_directory"></param>
        private static void CheckIfFileExists(SftpClient _client,SftpFile _file, string _directory)
        {
            try
            {
                Logger.LogMessage("Checking if file exists", false);
                string fileName = Path.GetFileNameWithoutExtension(_file.Name).ToString();
                string extension = Path.GetExtension(_file.Name).ToString();
                string fullpath = Path.Combine(_directory + "\\" + fileName + extension);
                int count = GetFileCount(_directory, fileName);
                if (File.Exists(fullpath))
                {
                    fileName = fileName + "_" + count;
                    Logger.LogMessage("Renaming the " + fileName, false);
                    File.Move(fullpath, Path.Combine(_directory + "\\" + fileName + extension));
                }
            }
            catch(Exception ex)
            {
                Logger.LogException(ex);
            }
        }


        /// <summary>
        /// Created by Ramachandran Narayanan
        /// This method is used to get the count of the files
        /// </summary>
        /// <param name="_directory"></param>
        /// <param name="_filename"></param>
        /// <returns></returns>
        private static int GetFileCount(string _directory, string _filename)
        {
            _directory = _directory + "\\";
            var fileCount = (from file in Directory.EnumerateFiles(_directory, _filename+"*.txt", SearchOption.AllDirectories)
                             select file).Count();
            return fileCount;
        }

        /// <summary>
        /// Created by Ramachandran Narayanan
        /// This method is used to get the count of the files by region wise
        /// </summary>
        private static void GetFileCountByRegion()
        {
            string[] _region = new string[] { "Your region array goes here" };
            int count = 0;
            foreach(string _reg in _region)
            {
                count = GetFileCount(_downloadPath, _reg);
                Logger.LogMessage("Files for " + _reg + " is :" + count.ToString(), false);
            }
        }
    }
}
