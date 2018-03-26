namespace Gallery.Infrastructure.Services
{
    public interface IEncryptionService : IService
    {
        string CreateSalt();
        string GetSha256Hash(string input);
        string EncryptPassword(string password, string salt);
    }
}
