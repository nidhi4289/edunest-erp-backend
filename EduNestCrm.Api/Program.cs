using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using EduNestCrm.Api.Model;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

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

// Single dev secret used for issuing & validating
var issuer  = "schoolcrm-dev";
var audience= "schoolcrm-client";
var secret  = "dev-super-secret-key-change-me-32bytes";
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
            ValidateAudience = false,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromMinutes(2)
        };
    });

builder.Services.AddAuthorization();

var app = builder.Build(); // Ensure `app` is initialized here

app.UseCors();
app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthentication();
app.UseAuthorization();

// ---- Health endpoint (should appear in Swagger) ----
app.MapGet("/health", () => Results.Ok(new { ok = true, ts = DateTime.UtcNow }))
   .WithOpenApi();

// ---- Dev login: returns a JWT with a tenant claim ----

app.MapPost("/auth/dev-login", (DevLogin req) =>
{
    var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
    var claims = new[]
    {
        new Claim(JwtRegisteredClaimNames.Sub, req.Email),
        new Claim(ClaimTypes.Name, req.Email),
        new Claim("tenant_id", req.TenantId ?? "demo"),
        new Claim(ClaimTypes.Role, "Admin")
    };
    var token = new JwtSecurityToken(
        issuer: issuer,
        audience: audience,
        claims: claims,
        expires: DateTime.UtcNow.AddHours(8),
        signingCredentials: creds);
    var jwt = new JwtSecurityTokenHandler().WriteToken(token);
    return Results.Ok(new { token = jwt });
})
.WithOpenApi();

// ---- Protected Students endpoint ----
var students = new List<Student> {
    new(1, "Ava Patel", "Grade 6"),
    new(2, "Liam Chen", "Grade 7"),
    new(3, "Maya Singh", "Grade 8"),
};

app.MapGet("/students", (ClaimsPrincipal user) =>
{
    var tenant = user.FindFirst("tenant_id")?.Value ?? "demo";
    // (Use tenant later for schema selection)
    return Results.Ok(students);
})
.RequireAuthorization()
.WithOpenApi();

app.Run();

// ---- Student class definition ----
public record Student(int Id, string Name, string Grade);