using MailKit.Security;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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

				var allValidationResults = new Dictionary<string, IEnumerable<ValidationResult>>();
				var index = 0;
				foreach (var user in options.Users)
				{
					var validationResults = new List<ValidationResult>();
					if (!System.ComponentModel.DataAnnotations.Validator.TryValidateObject(user, new ValidationContext(user), validationResults, validateAllProperties: true))
					{
						var key = !string.IsNullOrWhiteSpace(user.Folder)
							? $"с папкой `{user.Folder}`"
							: !string.IsNullOrWhiteSpace(user.Address)
								? $"с адресом `{user.Address}`"
								: string.Empty;

						key = string.IsNullOrWhiteSpace(key)
							? $"№ {index + 1}"
							: key;

						allValidationResults[key] = validationResults;
					}

					++index;
				}

				if (allValidationResults.Count > 0)
				{
					var errors = allValidationResults
						.Select(er => $"Пользователь {er.Key}{Environment.NewLine}{string.Join(Environment.NewLine, er.Value.Select(vr => vr.ErrorMessage))}")
					;

					return ValidateOptionsResult.Fail(errors);
				}

				return ValidateOptionsResult.Success;
			}
		}
	}
}
