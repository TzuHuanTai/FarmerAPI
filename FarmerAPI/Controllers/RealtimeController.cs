using FarmerAPI.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FarmerAPI.Controllers
{
	[Route("api/[controller]")]
	public class RealtimeController : Controller
	{
		private readonly IConfiguration _config;
		private readonly HubConnection connection;
		private static List<VmRealtime> _realtimeList;

		public RealtimeController(IConfiguration config)
		{
			_config = config;
			_realtimeList = new List<VmRealtime>();

			connection = new HubConnectionBuilder()
				.WithUrl(_config["Url:SensorHub"])
				.Build();

			connection.StartAsync().ContinueWith(t => DetectRecievedData());
		}

		// /api/Realtime/1
		//[AuthorizationFilter]
		[HttpGet("{StationId}")]
		public async Task<IActionResult> GetRealtime(int StationId)
		{
			var realtimeData = await IsRealtimeStationExistsAsync(StationId);
			if (realtimeData == null)
			{
				return NoContent();
			}
			else
			{
				return Ok(realtimeData);
			}
		}

		protected void DetectRecievedData()
		{
			connection.On<VmRealtime>("SensorDetected", async (DetectedData) =>
			{
				await SetRealtimeListAsync(_realtimeList, DetectedData);
			});
		}

		protected Task<VmRealtime> IsRealtimeStationExistsAsync(int StationId)
		{
			return Task.Run(() =>
			{
				return _realtimeList.FirstOrDefault(x => x.StationId == StationId);
			});
		}

		protected Task SetRealtimeListAsync(List<VmRealtime> realtimeList, VmRealtime DetectedData)
		{
			return Task.Run(async () =>
			{
				var realtimeData = await IsRealtimeStationExistsAsync(DetectedData.StationId);
				if (realtimeData == null)
				{
					realtimeData = DetectedData;
					realtimeList.Add(DetectedData);
				}
				else
				{
					realtimeData = DetectedData;
				}
			});
		}
	}
}