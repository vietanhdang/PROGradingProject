using BusinessLogic;
using BusinessLogic.Base;
using BusinessLogic.Service;
using Common.Helpers;
using BusinessLogic.Hubs;
using DataAccess.DatabaseContext;
using DataAccess.IRepository;
using DataAccess.Repository;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using PROGradingAPI.Attributes;
using PROGradingAPI.Middleware;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddSignalR();

builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.SuppressModelStateInvalidFilter = true;
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("CORSPolicy", builder => builder.AllowAnyMethod().AllowAnyHeader().AllowCredentials().SetIsOriginAllowed((hosts) => true));
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddRouting(option => option.LowercaseUrls = true);
builder.Services.AddAutoMapper(typeof(Program).Assembly, typeof(Common.Mappings.AutoMapperProfile).Assembly);
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(option =>
{
    var jwtKey = builder.Configuration["Jwt:Key"];
    if (string.IsNullOrEmpty(jwtKey))
    {
        throw new Exception("Jwt key is not configured");
    }
    option.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        ValidateIssuer = false,
        ValidateAudience = false,
        ClockSkew = TimeSpan.Zero,
        ValidateLifetime = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
    };
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("ViewInfoPolicy", policy =>
    {
        policy.Requirements.Add(new Requirement());
    });
});
builder.Services.AddSingleton<IAuthorizationHandler, ViewInfoAuthorizationHandler>();


// IOC Container
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<GradingSignalR>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IAutoMarkService, AutoMarkService>();
builder.Services.AddScoped<IExamService, ExamService>();
builder.Services.AddSingleton<IJwtHelper, JwtHelper>();
builder.Services.AddDbContext<AppDbContext>(option => option.UseSqlServer(builder.Configuration.GetConnectionString("SQLConnection")));
builder.Services.AddScoped(typeof(ICache<>), typeof(MemoryCache<>));
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<IAccountRepository, AccountRepository>();


var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseCors("CORSPolicy");
app.UseRouting();
app.UseAuthentication();
app.UseMiddleware<AuthHandlerMiddleware>();
app.UseAuthorization();
app.UseHttpsRedirection();
app.MapControllers();
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
    endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller}/{action}/{userId?}"
    );
    endpoints.MapHub<GradingSignalR>("/gradingsignalr");
});
app.UseMiddleware<ErrorHandlerMiddleware>();

app.Run();
