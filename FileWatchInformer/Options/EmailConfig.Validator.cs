using MailKit.Security;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileWatchInformer.Options
{
	internal partial class EmailConfig
	{
		internal class Validator : IValidateOptions<EmailConfig>
		{
            public ValidateOptionsResult Validate(string name, EmailConfig options)
			{
				if (!string.IsNullOrWhiteSpace(options.Security) && !Enum.TryParse<SecureSocketOptions>(options.Security, true, out var _))
				{
					return ValidateOptionsResult.Fail($"Некорректное значение для параметра `{nameof(EmailConfig.Security)}`");
				}

				return ValidateOptionsResult.Success;
			}
		}
	}
}
