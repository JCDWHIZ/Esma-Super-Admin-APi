using System;
using System.ComponentModel.DataAnnotations;
using Application.Interfaces;
using admin_service.Application.School.Queries;
using admin_service.Domain.Entities;
using admin_service.Domain.Enums;

namespace admin_service.Application.School.Commands.EditSchool;


public record IntiateEditSchoolRequestCommand : ICommand<SchoolItemDto>
{
    public string PublicId { get; init; } = string.Empty;
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
}

public record AddressDto
{
    public string State { get; init; } = string.Empty;
    public string Country { get; init; } = string.Empty;
    public string Lga { get; init; } = string.Empty;
    public string StreetAddress { get; init; } = string.Empty;
}

public record SubscriptionDto
{
    public SubscriptionType SubscriptionType { get; init; }
    public DateTime StartDate { get; init; }
    public DateTime EndDate { get; init; }
    public decimal Amount { get; init; }
}




public class EditSchoolRequestCommandHandler : ICommandHandler<IntiateEditSchoolRequestCommand, SchoolItemDto>
{
    private readonly IApplicationDbContext _dbContext;

    private readonly IMapper _mapper;
    public EditSchoolRequestCommandHandler(IApplicationDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public async Task<SchoolItemDto> Handle(IntiateEditSchoolRequestCommand request, CancellationToken cancellationToken)
    {
        var entity = await _dbContext.Schools
                .Include(s => s.Subscriptions) // Explicitly include subscriptions
                .FirstOrDefaultAsync(s => s.PublicId == request.PublicId, cancellationToken);

        Guard.Against.NotFound(request.PublicId, entity);

        entity.SchoolName = request.SchoolName;
        entity.LogoUrl = request.LogoUrl;
        entity.Address = new Address
        {
            State = request.Address.State,
            Country = request.Address.Country,
            LGA = request.Address.Lga,
            StreetAddress = request.Address.StreetAddress
        };
        entity.EmailAddress = request.EmailAddress;
        entity.PhoneNumber = request.PhoneNumber;
        entity.Modules = request.Modules;
        entity.DocumentUrl = request.DocumentUrl;

        // Update existing subscription or create new if not exists
        if (entity.Subscriptions == null)
        {
            entity.Subscriptions = new Subscriptions();
        }

        entity.Subscriptions.subscriptionType = request.Subscriptions.SubscriptionType;
        entity.Subscriptions.StartDate = request.Subscriptions.StartDate;
        entity.Subscriptions.EndDate = request.Subscriptions.EndDate;
        entity.Subscriptions.Amount = request.Subscriptions.Amount;

        await _dbContext.SaveChangesAsync(cancellationToken);
        return _mapper.Map<SchoolItemDto>(entity);
    }
}

