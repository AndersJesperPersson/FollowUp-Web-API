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


        /// <summary>
        /// To get all the answers that are connected with the correct survey and question. 
        /// </summary>
        /// <param name="id">question id</param>
        /// <param name="surveyId">survey id</param>
        /// <returns>List of answers connected to right survey and question</returns>
        [HttpPost("read/{id}")]
        public async Task<ActionResult<IEnumerable<Answer>>> GetAnswersRead(Guid id, SurveyIdDTO surveyId)
        {
            Guid newSurveyId = Guid.Parse(surveyId.Id);

            return await _context.SurveysAnswers
                .Include(x => x.Answer).ThenInclude(x => x.Question)
                .Where(x => x.SurveyId == newSurveyId)
                .Where(x => x.Answer.Question.Id == id)
                .Select(x => x.Answer).ToListAsync();
        }



/// <summary>
/// Create a new Answers. Comes in through a list cause the answers is created when answering a survey. 
/// </summary>
/// <param name="Id">Survey Id to create the relation between rows in db.</param>
/// <param name="answers">List of Answers.</param>
/// <returns></returns>
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
