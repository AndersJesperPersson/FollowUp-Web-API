#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SurveyAPI.DTO;
using SurveyAPI.Models;

namespace SurveyAPI.Controllers
{
    [Route("api/answers")]
    [ApiController]
    public class AnswersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<AnswersController> _logger;
        private readonly IMapper _mapper;
        public AnswersController(ApplicationDbContext context, ILogger<AnswersController> logger, IMapper mapper)
        {
            _context = context;
            _logger = logger;
            _mapper = mapper;
        }


        // GET: api/Answers/5
        [HttpGet("{id}")]
        public async Task<ActionResult<IEnumerable<Answer>>> GetAnswers(Guid id)
        {
           var survey =   _context.MissionSurveys.FirstOrDefault(x => x.MissionId == id);
           var answers = _context.SurveysAnswers
                         .Include(x=> x.Answer)
                         .ThenInclude(x=> x.Question).
                          Where(x=>x.SurveyId == survey.SurveyId).Select(x=> x.Answer).ToList();


            
            return answers.ToList();
        }



        // POST: api/Answers
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost("{id}")]
        public async Task<ActionResult<Answer>> PostAnswer(Guid Id, [FromBody] List<AnswerCreationDTO> answers)
        {
            var answerlist = new List<Answer>();

            foreach(var answersItem in answers)
            {
                var question = _context.Questions.FirstOrDefault(x => x.Id == answersItem.QuestionID);
                var answer = new Answer()
                {
                    Id = new Guid(),
                    Reply=answersItem.Reply,
                    Question = question         
                }; 

               _context.Add(answer);
                answerlist.Add(answer);
             
            }

            

            var survey = await _context.Surveys.Include(x => x.SurveysAnswers).ThenInclude(x => x.Answer).FirstOrDefaultAsync(x => x.SurveyId == Id);


            if (survey is not null)
            {
                foreach (var answer in answerlist)
                {
                    survey.SurveysAnswers.Add(new SurveysAnswers { SurveyId = survey.SurveyId, AnswerId = answer.Id });
                    _context.Update(survey);


                }
            }


            await _context.SaveChangesAsync();
            return NoContent();
        }


        private bool AnswerExists(Guid id)
        {
            return _context.Answers.Any(e => e.Id == id);
        }
    }
}
