using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.SignalR;
using FarmerAPI.Hubs;
using FarmerAPI.ViewModels;
using FarmerAPI.Models.SQLite;

namespace FarmerAPI.Controllers
{
    
    //[Produces("application/json")]
    [Route("api/[controller]")]
    public class ClimateController : Controller
    {
        private readonly GreenHouseContext _context;
        private readonly IHubContext<SensorHub> _sensorHub;
        private readonly ILogger<ClimateController> _logger;
        public ClimateController(
            GreenHouseContext greenHouseContext,
            IHubContext<SensorHub> sensorHub,
            ILogger<ClimateController> logger
        )
        {
            _logger = logger;
            _context = greenHouseContext;
            _sensorHub = sensorHub;
        }

        // From MongoDB
        [HttpGet("[action]")]
        public ActionResult Temperature(DateTime? beginDate, DateTime? endDate)
        {
            //var result = _greenHouseContext.CwbData.FirstOrDefault(x=>x.Station.Name== "三芝");
            Climate result = _context.Climate.FirstOrDefault();
            return Ok(result);
        }

        [HttpGet("[action]")]
        public IEnumerable<VmWeatherTemperature> Temperatures(int? StationId = 1, int SearchNum = 10000)
        {
            //DB抓資料出來
            var cwbData = _context.CwbData
                .Where(x => x.StationId == StationId)
                .Select(x=>new {x.ObsTime, x.Temperature })
                .Take(SearchNum).OrderBy(x=>x.ObsTime);

            List<VmWeatherTemperature> ReturnTemperature = new List<VmWeatherTemperature>();

            foreach (var data in cwbData)
            {
                ReturnTemperature.Add(new VmWeatherTemperature
                {
                    DateFormatted = data.ObsTime.ToString("yyyy-MM-dd-HH-mm"),
                    TemperatureC = data.Temperature
                });
            };
            return ReturnTemperature;
        }

        [HttpGet("[action]")]
        public IEnumerable<VmWeatherHumidities> Humidities(int? StationId = 1, int SearchNum = 10000)
        {
            //DB抓資料出來
            var cwbData = _context.CwbData
                .Where(x => x.StationId == StationId)
                .Select(x => new { x.ObsTime, x.Rh })
                .Take(SearchNum).OrderBy(x => x.ObsTime);

            List<VmWeatherHumidities> ReturnHumidities = new List<VmWeatherHumidities>();

            foreach (var data in cwbData)
            {
                ReturnHumidities.Add(new VmWeatherHumidities
                {
                    DateFormatted = data.ObsTime.ToString("yyyy-MM-dd-HH-mm"),
                    RelativeHumidities = data.Rh
                });
            };
            return ReturnHumidities;
        }

        [HttpPost("{StationId}")]
        public async Task<ActionResult> PostClimate([FromRoute] int StationId, [FromBody]Climate SensorData)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            using var transection = _context.Database.BeginTransaction();
            try
            {
                var realtimeData = new VmRealtime
                {
                    StationId = StationId,
                    DateFormatted = SensorData.ObsTime,
                    Temperature = SensorData.Temperature,
                    RH = SensorData.Rh,
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