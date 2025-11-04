using SharedKernel;

namespace Domain.Users;

public static class UserErrors
{
    public static Error NotFound(Guid userId) => Error.NotFound(
        "Users.NotFound",
        $"The user with the Id = '{userId}' was not found");

    public static Error ErrorOccured() => Error.Problem(
            "Users.ErrorOccured",
        "An error occured during the process");
    public static Error NotFound(string userId) => Error.NotFound(
        "Users.NotFound",
        $"The user with the Id = '{userId}' was not found");

    public static Error Unauthorized() => Error.Failure(
        "Users.Unauthorized",
        "You are not authorized to perform this action.");
    public static Error InCorrectCredentials() => Error.Failure(
        "Users.InvalidCredentials",
        "Your credentails are invalid.");
    public static Error InvalidCurrentPassword() => Error.Failure(
        "Users.InvalidCurrentPassword",
        "Your current password is invalid");
    public static Error TokenError() => Error.Problem(
            "Users.TokenExpired",
        "The token sent was either Invalid or expired");

    public static readonly Error NotFoundByEmail = Error.NotFound(
        "Users.NotFoundByEmail",
        "The user with the specified email was not found");
    public static readonly Error NotFoundInToken = Error.NotFound(
        "Users.NotFoundInToken",
        "The userId was not found in token");

    public static readonly Error EmailNotUnique = Error.Conflict(
        "Users.EmailNotUnique",
        "The provided email is not unique");

    public static Error AlreadyExists()
    {
        return Error.AlreadyExists(
            "Users.AlreadyExists",
        "The user with the specified email already exists");
    }
    public static Error PasswordIncorrect()
    {
        return Error.Problem(
            "Users.ErrorOccured",
        "The password specified is incorrect");
    }


}
