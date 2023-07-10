﻿using FileWatchInformer.Options;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Security;
using System.Reflection;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FileWatchInformer.Services
{
    internal class EmailSenderService
    {
        private readonly EmailConfig _emailConfig;
        private readonly ILogger<EmailSenderService> _logger;

        public EmailSenderService(
            IOptionsSnapshot<EmailConfig> emailConfig,
            ILogger<EmailSenderService> logger)
        {
            _emailConfig = emailConfig.Value;
            _logger = logger;
        }
    }
}