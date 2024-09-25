using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.IO;
using System.Windows.Forms;
using BluetoothKeepAlive.WinTrayService.Options;
using System.Threading;

namespace BluetoothKeepAlive.WinTrayService
{
	internal static class Program
	{
		private static IServiceProvider mServiceProvider;

		[STAThread]
		public static void Main()
		{
			// To customize application configuration such as set high DPI settings or default font,
			// see https://aka.ms/applicationconfiguration.
			ApplicationConfiguration.Initialize();

			IHost appHost = CreateHostBuilder( new string [ 0 ] ).Build();
			mServiceProvider = appHost.Services;

			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault( false );

			using (ProcessIcon processIcon = new ProcessIcon())
			{
				BluetoothKeepAliveWorkerService workerService = mServiceProvider
					.GetRequiredService<BluetoothKeepAliveWorkerService>();

				workerService.Start( CancellationToken.None );
				processIcon.OnExitRequested += (sender, args) => workerService.Stop();
				processIcon.Display();

				Application.Run();
			}
		}

		private static IHostBuilder CreateHostBuilder( string [] args )
		{
			return Host.CreateDefaultBuilder( args )
				.ConfigureAppConfiguration( ( hostContext, configBuilder ) =>
				{
					OnConfigureAppConfiguration( hostContext.HostingEnvironment.EnvironmentName,
						configBuilder );
				} )
				.ConfigureLogging( OnConfigureLogging )
				.ConfigureServices( OnConfigureServices );
		}

		private static void OnConfigureLogging( HostBuilderContext hostContext,
			ILoggingBuilder loggingBuilder )
		{
			loggingBuilder.ClearProviders();

			// The ILoggingBuilder minimum level determines the
			// the lowest possible level for logging. The log4net
			// level then sets the level that we actually log at.
			// see https://github.com/huorswords/Microsoft.Extensions.Logging.Log4Net.AspNetCore
			loggingBuilder.AddLog4Net( GetConfigFilePath() );
			loggingBuilder.SetMinimumLevel( LogLevel.Trace );
		}

		private static void OnConfigureServices( HostBuilderContext hostContext,
			IServiceCollection services )
		{
			services.AddSingleton<IConfigureOptions<BluetoothKeepAliveOptions>, BluetoothKeepAliveOptionsConfiguration>();
			services.AddTransient<BluetoothKeepAliveExecutor>();
			services.AddSingleton<BluetoothKeepAliveWorkerService>();
		}

		private static void OnConfigureAppConfiguration( string environmentName, IConfigurationBuilder configBuilder )
		{
			configBuilder.AddJsonFile( $"appsettings.json",
				optional: false,
				reloadOnChange: false );

			configBuilder.AddJsonFile( $"appsettings.{environmentName}.json",
				optional: true,
				reloadOnChange: false );
		}

		private static string GetConfigFilePath()
		{
			string directory = AppContext
				.BaseDirectory;

			string configFilePath = Path.Combine( directory,
				"log4net.config" );

			return configFilePath;
		}
	}
}