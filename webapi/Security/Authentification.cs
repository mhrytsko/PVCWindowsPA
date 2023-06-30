using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using webapi.Database;

namespace webapi.Security
{
    public class Authentification
    {
        public static readonly string TEMP_TOKEN = Guid.NewGuid().ToString();
        public static readonly string TEMP_AUDIENCE = "MH";
        public static readonly string TEMP_ISSUER = "MH";

        public static readonly int TEMP_EXPIRES = 12;

        private readonly AppDbContext _context;
        private readonly UserManager<Models.User> _userManager;

        public Authentification(AppDbContext context, UserManager<Models.User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<Models.User?> GetUserByUsernameAndPassword(string username, string password)
        {
            var user = await _context.Users
                .Where(u => u.UserName == username)
                .Include(u => u.PersonalDetail)
                .FirstOrDefaultAsync();

            if(user?.State == Framework.BaseEnums.RowState.Valid)
            {
                //var _password = Utils.Crypt(password);
                var pswValid = await _userManager.CheckPasswordAsync(user, password);

                if (pswValid)
                    return user;
            }

            return null;
        }
    }
}
