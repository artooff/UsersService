namespace UsersService.Application.Interfaces
{
    public interface IPasswordHashService
    {
        public string HashPassword(string password);
        public bool VerifyPassword(string password, string hash);
    }
}
