using FileWatchInformer.Extensions;
using FileWatchInformer.Options;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileWatchInformer.Services
{
    internal class WatcherService : BackgroundService
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await using (var scope = _serviceScopeFactory.CreateAsyncScope())
                {
                    var emailSender = scope.ServiceProvider.GetRequiredService<EmailSenderService>();
                }

                await Task.Delay(5000);
            }
        }

        public WatcherService(
            IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
        }

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
            ;

            services
                .AddScoped<EmailSenderService>()
            ;

            return services;
        }
    }
}
