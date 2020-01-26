using System;
using System.Collections.Generic;

namespace FarmerAPI.Models.Weather
{
    public partial class IactionRole
    {
        public int RoleId { get; set; }
        public int ActionId { get; set; }

        public Actions Action { get; set; }
        public RoleGroup Role { get; set; }
    }
}
