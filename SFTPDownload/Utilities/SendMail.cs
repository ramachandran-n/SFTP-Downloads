using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Utilities
{
    /// <summary>
    /// Created by Ramachandran Narayanan
    /// This class is used to trigger a mail for the status of the job
    /// </summary>
    public sealed class SendMail
    {
        /// <summary>
        /// Created by Ramachandran Narayanan
        /// This method is triggered to send the job status mail
        /// </summary>
        /// <param name="mailFrom"></param>
        /// <param name="mailTo"></param>
        /// <param name="mailCC"></param>
        /// <param name="mailSubject"></param>
        /// <param name="mailBody"></param>
        public static void SendJobStatus(string mailFrom, string mailTo,string mailCC, string mailSubject, string mailBody)
        {
            using(MailMessage _mailMessage = new MailMessage())
            {
                //Add the To address to the mail
                MailAddress _mailFromAddress = new MailAddress(mailFrom);
                _mailMessage.From = _mailFromAddress;
                //Loop through the CC mail address and add it to the mail
                foreach (var address in mailCC.Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries))
                {
                    _mailMessage.CC.Add(address);
                }
                _mailMessage.Subject = "Job Status - " + DateTime.Now.ToString("dd-MMM-yyyy");
                _mailMessage.Body = "Job is completed ";

                SmtpClient _smtpClient = new SmtpClient(ConfigurationManager.AppSettings["RelayServer"].ToString());
                _smtpClient.Port = 25;
                _smtpClient.SendAsync(_mailMessage,null);
            }
        }
    }
}
