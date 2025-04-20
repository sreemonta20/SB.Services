using Microsoft.AspNetCore.Http;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SBERP.EmailService.Models
{
    /// <summary>
    /// It defines the message, and it's whole content
    /// </summary>
    public class Message
    {
        public string? To { get; set; }
        public string? Name { get; set; }
        public string? Subject { get; set; }
        public string? Body { get; set; }

    }
}
