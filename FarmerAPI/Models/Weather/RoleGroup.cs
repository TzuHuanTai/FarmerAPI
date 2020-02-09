using System;
using System.Collections.Generic;

namespace FarmerAPI.Models.Weather
{
    public partial class RoleGroup
    {
        public RoleGroup()
        {
            IactionRole = new HashSet<IactionRole>();
            ImemRole = new HashSet<ImemRole>();
            ImenuRole = new HashSet<ImenuRole>();
        }

        public int RoleId { get; set; }
        public string RoleName { get; set; }
        public int SortNo { get; set; }
        public int AccessScope { get; set; }
        public string Description { get; set; }
        public int? ParentRoleId { get; set; }
        public bool RejectScope { get; set; }
        public bool ApproveScope { get; set; }
        public bool SubmitScope { get; set; }
        public bool PassScope { get; set; }
        public bool PrintScope { get; set; }

        public ICollection<IactionRole> IactionRole { get; set; }
        public ICollection<ImemRole> ImemRole { get; set; }
        public ICollection<ImenuRole> ImenuRole { get; set; }
    }
}
