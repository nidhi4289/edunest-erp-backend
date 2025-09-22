using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Npgsql;
using EduNestERP.Persistence.Repositories;
using EduNestERP.Application.Interfaces;
using EduNestERP.Application.Services;
using EduNestERP.Application;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddAWSLambdaHosting(LambdaEventSource.HttpApi);
builder.Services.AddMemoryCache();

// ---- CORS for Vite dev server ----
builder.Services.AddCors(o => o.AddDefaultPolicy(p =>
    p.AllowAnyOrigin()
        .AllowAnyHeader()
        .AllowAnyMethod()));

// ---- Swagger/OpenAPI ----
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(o =>
{
    o.SwaggerDoc("v1", new OpenApiInfo { Title = "School CRM API", Version = "v1" });
    var jwtScheme = new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT", 
        In = ParameterLocation.Header,
        Description = "Enter: Bearer {token}"
    };
    o.AddSecurityDefinition("Bearer", jwtScheme);
    o.AddSecurityRequirement(new OpenApiSecurityRequirement { [jwtScheme] = Array.Empty<string>() });
});

var jwtSection = builder.Configuration.GetSection("Jwt");
var issuer  = jwtSection["Issuer"];
var audience= jwtSection["Audience"];
var secret  = jwtSection["Key"];
var key     = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(opts =>
    {
        // DEV: relax issuer/audience so we don’t get 401 while wiring up
        opts.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = key,
            ValidateIssuer = false,
            ValidIssuer = issuer, 
            ValidateAudience = false,
            ValidAudience = audience, 
            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromMinutes(15)
        };
    });

builder.Services.AddAuthorization();
builder.Services.AddHttpContextAccessor();

builder.Services.AddMemoryCache();

// Persistence layer registrations
builder.Services.AddSingleton<TenantDbFactory>();
builder.Services.AddScoped<ITenantAccessor, HttpTenantAccessor>();
builder.Services.AddScoped<ITenantDataSourceProvider, TenantDataSourceProvider>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IStudentRepository, StudentRepository>();
builder.Services.AddScoped<IStaffRepository, StaffRepository>();
builder.Services.AddScoped<IStudentFeeRepository, StudentFeeRepository>();
builder.Services.AddScoped<IMasterDataRepository, MasterDataRepository>();
builder.Services.AddScoped<IAttendanceRepository, AttendanceRepository>();
builder.Services.AddScoped<IHomeworkRepository, HomeworkRepository>();
builder.Services.AddScoped<IStudentMarkRepository, StudentMarkRepository>();
builder.Services.AddScoped<ICommunicationRepository, CommunicationRepository>();
builder.Services.AddScoped<IFeeAdminRepository, FeeAdminRepository>();


builder.Services.AddAutoMapper(typeof(Program));



builder.Services.AddScoped<IStudentService, StudentService>();
builder.Services.AddScoped<IStaffService, StaffService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IStudentFeeService, StudentFeeService>();
builder.Services.AddScoped<IMasterDataService, MasterDataService>();
builder.Services.AddScoped<IAttendanceService, AttendanceService>();
builder.Services.AddScoped<ICommunicationService, CommunicationService>();
builder.Services.AddScoped<IHomeworkService, HomeworkService>();
builder.Services.AddScoped<IStudentMarkService, StudentMarkService>();

builder.Services.AddControllers();

var app = builder.Build(); // Ensure `app` is initialized here
app.UseCors();
app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// ---- Health endpoint (should appear in Swagger) ----
app.MapGet("/health", () => Results.Ok(new { ok = true, ts = DateTime.UtcNow }))
   .WithOpenApi();


app.Run();
