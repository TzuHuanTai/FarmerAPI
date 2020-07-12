using System;

namespace FarmerAPI.ViewModels
{
    public class VmWeatherData
    {
        
    }

    public class CwbClimate
    {
        public string DateFormatted { get; set; }
        public decimal? TemperatureC { get; set; }
        public decimal? RelativeHumidities { get; set; }
    }

    public class VmRealtime
    {
        public DateTime? DateFormatted { get; set; }
        public int StationId { get; set; }
        public decimal? Temperature { get; set; }
        public decimal? RH { get; set; }
		public decimal? Lux { get; set; }
	}
}
