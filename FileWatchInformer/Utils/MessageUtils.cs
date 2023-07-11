using FileWatchInformer.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileWatchInformer.Utils
{
	internal static class MessageUtils
	{
		private const string FileListMacro = "%FILELIST%";
		private const string UserNameMacro = "%USERNAME%";
		private const string FileListTemplate = "<li>{0}</li>";

		public static string PrepareMessage(string userName, string rootFolder, string userFolder, string body, IEnumerable<string> files)
		{
			if (string.IsNullOrWhiteSpace(body))
			{
				return string.Empty;
			}

			var userFiles = files
				.Select(f => Path.GetRelativePath(Path.Combine(rootFolder, userFolder), f))
				.Select(f => string.Format(FileListTemplate, f))
			;

			var message = body;

			if (!string.IsNullOrWhiteSpace(userName))
			{
				message = message.Replace(UserNameMacro, userName, StringComparison.OrdinalIgnoreCase);
			}

			return message
				.Replace(FileListMacro, string.Join(Environment.NewLine, userFiles), StringComparison.OrdinalIgnoreCase)
			;
		}
	}
}
