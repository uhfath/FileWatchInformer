using FileWatchInformer.Extensions;
using FileWatchInformer.Options;
using FileWatchInformer.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileWatchInformer
{
	internal static class Starter
	{
		public static IServiceCollection Configure(HostBuilderContext context, IServiceCollection services)
		{
			services
				.AddSingleton<IValidateOptions<EmailConfig>, EmailConfig.Validator>()
				.AddOptions<EmailConfig>()
				.Bind(context.Configuration.GetSection("Email"))
				.AddDataAnnotationsValidator()
				.ValidateOnStart()
			;

			services
				.AddSingleton<IValidateOptions<WatcherConfig>, WatcherConfig.Validator>()
				.AddOptions<WatcherConfig>()
				.Bind(context.Configuration.GetSection("Watcher"))
				.AddDataAnnotationsValidator()
				.ValidateOnStart()
			;

			services
				.AddSingleton<IValidateOptions<UsersConfig>, UsersConfig.Validator>()
				.AddOptions<UsersConfig>()
				.Bind(context.Configuration)
				.AddDataAnnotationsValidator()
				.ValidateOnStart()
			;

			services
				.AddSingleton<UserConfig.Validator>()
				.AddScoped<EmailSenderService>()
				.AddTransient<MessageService>()
				.AddHostedService<WatcherService>()
			;

			return services;
		}
	}
}
