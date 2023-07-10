using MailKit.Security;
using System.ComponentModel.DataAnnotations;

namespace FileWatchInformer.Options
{
    internal class UsersConfig
    {
        [Required]
        public IEnumerable<UserConfig> Users { get; init; }
    }
}
