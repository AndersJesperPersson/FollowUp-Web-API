namespace SurveyAPI.Controllers
{
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.IdentityModel.Tokens;
    using SurveyAPI.DTO;
    using System.IdentityModel.Tokens.Jwt;
    using System.Security.Claims;
    using System.Text;

    [ApiController]
    [Route("api/users")]
    public class UserController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IConfiguration _configuration;
        public UserController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
        }

        /// <summary>
        /// Creates a user in db and if suceeded a token is returned to frontend.
        /// </summary>
        /// <param name="userCredentials"></param>
        /// <returns></returns>
        /// 



        [HttpPost("create")]
        public async Task<ActionResult<AuthenticationResponse>> Create([FromBody] UserCredentials userCredentials)
        {
            var user = new IdentityUser {  UserName = userCredentials.Email, Email = userCredentials.Email };
            var result = await _userManager.CreateAsync(user, userCredentials.Password);

            if (result.Succeeded)
            {
                return await BuildToken(userCredentials);
            }
            else
            {
                return BadRequest(result.Errors);
            }
        }

        /// <summary>
        /// Creates a JWT token, that conceal vunerble user data.
        /// </summary>
        /// <param name="userCredentials"></param>
        /// <returns>The token with userinformation and when it´s expired.</returns>
        private async Task<AuthenticationResponse> BuildToken(UserCredentials userCredentials)
        {
            var claims = new List<Claim>()
            {
                new Claim("email", userCredentials.Email), // as little information as possible cause it´s possible to see in frontend.
          
            };
            var user = await _userManager.FindByEmailAsync(userCredentials.Email);
            var userClaims = await _userManager.GetClaimsAsync(user);

            if (userClaims.Count > 0)
            {
            claims.AddRange(userClaims);
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["keyjwt"]));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var expiration = DateTime.UtcNow.AddYears(1);

            var token = new JwtSecurityToken(issuer: null, audience: null, claims: claims,
                expires: expiration, signingCredentials: credentials);

            return new AuthenticationResponse()
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                Expiration = expiration
            };


        }

        [HttpPost("login")]
        public async Task <ActionResult<AuthenticationResponse>> Login(
            [FromBody] UserCredentials userCredentials)
        {
            var result = await _signInManager.PasswordSignInAsync(userCredentials.Email, userCredentials.Password, isPersistent: false, lockoutOnFailure: false);  // change to true in prod? isPersisent decides if its a sessions cookie or perment. permenet stannar efter browser stängs.
           
            if (result.Succeeded)
            {
                
                return await BuildToken(userCredentials);
            }
            else
            {
                return BadRequest("Inloggningen misslyckades"); // dont give to much information, more safe to not give away that only the password is missing for example. 
            }
        }


    }
}
