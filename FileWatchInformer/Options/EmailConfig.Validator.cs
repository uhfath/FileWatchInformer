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
			private readonly IOptionsSnapshot<UsersConfig> _usersConfig;

			public Validator(
				IOptionsSnapshot<UsersConfig> usersConfig)
            {
				this._usersConfig = usersConfig;
			}

            public ValidateOptionsResult Validate(string name, EmailConfig options)
			{
				if (string.IsNullOrWhiteSpace(options.DefaultSubject))
				{
					var emptySubjects = _usersConfig.Value.Users
						.Where(u => string.IsNullOrWhiteSpace(u.Subject))
						.Select(u => $"Нет темы письма по умолчанию в секции `Email.DefaultSubject` и для пользователя с папкой `{u.Folder}`.")
						.ToArray()
					;

					if (emptySubjects.Length > 0 )
					{
						return ValidateOptionsResult.Fail(emptySubjects);
					}
				}

				return ValidateOptionsResult.Success;
			}
		}
	}
}
