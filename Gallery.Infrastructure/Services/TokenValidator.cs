using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace Gallery.Infrastructure.Services
{
    public class TokenValidator : ITokenValidator
    {
        private readonly IUserService _usersService;
        private readonly ITokenStoreService _tokenStoreService;

        public TokenValidator(IUserService usersService, ITokenStoreService tokenStoreService)
        {
            _usersService = usersService;
            _tokenStoreService = tokenStoreService;
        }

        public async Task ValidateAsync(TokenValidatedContext context)
        {
            var userPrincipal = context.Principal;

            var claimsIdentity = userPrincipal.Identity as ClaimsIdentity;
            if (claimsIdentity?.Claims == null || !claimsIdentity.Claims.Any())
            {
                context.Fail("This is not our issued token. It has no claims.");
                return;
            }

            var serialNumber = claimsIdentity.FindFirst(ClaimTypes.SerialNumber);
            if (serialNumber == null)
            {
                context.Fail("This is not our issued token. It has no serial.");
                return;
            }

            var userIdString = claimsIdentity.FindFirst(ClaimTypes.UserData).Value;
            if (!int.TryParse(userIdString, out int userId))
            {
                context.Fail("This is not our issued token. It has no user-id.");
                return;
            }

            var user = await _usersService.GetUserAsync(userId);
            if (user == null || user.SerialNumber != serialNumber.Value || !user.IsLocked)
            {
                context.Fail("This token is expired. Please login again.");
            }

            if (!(context.SecurityToken is JwtSecurityToken accessToken) || string.IsNullOrWhiteSpace(accessToken.RawData) ||
                !await _tokenStoreService.IsValidTokenAsync(accessToken.RawData, userId))
            {
                context.Fail("This token is not in our database.");
                return;
            }

            await _usersService.UpdateUserLastActivityAsync(userId);
        }
    }
}
