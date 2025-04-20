using FluentEmail.Core;
using FluentEmail.Core.Models;
using FluentEmail.Smtp;
using SBERP.EmailService.Models;
using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Hosting;

namespace SBERP.EmailService.Service
{
    public class EmailSender : IEmailService
    {
        private readonly IHostEnvironment _environment;
        public EmailSender(IHostEnvironment environment)
        {
            _environment = environment;
        }

        // smtp.mail.yahoo.com	25, 587	TLS
        // smtp.mail.yahoo.com	465	SSL

        //////"EmailConfiguration": {
        //////"From": "sreemonta.bhowmik@yahoo.com",
        //////"Host": "localhost",
        //////"Port": 25,
        //////"Username": "sreemonta.bhowmik",
        //////"Password": "xasutzrityoeggzq"
        //////}
        public async Task<SendResponse> TestSendEmailAsync(EmailConfiguration emailConfig, Message message)
        {
            SmtpSender sender = new(() => new SmtpClient(host: emailConfig.Host)
            {
                EnableSsl = false,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                Port = 25,
            });
            Email.DefaultSender = sender;

            SendResponse email = await Email
                .From(emailAddress: emailConfig.From)
                .To(emailAddress: message.To, name: message.Name)
                .Subject(subject: message.Subject)
                .Body(body: message.Body)
                .SendAsync();

            return email;
        }

        public async Task<SendResponse> SendEmailAsync(EmailConfiguration emailConfig, Message message)
        {
            SmtpSender sender = new(() => new SmtpClient(emailConfig.Host)
            {
                UseDefaultCredentials = false,
                Port = 587,
                Credentials = new NetworkCredential(emailConfig.UserName, emailConfig.Password),
                EnableSsl = true,
            });

            Email.DefaultSender = sender;
            IFluentEmail email = Email
                .From(emailConfig.From, emailConfig.Name)
                .To(message.To, message.Name)
                .Subject(message.Subject)
                .Body(message.Body, true);


            SendResponse response = await email.SendAsync();
            return response;
        }

        public string PopulateBody(string path, string userName, string title, string url, string description)
        {
            string body = string.Empty;
            string combinePath = Path.Combine(_environment.ContentRootPath, "EmailTemplates/emailblocknotice.html");
            using (StreamReader reader = new(combinePath))
            {
                body = reader.ReadToEnd();
            }
            //var html = System.IO.File.ReadAllText(@"~/EmailTemplates/emailblocknotice.htm");
            body = body.Replace("{Name}", userName);
            body = body.Replace("{Title}", title);
            body = body.Replace("{Url}", url);
            body = body.Replace("{Description}", description);
            return body;
        }


    }
}
