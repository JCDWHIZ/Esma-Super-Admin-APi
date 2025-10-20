using Domain.Schools;
using Domain.Subscriptions;

namespace Application.School.CreateSchool;


// disable school if they haven't paid for their subscription
// add a background job to check for unpaid subscriptions and disable the school if necessary
public sealed class InitiateSchoolRequestHandler(IApplicationDbContext _dbContext) : ICommandHandler<CreateSchoolCommand, string>
{
    public async Task<Result<string>> Handle(CreateSchoolCommand command, CancellationToken cancellationToken)
    {
        Schools? existingSchool = await _dbContext.Schools
            .FirstOrDefaultAsync(x => x.SchoolName == command.SchoolName, cancellationToken);

        if (existingSchool != null)
        {
            throw new Exception("School already exists");
        }

        // Create school entity
        var schoolEntity = new Schools
        {
            SchoolName = command.SchoolName,
            LogoUrl = command.LogoUrl,
            Address = new Address
            {
                State = command.Address.State,
                Country = command.Address.Country,
                LGA = command.Address.Lga,
                StreetAddress = command.Address.StreetAddress
            },
            EmailAddress = command.EmailAddress,
            PhoneNumber = command.PhoneNumber,
            DocumentUrl = command.DocumentUrl,
            Modules = command.Modules,
            Subscriptions = new Subscriptions
            {
                SubscriptionType = command.Subscriptions.SubscriptionType,
                StartDate = command.Subscriptions.StartDate,
                EndDate = command.Subscriptions.EndDate,
                Amount = command.Subscriptions.Amount
            },

            User = new SchoolAdmins
            {
                Role = command.SchoolAdmin.Role,
                Username = command.SchoolAdmin.Username,
                FirstName = command.SchoolAdmin.FirstName,
                LastName = command.SchoolAdmin.LastName,
                Email = command.SchoolAdmin.Email,
                PhoneNumber = command.SchoolAdmin.PhoneNumber
            }
        };
        // _dbContext.Schools.Add(schoolEntity);
        // await _dbContext.SaveChangesAsync(cancellationToken);
        // var organizationId = await _keycloakService.CreateOrganizationAsync(schoolEntity.SchoolName);
        // schoolEntity.OrganizationId = organizationId;
        // await _dbContext.SaveChangesAsync(cancellationToken);

        _dbContext.Schools.Add(schoolEntity);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return Result.Success("School Created Succesfully");
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
