using SharedKernel;

namespace Domain.Templates;
public static class TemplateErrors
{
    public static Error NotFound(Guid templateId) => Error.NotFound(
        "Template.NotFound",
        $"The Template with the Id = '{templateId}' was not found");

    public static Error ErrorOccured() => Error.Problem(
            "Template.ErrorOccured",
        "An error occured during the process");

    public static Error Unauthorized() => Error.Failure(
        "Template.Unauthorized",
        "You are not authorized to perform this action.");
    public static Error AlreadyExists()
    {
        return Error.AlreadyExists(
            "Template.AlreadyExists",
        "The Template with the specified details already exists");
    }
}

