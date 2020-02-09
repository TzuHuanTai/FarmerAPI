using System;
using System.Collections.Generic;

namespace FarmerAPI.Models.Weather
{
    public partial class Token
    {
        public string Account { get; set; }
        public DateTime? AuthTime { get; set; }
        public DateTime? ExpiredTime { get; set; }
        public string TokenCode { get; set; }
        public string Ip { get; set; }

        public Member AccountNavigation { get; set; }
    }
}
