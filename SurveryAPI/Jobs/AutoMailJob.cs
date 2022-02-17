namespace SurveyAPI.Jobs
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using Quartz;
    using SurveyAPI.Helpers;
    using System.Diagnostics;
    using System.Threading.Tasks;

    public class AutoMailJob : IJob
    {
        private readonly IMailSender _mailSender;
        private readonly ApplicationDbContext _context;
        public AutoMailJob(IMailSender mailSender, ApplicationDbContext context)
        {
            _context = context;
            _mailSender = mailSender;
        }

        /// <summary>
        /// Looks for necessary conditions, such as correct send date and haven´t been sent before.
        /// Further on it takes out the correct survey ID and affected email-adresses. 
        /// Then it calls for the sending method. 
        /// </summary>
        /// <param name="context"></param>
        /// <returns>Task completed</returns>
        public Task Execute(IJobExecutionContext context)
        {


            var surveys = _context.Surveys.Where(x => x.IsSent == false && x.sendDate.Date == DateTime.Now.Date).ToList();


            foreach (var survey in surveys)
            {
                if (survey != null && !survey.IsSent)
                {
                    var missionId = _context.MissionSurveys.FirstOrDefault(x => x.SurveyId == survey.SurveyId);

                    if (missionId is not null)
                    {
                        var employees = _context.MissionEmployees.Include(x => x.Employee).Where(x => x.MissionId == missionId.MissionId);


                        if (employees.Any())
                        {
                            foreach (var employee in employees)
                            {

                                _mailSender.SendHtmlGmail(employee.Employee.Email, survey.SurveyId.ToString());
                            }


                            survey.IsSent = true;
                            _context.Surveys.Update(survey);

                        }
                    }


                }

            }


            _context.SaveChanges();

            return Task.CompletedTask;
        }
    }
}
