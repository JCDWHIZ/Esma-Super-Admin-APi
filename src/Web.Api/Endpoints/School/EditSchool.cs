using Application.School.EditSchool;
using Infrastructure.Authorization;
using AddressDto = Application.School.EditSchool.AddressDto;
using SubscriptionDto = Application.School.EditSchool.SubscriptionDto;

namespace Web.Api.Endpoints.School;

internal sealed class EditSchool : IEndpoint
{
    public sealed class Request
    {
        public string SchoolName { get; init; } = string.Empty;
        public string LogoUrl { get; init; } = string.Empty;
        public AddressDto Address { get; init; } = new();
        public string EmailAddress { get; init; } = string.Empty;
        public string PhoneNumber { get; init; } = string.Empty;
        public List<string> DocumentUrl { get; init; } = new();
        public List<Modules> Modules { get; init; } = new();
        public SubscriptionDto Subscriptions { get; init; } = new();
    }

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("schools/{publicId:guid}", async (
            Guid publicId,
            Request request,
            ICommandHandler<EditSchoolCommand, string> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new EditSchoolCommand
            {
                PublicId = publicId,
                SchoolName = request.SchoolName,
                LogoUrl = request.LogoUrl,
                Address = request.Address,
                EmailAddress = request.EmailAddress,
                PhoneNumber = request.PhoneNumber,
                DocumentUrl = request.DocumentUrl,
                Modules = request.Modules,
                Subscriptions = request.Subscriptions
            };

            Result<string> result = await handler.Handle(command, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithTags(Tags.Schools)
        .Produces<string>(StatusCodes.Status200OK)
        .WithAudit("Edited School")
        .RequireAuthorization(new RequirePermissionAttribute("school_edit"));
    }
}
