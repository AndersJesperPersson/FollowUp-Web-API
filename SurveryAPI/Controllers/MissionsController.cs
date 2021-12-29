#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SurveyAPI.Models;
using SurveyAPI;

namespace SurveyAPI.Controllers
{
    [Route("api/missions")]
    [ApiController] // Make sure the parameters are correct. No need to check if model is valid.
    public class MissionsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<MissionsController> _logger;
        
        //using dependicy injections for db and logger.
        public MissionsController(ApplicationDbContext context, ILogger<MissionsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: api/Missions
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Mission>>> GetMissons()
        {
            
            return await _context.Missions.ToListAsync();
        }

        // GET: api/Missions/active
        [HttpGet("active")]
        public async Task<ActionResult<IEnumerable<Mission>>> GetActiveMissons()
        {

            return await _context.Missions.Where(x => x.IsActive == true).ToListAsync();
        }

        // GET: api/Missions/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Mission>> GetMission(Guid id)
        {
            var mission = await _context.Missions.FindAsync(id);

            if (mission == null)
            {
                return NotFound();
            }

            return mission;
        }

        // PUT: api/Missions/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutMission(Guid id, Mission mission)
        {
            if (id != mission.Id)
            {
                return BadRequest();
            }

            _context.Entry(mission).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MissionExists(id))
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

        // POST: api/Missions
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Mission>> PostMission(Mission mission)
        {
            _context.Missions.Add(mission);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetMission", new { id = mission.Id }, mission);
        }

        // DELETE: api/Missions/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMission(Guid id)
        {
            var mission = await _context.Missions.FindAsync(id);
            if (mission == null)
            {
                return NotFound();
            }

            _context.Missions.Remove(mission);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool MissionExists(Guid id)
        {
            return _context.Missions.Any(e => e.Id == id);
        }
    }
}
