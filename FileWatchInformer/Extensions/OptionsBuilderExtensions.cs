using FileWatchInformer.CommonValidators;
using FileWatchInformer.Options;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileWatchInformer.Extensions
{
	internal static class OptionsBuilderExtensions
	{
		public static OptionsBuilder<TOptions> AddDataAnnotationsValidator<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties | DynamicallyAccessedMemberTypes.NonPublicProperties)] TOptions>(this OptionsBuilder<TOptions> optionsBuilder)
			where TOptions : class
		{
			optionsBuilder.Services.AddSingleton((IValidateOptions<TOptions>)new DataAnnotationsOptionsValidator<TOptions>(optionsBuilder.Name));
			return optionsBuilder;
		}
	}
}
