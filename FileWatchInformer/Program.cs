using Microsoft.Extensions.Hosting.WindowsServices;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace FileWatchInformer
{
	internal static class Program
	{
		private static IHostBuilder PrepareHost(string[] args)
		{
			var builder = Host.CreateDefaultBuilder()
				.ConfigureAppConfiguration(cfg =>
				{
					cfg
						.AddEnvironmentVariables("FWI_")
						.AddJsonFile("config.json", true, true)
						.AddCommandLine(args)
					;
				})
				.ConfigureLogging((ctx, cfg) =>
				{
					cfg
						.ClearProviders()
#if DEBUG
						.AddConsole()
#else
						.AddEventLog()
#endif
					;
				})
				.ConfigureServices((ctx, srv) =>
				{
					srv
						.AddHostedService<WatcherService>()
					;

					WatcherService.Configure(ctx, srv);
				})
			;

			if (WindowsServiceHelpers.IsWindowsService())
			{
				return builder
					.UseWindowsService();
			}

			if (OperatingSystem.IsWindows() || OperatingSystem.IsMacOS() || OperatingSystem.IsLinux() || OperatingSystem.IsFreeBSD())
			{
				return builder
					.UseConsoleLifetime();
			}

			throw new PlatformNotSupportedException();
		}

		private static async Task<int> Main(string[] args)
		{
			using var cancellationTokenSource = new CancellationTokenSource();

			using var host = PrepareHost(args).Build();

			var factory = host.Services.GetRequiredService<ILoggerFactory>();
			var logger = factory.CreateLogger<WatcherService>();

			try
			{
				await host.RunAsync(cancellationTokenSource.Token);
				return 0;
			}
			catch (OptionsValidationException ex)
			{
				var errors = ex.Failures
					.Select(er => $"\t{er}")
				;

				var message = string.Join(Environment.NewLine, errors);
				logger.LogError($"Ошибка валидации параметров приложения:{Environment.NewLine}{{message}}", message);

				return 2;
			}
			catch (Exception ex)
			{
				logger.LogError(ex, "Ошибка приложения");
				return 1;
			}
		}
	}
}