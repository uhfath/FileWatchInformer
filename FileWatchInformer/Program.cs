using Microsoft.Extensions.Hosting.WindowsServices;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text;
using FileWatchInformer.Services;

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
						.SetMinimumLevel(LogLevel.Information);
#else
						.AddEventLog()
						.SetMinimumLevel(LogLevel.Warning);
#endif
					;
				})
				.ConfigureServices((ctx, srv) =>
				{
					Starter.Configure(ctx, srv);
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

		private static void LogOptionsErrors(ILogger logger, IEnumerable<OptionsValidationException> exceptions)
		{
			var errors = exceptions
				.GroupBy(ex => ex.OptionsType.Name)
				.OrderBy(gr => gr.Key)
				.Select(gr => new
				{
					Options = gr.Key,
					Messages = gr.SelectMany(ex => ex.Failures)
				})
			;

			var messages = new StringBuilder();
			foreach (var error in errors)
			{
				messages.AppendLine($"\t`{error.Options.Replace("Config", string.Empty)}`");
				foreach (var message in error.Messages)
				{
					messages.AppendLine($"\t\t{message.Replace(Environment.NewLine, $"{Environment.NewLine}\t\t\t")}");
				}
			}

			logger.LogError($"Ошибка валидации параметров приложения:{Environment.NewLine}{{message}}", messages.ToString());
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
			catch (AggregateException ex)
				when (ex.InnerExceptions.Any(ie => ie is OptionsValidationException))
			{
				var inners = ex.Flatten().InnerExceptions;

				var invalids = inners
					.Where(ex => ex is OptionsValidationException)
					.Cast<OptionsValidationException>()
				;

				LogOptionsErrors(logger, invalids);

				var others = inners
					.Where(ex => ex is not OptionsValidationException)
					.ToArray()
				;

				if (others.Length > 0)
				{
					throw new AggregateException(others);
				}

				return 2;
			}
			catch (OptionsValidationException ex)
			{
				LogOptionsErrors(logger, new[] { ex });
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