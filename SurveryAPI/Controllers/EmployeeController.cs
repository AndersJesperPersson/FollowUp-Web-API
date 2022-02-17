namespace SurveyAPI.Controllers
{
    using AutoMapper;
    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using SurveyAPI.DTO;
    using SurveyAPI.Models;

    [Route("api/employee")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "IsAdmin")]
    public class EmployeeController : ControllerBase
    {

        private readonly ApplicationDbContext _context;
        private readonly ILogger<AnswersController> _logger;
        private readonly IMapper _mapper;
        public EmployeeController(ApplicationDbContext context, ILogger<AnswersController> logger, IMapper mapper)
        {
            _context = context;
            _logger = logger;
            _mapper = mapper;
        }

        [HttpPost("{id}")]
        public async Task<ActionResult<Employee>> PostAnswer(Guid Id, [FromBody] List<EmployeeCreationDTO> employees)
        {
            var employeeList = new List<Employee>();

            foreach (var employeeItem in employees)
            {
               
                var employee = new Employee()
                {
                    Id = new Guid(),
                    Email = employeeItem.Email
                };

                _context.Add(employee);
                employeeList.Add(employee);

            }

            var mission = await _context.Missions.Include(x => x.MissionEmployees).ThenInclude(x => x.Employee).FirstOrDefaultAsync(x => x.Id == Id);

      


            if (mission is not null)
            {
                foreach (var employee in employeeList)
                {
                    mission.MissionEmployees.Add(new MissionEmployees { MissionId = mission.Id, EmployeeId = employee.Id });
                    _context.Update(mission);


                }
            }


            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
