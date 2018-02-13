using System;
using System.Configuration;
using System.IO;

namespace Utilities
{
    /// <summary>
    /// Created by Ramachandran Narayanan
    /// This class is used to log the events in a log file
    /// </summary>
    public sealed class Logger
    {
        /// <summary>
        /// Created by Ramachandran Narayanan
        /// This class is used to log the exceptions
        /// </summary>
        /// <param name="ex"></param>
        public static void LogException(Exception ex)
        {
            try
            {
                StreamWriter strWriter;
                using (strWriter = File.AppendText(ConfigurationManager.AppSettings["LogLocation"].ToString() + ".log"))
                {
                    strWriter.NewLine = "\r\n";
                    strWriter.WriteLine("__________________________________________________________________________________________________________");
                    strWriter.WriteLine(ex.Message);
                    strWriter.WriteLine(ex.StackTrace);
                    strWriter.WriteLine("__________________________________________________________________________________________________________");
                    strWriter.Close();
                }
            }
            catch (Exception exception)
            {
                //Do nothing
            }
        }

        /// <summary>
        /// Created by Ramachandran Narayanan
        /// This class is used to log messages
        /// </summary>
        /// <param name="message"></param>
        /// <param name="startLine"></param>
        public static void LogMessage(string message, bool startLine)
        {
            StreamWriter strWriter;
            try
            {
                using (strWriter = File.AppendText(ConfigurationManager.AppSettings["LogLocation"].ToString() + ".log"))
                {
                    strWriter.NewLine = "\r\n";
                    if (startLine)
                        strWriter.WriteLine("*********************************************************************************************************");
                    strWriter.WriteLine(DateTime.Now.ToString() + ": " + message);
                    strWriter.Close();
                }
            }
            catch (Exception ex)
            {
                //Do nothing
            }
        }
    }
}
