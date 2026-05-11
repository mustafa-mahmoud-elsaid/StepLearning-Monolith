using Courses.Application;
using Courses.Infrastructure;
using Host.Api.Middleware;
using Identity.Application;
using Identity.Application.Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);

// ── Module Registrations ─────────────────────────────────────────
// Each module owns its DI setup. The Host just calls them.
builder.Services.AddCoursesApplication();
builder.Services.AddCoursesInfrastructure(builder.Configuration);
builder.Services.AddIdentityApplication(builder.Configuration);

// ── API Infrastructure ───────────────────────────────────────────
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "StepLearning API",
        Version = "v1",
        Description = "StepLearning Modular Monolith API"
    });
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
