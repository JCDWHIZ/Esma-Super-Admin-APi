using System;
using SharedKernel;

namespace Domain.HelpRequests;

public static class HelpRequestErrors
{
    public static Error NotFound(Guid publicId) => Error.NotFound(
        "HelpRequest.NotFound",
        $"The help request with the PublicId = '{publicId}' was not found.");

    public static Error NotFoundByTicket(string ticketId) => Error.NotFound(
        "HelpRequest.NotFoundByTicket",
        $"The help request with the TicketId = '{ticketId}' was not found.");

    public static readonly Error AlreadyClosed = Error.Conflict(
        "HelpRequest.AlreadyClosed",
        "The help request is already closed.");

    public static readonly Error CannotReopenResolved = Error.Conflict(
        "HelpRequest.CannotReopenResolved",
        "Cannot reopen a resolved help request.");

    public static readonly Error InvalidStatus = Error.Failure(
        "HelpRequest.InvalidStatus",
        "The specified help request status is not valid.");

    public static readonly Error InvalidCategory = Error.Failure(
        "HelpRequest.InvalidCategory",
        "The specified help request category is not valid.");
}