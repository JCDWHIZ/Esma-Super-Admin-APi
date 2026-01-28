using Application.School.CreateSchool;
using Infrastructure.Authorization;

namespace Web.Api.Endpoints.School;

internal sealed class CreateSchool : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/schools", async (Request request, ICommandHandler<CreateSchoolCommand, string> handler, CancellationToken cancellationToken) =>
        {
            var command = new CreateSchoolCommand
            {
                SchoolName = request.SchoolName,
                LogoUrl = request.LogoUrl,
                Address = new AddressDto
                {
                    State = request.Address.State,
                    Country = request.Address.Country,
                    Lga = request.Address.Lga,
                    StreetAddress = request.Address.StreetAddress
                },
                EmailAddress = request.EmailAddress,
                PhoneNumber = request.PhoneNumber,
                DocumentUrl = request.DocumentUrl,
                Modules = request.Modules,
                Subscriptions = new CreateSubscriptionDto
                {
                    SubscriptionType = request.Subscriptions.SubscriptionType,
                    StartDate = request.Subscriptions.StartDate,
                    EndDate = request.Subscriptions.EndDate,
                    Amount = request.Subscriptions.Amount
                },

                SchoolAdmin = new SchoolAdminRequest
                {
                    Role = request.SchoolAdmin.Role,
                    Username = request.SchoolAdmin.Username,
                    FirstName = request.SchoolAdmin.FirstName,
                    LastName = request.SchoolAdmin.LastName,
                    Email = request.SchoolAdmin.Email,
                    PhoneNumber = request.SchoolAdmin.PhoneNumber
                }
            };

            Result<string> result = await handler.Handle(command, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithName("CreateSchool")
        .WithTags(Tags.Schools)
        .Produces<string>(StatusCodes.Status201Created)
        .WithAudit("Created School")
        .RequireAuthorization(new RequirePermissionAttribute("school_create"));
    }

    public sealed class Request()
    {
        public string SchoolName { get; init; } = string.Empty;

        public string LogoUrl { get; init; } = string.Empty;

        public AddressDto Address { get; init; } = new();

        public string EmailAddress { get; init; } = string.Empty;

        public string PhoneNumber { get; init; } = string.Empty;

        public List<string> DocumentUrl { get; init; } = new();

        public List<Modules> Modules { get; init; } = new();

        public CreateSubscriptionDto Subscriptions { get; init; } = new();
        public SchoolAdminRequest SchoolAdmin { get; init; } = new();
    };
}
