using System;
using SharedKernel;

namespace Domain.Users;

public static class SchoolAdminErrors
{
    public static Error NotFound(Guid publicId) => Error.NotFound(
        "SchoolAdmin.NotFound",
        $"The school admin with the PublicId = '{publicId}' was not found.");

    public static Error NotFoundByEmail(string email) => Error.NotFound(
        "SchoolAdmin.NotFoundByEmail",
        $"The school admin with the email = '{email}' was not found.");

    public static Error NotFoundByUsername(string username) => Error.NotFound(
        "SchoolAdmin.NotFoundByUsername",
        $"The school admin with the username = '{username}' was not found.");

    public static readonly Error EmailAlreadyExists = Error.Conflict(
        "SchoolAdmin.EmailAlreadyExists",
        "A school admin with this email address already exists.");

    public static readonly Error UsernameAlreadyExists = Error.Conflict(
        "SchoolAdmin.UsernameAlreadyExists",
        "A school admin with this username already exists.");

    public static readonly Error InvalidEmail = Error.Failure(
        "SchoolAdmin.InvalidEmail",
        "The email address format is not valid.");

    public static readonly Error InvalidPassword = Error.Failure(
        "SchoolAdmin.InvalidPassword",
        "The password does not meet the required criteria.");

    public static readonly Error InvalidName = Error.Failure(
        "SchoolAdmin.InvalidName",
        "First name and last name are required and cannot exceed 100 characters.");

    public static readonly Error InvalidPhoneNumber = Error.Failure(
        "SchoolAdmin.InvalidPhoneNumber",
        "The phone number format is not valid.");

    public static readonly Error AccountDeactivated = Error.Failure(
        "SchoolAdmin.AccountDeactivated",
        "The school admin account has been deactivated.");
}