using SharedKernel;

namespace Domain.HelpRequests;

public static class HelpRequestMessageErrors
{
    public static Error NotFound(Guid publicId) => Error.NotFound(
        "HelpRequestMessage.NotFound",
        $"The help request message with the PublicId = '{publicId}' was not found.");

    public static readonly Error InvalidTitle = Error.Failure(
        "HelpRequestMessage.InvalidTitle",
        "The message title cannot exceed 255 characters.");

    public static readonly Error AttachmentTooLarge = Error.Failure(
        "HelpRequestMessage.AttachmentTooLarge",
        "One or more attachments exceed the maximum allowed size.");

    public static readonly Error InvalidAttachmentFormat = Error.Failure(
        "HelpRequestMessage.InvalidAttachmentFormat",
        "One or more attachments have an invalid format.");

    public static readonly Error CannotAddToClosedRequest = Error.Conflict(
        "HelpRequestMessage.CannotAddToClosedRequest",
        "Cannot add messages to a closed help request.");
}