using FileWatchInformer.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileWatchInformer.Options
{
    internal partial class UserConfig
    {
		[RequiredField]
		public string Name { get; init; }

		[RequiredField]
		public string Folder { get; init; }

		[RequiredField]
		public string Address { get; init; }

        public string Subject { get; init; }
        public string Body { get; init; }
        public string IncludePattern { get; init; }
        public string ExcludePattern { get; init; }
        public string IncludeWildcard { get; init; }
        public string ExcludeWildcard { get; init; }
    }
}
