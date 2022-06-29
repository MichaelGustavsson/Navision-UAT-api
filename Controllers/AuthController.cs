using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using navision.api.Interfaces;
using navision.api.Models;
using navision.api.ViewModels;

namespace navision.api.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class AuthController : ControllerBase
  {
    private readonly IAuthRepository _repo;
    private readonly IConfiguration _config;
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;
    public AuthController(IAuthRepository repo, IConfiguration config, UserManager<User> userManager, SignInManager<User> signInManager)
    {
      _signInManager = signInManager;
      _userManager = userManager;
      _config = config;
      _repo = repo;
    }

    [HttpPost("register")]
    // [Authorize(Policy = "RequireAdminRole")]
    public async Task<IActionResult> Register([FromBody] UserRegisterViewModel user)
    {
      if (!ModelState.IsValid)
      {
        return BadRequest(ModelState);
      }

      var userToCreate = new User
      {
        UserName = user.Username
      };

      var result = await _userManager.CreateAsync(userToCreate, user.Password);

      if (result.Succeeded)
      {
        var addedUser = _userManager.FindByNameAsync(user.Username).Result;
        await _userManager.AddToRoleAsync(addedUser, "User");
        return StatusCode(201);
      }

      return BadRequest(result.Errors);
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login(UserLoginViewModel model)
    {
      var user = await _userManager.FindByNameAsync(model.Username);

      if (user == null)
      {
        var errorMessage = new { message = $"Användare {model.Username} finns inte eller så är namnet felstavat" };

        var options = new JsonSerializerOptions
        {
          Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
          WriteIndented = true
        };
        var jsonString = JsonSerializer.Serialize(errorMessage, options);

        return BadRequest(jsonString);
      }
      var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, false);

      if (result.Succeeded)
      {
        return Ok(new
        {
          token = GenerateJwtToken(user),
          user
        });
      }

      return Unauthorized();
    }

    private async Task<string> GenerateJwtToken(User user)
    {
      var claims = new List<Claim>{
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.UserName)
            };

      var roles = await _userManager.GetRolesAsync(user);

      foreach (var role in roles)
      {
        claims.Add(new Claim(ClaimTypes.Role, role));
      }

      var secret = _config.GetSection("AppSettings:Token");

      var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.GetSection("AppSettings:Token").Value));
      var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

      var tokenDescriptor = new SecurityTokenDescriptor
      {
        Subject = new ClaimsIdentity(claims),
        Expires = DateTime.Now.AddDays(17),
        SigningCredentials = creds
      };

      var tokenHandler = new JwtSecurityTokenHandler();
      var token = tokenHandler.CreateToken(tokenDescriptor);
      return tokenHandler.WriteToken(token);
    }
  }
}