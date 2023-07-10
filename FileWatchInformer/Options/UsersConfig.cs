using MailKit.Security;
using System.ComponentModel.DataAnnotations;

namespace FileWatchInformer.Options
{
    internal class UsersConfig
    {
		public IEnumerable<UserConfig> Users { get; init; }
    }
}
