using System.Net.Mail;
using System.Net;

namespace B2b.Plugin.Mail
{
    public static class MailPlugin
    {
        public static bool MailPost(string host, int port, string email, string password, bool ssl, string receiverEmail, string subject, string body)
        {
            try
            {
                SmtpClient smtpClient = new(host)
                {
                    Port = port,
                    Credentials = new NetworkCredential(email, password),
                    EnableSsl = ssl
                };
                MailMessage mailMessage = new()
                {
                    From = new MailAddress(email),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true 
                };
                mailMessage.To.Add(receiverEmail);
                smtpClient.Send(mailMessage);
                return true;
            }
            catch
            {
               return false;
            }
        }
    }
}
