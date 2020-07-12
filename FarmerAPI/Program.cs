using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog.Web;
using System.Net;

namespace FarmerAPI
{
	public class Program
	{
		public static void Main(string[] args)
		{
			CreateHostBuilder(args).Build().Run();
		}

		public static IHostBuilder CreateHostBuilder(string[] args) =>
			Host.CreateDefaultBuilder(args)
				.ConfigureWebHostDefaults(webBuilder =>
				{
					webBuilder.UseStartup<Startup>()
						.UseKestrel(options =>
						{
							options.Listen(IPAddress.Any, 6080);
							options.Listen(IPAddress.Any, 6443, listenOptions =>
							{
								listenOptions.UseHttps("backend.pfx", "2ooixuui");
							});
						})
						.UseUrls("https://0.0.0.0:6443")
						.ConfigureLogging(logging =>
						{
							logging.ClearProviders();
							logging.SetMinimumLevel(LogLevel.Trace);
						})
						.UseNLog();
				});
	}
}