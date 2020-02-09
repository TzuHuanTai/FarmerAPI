using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using FarmerAPI.Models.Weather;
using FarmerAPI.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Cors;
using FarmerAPI.Filters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using FarmerAPI.Hubs;
using Microsoft.Extensions.Logging;
using FarmerAPI.Models.SQLite;

namespace FarmerAPI.Controllers
{    
    [Route("api/[controller]")]
    //[EnableCors("AllowAllOrigins")] //在Startup.cs做全域設定
    public class RealtimeController : Controller
    {
        private readonly WeatherContext _context;
		private readonly IHubContext<WeatherHub> _weatherHub;

		public RealtimeController(WeatherContext context,
			IHubContext<WeatherHub> weatherHub)
        {
            _context = context;
			_weatherHub = weatherHub;
		}

		// /api/Realtime/1
		//[AuthorizationFilter]
		[HttpGet("{StationId}")]
        public async Task<IActionResult> GetRealtime(int StationId)
        {
            if (StationExists(StationId))
            {
                //DB抓資料出來
                RealTime DbRealtimeData = await _context.RealTime.SingleOrDefaultAsync(x => x.Id == StationId);
                string DbStationName = await _context.StationInfo.Where(x => x.Id == StationId).Select(x => x.Name).FirstOrDefaultAsync();

                vmRealtime ReturnRealtimeData = new vmRealtime()
                {
                    DateFormatted = DateTime.Now,
                    StationId = StationId,
                    StationName = DbStationName,
                    RecTemp = DbRealtimeData.Temperature,
                    RecRH = DbRealtimeData.Rh,
					Lux = DbRealtimeData.Lux
				};
                return Ok(ReturnRealtimeData);               
            }
            else
            {
                //return new vmRealtime();
                return NotFound();
            }            
        }

		// This action at /api/Realtime/5 can bind form data (set individual parameters in body)
		// Content-Type: application/json, x-www-form-urlencoded is working
		// Insert/Update database realtime table through the recieved data on Raspberry pi DHT22 sensor.
		//[HttpPut("{StationId}")]
  //      public async Task<ActionResult> PutRealtime([FromRoute] int StationId, [FromBody] Climate SensorData)
  //      {
		//	if (StationId != SensorData.StationId)
		//	{
		//		return BadRequest();
		//	}

		//	using(var transection = _context.Database.BeginTransaction())
		//	{
		//		if (StationExists(SensorData.StationId))
		//		{
		//			try
		//			{
		//				// save into RMSDB
		//				RealTime TargetStation = _context.RealTime.SingleOrDefault(x => x.Id == SensorData.StationId);
		//				_context.Entry(TargetStation).State = EntityState.Modified;
		//				TargetStation.Temperature = SensorData.Temperature;
		//				TargetStation.Rh = SensorData.RH;
		//				TargetStation.Lux = SensorData.Lux;
		//				_context.SaveChanges();

		//				// save into mongoDb					
		//				await PostRealtime(StationId, SensorData);

		//				// Broadcast by SignalR
		//				await _weatherHub.Clients.All.SendAsync("TempRhSensorReceived", SensorData);
		//				transection.Commit();
		//			}
		//			catch (DbUpdateConcurrencyException)
		//			{
		//				return BadRequest("Rollback");
		//			}
		//		}
		//		return Ok();
		//	}
  //      }

		//新增資料進入MongoDB
		//[HttpPost("{StationId}")]
		//public async Task<ActionResult> PostRealtime([FromRoute] int StationId, [FromBody]Climate DetectedData)
		//{
		//	if (StationId != DetectedData.StationId)
		//	{
		//		return BadRequest();
		//	}

		//	try
		//	{
		//		await _weatherService.Create(DetectedData);
		//	}
		//	catch(Exception ex)
		//	{
		//		throw ex;
		//	}			

		//	return NoContent();
		//}

		// This action at /api/values/Realtime/5 can bind type of JSON in body directly because of [FromBody]
		// Content-Type: application/json
		// Using Query String Parameters will show error: 415 Unsupported Media Type
		//[HttpPost("[action]/{StationId}")]
		//public void Realtime([FromBody]vmRealtime realtime)
		//{
		//    if (StationExists(realtime.StationId))
		//    {
		//        //Update database realtime table through the recieved data on Raspberry pi DHT22 sensor.
		//        RealTime TargetStation = _context.RealTime.SingleOrDefault(x => x.Id == realtime.StationId);
		//        TargetStation.Temperature = realtime.RecTemp;
		//        TargetStation.Rh = realtime.RecRH;
		//        _context.Entry(TargetStation).State = EntityState.Modified;
		//        try
		//        {
		//            _context.SaveChanges();
		//        }
		//        catch (DbUpdateConcurrencyException)
		//        {
		//            throw;
		//        }
		//    }
		//}  	     

		private bool StationExists(int StationId)
        {
            return _context.RealTime.Any(x => x.Id == StationId);
        }
    }
}
