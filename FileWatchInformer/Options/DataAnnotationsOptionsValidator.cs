using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace FileWatchInformer.Options
{
	internal class DataAnnotationsOptionsValidator<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties | DynamicallyAccessedMemberTypes.NonPublicProperties)] TOptions> : IValidateOptions<TOptions>
		where TOptions : class
	{
		public string Name { get; }

		[RequiresUnreferencedCode("The implementation of Validate method on this type will walk through all properties of the passed in options object, and its type cannot be statically analyzed so its members may be trimmed.")]
		public DataAnnotationsOptionsValidator(string name)
		{
			Name = name;
		}

		[UnconditionalSuppressMessage("ReflectionAnalysis", "IL2026:RequiresUnreferencedCode", Justification = "Suppressing the warnings on this method since the constructor of the type is annotated as RequiresUnreferencedCode.")]
		public ValidateOptionsResult Validate(string name, TOptions options)
		{
			if (Name != null && Name != name)
			{
				return ValidateOptionsResult.Skip;
			}

			if (options is null)
			{
				throw new ArgumentNullException(nameof(options));
			}

			var validationResults = new List<ValidationResult>();
			if (Validator.TryValidateObject(options, new ValidationContext(options), validationResults, validateAllProperties: true))
			{
				return ValidateOptionsResult.Success;
			}

			var errors = validationResults
				.Select(er => er.ErrorMessage)
			;

			return ValidateOptionsResult.Fail(errors);
		}
	}
}
