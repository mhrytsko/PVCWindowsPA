using Microsoft.EntityFrameworkCore;
using webapi.Database;
using Microsoft.Extensions;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.FileProviders;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using webapi.Framework;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System.Security.Claims;
using webapi.Framework.BaseEnums;
using Microsoft.Extensions.Logging.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using Microsoft.AspNetCore.StaticFiles;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers(options =>
{
    options.ModelMetadataDetailsProviders.Add(new SystemTextJsonValidationMetadataProvider());

    options.ModelBinderProviders.Insert(0, new webapi.Framework.GuidModelBinderProvider());
}).ConfigureApiBehaviorOptions(options =>
{
    //options.SuppressInferBindingSourcesForParameters = true;
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            Description = "JWT Authorization header using the Bearer scheme",
            Type = SecuritySchemeType.Http,
            Scheme = "bearer"
        });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                },
                Array.Empty<string>()
            }
        });
});

//builder.Logging.AddEventLog();

// Configurar a autentificação com JWT
builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters()
        {
            ClockSkew = TimeSpan.Zero,
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration.GetValue<string>("Jwt:Issuer") ?? webapi.Security.Authentification.TEMP_ISSUER,
            ValidAudience = builder.Configuration.GetValue<string>("Jwt:Audience") ?? webapi.Security.Authentification.TEMP_AUDIENCE,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration.GetValue<string>("Jwt:Key") ?? webapi.Security.Authentification.TEMP_TOKEN)
            )
        };
    });

// Politicas de autorizações
builder.Services
    .AddAuthorization(options =>
    {
        // Public access
        options.AddPolicy("Public", policy =>
        {
            policy.AddAuthenticationSchemes(new[] { JwtBearerDefaults.AuthenticationScheme });
            policy.RequireAssertion(context => 
                (context?.User?.Identity?.IsAuthenticated ?? false) == false ||
                context.User.HasClaim(c => c.Type == ClaimTypes.Role 
                    && Enum.TryParse<PermissionLevel>(c.Value, out var role) && role >= PermissionLevel.Public)
            );
        });

        // Logged in
        options.AddPolicy("Client", policy =>
        {
            policy.AddAuthenticationSchemes(new[] { JwtBearerDefaults.AuthenticationScheme });
            policy.RequireAssertion(context =>
                context.User.HasClaim(c => c.Type == ClaimTypes.Role 
                    && Enum.TryParse<PermissionLevel>(c.Value, out var role) && role >= PermissionLevel.Client)
            );
        });

        // Technic
        options.AddPolicy("Technic", policy =>
        {
            policy.AddAuthenticationSchemes(new[] { JwtBearerDefaults.AuthenticationScheme });
            policy.RequireAssertion(context =>
                context.User.HasClaim(c => c.Type == ClaimTypes.Role
                    && Enum.TryParse<PermissionLevel>(c.Value, out var role) && role >= PermissionLevel.Technic)
            );
        });

        // Admin
        options.AddPolicy("Admin", policy =>
        {
            policy.AddAuthenticationSchemes(new[] { JwtBearerDefaults.AuthenticationScheme });
            policy.RequireAssertion(context =>
                context.User.HasClaim(c => c.Type == ClaimTypes.Role 
                    && Enum.TryParse<PermissionLevel>(c.Value, out var role) && role >= PermissionLevel.Admin )
            );
        });
    });

// Para aceder a contexto HTTP
builder.Services
    .AddHttpContextAccessor();

builder.Services
    .AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services
    .AddScoped<UserContext, UserContext>();


// Register the Database Context
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? builder.Configuration.GetConnectionString("LocalSqlServer");
if (string.IsNullOrEmpty(connectionString))
    throw new Exception("Erro - The connection string is missing!");

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(connectionString);
});


// Registar o Identity provider
builder.Services.AddIdentity<webapi.Models.User, IdentityRole<Guid>>(options =>
    {
        //options.SignIn.RequireConfirmedAccount = false;
        //options.User.RequireUniqueEmail = true;
        options.Password.RequireDigit = false;
        options.Password.RequiredLength = 5;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequireUppercase = false;
        options.Password.RequireLowercase = false;

        options.Lockout.MaxFailedAccessAttempts = 3;
        options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);// TODO: Production, 3h
    })
    .AddEntityFrameworkStores<AppDbContext>();
//.AddDefaultTokenProviders();

// Auto mapper
builder.Services
    .AddAutoMapper(options =>
    {
        options.AddProfile<MappingProfile>();
    });

var app = builder.Build();

app.UseMiddleware<ExceptionHandlerMiddleware>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.EnableDeepLinking();
        options.EnablePersistAuthorization();
    });
}
else
{
    // Adicionar os MIME types
    var provider = new FileExtensionContentTypeProvider();
    provider.Mappings[".hdr"] = "image/vnd.radiance";
    provider.Mappings[".gltf"] = "model/gltf+json";
    provider.Mappings[".glb"] = "model/gltf-binary";

    app.UseDefaultFiles();
    app.UseStaticFiles(new StaticFileOptions
    {
        ContentTypeProvider = provider
    });
}

// Criar a base de dados caso se ainda não existir
DbInitializer.CreateDbIfNotExists(app);

if (!app.Environment.IsDevelopment())
{
    //app.UseHttpsRedirection();
    //app.UseCors();
}


// Ativar a autentificação
app.UseAuthentication();
app.UseAuthorization();


app.MapControllers();

app.Run();
