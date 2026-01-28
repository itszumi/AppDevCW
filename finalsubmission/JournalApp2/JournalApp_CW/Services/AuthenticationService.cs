//using Microsoft.EntityFrameworkCore;
//using JournalApp_CW.Data;
//using JournalApp_CW.Models;

//namespace JournalApp_CW.Services
//{
//    public class AuthService
//    {
//        // Holds the logged-in user. If null, no one is logged in.
//        public User? CurrentUser { get; private set; }

//        public bool IsLoggedIn => CurrentUser != null;

//        public async Task<bool> LoginAsync(string username, string password)
//        {
//            using var context = new JournalDbContext();
//            var user = await context.Users.FirstOrDefaultAsync(u => u.Username == username && u.Password == password);

//            if (user != null)
//            {
//                CurrentUser = user;
//                return true;
//            }
//            return false;
//        }

//        public async Task<bool> RegisterAsync(string username, string password)
//        {
//            using var context = new JournalDbContext();

//            // Check if user exists
//            if (await context.Users.AnyAsync(u => u.Username == username))
//                return false;

//            var newUser = new User { Username = username, Password = password };
//            context.Users.Add(newUser);
//            await context.SaveChangesAsync();

//            CurrentUser = newUser; // Auto-login after register
//            return true;
//        }

//        public void Logout()
//        {
//            CurrentUser = null;
//        }
//    }
//}


using Microsoft.EntityFrameworkCore;
using JournalApp_CW.Data;
using JournalApp_CW.Models;

namespace JournalApp_CW.Services
{
    public class AuthService
    {
        public User? CurrentUser { get; private set; }
        public bool IsLoggedIn => CurrentUser != null;

        public async Task<bool> LoginAsync(string username, string password)
        {
            using var context = new JournalDbContext();
            var user = await context.Users.FirstOrDefaultAsync(u => u.Username == username && u.Password == password);
            if (user != null)
            {
                CurrentUser = user;
                return true;
            }
            return false;
        }

        public async Task<bool> RegisterAsync(string username, string password)
        {
            using var context = new JournalDbContext();
            if (await context.Users.AnyAsync(u => u.Username == username))
                return false;

            var newUser = new User { Username = username, Password = password };
            context.Users.Add(newUser);
            await context.SaveChangesAsync();

            CurrentUser = newUser;
            return true;
        }

        public void Logout() => CurrentUser = null;
    }
}
