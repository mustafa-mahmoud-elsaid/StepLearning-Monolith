using Bogus;
using EFCore.BulkExtensions;
using Enrollment.Application.Data;
using Enrollment.Application.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Commerce.Application.Payment.Domain.Entities;
using Commerce.Infrastructure.Data;

namespace Host.Api.DataSeeders;

public static class PaymentAndEnrollmentSeeder
{
    public static async Task SeedAsync(IServiceProvider serviceProvider, List<Guid> studentIds, List<Guid> courseIds)
    {
        if (studentIds == null || studentIds.Count == 0 || courseIds == null || courseIds.Count == 0)
            return;

        using var scope = serviceProvider.CreateScope();
        var paymentContext = scope.ServiceProvider.GetRequiredService<CommerceDbContext>();
        var enrollmentContext = scope.ServiceProvider.GetRequiredService<EnrollmentDbContext>();

        // If either context has data, skip seeding to prevent duplicates
        //if (await paymentContext.PaymentRecords.AnyAsync() || await enrollmentContext.Enrollments.AnyAsync())
        //    return;

        var faker = new Faker("en");

        var payments = new List<PaymentRecord>();
        // Fully qualified to avoid ambiguity if there are other Enrollment classes
        var enrollments = new List<Enrollment.Application.Domain.Entities.Enrollment>();

        int batchSize = 500; // Batch students to manage memory effectively
        
        paymentContext.ChangeTracker.AutoDetectChangesEnabled = false;
        enrollmentContext.ChangeTracker.AutoDetectChangesEnabled = false;

        try
        {
            for (int b = 0; b < studentIds.Count; b += batchSize)
            {
                var batchStudents = studentIds.Skip(b).Take(batchSize).ToList();

                payments.Clear();
                enrollments.Clear();

                foreach (var studentId in batchStudents)
                {
                    // Pick random number of courses based on given requirements (0, 1, 3, 5, 8...)
                    int[] possibleCourseCounts = { 0, 1, 3, 5, 8 };
                    int courseCountToEnroll = faker.PickRandom(possibleCourseCounts);

                    if (courseCountToEnroll == 0)
                        continue;

                    // Ensure we don't pick more courses than exist
                    courseCountToEnroll = Math.Min(courseCountToEnroll, courseIds.Count);
                    
                    var selectedCourses = faker.PickRandom(courseIds, courseCountToEnroll).ToList();

                    foreach (var courseId in selectedCourses)
                    {
                        var amount = Math.Round(faker.Random.Decimal(10, 200), 2);
                        
                        // Create payment intent
                        var payment = PaymentRecord.CreateCheckout(studentId, courseId, amount);

                        bool isSuccess = faker.Random.Bool(0.8f); // 80% of payments succeed

                        if (isSuccess)
                        {
                            payment.MarkSucceeded(faker.Random.AlphaNumeric(12).ToUpper());
                            
                            // If payment succeeds, they get enrolled
                            var enrollment = Enrollment.Application.Domain.Entities.Enrollment.Create(
                                studentId, 
                                courseId, 
                                payment.Id, 
                                EnrollmentStatus.Active);
                            
                            enrollments.Add(enrollment);
                        }
                        else
                        {
                            payment.MarkFailed();
                        }

                        payments.Add(payment);
                    }
                }

                var bulkConfig = new BulkConfig { BulkCopyTimeout = 0 };

                // Insert into their respective DbContexts using BulkExtensions
                if (payments.Any())
                    await paymentContext.BulkInsertAsync(payments, bulkConfig);
                
                if (enrollments.Any())
                    await enrollmentContext.BulkInsertAsync(enrollments, bulkConfig);

                // Flush tracking memory
                paymentContext.ChangeTracker.Clear();
                enrollmentContext.ChangeTracker.Clear();
            }
        }
        finally
        {
            paymentContext.ChangeTracker.AutoDetectChangesEnabled = true;
            enrollmentContext.ChangeTracker.AutoDetectChangesEnabled = true;
        }
    }
}
