using proje_okan.Models;

namespace proje_okan.Services
{
    public interface IUserService
    {
        User? Authenticate(string username, string password);
        User Register(string username, string password);
        IEnumerable<User> GetAll();
    }
}
