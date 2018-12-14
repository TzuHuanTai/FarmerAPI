using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FarmerAPI.Models;
using FarmerAPI.ViewModels;

namespace FarmerAPI.Controllers
{
    [Produces("application/json")]
    [Route("api/RoleGroups")]
    public class RoleGroupsController : Controller
    {
        private readonly WeatherContext _context;

        public RoleGroupsController(WeatherContext context)
        {
            _context = context;
        }

        [HttpGet("[action]")]
        public IEnumerable<RoleGroupNode> GetRoleGroupTree()
        {
            IEnumerable<RoleGroup> Roles = _context.RoleGroup;

            List<RoleGroupNode> ReturnRole = new List<RoleGroupNode>();

            //有很多棵tree，每個ParentRoleId為null的都是root of tree
            foreach (RoleGroup Root in Roles.Where(x => x.ParentRoleId == null))
            {
                //找出所有root底下的leafs
                RoleGroupNode RootImenu = new RoleGroupNode() { Id = Root.RoleId, Name = Root.RoleName };
                List<RoleGroupNode> Tree = TreeRoleGroup(RootImenu, Roles);

                //加入回傳的tree
                ReturnRole.Add(Tree[0]);
            };

            return ReturnRole;
        }

        private List<RoleGroupNode> TreeRoleGroup(RoleGroupNode Root, IEnumerable<RoleGroup> AllRoles)
        {
            List<RoleGroupNode> ReturnTreeRole = new List<RoleGroupNode>();

            RoleGroupNode TreeRoot = new RoleGroupNode()
            {
                Id = Root.Id,
                Name = Root.Name,
                
            };

            if (AllRoles.Any(x => x.ParentRoleId == Root.Id))
            {
                TreeRoot.Children = new List<RoleGroupNode>();
                //找root及其底下leafs
                foreach (RoleGroup Item in AllRoles.Where(x => x.ParentRoleId == Root.Id)) // && x.MenuId == Root.MenuId
                {
                    RoleGroupNode ItemImenu = new RoleGroupNode() { Id = Item.RoleId, Name = Item.RoleName };
                    List<RoleGroupNode> Node = TreeRoleGroup(ItemImenu, AllRoles);
                    TreeRoot.Children.Add(Node[0]);
                };
            }            

            ReturnTreeRole.Add(TreeRoot);

            return ReturnTreeRole;
        }

        // GET: api/RoleGroups
        [HttpGet]
        public IEnumerable<RoleGroup> GetRoleGroup()
        {
            return _context.RoleGroup;
        }

        // GET: api/RoleGroups/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetRoleGroup([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var roleGroup = await _context.RoleGroup.SingleOrDefaultAsync(m => m.RoleId == id);

            if (roleGroup == null)
            {
                return NotFound();
            }

            return Ok(roleGroup);
        }

        // PUT: api/RoleGroups/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutRoleGroup([FromRoute] int id, [FromBody] RoleGroup roleGroup)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != roleGroup.RoleId)
            {
                return BadRequest();
            }

            _context.Entry(roleGroup).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RoleGroupExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/RoleGroups
        [HttpPost]
        public async Task<IActionResult> PostRoleGroup([FromBody] RoleGroup roleGroup)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.RoleGroup.Add(roleGroup);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (RoleGroupExists(roleGroup.RoleId))
                {
                    return new StatusCodeResult(StatusCodes.Status409Conflict);
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetRoleGroup", new { StationId = roleGroup.RoleId }, roleGroup);
        }

        // DELETE: api/RoleGroups/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRoleGroup([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var roleGroup = await _context.RoleGroup.SingleOrDefaultAsync(m => m.RoleId == id);
            if (roleGroup == null)
            {
                return NotFound();
            }

            _context.RoleGroup.Remove(roleGroup);
            await _context.SaveChangesAsync();

            return Ok(roleGroup);
        }

        private bool RoleGroupExists(int id)
        {
            return _context.RoleGroup.Any(e => e.RoleId == id);
        }
    }
}