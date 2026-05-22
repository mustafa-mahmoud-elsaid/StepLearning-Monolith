using Courses.Application;
using Courses.Infrastructure;
using Enrollment.Application;
using Enrollment.Infrastructure;
using Enrollment.Application.Consumers;
using Host.Api.Messaging;
using Host.Api.Middleware;
using Identity.Application;
using Identity.Infrastructure;
using Host.Api.DataSeeders;
using Courses.Infrastructure.Data;
using Identity.Infrastructure.Data;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Notifications.Infrastructure;
using Notifications.Infrastructure.Consumers;
using Commerce.Application;
using Commerce.Infrastructure;
using Commerce.Infrastructure.Data;
using Enrollment.Infrastructure.Data;
using StepLearning.Shared.Abstraction;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// ── Module Registrations ─────────────────────────────────────────
// Each module owns its DI setup. The Host just calls them.
builder.Services.AddCoursesApplication();
builder.Services.AddCoursesInfrastructure(builder.Configuration);
builder.Services.AddIdentityApplication(builder.Configuration);
builder.Services.AddIdentityInfrastructure(builder.Configuration);
builder.Services.AddEnrollmentApplication(builder.Configuration);
builder.Services.AddEnrollmentInfrastructure(builder.Configuration);
builder.Services.AddCommerceApplication();
builder.Services.AddCommerceInfrastructure(builder.Configuration);
builder.Services.AddNotificationsInfrastructure();

builder.Services.AddScoped<IIntegrationEventPublisher, MassTransitIntegrationEventPublisher>();
builder.Services.AddMassTransit(cfg =>
{
    cfg.AddConsumer<PaymentSucceededConsumer>();
    cfg.AddConsumer<EnrollmentCompletedNotificationConsumer>();

    cfg.UsingRabbitMq((context, rabbit) =>
    {
        var host = builder.Configuration["RabbitMq:Host"] ?? "localhost";
        var virtualHost = builder.Configuration["RabbitMq:VirtualHost"] ?? "/";
        var username = builder.Configuration["RabbitMq:Username"] ?? "guest";
        var password = builder.Configuration["RabbitMq:Password"] ?? "guest";
        var port = builder.Configuration.GetValue<ushort>("RabbitMq:Port", 5672);

        rabbit.Host(host, port, virtualHost, h =>
        {
            h.Username(username);
            h.Password(password);
        });

        rabbit.ConfigureEndpoints(context);
    });
});

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

// ── Run Pending Migrations ───────────────────────────────────────
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    services.GetRequiredService<CoursesDbContext>().Database.Migrate();
    services.GetRequiredService<UsersDbContext>().Database.Migrate();
    services.GetRequiredService<EnrollmentDbContext>().Database.Migrate();
    services.GetRequiredService<CommerceDbContext>().Database.Migrate();
}

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

app.MapPost("/api/seed", async (IServiceProvider sp, ILogger<Program> logger) =>
{
    var sw = System.Diagnostics.Stopwatch.StartNew();
    logger.LogInformation("Starting database seeding process...");

    try
    {
        // 1. Roles
        logger.LogInformation("Step 1/4: Seeding Roles...");
        await IdentitySeeder.SeedRolesAsync(sp);

        // 2. Identity (Students & Instructors)
        logger.LogInformation("Step 2/4: Seeding Students & Instructors...");
        var studentIds = await IdentitySeeder.SeedStudentsAsync(sp, 4000);
        var instructorIds = await IdentitySeeder.SeedInstructorsAsync(sp, 500);

        // 3. Courses, Sections, Items
        logger.LogInformation("Step 3/4: Seeding Courses, Sections, and Items...");
        var courseIds = await CourseSeeder.SeedCoursesAsync(sp, instructorIds);

        // 4. Payments & Enrollments
        logger.LogInformation("Step 4/4: Seeding Payments & Enrollments...");
        await PaymentAndEnrollmentSeeder.SeedAsync(sp, studentIds, courseIds);

        sw.Stop();
        logger.LogInformation("Seeding completed successfully in {ElapsedMilliseconds}ms.", sw.ElapsedMilliseconds);

        return Results.Ok(new 
        { 
            Message = "Database seeded successfully!",
            ElapsedMilliseconds = sw.ElapsedMilliseconds,
            StudentsSeeded = studentIds.Count,
            InstructorsSeeded = instructorIds.Count,
            CoursesSeeded = courseIds.Count
        });
    }
    catch (Exception ex)
    {
        sw.Stop();
        logger.LogError(ex, "Seeding failed after {ElapsedMilliseconds}ms.", sw.ElapsedMilliseconds);
        return Results.Problem(detail: ex.Message, title: "Seeding Failed");
    }
})
.WithName("SeedDatabase")
.WithTags("System");
app.Run();
