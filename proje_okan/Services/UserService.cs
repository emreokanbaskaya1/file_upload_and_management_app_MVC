using proje_okan.Models;

namespace proje_okan.Services
{
    public class UserService : IUserService
    {
        private readonly List<User> _users = new();

        public User Register(string username, string password)
        {
            var user = new User { Username = username, Password = password };
            _users.Add(user);
            return user;
        }

        public User? Authenticate(string username, string password)
        {
            return _users.FirstOrDefault(u => u.Username == username && u.Password == password);

        }

        public IEnumerable<User> GetAll() => _users;
    }
}
