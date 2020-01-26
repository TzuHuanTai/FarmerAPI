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
    [Route("api/Ctrls")]
    public class CtrlsController : Controller
    {
        private readonly WeatherContext _context;

        public CtrlsController(WeatherContext context)
        {
            _context = context;
        }

        // GET: api/Ctrls
        [HttpGet]
        public IEnumerable<Ctrl> GetCtrl()
        {
            return _context.Ctrl;
        }

        // GET: api/Ctrls/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCtrl([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var ctrl = await _context.Ctrl.SingleOrDefaultAsync(m => m.Id == id);

            if (ctrl == null)
            {
                return NotFound();
            }

            return Ok(ctrl);
        }

        // PUT: api/Ctrls/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCtrl([FromRoute] int id, [FromBody] Ctrl ctrl)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != ctrl.Id)
            {
                return BadRequest();
            }

            _context.Entry(ctrl).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CtrlExists(id))
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

        // POST: api/Ctrls
        [HttpPost]
        public async Task<IActionResult> PostCtrl([FromBody] Ctrl ctrl)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Ctrl.Add(ctrl);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCtrl", new { StationId = ctrl.Id }, ctrl);
        }

        // DELETE: api/Ctrls/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCtrl([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var ctrl = await _context.Ctrl.SingleOrDefaultAsync(m => m.Id == id);
            if (ctrl == null)
            {
                return NotFound();
            }

            _context.Ctrl.Remove(ctrl);
            await _context.SaveChangesAsync();

            return Ok(ctrl);
        }

        private bool CtrlExists(int id)
        {
            return _context.Ctrl.Any(e => e.Id == id);
        }
    }
}