using System;
using System.Collections.Generic;

namespace FarmerAPI.Models.Weather
{
    public partial class Actions
    {
        public Actions()
        {
            IactionRole = new HashSet<IactionRole>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Method { get; set; }
        public int ControllerId { get; set; }
        public string Description { get; set; }

        public Ctrl Controller { get; set; }
        public ICollection<IactionRole> IactionRole { get; set; }
    }
}
