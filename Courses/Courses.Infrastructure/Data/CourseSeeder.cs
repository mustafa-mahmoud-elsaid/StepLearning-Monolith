using Bogus;
using Courses.Domain.Entities;
using EFCore.BulkExtensions;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Courses.Infrastructure.Data;

public static class CourseSeeder
{
    public static async Task<List<Guid>> SeedCoursesAsync(IServiceProvider serviceProvider, List<Guid> instructorIds)
    {
        if (instructorIds == null || instructorIds.Count == 0)
            return new List<Guid>();

        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<CoursesDbContext>();

        // If courses already exist, assume seeding is done to avoid duplication.
        //if (await context.Courses.AnyAsync())
        //    return await context.Courses.Select(c => c.Id).ToListAsync();

        var generatedCourseIds = new List<Guid>();

        var faker = new Faker("en");
        
        var courses = new List<Course>();
        var sections = new List<Section>();
        var sectionItems = new List<SectionItem>();

        int batchSize = 100; // Batch by instructors to manage memory efficiently

        context.ChangeTracker.AutoDetectChangesEnabled = false;

        try
        {
            for (int b = 0; b < instructorIds.Count; b += batchSize)
            {
                var batchInstructors = instructorIds.Skip(b).Take(batchSize).ToList();
                
                // Clear lists per batch
                courses.Clear();
                sections.Clear();
                sectionItems.Clear();

                foreach (var instructorId in batchInstructors)
                {
                    // Generate 1 to 3 courses per instructor
                    int numCourses = faker.Random.Int(1, 3); 
                    for (int c = 0; c < numCourses; c++)
                    {
                        var course = Course.Create(faker.Commerce.ProductName() + " Course", faker.Lorem.Paragraphs(2), instructorId);
                        course.Update(null, null, Math.Round(faker.Random.Decimal(10, 200), 2));
                        SetDates(course);
                        
                        courses.Add(course);
                        generatedCourseIds.Add(course.Id);

                        // Generate 3 to 6 sections per course
                        int numSections = faker.Random.Int(3, 6);
                        for (int s = 0; s < numSections; s++)
                        {
                            var section = Section.Create(
                                title: faker.Commerce.Department() + " Fundamentals",
                                courseId: course.Id,
                                displayOrder: s + 1
                            );
                            
                            // 80% published
                            if (faker.Random.Bool(0.8f))
                            {
                                typeof(Section).GetProperty("IsPublished")?.SetValue(section, true);
                            }

                            SetDates(section);
                            sections.Add(section);

                            // Generate 2 to 5 items per section
                            int numItems = faker.Random.Int(2, 5);
                            for (int i = 0; i < numItems; i++)
                            {
                                bool isVideo = faker.Random.Bool(0.7f); // 70% video
                                if (isVideo)
                                {
                                    var video = VideoItem.Create(
                                        title: faker.Commerce.ProductMaterial() + " Tutorial",
                                        videoUrl: faker.Internet.Url() + "/video.mp4",
                                        duration: TimeSpan.FromMinutes(faker.Random.Int(3, 20)),
                                        sectionId: section.Id,
                                        displayOrder: i + 1
                                    );
                                    SetDates(video);
                                    sectionItems.Add(video);
                                }
                                else
                                {
                                    var pdf = PdfItem.Create(
                                        title: faker.Commerce.ProductMaterial() + " Reference",
                                        fileUrl: faker.Internet.Url() + "/resource.pdf",
                                        sectionId: section.Id,
                                        displayOrder: i + 1
                                    );
                                    SetDates(pdf);
                                    sectionItems.Add(pdf);
                                }
                            }
                        }
                    }
                }

                // Bulk Insert for the current batch (with disabled timeout for large seeds)
                var bulkConfig = new BulkConfig { BulkCopyTimeout = 0 };
                await context.BulkInsertAsync(courses, bulkConfig);
                await context.BulkInsertAsync(sections, bulkConfig);
                await context.BulkInsertAsync(sectionItems, bulkConfig);
                
                // Clear tracker memory
                context.ChangeTracker.Clear();
            }
        }
        finally
        {
            context.ChangeTracker.AutoDetectChangesEnabled = true;
        }

        return generatedCourseIds;
    }

    /// <summary>
    /// BaseEntity properties are protected, we use reflection to set them for Bulk Insertion.
    /// Standard SaveChanges handles this automatically, but BulkInsert skips EF trackers.
    /// </summary>
    private static void SetDates(BaseEntity entity)
    {
        var now = DateTimeOffset.UtcNow;
        typeof(BaseEntity).GetProperty("CreatedAt")?.SetValue(entity, now);
        typeof(BaseEntity).GetProperty("UpdatedAt")?.SetValue(entity, now);
    }
}
