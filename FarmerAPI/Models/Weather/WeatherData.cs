using System;
using System.Collections.Generic;

namespace FarmerAPI.Models
{
    public partial class WeatherData
    {
        public int StationId { get; set; }
        public DateTime ObsTime { get; set; }
        public decimal? StnPres { get; set; }
        public decimal? SeaPres { get; set; }
        public decimal? Temperature { get; set; }
        public decimal? Td { get; set; }
        public decimal? Rh { get; set; }
        public decimal? Ws { get; set; }
        public decimal? Wd { get; set; }
        public decimal? Wsgust { get; set; }
        public decimal? Wdgust { get; set; }
        public decimal? Precp { get; set; }
        public decimal? PrecpHour { get; set; }
        public decimal? SunShine { get; set; }
        public decimal? GlobalRad { get; set; }
        public decimal? Visb { get; set; }

        public StationInfo StationIdNavigation { get; set; }
    }
}
