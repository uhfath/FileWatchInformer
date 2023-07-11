using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileWatchInformer.Services.Emails
{
	internal record EmailInfo
	{
        public string UserName { get; init; }
        public string RootFolder { get; init; }
        public string UserFolder { get; init; }
        public string Body { get; init; }
        public IEnumerable<string> Files { get; init; }
    }
}
