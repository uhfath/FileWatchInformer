using FileWatchInformer.Attributes;
using MailKit.Security;
using System.ComponentModel.DataAnnotations;

namespace FileWatchInformer.Options
{
    internal partial class WatcherConfig
    {
		[RequiredField]
		public string Folder { get; init; }
        public TimeSpan? Interval { get; init; }
        public TimeSpan? Delay { get; init; }
        public string DefaultIncludePattern { get; init; }
        public string DefaultExcludePattern { get; init; }
        public string DefaultIncludeWildcard { get; init; }
        public string DefaultExcludeWildcard { get; init; }
    }
}
