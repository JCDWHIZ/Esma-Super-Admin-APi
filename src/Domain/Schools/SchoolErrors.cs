using SharedKernel;

namespace Domain.Schools;

public static class SchoolErrors
{
    public static Error NotFound(Guid publicId) => Error.NotFound(
        "School.NotFound",
        $"The school with the PublicId = '{publicId}' was not found.");
    public static Error NotFoundList(List<Guid>? ids)
    {
        return Error.NotFound(
       "School.NotFound",
       $"The school with the PublicId = '{string.Join(", ", ids ?? new List<Guid>())}' was not found.");
    }

    public static Error NotFoundByOrganization(string organizationId) => Error.NotFound(
        "School.NotFoundByOrganization",
        $"The school with the OrganizationId = '{organizationId}' was not found.");

    public static readonly Error Unauthorized = Error.Failure(
        "School.Unauthorized",
        "You are not authorized to perform this action.");

    public static readonly Error SchoolAlreadyApproved = Error.Conflict(
        "School.AlreadyApproved",
        "The school is already approved.");

    public static readonly Error SchoolAlreadyRejected = Error.Conflict(
        "School.AlreadyRejected",
        "The school is already rejected.");

    public static readonly Error InvalidSchoolName = Error.Failure(
        "School.InvalidSchoolName",
        "The school name cannot be empty or exceed 255 characters.");

    public static readonly Error InvalidEmailAddress = Error.Failure(
        "School.InvalidEmailAddress",
        "The email address format is not valid.");

    public static readonly Error EmailAlreadyExists = Error.Conflict(
        "School.EmailAlreadyExists",
        "A school with this email address already exists.");

    public static readonly Error SubscriptionRequired = Error.Failure(
        "School.SubscriptionRequired",
        "An active subscription is required to perform this action.");

    public static readonly Error InvalidPhoneNumber = Error.Failure(
        "School.InvalidPhoneNumber",
        "The phone number format is not valid.");

    public static readonly Error InvalidModuleKeys = Error.Failure(
        "School.InvalidModuleKeys",
        "One or more module keys are invalid.");
}
