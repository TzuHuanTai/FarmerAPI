using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FarmerAPI.Models.Weather;

namespace FarmerAPI.Controllers
{
    [Produces("application/json")]
    [Route("api/IactionRoles")]
    public class IactionRolesController : Controller
    {
        private readonly WeatherContext _context;

        public IactionRolesController(WeatherContext context)
        {
            _context = context;
        }

        // GET: api/IactionRoles
        [HttpGet]
        public IEnumerable<IactionRole> GetIactionRole()
        {
            return _context.IactionRole;
        }

        // GET: api/IactionRoles/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetIactionRole([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var iactionRole = await _context.IactionRole.SingleOrDefaultAsync(m => m.ActionId == id);

            if (iactionRole == null)
            {
                return NotFound();
            }

            return Ok(iactionRole);
        }

        // POST: api/IactionRoles
        [HttpPost]
        public async Task<IActionResult> PostIactionRole([FromBody] IactionRole iactionRole)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.IactionRole.Add(iactionRole);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (IactionRoleExists(iactionRole.ActionId))
                {
                    return new StatusCodeResult(StatusCodes.Status409Conflict);
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetIactionRole", new { StationId = iactionRole.ActionId }, iactionRole);
        }

        // DELETE: api/IactionRoles/5
        [HttpDelete("{ActionId}/{RoleId}")]
        public async Task<IActionResult> DeleteIactionRole([FromRoute] int ActionId, [FromRoute] int RoleId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            //IActionRole是雙主鍵！兩個pk都要判斷！
            var iactionRole = await _context.IactionRole.SingleOrDefaultAsync(m => m.ActionId == ActionId && m.RoleId == RoleId);
            if (iactionRole == null)
            {
                return NotFound();
            }

            _context.IactionRole.Remove(iactionRole);
            await _context.SaveChangesAsync();

            return Ok(iactionRole);
        }

        private bool IactionRoleExists(int id)
        {
            return _context.IactionRole.Any(e => e.ActionId == id);
        }
    }
}