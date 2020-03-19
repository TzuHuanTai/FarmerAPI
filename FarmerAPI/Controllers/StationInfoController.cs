using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using FarmerAPI.Models.SQLite;

namespace FarmerAPI.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class StationInfoController : Controller
    {

        private readonly GreenHouseContext _greenHouseContext;
        public StationInfoController(GreenHouseContext greenHouseContext)
        {
            _greenHouseContext = greenHouseContext;
        }

        [HttpGet]
        /// <summary>
        /// Get StationInfo.
        /// </summary>
        /// <param name="id"></param> 
        public IEnumerable<StationInfo> GetStationInfo()
        {
            return _greenHouseContext.StationInfo;
        }

        [HttpGet("[action]")]
        public IEnumerable<StationInfo> GetInsideStations(decimal minLat, decimal maxLag, decimal minLng, decimal maxLng)
        {
            return _greenHouseContext.StationInfo.Where(x =>
                x.Latitude >= minLat
                && x.Latitude <= maxLag
                && x.Longitude >= minLng
                && x.Longitude <= maxLng
            );
        }

        [HttpGet("{StationId}")]
        public async Task<IActionResult> GetStationInfo([FromRoute] int StationId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            StationInfo Station = await _greenHouseContext.StationInfo.SingleOrDefaultAsync(m => m.StationId == StationId);

            if (Station == null)
            {
                return NotFound();
            }

            return Ok(Station);
        }

        [HttpPut("{StationId}")]
        public async Task<IActionResult> PutStationInfo([FromRoute] int StationId, [FromBody] StationInfo StationInfo)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (StationId != StationInfo.StationId)
            {
                return BadRequest();
            }

            _greenHouseContext.Entry(StationInfo).State = EntityState.Modified;

            try
            {
                await _greenHouseContext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!StationInfoExists(StationId))
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

        // POST: api/V34
        [HttpPost]
        public async Task<IActionResult> PostStationInfo([FromBody] StationInfo StationInfo)
        {            
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _greenHouseContext.StationInfo.Add(StationInfo);           
            try
            {
                await _greenHouseContext.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (StationInfoExists(StationInfo.StationId))
                {
                    return new StatusCodeResult(StatusCodes.Status409Conflict);
                }
                else
                {
                    throw;
                }
            }

            return Ok(StationInfo.StationId);
        }

        // DELETE: api/V34/5
        [HttpDelete("{StationId}")]
        public async Task<IActionResult> DeleteStationInfo([FromRoute] int StationId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            StationInfo Station = await _greenHouseContext.StationInfo.SingleOrDefaultAsync(m => m.StationId == StationId);

            if (Station == null)
            {
                return NotFound();
            }

            _greenHouseContext.StationInfo.Remove(Station);
            await _greenHouseContext.SaveChangesAsync();

            return NoContent();
        }

        protected bool StationInfoExists(int StationId)
        {
            return _greenHouseContext.StationInfo.Any(e => e.StationId == StationId);
        }
    }
}