using System;

namespace FarmerAPI.Models.SQLite
{
	public class Climate
	{
		public DateTime ObsTime { get; set; }
		public decimal? Temperature { get; set; }
		public decimal? Rh { get; set; }
		public decimal? Lux { get; set; }
	}
}