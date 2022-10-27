using FarmerAPI.Hubs;
using FarmerAPI.Models.SQLite;
using FarmerAPI.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FarmerAPI.Controllers
{
	//[Produces("application/json")]
	[Route("api/[controller]")]
	public class ClimateController : Controller
	{
		private readonly GreenHouseContext _context;
		private readonly IHubContext<SensorHub> _sensorHub;
		private readonly ILogger<ClimateController> _logger;
		protected List<Climate> buffer;

		public ClimateController(
			GreenHouseContext greenHouseContext,
			IHubContext<SensorHub> sensorHub,
			ILogger<ClimateController> logger
		)
		{
			_logger = logger;
			_context = greenHouseContext;
			_sensorHub = sensorHub;
			buffer = new List<Climate>();
		}

		// From MongoDB
		[HttpGet("[action]")]
		public IEnumerable<Climate> GreenHouse(int SearchNum = 10000)
		{
			var result = _context.Climate
				.Skip(Math.Max(0, _context.Climate.Count() - SearchNum));
			return result;
		}

		[HttpGet("[action]")]
		public IEnumerable<Climate> Cwb(int? StationId = 1, int SearchNum = 10000)
		{
			var targetCwbData = _context.CwbData
				.Where(x => x.StationId == StationId);
			var result = targetCwbData
				.Skip(Math.Max(0, targetCwbData.Count() - SearchNum))
				.Select(x => new Climate { ObsTime = x.ObsTime, Temperature = x.Temperature, Rh = x.Rh });
			return result;
		}

		[HttpPost("{StationId}")]
		public async Task<ActionResult> PostClimate([FromRoute] int StationId, [FromBody]Climate SensorData)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}

			if (SensorData.Rh == null || SensorData.Temperature == null || SensorData.Rh < 0 || SensorData.Rh > 100 || SensorData.Temperature < -40 || SensorData.Temperature > 80)
			{
				return BadRequest("Noised data");
			}

			if(buffer.Count > 0)
			{
				var avg_temperature = (buffer.Select(x => x.Temperature).Sum() / buffer.Count);
				var avg_Rh = buffer.Select(x => x.Rh).Sum() / buffer.Count;
				if ((SensorData.Temperature - avg_temperature) / avg_temperature > (decimal)0.3 && (SensorData.Rh - avg_Rh) / avg_Rh > (decimal)0.3)
				{
					return BadRequest("Data with too much amplitude");
				}
				while (buffer.Count > 10)
				{
					buffer.RemoveAt(0);
				}
			}
			buffer.Add(SensorData);

			using var transection = _context.Database.BeginTransaction();
			try
			{
				var realtimeData = new VmRealtime
				{
					StationId = StationId,
					DateFormatted = SensorData.ObsTime,
					Temperature = SensorData.Temperature,
					RH = SensorData.Rh >= 0 && SensorData.Rh <= 100 ? SensorData.Rh : null,
					Lux = SensorData.Lux
				};

				await _sensorHub.Clients.All.SendAsync("SensorDetected", realtimeData);

				// 小溫室本身資料要存檔
				if (StationId == 0)
				{
					_context.Climate.Add(SensorData);
					await _context.SaveChangesAsync();
					transection.Commit();
				}
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}

			return Ok();
		}
	}
}