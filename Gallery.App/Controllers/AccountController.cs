using System.Security.Claims;
using System.Threading.Tasks;
using Gallery.Data.Models;
using Gallery.Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace Gallery.App.Controllers
{
    [Route("api/[controller]")]
    public class AccountController : Controller
    {
        private readonly IUserService _userService;
        private readonly ITokenStoreService _tokenStoreService;

        public AccountController(IUserService userService,
            ITokenStoreService tokenStoreService)
        {
            _userService = userService;
            _tokenStoreService = tokenStoreService;
        }

        [AllowAnonymous]
        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] User loginUser)
        {
            if (loginUser == null)
            {
                return BadRequest("user is not set.");
            }

            var user = await _userService.FindUserAsync(loginUser.Username, loginUser.HashedPassword);
            if (user == null || !user.IsLocked)
            {
                return Unauthorized();
            }

            var (accessToken, refreshToken) = await _tokenStoreService.CreateJwtTokens(user, refreshTokenSource: null);
            return Ok(new { access_token = accessToken, refresh_token = refreshToken });
        }

        [AllowAnonymous]
        [HttpPost("RefreshToken")]
        public async Task<IActionResult> RefreshToken([FromBody]JToken jsonBody)
        {
            var refreshToken = jsonBody.Value<string>("refreshToken");
            if (string.IsNullOrWhiteSpace(refreshToken))
            {
                return BadRequest("refreshToken is not set.");
            }

            var token = await _tokenStoreService.FindTokenAsync(refreshToken);
            if (token == null)
            {
                return Unauthorized();
            }

            var (accessToken, newRefreshToken) = await _tokenStoreService.CreateJwtTokens(token.User, refreshToken);
            return Ok(new { access_token = accessToken, refresh_token = newRefreshToken });
        }

        [AllowAnonymous]
        [HttpGet("Logout"), HttpPost("Logout")]
        public async Task<bool> Logout([FromBody]JToken jsonBody)
        {
            var claimsIdentity = this.User.Identity as ClaimsIdentity;
            var userIdValue = claimsIdentity.FindFirst(ClaimTypes.UserData)?.Value;
            var refreshToken = jsonBody.Value<string>("refreshToken");

            await _tokenStoreService.RevokeUserBearerTokensAsync(userIdValue, refreshToken);

            return true;
        }

        [HttpGet("[action]"), HttpPost("[action]")]
        public bool IsAuthenticated()
        {
            return User.Identity.IsAuthenticated;
        }

        [HttpGet("Admin"), HttpPost("Admin")]
        public IActionResult GetUserInfo()
        {
            var claimsIdentity = User.Identity as ClaimsIdentity;
            return Json(new { Username = claimsIdentity.Name });
        }

        [HttpGet("account")]
        public IActionResult Get([FromBody] User request)
            => Content($"Hello {User.Identity.Name}");
    }
}

