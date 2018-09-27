using System;
using System.Net;
using System.Net.Mail;

namespace Projekat
{
    public static class EmailClass
    {
        public static void Email(string mailTo, string subject, string body)
        {
            MailMessage message = new MailMessage();
            MailAddress addressTo = new MailAddress(mailTo);
            MailAddress addressFrom = new MailAddress("jelena96sm@gmail.com");
            message.To.Add(addressTo);
            message.From = addressFrom;
            message.Subject = subject;
            message.Body = body;
           
                using (SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587))
            {
                var credential = new NetworkCredential
                {
                    UserName = "jelena96sm@gmail.com",
                    Password = "mimijeja"
                };
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = credential;
                smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtp.EnableSsl = true;
                smtp.Send(message);
            }
           
        }
    }
}