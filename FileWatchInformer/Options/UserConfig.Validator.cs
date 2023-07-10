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
			private readonly IOptionsSnapshot<EmailConfig> _emailConfig;

			public Validator(
				IOptionsSnapshot<EmailConfig> emailConfig)
            {
				this._emailConfig = emailConfig;
			}

            public ValidateOptionsResult Validate(string name, UserConfig options)
			{
				if (!string.IsNullOrWhiteSpace(options.IncludeMask))
				{
					var result = RegexUtils.ValidateRegexMask(options.IncludeMask, nameof(UserConfig.IncludeMask));
					if (result.Failed)
					{
						return result;
					}
				}

				if (!string.IsNullOrWhiteSpace(options.ExcludeMask))
				{
					var result = RegexUtils.ValidateRegexMask(options.ExcludeMask, nameof(UserConfig.ExcludeMask));
					if (result.Failed)
					{
						return result;
					}
				}

				if (string.IsNullOrWhiteSpace(options.Subject) && string.IsNullOrWhiteSpace(_emailConfig.Value?.DefaultSubject))
				{
					return ValidateOptionsResult.Fail($"Не указана тема сообщения. Необходимо указать её для пользователя, либо в секции `Email.{nameof(EmailConfig.DefaultSubject)}`");
				}

				return ValidateOptionsResult.Success;
			}
		}
	}
}
