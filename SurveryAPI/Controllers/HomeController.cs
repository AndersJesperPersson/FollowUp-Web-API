namespace SurveyAPI.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using SurveyAPI.Helpers;


    [Route("api/home")]
    [ApiController]
    public class HomeController : Controller
    {
        private readonly IMailSender _mailsender;
        public HomeController(IMailSender mailSender)
        {
            _mailsender = mailSender;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            _mailsender.SendHtmlGmail("92jesper@gafe.molndal.se");

            return Ok();
        }
    }
}

