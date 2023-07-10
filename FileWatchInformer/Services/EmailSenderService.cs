using FileWatchInformer.Options;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;

namespace FileWatchInformer.Services
{
	internal class EmailSenderService : IDisposable, IAsyncDisposable
    {
        private readonly EmailConfig _emailConfig;
		private readonly SmtpClient _currentSmtpClient;
		private readonly ILogger<EmailSenderService> _logger;
		private bool isDisposed;

		private async Task<SmtpClient> PrepareEmailClientAsync(CancellationToken cancellationToken)
		{
			if (!_currentSmtpClient.IsConnected)
			{
				await _currentSmtpClient.ConnectAsync(_emailConfig.Server, _emailConfig.Port.Value, Enum.Parse<SecureSocketOptions>(_emailConfig.Security, true), cancellationToken);
				await _currentSmtpClient.AuthenticateAsync(_emailConfig.Login, _emailConfig.Password, cancellationToken);
			}

			return _currentSmtpClient;
		}

		private void Dispose(bool disposing)
		{
			if (!isDisposed)
			{
				if (disposing)
				{
					if (_currentSmtpClient.IsConnected)
					{
						_currentSmtpClient.Disconnect(true);
					}

					_currentSmtpClient.Dispose();
				}

				isDisposed = true;
			}
		}

		private async ValueTask DisposeAsyncCore(bool disposing)
		{
			if (!isDisposed)
			{
				if (disposing)
				{
					if (_currentSmtpClient.IsConnected)
					{
						await _currentSmtpClient.DisconnectAsync(true);
					}

					_currentSmtpClient.Dispose();
				}

				isDisposed = true;
			}
		}

		public EmailSenderService(
			IOptionsSnapshot<EmailConfig> emailConfig,
            ILogger<EmailSenderService> logger)
        {
            _emailConfig = emailConfig.Value;
            _logger = logger;

			_currentSmtpClient = new SmtpClient();
			if (!_emailConfig.ValidateCertificate)
			{
				_currentSmtpClient.ServerCertificateValidationCallback = (_, __, ___, ____) => true;
			}
		}

		public async Task SendEmailAsync(string address, string subject, string text, CancellationToken cancellationToken = default)
		{
			await PrepareEmailClientAsync(cancellationToken);

			using var message = new MimeMessage();
			message.From.Add(MailboxAddress.Parse(_emailConfig.From));
			message.To.Add(MailboxAddress.Parse(address));
			message.Subject = subject;

			if (!string.IsNullOrWhiteSpace(text))
			{
				message.Body = new TextPart(MimeKit.Text.TextFormat.Html)
				{
					Text = text,
				};
			}

			await _currentSmtpClient.SendAsync(message, cancellationToken);
		}

		public async ValueTask DisposeAsync()
		{
			await DisposeAsyncCore(disposing: true);

			Dispose(disposing: false);
			GC.SuppressFinalize(this);
		}

		public void Dispose()
		{
			Dispose(disposing: true);
			GC.SuppressFinalize(this);
		}
	}
}
