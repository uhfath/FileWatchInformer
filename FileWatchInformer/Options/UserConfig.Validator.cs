using FileWatchInformer.Utils;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FileWatchInformer.Options
{
	internal partial class UserConfig
	{
		internal class Validator : IValidateOptions<UserConfig>
		{
			private readonly IOptionsMonitor<EmailConfig> _emailConfig;

			public Validator(
				IOptionsMonitor<EmailConfig> emailConfig)
            {
				this._emailConfig = emailConfig;
			}

            public ValidateOptionsResult Validate(string name, UserConfig options)
			{
				if (!string.IsNullOrWhiteSpace(options.IncludePattern) && !string.IsNullOrWhiteSpace(options.IncludeWildcard))
				{
					return ValidateOptionsResult.Fail($"Нельзя указывать одновременно `{nameof(UserConfig.IncludePattern)}` и `{nameof(UserConfig.IncludeWildcard)}`. Допустим только один критерий.");
				}

				if (!string.IsNullOrWhiteSpace(options.ExcludePattern) && !string.IsNullOrWhiteSpace(options.ExcludeWildcard))
				{
					return ValidateOptionsResult.Fail($"Нельзя указывать одновременно `{nameof(UserConfig.ExcludePattern)}` и `{nameof(UserConfig.ExcludeWildcard)}`. Допустим только один критерий.");
				}

				if (!string.IsNullOrWhiteSpace(options.IncludePattern))
				{
					var result = RegexUtils.ValidateRegexPattern(options.IncludePattern, nameof(UserConfig.IncludePattern));
					if (result.Failed)
					{
						return result;
					}
				}

				if (!string.IsNullOrWhiteSpace(options.ExcludePattern))
				{
					var result = RegexUtils.ValidateRegexPattern(options.ExcludePattern, nameof(UserConfig.ExcludePattern));
					if (result.Failed)
					{
						return result;
					}
				}

				if (string.IsNullOrWhiteSpace(options.Subject) && string.IsNullOrWhiteSpace(_emailConfig.CurrentValue?.DefaultSubject))
				{
					return ValidateOptionsResult.Fail($"Не указана тема сообщения. Необходимо указать её для пользователя, либо в секции `Email.{nameof(EmailConfig.DefaultSubject)}`");
				}

				return ValidateOptionsResult.Success;
			}
		}
	}
}
