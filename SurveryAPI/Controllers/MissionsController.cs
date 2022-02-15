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
using SurveyAPI.DTO;
using AutoMapper;
using SurveyAPI.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace SurveyAPI.Controllers
{
    [Route("api/missions")]
    [ApiController] 
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "IsAdmin")]
 
    public class MissionsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<MissionsController> _logger;
        private readonly IMapper _mapper;
        private readonly IFileStorageService _fileStorageService;
        private readonly string containerName = "missions";

       
        public MissionsController(ApplicationDbContext context, ILogger<MissionsController> logger, IMapper imapper,
            IFileStorageService fileStorageService)
        {
            _context = context;
            _logger = logger;
            _mapper = imapper;
            _fileStorageService = fileStorageService;
        }

       

       /// <summary>
       /// Gets the a list of mission. It´s limitated to the nummber that are hardcoded into PaginationDTO. 
       /// </summary>
       /// <param name="paginationDTO"></param>
       /// <returns>A list of basic information about the mission. No questions or emplooyees.</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MissionLandingPageDTO>>> GetMissons([FromQuery] PaginationDTO paginationDTO)
        {
            var queryable = _context.Missions.AsQueryable();
            await HttpContext.InsertParametersPaginationsInHeader(queryable);

            var mission = await queryable.Paginate(paginationDTO).ToListAsync(); // Can change the order of the mission if desirable.

            if (mission == null)
            {
                return NotFound();
            }

            return _mapper.Map<List<MissionLandingPageDTO>>(mission);


        }


        // GET: api/Missions/active
        [HttpGet("active")]
        public async Task<ActionResult<IEnumerable<Mission>>> GetActiveMissons()
        {

            return await _context.Missions.Where(x => x.IsActive == true).ToListAsync();
        }

        // GET: api/Missions/5
        [HttpGet("{id}")]
        public async Task<ActionResult<MissionDTO>> GetMission(Guid id)
        {
            var mission = await _context.Missions.Include(x => x.MissionSurveys).ThenInclude(x => x.Survey).ThenInclude(x => x.SurveysQuestions).ThenInclude(x => x.Question)
                .Include(x => x.MissionSurveys).ThenInclude(x => x.Survey).ThenInclude(x => x.SurveysAnswers).ThenInclude(x => x.Answer)
                .Include(x => x.MissionEmployees).ThenInclude(x => x.Employee)
                .FirstOrDefaultAsync(x => x.Id == id);

            mission.StartDate = mission.StartDate.Date;

            

            if (mission is null)
            {
          
                return NotFound();
            }

            var mappedMission = _mapper.Map<MissionDTO>(mission);

            if (mappedMission.Surveys.Count > 0)
            {
            mappedMission.Surveys[0].sendDate = mission.MissionSurveys[0].Survey.sendDate;
            }


            if (mission.MissionSurveys.Count > 0 && mission.MissionSurveys[0].Survey.IsSent) 
            { 
                mappedMission.Surveys[0].IsSent = true;
            }

            return mappedMission;
        }

        /// <summary>
        /// Updates the mission and expect the object to come from a form. Alså takes the ID as inparameter. 
        /// Also updates the image in Azure storage. 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="missionCreationDTO"></param>
        /// <returns>NotFound if mission dosen´t exist otherwise OK.</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> PutMission(Guid id, [FromForm] MissionCreationDTO missionCreationDTO )
        {
            var mission = await _context.Missions.FirstOrDefaultAsync(x => x.Id == id);
            var savedate = mission.StartDate;
            if(mission is null)
            {
                return NotFound();
            }

         
            mission = _mapper.Map(missionCreationDTO, mission);
            mission.StartDate = savedate;

            if(missionCreationDTO.Image is not null)
            {
                mission.Image = await _fileStorageService.EditFile(containerName, 
                                missionCreationDTO.Image, mission.Image);
            }

           
            await _context.SaveChangesAsync();

            return Ok();


        }


        /// <summary>
        /// Create a mission. Expect the object to come from a form
        /// </summary>
        /// <param name="missionCreationDTO"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult<Mission>> PostMission([FromForm]MissionCreationDTO missionCreationDTO)
        {
          
            var mission = _mapper.Map<Mission>(missionCreationDTO);
            //TODO: lägg till en default bild här om använd
            if (missionCreationDTO.Image != null)
            {
                mission.Image = await _fileStorageService.SaveFile(containerName, missionCreationDTO.Image);
            }

            mission.StartDate = DateTime.Now;

            _context.Missions.Add(mission);
            await _context.SaveChangesAsync();

            return Ok();
        }


        // TODO: Ta bort bilder. 
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
