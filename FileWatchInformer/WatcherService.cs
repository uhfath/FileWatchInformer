using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileWatchInformer
{
	internal class WatcherService : BackgroundService
	{
		private readonly IServiceScopeFactory _serviceScopeFactory;
		protected override async Task ExecuteAsync(CancellationToken stoppingToken)
		{
			while (!stoppingToken.IsCancellationRequested)
			{
			}
		}

		public WatcherService(
			IServiceScopeFactory serviceScopeFactory)
        {
			this._serviceScopeFactory = serviceScopeFactory;
		}

		public static IServiceCollection Configure(HostBuilderContext context, IServiceCollection services)
		{
			services
				.AddOptions<EmailConfig>()
				.BindConfiguration("Email")
			;

			//services
			//	.AddOptions<WatcherConfig>()
			//	.BindConfiguration("Watcher")
			//;

			services
				.AddScoped<EmailSenderService>()
			;

			return services;
		}
	}
}
