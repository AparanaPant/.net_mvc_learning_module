using Microsoft.AspNetCore.Identity.UI.Services;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace GraceProject.Services
{
    public class EmailSender : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            // Configure the SMTP client for Auburn University Mail Relay
            var smtpClient = new SmtpClient("mailrelay.auburn.edu")
            {
                Port = 25, // Ensure this is the correct port for Auburn's mail server
                EnableSsl = false, // Auburn's relay may not support SSL
                Credentials = CredentialCache.DefaultNetworkCredentials // Uses default network authentication if required
            };

            // Configure the email message
            var mailMessage = new MailMessage
            {
                From = new MailAddress("grace@auburn.edu"), // Your sender email
                Subject = subject,
                Body = htmlMessage,
                IsBodyHtml = true,
            };
            mailMessage.To.Add(email); // Add recipient

            // Send the email asynchronously
            return smtpClient.SendMailAsync(mailMessage);
        }
    }
}


//using Microsoft.AspNetCore.Identity.UI.Services;
//using System;
//using System.Net.Mail;
//using System.Threading.Tasks;

//namespace GraceProject.Services
//{
//    public class EmailSender : IEmailSender
//    {
//        private readonly string _smtpServer = "mailrelay.auburn.edu";
//        private readonly int _port = 25; // Ensure the correct port
//        private readonly string _senderEmail = "grace@auburn.edu";

//        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
//        {
//            using SmtpClient smtpClient = new SmtpClient(_smtpServer)
//            {
//                Port = _port,
//                EnableSsl = false // Change to true if required
//            };

//            MailMessage message = new MailMessage
//            {
//                From = new MailAddress(_senderEmail),
//                Subject = subject,
//                Body = htmlMessage,
//                IsBodyHtml = true
//            };
//            message.To.Add(new MailAddress(email));

//            try
//            {
//                await smtpClient.SendMailAsync(message);
//            }
//            catch (Exception ex)
//            {
//                Console.WriteLine($"Email sending failed: {ex.Message}");
//            }
//        }
//    }
//}


