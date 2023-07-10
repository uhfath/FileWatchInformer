using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileWatchInformer.Options
{
    internal class UserConfig
    {
        [Required]
        public string Folder { get; init; }
        [Required]
        public string Address { get; init; }
        public string Subject { get; init; }
        public string Body { get; init; }
        public string IncludeMask { get; init; }
        public string ExcludeMask { get; init; }
    }
}
