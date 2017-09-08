using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace RatulCore.Utility.Email
{
    public class EmailSender
    {
        private SmtpClient _smtpClient;
        public delegate void SendCompletedCallback(object sender, AsyncCompletedEventArgs completedEvent);

        public EmailSender(EmailSettings settings)
        {
            _smtpClient = new SmtpClient();
            _smtpClient.Host = settings.Host;
            _smtpClient.Port = settings.Port;
            _smtpClient.EnableSsl = settings.EnableSsl;
            _smtpClient.UseDefaultCredentials = false;
            _smtpClient.Credentials = new NetworkCredential(settings.UserName, settings.Password);
            _smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
        }

        private MailMessage GetMailMessage(MessageSettings settings)
        {
            MailMessage message = new MailMessage();
            settings.SetMailAddressCollectionForToList(message.To);
            message.From = new MailAddress(settings.From.EmailAddress, settings.From.Name);
            settings.SetMailAddressCollectionForReplyToList(message.ReplyToList);
            message.Subject = settings.Subject;
            message.Body = settings.Body;
            message.IsBodyHtml = settings.IsBodyHtml;
            message.Priority = MailPriority.High;
            return message;
        }

        

        public void Send(MessageSettings settings)
        {
            MailMessage message = null;
            try
            {
                message = this.GetMailMessage(settings);
                _smtpClient.Send(message);
            }
            catch (Exception) 
            {
                throw;
            }
            finally
            {
                message.Dispose();
                _smtpClient.Dispose();
            }
        }
        public void SendAsync(MessageSettings settings)
        {
            try
            {
                MailMessage message = this.GetMailMessage(settings);
                _smtpClient.SendAsync(this.GetMailMessage(settings), new Object());
                _smtpClient.SendCompleted += (sender, completedEvent) =>
                {
                    _smtpClient.Dispose();
                    message.Dispose();
                };
            }
            catch (Exception)
            {
                throw;
            }
        }
        public void SendAsync(MessageSettings settings, SendCompletedCallback callback)
        {
            try
            {
                MailMessage message = this.GetMailMessage(settings);
                _smtpClient.SendAsync(this.GetMailMessage(settings), new Object());
                _smtpClient.SendCompleted += (sender, completedEvent) =>
                {
                    callback(sender, completedEvent);
                    _smtpClient.Dispose();
                    message.Dispose();
                };
            }
            catch (Exception)
            {
                throw;
            }
        }
        public void SendAsyncCancel()
        {
            _smtpClient.SendAsyncCancel();
        }
    }
}
