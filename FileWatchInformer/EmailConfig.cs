using MailKit.Security;

namespace FileWatchInformer
{
	internal class EmailConfig
	{
		public string Server { get; init; }
		public int Port { get; init; }
		public SecureSocketOptions Security { get; init; }
		public string Login { get; init; }
		public string Password { get; init; }
		public string From { get; init; }
		public string DefaultSubject { get; init; }
		public string DefaultBody { get; init; }
	}
}
