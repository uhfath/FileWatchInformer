using MailKit.Security;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileWatchInformer.Options
{
	internal partial class UsersConfig
	{
		internal class Validator : IValidateOptions<UsersConfig>
		{
			public ValidateOptionsResult Validate(string name, UsersConfig options)
			{
				if (!(options.Users?.Any() ?? false))
				{
					return ValidateOptionsResult.Fail($"Блок является обязательным к заполнению");
				}

				return ValidateOptionsResult.Success;
			}
		}
	}
}
