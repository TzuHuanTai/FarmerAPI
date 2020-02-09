using System;
using System.Collections.Generic;

namespace FarmerAPI.Models.SQLite
{
    public partial class City
    {
        public City()
        {
            StationInfo = new HashSet<StationInfo>();
        }

        public int CityId { get; set; }
        public string Name { get; set; }

        public virtual ICollection<StationInfo> StationInfo { get; set; }
    }
}
