using FileWatchInformer.Extensions;
using FileWatchInformer.Options;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Hosting.Internal;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FileWatchInformer.Services
{
    internal class WatcherService : BackgroundService
    {
		private readonly IOptionsMonitor<WatcherConfig> _watcherConfig;
		private readonly IOptionsMonitor<UsersConfig> _usersConfig;
		private readonly IHostApplicationLifetime _applicationLifetime;
		private readonly IServiceScopeFactory _serviceScopeFactory;
		private readonly ILogger<WatcherService> _logger;

		protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    foreach (var user in _usersConfig.CurrentValue.Users)
                    {
						var folder = Path.Combine(_watcherConfig.CurrentValue.Folder, user.Folder);

                        var excludeWildcard = string.IsNullOrWhiteSpace(user.ExcludeWildcard)
                            ? _watcherConfig.CurrentValue.DefaultExcludeWildcard
                            : user.ExcludeWildcard;

                        var excludedFiles = new HashSet<string>();
                        if (!string.IsNullOrWhiteSpace(excludeWildcard))
                        {
                            excludedFiles = Directory.GetFiles(folder, excludeWildcard, SearchOption.AllDirectories)
                                .ToHashSet();
                        }

                        var includeWildcard = string.IsNullOrWhiteSpace(user.IncludeWildcard)
                            ? "*.*"
                            : user.IncludeWildcard;

                        var filesQuery = Directory.EnumerateFiles(folder, includeWildcard, SearchOption.AllDirectories)
                            .Where(f => !excludedFiles.Contains(f))
                            .AsEnumerable()
                        ;

						var excludePattern = string.IsNullOrWhiteSpace(user.ExcludePattern)
							? _watcherConfig.CurrentValue.DefaultExcludePattern
							: user.ExcludePattern;

						if (!string.IsNullOrWhiteSpace(excludePattern))
                        {
							var regex = new Regex(excludePattern, RegexOptions.Compiled);
                            filesQuery = filesQuery
                                .Where(f => !regex.IsMatch(Path.GetFileName(f)))
                            ;
						}

						var includePattern = string.IsNullOrWhiteSpace(user.IncludePattern)
							? _watcherConfig.CurrentValue.DefaultIncludePattern
							: user.IncludePattern;

						if (!string.IsNullOrWhiteSpace(includePattern))
                        {
							var regex = new Regex(includePattern, RegexOptions.Compiled);
                            filesQuery = filesQuery
                                .Where(f => regex.IsMatch(Path.GetFileName(f)))
                            ;
						}

                        var files = filesQuery
                            .ToArray()
                        ;

						if (files.Length > 0)
                        {
                            await using (var scope = _serviceScopeFactory.CreateAsyncScope())
                            {
                                var emailSender = scope.ServiceProvider.GetRequiredService<EmailSenderService>();

                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Ошибка обработки");
                }

                if (_watcherConfig.CurrentValue.Interval == null)
                {
                    _applicationLifetime.StopApplication();
				}
                else
                {
                    await Task.Delay(_watcherConfig.CurrentValue.Interval.Value);
                }
            }
        }

        public WatcherService(
            IHostApplicationLifetime applicationLifetime,
            IServiceScopeFactory serviceScopeFactory,
			IOptionsMonitor<WatcherConfig> watcherConfig,
			IOptionsMonitor<UsersConfig> usersConfig,
            ILogger<WatcherService> logger)
        {
			this._applicationLifetime = applicationLifetime;
			this._serviceScopeFactory = serviceScopeFactory;
			this._watcherConfig = watcherConfig;
			this._usersConfig = usersConfig;
			this._logger = logger;
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
