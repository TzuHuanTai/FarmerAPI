using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR.Client;
using FarmerAPI.ViewModels;
using Microsoft.Extensions.Configuration;

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
                .WithUrl(_config["SignalR:Url"])
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

        private void DetectRecievedData()
        {
            connection.On<VmRealtime>("SensorDetected", async (DetectedData) =>
            {
                await SetRealtimeListAsync(_realtimeList, DetectedData);
            });
        }

        private Task<VmRealtime> IsRealtimeStationExistsAsync(int StationId)
        {
            return Task.Run(() =>
            {
                return _realtimeList.FirstOrDefault(x => x.StationId == StationId);
            });
        }

        public Task SetRealtimeListAsync(List<VmRealtime> realtimeList, VmRealtime DetectedData)
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
