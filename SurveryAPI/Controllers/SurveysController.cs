#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SurveyAPI.DTO;
using SurveyAPI.Models;

namespace SurveyAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "IsAdmin")]
    public class SurveysController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public SurveysController(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        /// <summary>
        /// To get survey with questions and answers. 
        /// </summary>
        /// <param name="id">Of survey.</param>
        /// <returns>SurveyDTO object that hold list of answers and questions.</returns>
        // GET: api/Surveys/5
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<SurveyDTO>> GetSurvey(Guid id)
        {
            var survey = await _context.Surveys
                .Include(x => x.SurveysQuestions).ThenInclude(x => x.Question)
                .Include(x => x.SurveysAnswers).ThenInclude(x => x.Answer)
                .FirstOrDefaultAsync(x => x.SurveyId == id);

            var surveyDto = _mapper.Map<SurveyDTO>(survey);

            return surveyDto;
        }


        // POST: api/Surveys
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<SurveyCreationDTO>> PostSurvey(SurveyCreationDTO surveyCreationDTO)
        {

            var survey = _mapper.Map<Survey>(surveyCreationDTO);

            survey.Created = DateTime.Now;

            _context.Add(survey);
            await _context.SaveChangesAsync();

            if (surveyCreationDTO.missionId is not null)
            {
                var mission = await _context.Missions
                    .Include(x => x.MissionSurveys)
                    .ThenInclude(x => x.Survey)
                    .FirstOrDefaultAsync(x => x.Id == surveyCreationDTO.missionId);

                if (mission is not null)
                {
                    mission.MissionSurveys.Add(new MissionSurveys { SurveyId = survey.SurveyId });
                    _context.Update(mission);
                    await _context.SaveChangesAsync();
                }

            }

            return Ok();
        }


        [HttpPut("{id}")]
        public async Task<ActionResult<SurveyCreationDTO>> PutSurvey(Guid id, SurveyCreationDTO surveyCreationDTO)
        {



            var survey = _context.Surveys
                .Include(x => x.SurveysQuestions).ThenInclude(x => x.Question)
                .Include(x => x.SurveysAnswers).ThenInclude(x => x.Answer)
                .FirstOrDefault(x => x.SurveyId == id);

            var mappedSurvey = _mapper.Map<Survey>(surveyCreationDTO);


            foreach (var item in survey.SurveysQuestions)
            {
                _context.SurveysQuestions.Remove(item);
            }

            _context.Add(mappedSurvey);
            _context.Remove(survey);

            var missionSurvey = await _context.MissionSurveys.FirstOrDefaultAsync(x => x.SurveyId == survey.SurveyId);
            _context.MissionSurveys.Remove(missionSurvey);

            var mission = await _context.Missions
             .Include(x => x.MissionSurveys)
            .ThenInclude(x => x.Survey)
             .FirstOrDefaultAsync(x => x.Id == missionSurvey.MissionId);

            mission.MissionSurveys.Add(new MissionSurveys { SurveyId = mappedSurvey.SurveyId });
            _context.Update(mission);

            await _context.SaveChangesAsync();



      




            //mappedSurvey.Created = DateTime.Now;
            //mappedSurvey.SurveyId = survey.SurveyId;

            //foreach(var item in mappedSurvey.SurveysQuestions)
            //{
            //_context.SurveysQuestions.Update(item);
            //}

            //_context.Update(mappedSurvey);
            //await _context.SaveChangesAsync();


            return Ok();
        }


        // DELETE: api/Surveys/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSurvey(Guid id)
        {
            var survey = await _context.Surveys.FindAsync(id);
            if (survey == null)
            {
                return NotFound();
            }

            _context.Surveys.Remove(survey);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool SurveyExists(Guid id)
        {
            return _context.Surveys.Any(e => e.SurveyId == id);
        }
    }
}
