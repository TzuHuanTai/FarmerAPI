using System;
using System.Collections.Generic;

namespace FarmerAPI.Models
{
    public partial class RealTime
    {
        public int Id { get; set; }
        public decimal? Temperature { get; set; }
        public decimal? Rh { get; set; }
		public decimal? Lux { get; set; }
	}
}
