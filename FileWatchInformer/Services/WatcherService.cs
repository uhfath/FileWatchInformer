using FileWatchInformer.Options;
using FileWatchInformer.Services.Emails;
using FileWatchInformer.Utils;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text.RegularExpressions;

namespace FileWatchInformer.Services
{
    internal class WatcherService : BackgroundService
    {
		private readonly IOptionsMonitor<WatcherConfig> _watcherConfig;
		private readonly IOptionsMonitor<UsersConfig> _usersConfig;
		private readonly IHostApplicationLifetime _applicationLifetime;
		private readonly IServiceScopeFactory _serviceScopeFactory;
		private readonly ILogger<WatcherService> _logger;

        private IDictionary<UserConfig, IEnumerable<string>> GetFoundFiles()
        {
			var foundFiles = new Dictionary<UserConfig, IEnumerable<string>>();
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
					foundFiles.Add(user, files);
				}
			}

			return foundFiles;
		}

		protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
					var foundFiles = GetFoundFiles();
					if (foundFiles.Any())
					{
						await using var scope = _serviceScopeFactory.CreateAsyncScope();

						var emailSenderService = scope.ServiceProvider.GetRequiredService<EmailSenderService>();
						foreach (var file in foundFiles )
						{
							var emailInfo = new EmailInfo
							{
								UserName = file.Key.Name,
								RootFolder = _watcherConfig.CurrentValue.Folder,
								UserFolder = file.Key.Folder,
								Body = file.Key.Body,
								Files = file.Value,
							};

							await emailSenderService.SendEmailAsync(file.Key.Address, file.Key.Subject, emailInfo, stoppingToken);
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
    }
}
