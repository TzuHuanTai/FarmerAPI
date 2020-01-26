using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FarmerAPI.Models.Weather;
using FarmerAPI.ViewModels;

namespace FarmerAPI.Controllers
{
    [Produces("application/json")]
    [Route("api/Actions")]
    public class ActionsController : Controller
    {
        private readonly WeatherContext _context;

        public ActionsController(WeatherContext context)
        {
            _context = context;
        }

        /// <summary>
        /// 查詢使用者清單
        /// </summary>
        /// <param name="q">查詢使用者名稱</param>
        /// <returns>使用者清單</returns>
        [HttpGet("[action]")]
        public IEnumerable<CtrlActionNode> GetActionTree()
        {
            IEnumerable<Actions> ActionsList = _context.Actions
                .OrderBy(x => x.ControllerId)
                .ThenByDescending(x => x.Method)
                .ThenByDescending(x => x.Name);

            IEnumerable<Ctrl> CtrlsList = _context.Ctrl
                .OrderBy(x => x.Name);            

            List<CtrlActionNode> ReturnList = new List<CtrlActionNode>();

            foreach(var Ctrl in CtrlsList)
            {
                List<Actions> ChildList = ActionsList.Where(x=>x.ControllerId==Ctrl.Id).ToList();

                List<Actions> Child = new List<Actions>();
                foreach (Actions action in ChildList)
                {
                    Child.Add(new Actions
                    {
                        Id=action.Id,
                        Name=action.Name,
                        Method=action.Method
                    });
                }

                ReturnList.Add(new CtrlActionNode
                {
                    Id=Ctrl.Id,
                    Name = Ctrl.Name,
                    Children = Child
                });
            }   

            return ReturnList;
        }

        // GET: api/Actions
        [HttpGet]
        public IEnumerable<Actions> GetActions()
        {
            return _context.Actions;
        }

        // GET: api/Actions/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetActions([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var actions = await _context.Actions.SingleOrDefaultAsync(m => m.Id == id);

            if (actions == null)
            {
                return NotFound();
            }

            return Ok(actions);
        }

        // PUT: api/Actions/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutActions([FromRoute] int id, [FromBody] Actions actions)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != actions.Id)
            {
                return BadRequest();
            }

            _context.Entry(actions).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ActionsExists(id))
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

        // POST: api/Actions
        [HttpPost]
        public async Task<IActionResult> PostActions([FromBody] Actions actions)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Actions.Add(actions);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetActions", new { StationId = actions.Id }, actions);
        }

        // DELETE: api/Actions/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteActions([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var actions = await _context.Actions.SingleOrDefaultAsync(m => m.Id == id);
            if (actions == null)
            {
                return NotFound();
            }

            _context.Actions.Remove(actions);
            await _context.SaveChangesAsync();

            return Ok(actions);
        }

        private bool ActionsExists(int id)
        {
            return _context.Actions.Any(e => e.Id == id);
        }
    }
}