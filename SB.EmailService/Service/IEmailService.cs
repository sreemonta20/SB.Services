using FluentEmail.Core.Models;
using SB.EmailService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SB.EmailService.Service
{
    public interface IEmailService
    {
        Task<SendResponse> TestSendEmailAsync(EmailConfiguration emailConfig, Message message);
        Task<SendResponse> SendEmailAsync(EmailConfiguration emailConfig, Message message);
        public string PopulateBody(string path, string userName, string title, string url, string description);
    }
}
