using System.ComponentModel.DataAnnotations;
using admin_service.Application.Common.Exceptions;
using admin_service.Application.Common.Interfaces;
using admin_service.Application.School.Queries;
using admin_service.Domain.Entities;
using admin_service.Domain.Enums;

namespace Microsoft.Extensions.DependencyInjection.School.Commands;

public record InitiateSchoolRequestCommand : ICommand<SchoolItemDto>
{
    [Required]
    public string SchoolName { get; init; } = string.Empty;

    public string LogoUrl { get; init; } = string.Empty;

    [Required]
    public AddressDto Address { get; init; } = new();

    [EmailAddress]
    public string EmailAddress { get; init; } = string.Empty;

    [Phone]
    public string PhoneNumber { get; init; } = string.Empty;

    public List<string> DocumentUrl { get; init; } = new();

    public List<Modules> Modules { get; init; } = new();

    [Required]
    public SubscriptionDto Subscriptions { get; init; } = new();
    [Required]
    public UserDto User { get; init; } = new();

}

public record AddressDto
{
    public string State { get; init; } = string.Empty;
    public string Country { get; init; } = string.Empty;
    public string Lga { get; init; } = string.Empty;
    public string StreetAddress { get; init; } = string.Empty;
}

public record UserDto
{
    public Roles Role { get; init; }
    public string Username { get; init; } = string.Empty;
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string PhoneNumber { get; init; } = string.Empty;
}

public record SubscriptionDto
{
    public SubscriptionType SubscriptionType { get; init; }
    public DateTime StartDate { get; init; }
    public DateTime EndDate { get; init; }
    public decimal Amount { get; init; }
}


public class InitiateSchoolRequestHandler : ICommandHandler<InitiateSchoolRequestCommand, SchoolItemDto>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly KeycloakService _keycloakService;

    private readonly IMapper _mapper;
    public InitiateSchoolRequestHandler(KeycloakService keycloakService, IApplicationDbContext dbContext, IMapper mapper)
    {
        _keycloakService = keycloakService;
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public async Task<SchoolItemDto> Handle(InitiateSchoolRequestCommand request, CancellationToken cancellationToken)
    {
        var existingSchool = await _dbContext.Schools
            .FirstOrDefaultAsync(x => x.SchoolName == request.SchoolName, cancellationToken);

        if (existingSchool != null)
        {
            throw new AlreadyExistsException("School already exists");
        }

        // Create school entity
        var schoolEntity = new Schools
        {
            SchoolName = request.SchoolName,
            LogoUrl = request.LogoUrl,
            Address = new Address
            {
                State = request.Address.State,
                Country = request.Address.Country,
                LGA = request.Address.Lga,
                StreetAddress = request.Address.StreetAddress
            },
            EmailAddress = request.EmailAddress,
            PhoneNumber = request.PhoneNumber,
            DocumentUrl = request.DocumentUrl,
            Modules = request.Modules,
            Subscriptions = new Subscriptions
            {
                subscriptionType = request.Subscriptions.SubscriptionType,
                StartDate = request.Subscriptions.StartDate,
                EndDate = request.Subscriptions.EndDate,
                Amount = request.Subscriptions.Amount
            },

            User = new User
            {
                Role = request.User.Role,
                Username = request.User.Username,
                FirstName = request.User.FirstName,
                LastName = request.User.LastName,
                Email = request.User.Email,
                PhoneNumber = request.User.PhoneNumber
            }
        };
        // _dbContext.Schools.Add(schoolEntity);
        // await _dbContext.SaveChangesAsync(cancellationToken);
        // var organizationId = await _keycloakService.CreateOrganizationAsync(schoolEntity.SchoolName);
        // schoolEntity.OrganizationId = organizationId;
        // await _dbContext.SaveChangesAsync(cancellationToken);

        _dbContext.Schools.Add(schoolEntity);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return _mapper.Map<SchoolItemDto>(schoolEntity);
    }

    // async Task<SchoolItemDto> ICommandHandler<IntiateSchoolRequestCommand, SchoolItemDto>.Handle(IntiateSchoolRequestCommand request, CancellationToken cancellationToken)
    // {
    //     var entity = new Schools {
    //         SchoolName = request.SchoolName,
    //         Address = request.Address,
    //         DocumentUrl = request.DocumentUrl,
    //         EmailAddress = request.EmailAddress,
    //         LogoUrl = request.LogoUrl,
    //         Modules = request.Modules,
    //         PhoneNumber = request.PhoneNumber,
    //         Subscriptions = new Subscriptions 
    //         {
    //             StartDate = request.Subscriptions.StartDate,
    //             EndDate = request.Subscriptions.EndDate,
    //             Amount = request.Subscriptions.Amount,
    //             subscriptionType = request.Subscriptions.subscriptionType
    //         }
    //     };
    //     if(_dbContext.Schools.Any(x => x.SchoolName == request.SchoolName))
    //     {
    //         throw new Exception("School already exists");
    //     }
    //     Guard.Against.NotFound(request.SchoolName, entity);
    //     _dbContext.Schools.Add(entity);
    //     await _dbContext.SaveChangesAsync(cancellationToken);
    //     return _mapper.Map<SchoolItemDto>(entity);
    // }
}
