using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using FarmerAPI.Models.Weather;
using FarmerAPI.Models.SQLite;
using FarmerAPI.ViewModels;
using Microsoft.Extensions.Logging;

namespace FarmerAPI.Controllers
{
    
    //[Produces("application/json")]
    [Route("api/[controller]")]
    public class ClimateController : Controller
    {
        private readonly WeatherContext _context;
        private readonly GreenHouseContext _greenHouseContext;
        private readonly ILogger<ClimateController> _logger;
        public ClimateController(WeatherContext context,
            GreenHouseContext greenHouseContext,
            ILogger<ClimateController> logger)
        {
            _context = context;
            _logger = logger;
            _greenHouseContext = greenHouseContext;
        }

        // From MongoDB
        [HttpGet("[action]")]
        public ActionResult Temperature(DateTime? beginDate, DateTime? endDate)
        {
            _logger.LogError("hello sqlite!");
            var result = _greenHouseContext.CwbData.FirstOrDefault(x=>x.Station.Name== "三芝");
            return Ok(result);
        }

        [HttpGet("[action]")]
        public IEnumerable<vmWeatherTemperature> Temperatures(int? StationId = 1, int SearchNum = 10000)
        {
            //DB抓資料出來
            IEnumerable<WeatherData> DbWeatherData = _context.WeatherData.Where(x => x.StationId == StationId).Take(SearchNum).OrderBy(x=>x.ObsTime);

            List<vmWeatherTemperature> ReturnTemperature = new List<vmWeatherTemperature>();

            foreach (WeatherData data in DbWeatherData)
            {
                ReturnTemperature.Add(new vmWeatherTemperature
                {
                    DateFormatted = data.ObsTime.ToString("yyyy-MM-dd-HH-mm"),
                    TemperatureC = data.Temperature
                });
            };
            return ReturnTemperature;           
        }

        [HttpGet("[action]")]
        public IEnumerable<vmWeatherHumidities> Humidities(int? StationId = 1, int SearchNum = 10000)
        {
            //DB抓資料出來
            IEnumerable<WeatherData> DbWeatherData = _context.WeatherData.Where(x => x.StationId == StationId).Take(SearchNum).OrderBy(x => x.ObsTime);

            List<vmWeatherHumidities> ReturnHumidities = new List<vmWeatherHumidities>();

            foreach (WeatherData data in DbWeatherData)
            {
                ReturnHumidities.Add(new vmWeatherHumidities
                {
                    DateFormatted = data.ObsTime.ToString("yyyy-MM-dd-HH-mm"),
                    RelativeHumidities = data.Rh
                });
            };
            return ReturnHumidities;
        }

        [HttpGet("[action]")]
        public IEnumerable<vmWeatherStation> Stations()
        {
            //DB抓資料出來
            IEnumerable<Models.Weather.StationInfo> DbStationData = _context.StationInfo;

            List<vmWeatherStation> ReturnStations = new List<vmWeatherStation>();

            foreach (Models.Weather.StationInfo data in DbStationData)
            {
                ReturnStations.Add(new vmWeatherStation
                {
                    StationId = data.Id,
                    StationName = data.Name
                });
            };
            return ReturnStations;
        }



      
    }
}