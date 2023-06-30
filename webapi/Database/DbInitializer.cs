using webapi.Framework.BaseEnums;
using webapi.Models;

namespace webapi.Database
{
    public static class DbInitializer
    {
        public static void CreateDbIfNotExists(WebApplication? app)
        {
            if (app != null)
            {
                using var scope = app.Services.CreateScope();
                var services = scope.ServiceProvider;
                try
                {
                    var dbContext = services.GetRequiredService<AppDbContext>();
                    // Cria o base de dados se ele não existir
                    if (app.Environment.IsDevelopment())
                        dbContext.Database.EnsureCreated();

                    Initialize(dbContext, app.Environment.IsDevelopment());
                }
                catch (Exception ex)
                {
                    app.Logger.LogError(ex, "An error occurred creating the DB.");
                }
            }
        }

        public static void Initialize(AppDbContext context, bool isDevelopment)
        {
            if(isDevelopment)
                context.Database.EnsureCreated();

            // dotnet ef database update

            var defaultUsers = context.Users.Where(u => u.UserName == Models.User.USER_GUEST || u.UserName == Models.User.USER_ADMIN);
            bool saveChanges = false;

            // Verificar se temos o utilizador de Guest
            if(!defaultUsers.Any(u => u.UserName == Models.User.USER_GUEST))
            {
                // Guest user (Default)
                var guestUser = new Models.User()
                {
                    UserName = Models.User.USER_GUEST,
                    Role = PermissionLevel.Public,
                    State = RowState.Valid
                };

                context.UserManager?.CreateAsync(guestUser, Models.User.USER_GUEST_PASSWORD).Wait();
                saveChanges = true;
            }

            // Verificar se temos o utilizador de Admin
            if (!defaultUsers.Any(u => u.UserName == Models.User.USER_ADMIN))
            {
                // Admin user
                var userAdmin = new Models.User()
                {
                    UserName = Models.User.USER_ADMIN,
                    Role = PermissionLevel.Admin,
                    State = RowState.Valid,
                    Email = "maksym.hrytsko@istec.pt",
                    EmailConfirmed = true,
                    PersonalDetail = new PersonalDetail()
                    {
                        FirstName = "Maksym",
                        LastName = "Hrytsko",
                        Email = "maksym.hrytsko@istec.pt",
                        Phone = "+351 920 000 000",
                        State = RowState.Valid
                    }
                };
                context.UserManager?.CreateAsync(userAdmin, Models.User.USER_ADMIN_PASSWORD).Wait();
                saveChanges = true;
            }

            if (saveChanges)
                context.SaveChanges();
        }
    }
}
