using Courses.Application;
using Courses.Infrastructure;
using Enrollment.Application;
using Host.Api.Middleware;
using Identity.Application;
using Identity.Application.Infrastructure.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// ── Module Registrations ─────────────────────────────────────────
// Each module owns its DI setup. The Host just calls them.
builder.Services.AddCoursesApplication();
builder.Services.AddCoursesInfrastructure(builder.Configuration);
builder.Services.AddIdentityApplication(builder.Configuration);
builder.Services.AddEnrollmentApplication(builder.Configuration);

// ── API Infrastructure ───────────────────────────────────────────
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new()
    {
        Title = "StepLearning API",
        Version = "v1",
        Description = "StepLearning Modular Monolith API"
    });
});

// ── JWT Configurations ───────────────────────────────────────────

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters()
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,


        ValidIssuer = builder.Configuration["JWT:Issuer"],
        ValidAudience = builder.Configuration["JWT:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Key"]!)),
        ClockSkew = TimeSpan.Zero
    };
});


var app = builder.Build();

// ── Middleware Pipeline ──────────────────────────────────────────
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "StepLearning API v1");
    });
}

app.UseHttpsRedirection();
app.UseMiddleware<GlobalExceptionHandlerMiddleware>();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// ── Seed Data ────────────────────────────────────────────────────
await IdentitySeeder.SeedRolesAsync(app.Services);

app.Run();
