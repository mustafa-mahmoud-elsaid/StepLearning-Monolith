using Bogus;
using EFCore.BulkExtensions;
using Identity.Application.Domain;
using Identity.Application.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
namespace Identity.Application.Infrastructure.Data;

public static class IdentitySeeder
{
    /// <summary>
    /// Seeds the required application roles into the database.
    /// Call this during application startup after building the host.
    /// </summary>
    public static async Task SeedRolesAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();

        foreach (var role in AppRoles.All)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole<Guid>(role));
            }
        }
    }

    /// <summary>
    /// Seeds students using Bogus and EFCore.BulkExtensions.
    /// Returns the generated Student IDs.
    /// </summary>
    public static async Task<List<Guid>> SeedStudentsAsync(IServiceProvider serviceProvider, int count = 4000)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<UsersDbContext>();
        
        // If already seeded, return existing IDs
        //if (await context.Students.AnyAsync())
        //{
        //    return await context.Students.Select(s => s.Id).ToListAsync();
        //}

        var generatedIds = new List<Guid>();
        var studentRole = await context.Roles.FirstOrDefaultAsync(r => r.Name == AppRoles.Student);

        if (studentRole == null)
            throw new Exception("Student role not seeded yet. Please call SeedRolesAsync first.");

        var passwordHasher = new PasswordHasher<ApplicationUser>();
        // Hash password once to avoid CPU overload
        var dummyUser = new ApplicationUser();
        var fixedPasswordHash = passwordHasher.HashPassword(dummyUser, "Password@123!");

        int batchSize = 1000;
        var faker = new Faker("en");

        try
        {
            context.ChangeTracker.AutoDetectChangesEnabled = false;

            // Seed Students in batches
            for (int b = 0; b < count; b += batchSize)
            {
                int currentBatchSize = Math.Min(batchSize, count - b);
                var users = new List<ApplicationUser>(currentBatchSize);
                var students = new List<Student>(currentBatchSize);
                var userRoles = new List<IdentityUserRole<Guid>>(currentBatchSize);

                for (int i = 0; i < currentBatchSize; i++)
                {
                    var email = faker.Internet.Email(uniqueSuffix: Guid.NewGuid().ToString()[..8]);
                    var user = ApplicationUser.Create(email);
                    user.Id = Guid.NewGuid();
                    user.NormalizedEmail = email.ToUpper();
                    user.NormalizedUserName = email.ToUpper();
                    user.SecurityStamp = Guid.NewGuid().ToString();
                    user.PasswordHash = fixedPasswordHash;
                
                    users.Add(user);
                    userRoles.Add(new IdentityUserRole<Guid> { UserId = user.Id, RoleId = studentRole.Id });

                    var student = Student.Create(
                        faker.Name.FullName(),
                        user.Id,
                        faker.Date.PastDateOnly(20, DateOnly.FromDateTime(DateTime.Today.AddYears(-10))),
                        faker.Internet.Avatar()
                    );
                    students.Add(student);
                    generatedIds.Add(student.Id);
                }

                // Bulk Insert in patches via EFCore.BulkExtensions (disable timeout)
                var bulkConfig = new BulkConfig { BulkCopyTimeout = 0 };
                await context.BulkInsertAsync(users, bulkConfig);
                await context.BulkInsertAsync(userRoles, bulkConfig);
                await context.BulkInsertAsync(students, bulkConfig);
            
                // Clear change tracker to ensure no memory leaks
                context.ChangeTracker.Clear();
            }
        }
        finally
        {
            context.ChangeTracker.AutoDetectChangesEnabled = true;
        }

        return generatedIds;
    }

    /// <summary>
    /// Seeds instructors using Bogus and EFCore.BulkExtensions.
    /// Returns the generated Instructor IDs.
    /// </summary>
    public static async Task<List<Guid>> SeedInstructorsAsync(IServiceProvider serviceProvider, int count = 500)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<UsersDbContext>();
        
        // If already seeded, return existing IDs
        //if (await context.Instructors.AnyAsync())
        //{
        //    return await context.Instructors.Select(i => i.Id).ToListAsync();
        //}

        var generatedIds = new List<Guid>();
        var instructorRole = await context.Roles.FirstOrDefaultAsync(r => r.Name == AppRoles.Instructor);

        if (instructorRole == null)
            throw new Exception("Instructor role not seeded yet. Please call SeedRolesAsync first.");

        var passwordHasher = new PasswordHasher<ApplicationUser>();
        var dummyUser = new ApplicationUser();
        var fixedPasswordHash = passwordHasher.HashPassword(dummyUser, "Password@123!");

        int batchSize = 1000;
        var faker = new Faker("en");

        try
        {
            context.ChangeTracker.AutoDetectChangesEnabled = false;

            // Seed Instructors in batches
            for (int b = 0; b < count; b += batchSize)
            {
                int currentBatchSize = Math.Min(batchSize, count - b);
                var users = new List<ApplicationUser>(currentBatchSize);
                var instructors = new List<Instructor>(currentBatchSize);
                var userRoles = new List<IdentityUserRole<Guid>>(currentBatchSize);

                for (int i = 0; i < currentBatchSize; i++)
                {
                    var email = faker.Internet.Email(uniqueSuffix: Guid.NewGuid().ToString()[..8]);
                    var user = ApplicationUser.Create(email);
                    user.Id = Guid.NewGuid();
                    user.NormalizedEmail = email.ToUpper();
                    user.NormalizedUserName = email.ToUpper();
                    user.SecurityStamp = Guid.NewGuid().ToString();
                    user.PasswordHash = fixedPasswordHash;
                
                    users.Add(user);
                    userRoles.Add(new IdentityUserRole<Guid> { UserId = user.Id, RoleId = instructorRole.Id });

                    var instructor = Instructor.Create(
                        faker.Name.FirstName(),
                        faker.Name.LastName(),
                        user.Id,
                        faker.Internet.Avatar(),
                        faker.Lorem.Paragraph()
                    );
                    instructors.Add(instructor);
                    generatedIds.Add(instructor.Id);
                }

                var bulkConfig = new BulkConfig { BulkCopyTimeout = 0 };
                await context.BulkInsertAsync(users, bulkConfig);
                await context.BulkInsertAsync(userRoles, bulkConfig);
                await context.BulkInsertAsync(instructors, bulkConfig);

                // Clear change tracker
                context.ChangeTracker.Clear();
            }
        }
        finally
        {
            context.ChangeTracker.AutoDetectChangesEnabled = true;
        }

        return generatedIds;
    }
}
