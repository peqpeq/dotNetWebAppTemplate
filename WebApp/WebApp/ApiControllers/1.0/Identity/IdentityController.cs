using System.Threading.Tasks;
using Domain.App.Identity;
using Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using PublicApi.DTO.v1.DTO.Items.Identity;

namespace WebApp.ApiControllers._1._0.Identity
{
/// <summary>
    /// Api endpoint for registering new user and user log-in (jwt token generation)
    /// </summary>
    [Route("api/v{version:apiVersion}/[controller]/[action]")]
    [ApiController]
    [ApiVersion("1.0")]

    public class AccountController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly UserManager<AppUser> _userManager;
        private readonly ILogger<AccountController> _logger;
        private readonly SignInManager<AppUser> _signInManager;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="configuration">Configuration</param>
        /// <param name="userManager">User manager</param>
        /// <param name="logger">Logger</param>
        /// <param name="signInManager">Sign in manager</param>
        public AccountController(IConfiguration configuration, UserManager<AppUser> userManager,
            ILogger<AccountController> logger, SignInManager<AppUser> signInManager)
        {
            _configuration = configuration;
            _userManager = userManager;
            _logger = logger;
            _signInManager = signInManager;
        }

        /// <summary>
        /// Endpoint for user log-in (jwt generation)
        /// </summary>
        /// <param name="model">Login dto</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult<string>> Login([FromBody]LoginDTO model)
        {
            var appUser = await _userManager.FindByEmailAsync(model.Email);

            if (appUser == null)
            {
                _logger.LogInformation($"Web-Api login. User {model.Email} not found!");
                return StatusCode(403);
            }

            var result = await _signInManager.CheckPasswordSignInAsync(appUser, model.Password, false);
            if (result.Succeeded)
            { 
                var claimsPrincipal = await _signInManager.CreateUserPrincipalAsync(appUser); //get the User analog
                var jwt = IdentityExtensions.GenerateJWT(claimsPrincipal.Claims,
                    _configuration["JWT:SigningKey"],
                    _configuration["JWT:Issuer"],
                    _configuration.GetValue<int>("JWT:ExpirationInDays")
                    );
                
                return Ok(new {token = jwt, status = "Logged in"});
            }
            
            _logger.LogInformation($"Web-Api login. User {model.Email} attempted to log-in with bad password!");
            return StatusCode(403);
        }

        
        /// <summary>
        /// Endpoint for user registration and immediate log-in (jwt generation) 
        /// </summary>
        /// <param name="model">Register DTO</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult<string>> Register([FromBody]RegisterDTO model)
        {
            var appUser = new AppUser()
            {
                UserName = model.Email,
                Email = model.Email,
                Name = model.Name
            };
            var result = await _userManager.CreateAsync(appUser, model.Password);
            if (result.Succeeded)
            { 
                var claimsPrincipal = await _signInManager.CreateUserPrincipalAsync(appUser); //get the User analog
                var jwt = IdentityExtensions.GenerateJWT(claimsPrincipal.Claims,
                    _configuration["JWT:SigningKey"],
                    _configuration["JWT:Issuer"],
                    _configuration.GetValue<int>("JWT:ExpirationInDays")
                );
                
                return Ok(new {token = jwt, status = "Registered and logged-in"});
            }
            
            return StatusCode(500);
        }
    }

}