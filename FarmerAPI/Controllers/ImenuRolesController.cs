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
    [Route("api/ImenuRoles")]
    public class ImenuRolesController : Controller
    {
        private readonly WeatherContext _context;

        public ImenuRolesController(WeatherContext context)
        {
            _context = context;
        }                

        // GET: api/ImenuRoles
        [HttpGet]
        public IEnumerable<ImenuRole> GetImenuRole()
        {
            return _context.ImenuRole;
        }

        // GET: api/ImenuRoles/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetImenuRole([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var imenuRole = await _context.ImenuRole.SingleOrDefaultAsync(m => m.MenuId == id);

            if (imenuRole == null)
            {
                return NotFound();
            }

            return Ok(imenuRole);
        }

        // POST: api/ImenuRoles
        [HttpPost]
        public async Task<IActionResult> PostImenuRole([FromBody] ImenuRole imenuRole)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.ImenuRole.Add(imenuRole);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (ImenuRoleExists(imenuRole.MenuId, imenuRole.RoleId))
                {
                    return new StatusCodeResult(StatusCodes.Status409Conflict);
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetImenuRole", new { StationId = imenuRole.MenuId }, imenuRole);
        }

        // DELETE: api/ImenuRoles/5
        [HttpDelete("{MenuId}/{RoleId}")]
        public async Task<IActionResult> DeleteImenuRole([FromRoute] int MenuId, [FromRoute] int RoleId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            //ImenuRole是雙主鍵！兩個pk都要判斷！
            var imenuRole = await _context.ImenuRole.SingleOrDefaultAsync(m => m.MenuId == MenuId && m.RoleId == RoleId);
            if (imenuRole == null)
            {
                return NotFound();
            }

            _context.ImenuRole.Remove(imenuRole);
            await _context.SaveChangesAsync();

            return Ok(imenuRole);
        }

        private bool ImenuRoleExists(int MenuId, int RoleId)
        {
            //ImenuRole是雙主鍵！兩個pk都要判斷！
            return _context.ImenuRole.Any(e => e.MenuId == MenuId && e.RoleId == RoleId);
        }
    }
}