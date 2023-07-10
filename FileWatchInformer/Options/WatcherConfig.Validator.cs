using FileWatchInformer.Utils;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileWatchInformer.Options
{
	internal partial class WatcherConfig
	{
		internal class Validator : IValidateOptions<WatcherConfig>
		{
			public ValidateOptionsResult Validate(string name, WatcherConfig options)
			{
				if (!string.IsNullOrWhiteSpace(options.DefaultIncludeMask))
				{
					var result = RegexUtils.ValidateRegexMask(options.DefaultIncludeMask, nameof(WatcherConfig.DefaultIncludeMask));
					if (result.Failed)
					{
						return result;
					}
				}

				if (!string.IsNullOrWhiteSpace(options.DefaultExcludeMask))
				{
					var result = RegexUtils.ValidateRegexMask(options.DefaultExcludeMask, nameof(WatcherConfig.DefaultExcludeMask));
					if (result.Failed)
					{
						return result;
					}
				}

				return ValidateOptionsResult.Success;
			}
		}
	}
}
