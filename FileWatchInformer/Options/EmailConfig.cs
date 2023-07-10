using MailKit.Security;
using System.ComponentModel.DataAnnotations;

namespace FileWatchInformer.Options
{
    internal partial class EmailConfig
    {
        [RequiredField]
        public string Server { get; init; }

		[RequiredField]
		public int? Port { get; init; } = 587;
        public SecureSocketOptions Security { get; init; } = SecureSocketOptions.Auto;

		[RequiredField(nameof(Messages.RequiredFieldError))]
		[RequiredField]
		public string Login { get; init; }

		[RequiredField]
		public string Password { get; init; }

		[RequiredField]
		public string From { get; init; }

        public string DefaultSubject { get; init; } = "Информирование о наличии невостребованных файлов";
        public string DefaultBody { get; init; }
    }
}
