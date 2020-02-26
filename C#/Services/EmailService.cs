/*
Service to send emails via SmtpClient
*/
using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace EmailService
{
    public class EmailService
    {
        private readonly MailAddress _sender;
        private readonly string _subjectPrefix;
        private readonly string _logoUrl;
        private readonly string _clientHost;
        private readonly int _clientPort;
        private readonly NetworkCredential _clientCredentials;
        private readonly string _overrideRecipient;

        public EmailService(string senderAddress, string senderDisplayName, string subjectPrefix, string logoUrl)
        {
            _sender = new MailAddress(senderAddress, senderDisplayName);
            _subjectPrefix = subjectPrefix;
            _logoUrl = logoUrl;

            _clientHost = System.Configuration.ConfigurationManager.AppSettings["SmtpHost"];
            _clientPort = int.Parse(System.Configuration.ConfigurationManager.AppSettings["SmtpPort"]);
            var username = System.Configuration.ConfigurationManager.AppSettings["SmtpUsername"];
            var password = System.Configuration.ConfigurationManager.AppSettings["SmtpPassword"];
            _clientCredentials = new NetworkCredential(username, password);

            _overrideRecipient = System.Configuration.ConfigurationManager.AppSettings["SmtpOverrideRecipient"];
        }

        public async ValueTask<bool> SendEmailAsync(string recipient, string subject, string bodyWithLayout)
        {
            const int timeout = 10 * 1000;

            var emailSendTask = Task.Run(async () =>
            {
                try
                {
                    using (var msg = new MailMessage())
                    {
                        msg.From = _sender;
                        msg.To.Add(string.IsNullOrWhiteSpace(_overrideRecipient) ? recipient : _overrideRecipient);
                        msg.Subject = _subjectPrefix + subject;
                        msg.IsBodyHtml = true;
                        msg.Body = bodyWithLayout;

                        using (var smtpClient = new SmtpClient(_clientHost, _clientPort))
                        {
                            smtpClient.UseDefaultCredentials = false;
                            smtpClient.Credentials = _clientCredentials;
                            await smtpClient.SendMailAsync(msg);
                        }
                    }

                    return true;
                }
                catch (Exception ex)
                {
                    //todo: Log exception
                    return false;
                }
            });

            if (await Task.WhenAny(emailSendTask, Task.Delay(timeout)) == emailSendTask)
            {
                return emailSendTask.Result;
            }
            else
            {
                //todo: Log exception
                return false;
            }
        }

    }
}
