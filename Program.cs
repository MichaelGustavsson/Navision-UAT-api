using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using navision.api.Data;
using navision.api.Interfaces;
using navision.api.Models;
using navision.api.Repositories;

string AllowedOrigins = "_allowedOrigins";
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<DataContext>(opt => opt.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

IdentityBuilder idBuilder = builder.Services.AddIdentityCore<User>(opt =>
{
  opt.Password.RequireDigit = false;
  opt.Password.RequiredLength = 6;
  opt.Password.RequireNonAlphanumeric = false;
  opt.Password.RequireUppercase = false;
});

idBuilder = new IdentityBuilder(idBuilder.UserType, typeof(Role), builder.Services);
idBuilder.AddEntityFrameworkStores<DataContext>();
idBuilder.AddRoleValidator<RoleValidator<Role>>();
idBuilder.AddRoleManager<RoleManager<Role>>();
idBuilder.AddSignInManager<SignInManager<User>>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(opt =>
    {
      opt.TokenValidationParameters = new TokenValidationParameters
      {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII
              .GetBytes(builder.Configuration.GetSection("AppSettings:Token").Value)),
        ValidateIssuer = false,
        ValidateAudience = false
      };
    });

builder.Services.AddAuthorization(opt =>
{
  opt.AddPolicy("RequireAdminRole", policy => policy.RequireRole("Admin"));
});

builder.Services.AddControllers(opt =>
{
  // Add global authorization demand.
  var policy = new AuthorizationPolicyBuilder()
      .RequireAuthenticatedUser()
      .Build();

  opt.Filters.Add(new AuthorizeFilter(policy));
});

builder.Services.AddScoped<IAuthRepository, AuthRepository>();
builder.Services.AddScoped<IIntegrationRepository, IntegrationRepository>();
builder.Services.AddScoped<IInternalCarsRepository, InternalCarsRepository>();
builder.Services.AddScoped<INaviProRepository, NaviProRepository>();
builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();

builder.Services.AddCors(opt =>
{
  opt.AddPolicy(AllowedOrigins, builder =>
  {
    builder.WithOrigins("*")
    .AllowAnyHeader()
    .AllowAnyHeader();
  });
});

// builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();

app.UseCors(AllowedOrigins);
app.UseHttpsRedirection();

app.UseAuthorization();
app.UseAuthentication();

app.MapControllers();

app.Run();
