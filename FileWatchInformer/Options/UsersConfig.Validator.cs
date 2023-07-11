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
			private readonly UserConfig.Validator _userValidator;

			private static string GetUserConfigKey(UserConfig userConfig, int index) =>
				userConfig switch
				{
					_ when !string.IsNullOrWhiteSpace(userConfig.Name) => $"с именем `{userConfig.Name}`",
					_ when !string.IsNullOrWhiteSpace(userConfig.Folder) => $"с папкой `{userConfig.Folder}`",
					_ when !string.IsNullOrWhiteSpace(userConfig.Address) => $"с адресом `{userConfig.Address}`",
					_ => $"№ {index + 1}"
				};

			public Validator(
				UserConfig.Validator userValidator)
            {
				this._userValidator = userValidator;
			}

            public ValidateOptionsResult Validate(string name, UsersConfig options)
			{
				if (!(options.Users?.Any() ?? false))
				{
					return ValidateOptionsResult.Fail($"Блок является обязательным к заполнению");
				}

				var allValidationResults = new Dictionary<string, IEnumerable<string>>();
				var index = 0;
				foreach (var user in options.Users)
				{
					var key = GetUserConfigKey(user, index);
					var validationResults = new List<ValidationResult>();
					if (!System.ComponentModel.DataAnnotations.Validator.TryValidateObject(user, new ValidationContext(user), validationResults, validateAllProperties: true))
					{
						allValidationResults[key] = validationResults.Select(vr => vr.ErrorMessage);
					}

					if (validationResults.Count == 0)
					{
						var validationResult = _userValidator.Validate(string.Empty, user);
						if (validationResult.Failed)
						{
							allValidationResults[key] = validationResult.Failures;
						}
					}

					++index;
				}

				if (allValidationResults.Count > 0)
				{
					var errors = allValidationResults
						.Select(er => $"Пользователь {er.Key}{Environment.NewLine}{string.Join(Environment.NewLine, er.Value)}")
					;

					return ValidateOptionsResult.Fail(errors);
				}

				return ValidateOptionsResult.Success;
			}
		}
	}
}
