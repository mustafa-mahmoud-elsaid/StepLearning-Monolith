namespace StepLearning.Shared.Events;

public record EnrolledCourseDetails(string CourseName, string? CourseThumbnailUrl);

public record EnrollmentCompletedEvent(
    string StudentEmail,
    List<EnrolledCourseDetails> Courses,
    DateTime OccurredAtUtc);
