using MailKit.Security;
using System.ComponentModel.DataAnnotations;

namespace FileWatchInformer.Options
{
    internal partial class EmailConfig
    {
        [RequiredField(nameof(Messages.RequiredFieldError))]
        public string Server { get; init; }

		[RequiredField(nameof(Messages.RequiredFieldError))]
		public int? Port { get; init; } = 587;
        public SecureSocketOptions Security { get; init; } = SecureSocketOptions.Auto;

		[RequiredField(nameof(Messages.RequiredFieldError))]
		public string Login { get; init; }

		[RequiredField(nameof(Messages.RequiredFieldError))]
		public string Password { get; init; }

		[RequiredField(nameof(Messages.RequiredFieldError))]
		public string From { get; init; }

        public string DefaultSubject { get; init; } = "Информирование о наличии невостребованных файлов";
        public string DefaultBody { get; init; }
    }
}
