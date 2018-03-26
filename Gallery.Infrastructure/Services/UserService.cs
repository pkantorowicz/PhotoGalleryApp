using System;
using System.Threading.Tasks;
using Gallery.Data.Models;
using Gallery.Data.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Gallery.Infrastructure.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IUserRoleRepository _userRoleRepository;
        private readonly IEncryptionService _encryptionService;

        public UserService(IUserRepository userRepository, IRoleRepository roleRepository,
            IUserRoleRepository userRoleRepository, IEncryptionService encryptionService)
        {
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _userRoleRepository = userRoleRepository;
            _encryptionService = encryptionService;
        }

        public async Task<User> GetUserAsync(int userId)
            => await _userRepository.GetSingleAsync(userId);

        public async Task<User> FindUserAsync(string username, string password)
        {
            var user = await _userRepository.GetSingleByUsernameAsync(username);

            var passwordHash = _encryptionService.EncryptPassword(password, user.Salt);
            return await _userRepository.QueryAll().FirstOrDefaultAsync
                (x => x.Username == username && x.HashedPassword == passwordHash);
        }

        public async Task LoginAsync(string username, string password)
        {
            var user = await _userRepository.GetSingleByUsernameAsync(username);

            if (user == null)
            {
                throw new Exception("Invalid Credentials.");
            }

            var hash = _encryptionService.EncryptPassword(password, user.Salt);
            if (user.HashedPassword == hash)
            {
                return;
            }
            throw new Exception("Invalid Credentials.");           
        }

        public async Task<User> RegisterAsync(string username, string email, string password, int[] roles)
        {
            var userCheck = await _userRepository.GetSingleByUsernameAsync(username);

            if (userCheck != null)
            {
                throw new Exception("User with this username already exists.");
            }

            var passwordSalt = _encryptionService.CreateSalt();

            var user = new User
            {
                Username = username,
                Salt = passwordSalt,
                Email = email,
                IsLocked = false,
                HashedPassword = _encryptionService.EncryptPassword(password, passwordSalt),
                DateCreated = DateTime.Now
            };

            await _userRepository.AddAsync(user);
            await _userRepository.CommitAsync();

            if (roles != null || roles.Length > 0)
            {
                foreach (var role in roles)
                {
                    AddUserToRole(user, role);
                }
            }

            await _userRepository.CommitAsync();

            return user;
        }

        public async Task UpdateUserLastActivityAsync(int userId)
        {
            var user = await GetUserAsync(userId);
            if (user.LastLoggedIn != null)
            {
                var upgLastActivity = TimeSpan.FromMinutes(2);
                var currentUtc = DateTimeOffset.UtcNow;
                var timeElapsed = currentUtc.Subtract(user.LastLoggedIn.Value);
                if (timeElapsed < upgLastActivity)
                {
                    return;
                }
            }
            user.LastLoggedIn = DateTimeOffset.UtcNow;
            await _userRepository.CommitAsync();
        }

        private void AddUserToRole(User user, int roleId)
        {
            var role = _roleRepository.GetSingle(roleId);

            if (role == null)
                throw new Exception("Role could not be found.");

            var userRole = new UserRole
            {
                RoleId = role.Id,
                UserId = user.Id
            };

            _userRoleRepository.AddAsync(userRole);
            _userRepository.CommitAsync();
        }
    }
}