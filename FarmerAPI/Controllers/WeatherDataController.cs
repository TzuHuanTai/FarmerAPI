using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using FarmerAPI.Models;
using FarmerAPI.ViewModels;

namespace FarmerAPI.Controllers
{
    
    //[Produces("application/json")]
    [Route("api/[controller]")]
    public class WeatherDataController : Controller
    {
        private readonly WeatherContext _context;
        public WeatherDataController(WeatherContext context)
        {
            _context = context;
        }
                
        [HttpGet("[action]")]
        public IEnumerable<vmWeatherTemperature> Temperatures(int? StationId = 1, int SearchNum = 10000)
        {
            //DB抓資料出來
            IEnumerable<WeatherData> DbWeatherData = _context.WeatherData.Where(x => x.StationNum == StationId).Take(SearchNum).OrderBy(x=>x.ObsTime);

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
            IEnumerable<WeatherData> DbWeatherData = _context.WeatherData.Where(x => x.StationNum == StationId).Take(SearchNum).OrderBy(x => x.ObsTime);

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
            IEnumerable<StationInfo> DbStationData = _context.StationInfo;

            List<vmWeatherStation> ReturnStations = new List<vmWeatherStation>();

            foreach (StationInfo data in DbStationData)
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