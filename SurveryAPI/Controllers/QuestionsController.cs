#nullable disable

using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SurveyAPI.DTO;
using SurveyAPI.Helpers;
using SurveyAPI.Models;

namespace SurveyAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuestionsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public QuestionsController(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;

        }
        // GET
        [HttpGet()]
        public async Task<ActionResult<IEnumerable<QuestionDTO>>> GetQuestion([FromQuery] PaginationDTO paginationDTO)
        {
            var queryable = _context.Questions.AsQueryable();
            await HttpContext.InsertParametersPagninationsInHeader(queryable);

            var question = await queryable.ToArrayAsync(); // välj här hur önskar sortera responsen. Kan va nice med bokstav på missions.

            if (question == null)
            {
                return NotFound();
            }

            return _mapper.Map<List<QuestionDTO>>(question);
        }
        // GET: api/Answers/5
        [HttpGet("{id}")]
        public async Task<ActionResult<QuestionDTO>> GetQuestion(Guid id)
        {
            var question = await _context.Questions.FindAsync(id);

            if (question == null)
            {
                return NotFound();
            }

            return _mapper.Map<QuestionDTO>(question); ;
        }


        // POST: api/Questions
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<QuestionDTO>> PostQuestion(QuestionCreationDTO questionCreationDTO)
        {

            var question = _mapper.Map<Question>(questionCreationDTO);
            _context.Questions.Add(question);
            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpPut]
        public async Task<IActionResult> Put(Guid id, Question question)
        {
            if (id != question.Id)
            {
                return BadRequest();
            }

            _context.Entry(question).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!QuestionExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Ok();

        }

        private bool QuestionExists(Guid id)
        {
            return _context.Questions.Any(e => e.Id == id);

        }



    }
}
