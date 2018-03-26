using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace Gallery.Infrastructure.Services
{
    public interface ITokenValidator : IService
    {
        Task ValidateAsync(TokenValidatedContext context);
    }
}
