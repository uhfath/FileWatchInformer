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
				if (!string.IsNullOrWhiteSpace(options.DefaultIncludePattern) && !string.IsNullOrWhiteSpace(options.DefaultIncludeWildcard))
				{
					return ValidateOptionsResult.Fail($"Нельзя указывать одновременно `{nameof(WatcherConfig.DefaultIncludePattern)}` и `{nameof(WatcherConfig.DefaultIncludeWildcard)}`. Допустим только один критерий.");
				}

				if (!string.IsNullOrWhiteSpace(options.DefaultExcludePattern) && !string.IsNullOrWhiteSpace(options.DefaultExcludeWildcard))
				{
					return ValidateOptionsResult.Fail($"Нельзя указывать одновременно `{nameof(WatcherConfig.DefaultExcludePattern)}` и `{nameof(WatcherConfig.DefaultExcludeWildcard)}`. Допустим только один критерий.");
				}

				if (!string.IsNullOrWhiteSpace(options.DefaultIncludePattern))
				{
					var result = RegexUtils.ValidateRegexPattern(options.DefaultIncludePattern, nameof(WatcherConfig.DefaultIncludePattern));
					if (result.Failed)
					{
						return result;
					}
				}

				if (!string.IsNullOrWhiteSpace(options.DefaultExcludePattern))
				{
					var result = RegexUtils.ValidateRegexPattern(options.DefaultExcludePattern, nameof(WatcherConfig.DefaultExcludePattern));
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
