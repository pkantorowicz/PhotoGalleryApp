using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Gallery.Data.Models;
using Gallery.Data.Repositories;
using Gallery.Infrastructure.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Gallery.Infrastructure.Services
{
    public class TokenStoreService : ITokenStoreService
    {
        private readonly IEncryptionService _encryptionService;
        private readonly IOptionsSnapshot<BearerTokensOptions> _configuration;
        private readonly ITokenRepository _tokenRepository;
        private readonly IRoleService _rolesService;

        public TokenStoreService(IEncryptionService encryptionService, IOptionsSnapshot<BearerTokensOptions> configuration,
            ITokenRepository tokenRepository, IRoleService rolesService)
        {
            _encryptionService = encryptionService;
            _configuration = configuration;
            _tokenRepository = tokenRepository;
            _rolesService = rolesService;
        }

        public async Task AddUserTokenAsync(UserToken userToken)
        {
            if (!_configuration.Value.AllowMultipleLoginsFromTheSameUser)
            {
                await InvalidateUserTokensAsync(userToken.UserId);
            }
            await DeleteTokensWithSameRefreshTokenSourceAsync(userToken.RefreshTokenIdHashSource);
            await _tokenRepository.AddAsync(userToken);
        }

        public async Task AddUserTokenAsync(User user, string refreshToken, string accessToken, string refreshTokenSource)
        {
            var now = DateTimeOffset.UtcNow;
            var token = new UserToken
            {
                UserId = user.Id,
                RefreshTokenIdHash = _encryptionService.GetSha256Hash(refreshToken),
                RefreshTokenIdHashSource = string.IsNullOrWhiteSpace(refreshTokenSource)
                    ? null
                    : _encryptionService.GetSha256Hash(refreshTokenSource),
                AccessTokenHash = _encryptionService.GetSha256Hash(accessToken),
                RefreshTokenExpiresDateTime = now.AddMinutes(_configuration.Value.RefreshTokenExpirationMinutes),
                AccessTokenExpiresDateTime = now.AddMinutes(_configuration.Value.AccessTokenExpirationMinutes)
            };

            await AddUserTokenAsync(token);
        }

        public async Task<bool> IsValidTokenAsync(string accessToken, int userId)
        {
            var accessTokenHash = _encryptionService.GetSha256Hash(accessToken);
            var userToken = await _tokenRepository.GetSingleAsync(t => t.AccessTokenHash == accessTokenHash && t.UserId == userId);

            return userToken?.AccessTokenExpiresDateTime >= DateTimeOffset.UtcNow;
        }

        public async Task DeleteExpiredTokensAsync()
        {
            var now = DateTimeOffset.UtcNow;
            await _tokenRepository.QueryAll().Where(x => x.RefreshTokenExpiresDateTime < now)
                .ForEachAsync(userToken =>
                {
                    _tokenRepository.Delete(userToken);
                });
        }
    

        public Task<UserToken> FindTokenAsync(string refreshToken)
        {
            if (string.IsNullOrWhiteSpace(refreshToken))
            {
                return null;
            }
            var refreshTokenIdHash = _encryptionService.GetSha256Hash(refreshToken);
            return _tokenRepository.QueryAll().Include(x => x.User)
                .FirstOrDefaultAsync(x => x.RefreshTokenIdHash == refreshTokenIdHash);
        }

        public async Task DeleteTokenAsync(string refreshToken)
        {
            var token = await FindTokenAsync(refreshToken);
            if (token != null)
            {
                _tokenRepository.Delete(token);
            }
        }

        public async Task DeleteTokensWithSameRefreshTokenSourceAsync(string refreshTokenIdHashSource)
        {
            if (string.IsNullOrWhiteSpace(refreshTokenIdHashSource))
            {
                return;
            }
            await _tokenRepository.QueryAll().Where(t => t.RefreshTokenIdHashSource == refreshTokenIdHashSource)
                                             .ForEachAsync(userToken =>
                                             {
                                                 _tokenRepository.Delete(userToken);
                                             });
        }

        public async Task InvalidateUserTokensAsync(int userId)
        {
            await _tokenRepository.QueryAll().Where(t => t.UserId == userId)
                                  .ForEachAsync(token =>
                                  {
                                      _tokenRepository.Delete(token);
                                  });
        }

        public async Task<(string accessToken, string refreshToken)> CreateJwtTokens(User user,
            string refreshTokenSource)
        {
            var accessToken = await CreateAccessTokenAsync(user);
            var refreshToken = Guid.NewGuid().ToString().Replace("-", "");
            await AddUserTokenAsync(user, refreshToken, accessToken, refreshTokenSource);
            await _tokenRepository.CommitAsync();

            return (accessToken, refreshToken);
        }

        public async Task RevokeUserBearerTokensAsync(string userIdValue, string refreshToken)
        {
            if (!string.IsNullOrWhiteSpace(userIdValue) && int.TryParse(userIdValue, out int userId))
            {
                if (_configuration.Value.AllowSignoutAllUserActiveClients)
                {
                    await InvalidateUserTokensAsync(userId);
                }
            }

            if (!string.IsNullOrWhiteSpace(refreshToken))
            {
                var refreshTokenIdHash = _encryptionService.GetSha256Hash(refreshToken);
                await DeleteTokensWithSameRefreshTokenSourceAsync(refreshTokenIdHash);
            }

            await DeleteExpiredTokensAsync();
            await _tokenRepository.CommitAsync();
        }

        private async Task<string> CreateAccessTokenAsync(User user)
        {
            var claims = new List<Claim>
            {
                // Unique Id for all Jwt tokes
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                // Issuer
                new Claim(JwtRegisteredClaimNames.Iss, _configuration.Value.Issuer),
                // Issued at
                new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim("Email", user.Email),
                // to invalidate the cookie
                new Claim(ClaimTypes.SerialNumber, user.SerialNumber),
                // custom data
                new Claim(ClaimTypes.UserData, user.Id.ToString())
            };

            // add roles
            var roles = await _rolesService.FindUserRolesAsync(user.Id);
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role.Name));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.Value.Key));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var now = DateTime.UtcNow;
            var token = new JwtSecurityToken(
                issuer: _configuration.Value.Issuer,
                audience: _configuration.Value.Audience,
                claims: claims,
                notBefore: now,
                expires: now.AddMinutes(_configuration.Value.AccessTokenExpirationMinutes),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}