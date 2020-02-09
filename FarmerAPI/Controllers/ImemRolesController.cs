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
    [Route("api/ImemRoles")]
    public class ImemRolesController : Controller
    {
        private readonly WeatherContext _context;

        public ImemRolesController(WeatherContext context)
        {
            _context = context;
        }

        // GET: api/ImemRoles
        [HttpGet]
        public IEnumerable<ImemRole> GetImemRole()
        {
            return _context.ImemRole;
        }

        // GET: api/ImemRoles/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetImemRole([FromRoute] string id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var imemRole = await _context.ImemRole.Where(m => m.Account == id).ToListAsync();

            if (imemRole == null)
            {
                return NotFound();
            }

            return Ok(imemRole);
        }

        // POST: api/ImemRoles
        [HttpPost]
        public async Task<IActionResult> PostImemRole([FromBody] ImemRole imemRole)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.ImemRole.Add(imemRole);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (ImemRoleExists(imemRole.Account))
                {
                    return new StatusCodeResult(StatusCodes.Status409Conflict);
                }
                else
                {
                    throw;
                }
            }
            return CreatedAtAction("GetImemRole", new { id = imemRole.Account }, imemRole);
        }

        // DELETE: api/ImemRoles/5
        [HttpDelete("{Account}/{RoleId}")]
        public async Task<IActionResult> DeleteImemRole([FromRoute] string Account, [FromRoute] int RoleId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            //ImemRole是雙主鍵！兩個pk都要判斷！
            var imemRole = await _context.ImemRole.SingleOrDefaultAsync(m => m.Account == Account && m.RoleId == RoleId);
            if (imemRole == null)
            {
                return NotFound();
            }

            _context.ImemRole.Remove(imemRole);
            await _context.SaveChangesAsync();

            return Ok(imemRole);
        }

        private bool ImemRoleExists(string id)
        {
            return _context.ImemRole.Any(e => e.Account == id);
        }
    }
}