using MailKit.Security;
using System.ComponentModel.DataAnnotations;

namespace FileWatchInformer.Options
{
    internal partial class UsersConfig
    {
		public IEnumerable<UserConfig> Users { get; init; }
    }
}
