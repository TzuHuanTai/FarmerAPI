using System;
using System.Collections.Generic;

namespace FarmerAPI.Models.SQLite
{
    public partial class StationInfo
    {
        public StationInfo()
        {
            CwbData = new HashSet<CwbData>();
        }

        public int StationId { get; set; }
        public string Name { get; set; }
        public int? CityId { get; set; }
        public string Address { get; set; }
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }

        public virtual City City { get; set; }
        public virtual ICollection<CwbData> CwbData { get; set; }
    }
}
