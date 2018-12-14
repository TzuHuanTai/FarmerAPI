using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FarmerAPI.ViewModels
{
    public class vmWeatherData
    {
        
    }

    public class vmWeatherTemperature
    {
        public string DateFormatted { get; set; }
        public decimal? TemperatureC { get; set; }
    }

    public class vmWeatherHumidities
    {
        public string DateFormatted { get; set; }
        public decimal? RelativeHumidities { get; set; }
    }

    public class vmWeatherStation
    {
        public int StationId { get; set; }
        public string StationName { get; set; }
    }

    public class vmRealtime
    {
        public DateTime DateFormatted { get; set; }
        public int StationId { get; set; }
        public string StationName { get; set; }
        public decimal? RecTemp { get; set; }
        public decimal? RecRH { get; set; }
		public decimal? Lux { get; set; }
	}
}
