using FileWatchInformer.Attributes;
using MailKit.Security;
using System.ComponentModel.DataAnnotations;

namespace FileWatchInformer.Options
{
    internal class WatcherConfig
    {
		[RequiredField]
		public string Folder { get; init; }
        public TimeSpan? Interval { get; init; }
        public TimeSpan? Delay { get; init; }
        public string DefaultIncludeMask { get; init; }
        public string DefaultExcludeMask { get; init; }
    }
}
