using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Web;

namespace WebchatBuilder.Services
{
    public class EmailServices
    {
        private static readonly string SmtpUsername = ConfigurationManager.AppSettings["SmtpUsername"];
        private static readonly string SmtpPassword = ConfigurationManager.AppSettings["SmtpPassword"];
        private static readonly string SmtpFrom = ConfigurationManager.AppSettings["SmtpFromEmail"];
        private static readonly string SmtpFromName = ConfigurationManager.AppSettings["SmtpFromName"];
        private static readonly string SmtpAddress = ConfigurationManager.AppSettings["SmtpAddress"];
        private static readonly string SmtpPort = ConfigurationManager.AppSettings["SmtpPort"];
        private static readonly string SmtpBcc = ConfigurationManager.AppSettings["SmtpBcc"];
        private static readonly string SmtpSslEnabled = ConfigurationManager.AppSettings["SmtpSslEnabled"];

        public static void SendTranscript(string emailAddress, string subject, string body)
        {
            SendEmail(emailAddress,emailAddress,subject,body,false);
        }


        public static void SendEmail(string emailAddress, string name, string subject, string body, bool isHtml)
        {
            try
            {
                var address = SmtpAddress;
                var from = SmtpFrom;
                var fromName = SmtpFromName;
                var user = SmtpUsername;
                var password = SmtpPassword;
                var bcc = SmtpBcc;
                var sslEnabled = SmtpSslEnabled.ToLower() == "true";

                if (String.IsNullOrWhiteSpace(address) || String.IsNullOrWhiteSpace(from) || String.IsNullOrWhiteSpace(fromName) || String.IsNullOrWhiteSpace(user) || String.IsNullOrWhiteSpace(password))
                {
                    LoggingService.GetInstance().LogNote("Email Failed: Email Settings are not configured.");
                    return;
                }

                var message = new MailMessage(new MailAddress(SmtpFrom, SmtpFromName), new MailAddress(emailAddress, name))
                {
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = isHtml,
                    BodyEncoding = Encoding.UTF8,
                    Bcc = { }
                };

                try
                {
                    if (!String.IsNullOrWhiteSpace(bcc))
                    {
                        message.Bcc.Add(new MailAddress(bcc));
                    }
                }
                catch (Exception)
                {
                }

                int port;
                if (!Int32.TryParse(SmtpPort, out port))
                {
                    port = 587;
                }

                var smtpClient = new SmtpClient(SmtpAddress, port)
                {
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(SmtpUsername, SmtpPassword),
                    EnableSsl = sslEnabled,
                    DeliveryMethod = SmtpDeliveryMethod.Network
                };

                smtpClient.Send(message);
            }
            catch (Exception e)
            {
                LoggingService.GetInstance().LogNote("Email Failed");
                LoggingService.GetInstance().LogException(e);
            }
        }
    }
}