using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Utilities
{
    public static class Mailer
    {
        static string[] _region = new string[] { "your region array goes here" };

        /// <summary>
        /// Created by Ramachandran Narayanan
        /// This method is used to send the status update
        /// </summary>
        public static void SendStatus()
        {
            MailMessage _mailMessage = new MailMessage();

            MailAddress _from = new MailAddress(ConfigurationManager.AppSettings["FromAddress"].ToString());
            _mailMessage.From = _from;
            _mailMessage.To.Add(ConfigurationManager.AppSettings["ToAddress"]);
            _mailMessage.Subject = "Daily Monitoring - " + DateTime.Now.ToString("dd-MMM-yyyy");
            _mailMessage.Body = GetHtmlFrame(GetMessageContent());
            _mailMessage.IsBodyHtml = true;
            SmtpClient _smtp = new SmtpClient(ConfigurationManager.AppSettings["RelayServer"]);
            _smtp.UseDefaultCredentials = true;
            _smtp.Port = Int32.Parse(ConfigurationManager.AppSettings["RelayPort"]);
            _smtp.Send(_mailMessage);
        }

        /// <summary>
        /// Created by Ramachandran Narayanan
        /// This method is used to get the count list of file names
        /// </summary>
        /// <param name="region"></param>
        /// <returns></returns>
        private static List<string> GetCount(string region)
        {
            string _downloadPath = Path.Combine(ConfigurationManager.AppSettings["SharedDownloadBox"].ToString(), DateTime.Now.ToString("dd-MM-yyyy"));
            DirectoryInfo _directoryInfo = new DirectoryInfo(_downloadPath);
            List<string> files = _directoryInfo.GetFiles(region + "*.txt")
                                        .Select(file => file.Name).ToList();
            return files;
        }

        /// <summary>
        /// Created by Ramachandran Narayanan
        /// This method is used to form the message content of the mail
        /// </summary>
        /// <returns></returns>
        private static StringBuilder GetMessageContent()
        {
            StringBuilder _sbuilder = new StringBuilder();

            _sbuilder.Append("Hello,");
            _sbuilder.Append("<p><table border=1>");
            _sbuilder.Append("<p><thead><tr><th>Country ID</th><th>Count of Files</th><th>Files</th></tr></thead>");
            _sbuilder.Append("<tbody>");
            foreach(string _reg in _region)
            {
                _sbuilder.Append("<tr>");
                var _fileList = GetCount(_reg);
                _sbuilder.Append("<td rowspan=" + (_fileList.Count + 1) +">" + _reg + "</td>");
                _sbuilder.Append("<td rowspan=" + (_fileList.Count + 1) + ">" + _fileList.Count + "</td>");
                if (_fileList!=null && _fileList.Count > 0)
                {
                    foreach (string _file in _fileList)
                    {
                        _sbuilder.Append("<tr><td> " + _file + "</td></tr>");
                    }
                }
                else
                {
                    _sbuilder.Append("<td> 0 Files</td>");
                }
                _sbuilder.Append("</tr>");
            }
            _sbuilder.Append("</tbody></table>");
            return _sbuilder;
        }


        /// <summary>
        /// Created by Ramachandran Narayanan
        /// This forms as a base of the HTML document generation
        /// </summary>
        /// <param name="_mainContent"></param>
        /// <returns></returns>
        private static string GetHtmlFrame(StringBuilder _mainContent)
        {
            string strHtmlFrame;
            strHtmlFrame = "<html>" +
                              "<head>" +
                                 "<style>table, th, td { border: 1px solid black;}"+
                                 "</style></head>"+
                               "<body>" +
                                "<div>" +
                                    "";

            strHtmlFrame = strHtmlFrame + _mainContent;
            strHtmlFrame = strHtmlFrame + "</div>" + "</body>" + "</html>";
            return strHtmlFrame;
        }
    }
}
