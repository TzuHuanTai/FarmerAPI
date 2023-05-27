using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog.Web;
using System.IO;
using System.Net;
using System.Security.Cryptography.X509Certificates;

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
							var certPem = File.ReadAllText("/etc/letsencrypt/live/rich-greenhouse.ddns.net/fullchain.pem");
							var keyPem = File.ReadAllText("/etc/letsencrypt/live/rich-greenhouse.ddns.net/privkey.pem");
							var x509 = X509Certificate2.CreateFromPem(certPem, keyPem);

							options.Listen(IPAddress.Any, 6080);
							options.Listen(IPAddress.Any, 6443, listenOptions =>
							{
								listenOptions.UseHttps(x509);
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