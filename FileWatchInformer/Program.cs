using Microsoft.Extensions.Hosting.WindowsServices;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

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

			try
			{
				await PrepareHost(args)
					.Build()
					.RunAsync(cancellationTokenSource.Token);

				return 0;
			}
			catch (Exception ex)
			{
				Console.Error.WriteLine(ex.ToString());
				return 1;
			}
		}
	}
}