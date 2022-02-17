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
        private readonly string [] ImgArray = new string[]{
                    "https://surveysapi.blob.core.windows.net/missions/60b335f2-6bce-457f-b6a8-844b0e12ede4.jfif",
                    "https://surveysapi.blob.core.windows.net/missions/0e2e4930-e34c-4918-8630-193c5f3b6eef.jfif",
                    "https://surveysapi.blob.core.windows.net/missions/a792bbe3-cbd0-4c63-afa8-eba62f0991b3.jfif"};


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



       /// <summary>
       /// Get a specific mission. Includes a lot of data.This part could be refactorated by updating some DTOs. 
       /// Beacuse of lack of time I´ll leave it like this.
       /// </summary>
       /// <param name="id"></param>
       /// <returns>A mission with survey, questions and answers if existing</returns>
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
        /// Create a mission. Expect the object to come from a form. If no img is choosen, a random pick between 3 images is made. 
        /// </summary>
        /// <param name="missionCreationDTO"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult<Mission>> PostMission([FromForm]MissionCreationDTO missionCreationDTO)
        {
          
            var mission = _mapper.Map<Mission>(missionCreationDTO);
         
            if (missionCreationDTO.Image != null)
            {
                mission.Image = await _fileStorageService.SaveFile(containerName, missionCreationDTO.Image);
            }
            else
            {


                var rnd = new Random();
                var nr = rnd.Next(3);
                mission.Image = ImgArray[nr];
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
            var mission = await _context.Missions.Include(x => x.MissionSurveys).ThenInclude(x => x.Survey).ThenInclude(x => x.SurveysQuestions).ThenInclude(x => x.Question)
                .Include(x => x.MissionSurveys).ThenInclude(x => x.Survey).ThenInclude(x => x.SurveysAnswers).ThenInclude(x => x.Answer)
                .Include(x => x.MissionEmployees).ThenInclude(x => x.Employee)
                .FirstOrDefaultAsync(x => x.Id == id);
            if (mission == null)
            { 
                return NotFound();
            }

            if (mission.MissionSurveys.Count > 0)

            {

                foreach (var item in mission?.MissionSurveys[0].Survey.SurveysAnswers)
            {
                _context.SurveysAnswers.Remove(item);
                _context.Answers.Remove(item.Answer);
            }

            foreach (var item in mission.MissionSurveys[0]?.Survey?.SurveysQuestions)
            {
                _context.SurveysQuestions.Remove(item);
            }

                _context.Surveys.Remove(mission.MissionSurveys[0]?.Survey);
            }

            foreach (var item in mission.MissionEmployees)
            {
                _context.MissionEmployees.Remove(item);
                _context.Employees.Remove(item.Employee);
            }

            if(mission.Image != ImgArray[0] && mission.Image != ImgArray[1] && mission.Image != ImgArray[2])
            {
                await _fileStorageService.DeleteFile(mission.Image, containerName);
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
