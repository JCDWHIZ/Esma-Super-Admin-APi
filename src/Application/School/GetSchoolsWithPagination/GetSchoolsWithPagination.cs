using System;
using Application.Interfaces;
using admin_service.Application.Common.Mappings;
using Application.Abstractions.Models;
using admin_service.Application.TodoItems.Queries.GetTodoItemsWithPagination;
using admin_service.Domain.Entities;
using admin_service.Domain.Enums;
using Microsoft.AspNetCore.Mvc;

namespace admin_service.Application.School.Queries.GetSchoolsWithPagination;


public record GetSchoolsWithPaginationQuery : ICommand<PaginatedList<SchoolItemDto>>
{
    public string? SchoolName { get; set; }
    public string? LogoUrl { get; set; }
    public string? AddressCountry { get; set; }
    public string? AddressState { get; set; }
    public string? AddressLga { get; set; }
    public string? AddressStreetAddress { get; set; }
    public string? EmailAddress { get; set; }
    public bool? Subscribed { get; set; }
    public SchoolStatus? Status { get; set; }
    public string? PhoneNumber { get; set; }
    // public ICollection<string>? DocumentUrl { get; set; }
    // public ICollection<Modules>? Modules { get; set; }
    public SubscriptionType? SubscriptionType { get; set; }
    public int? PageNumber { get; set; } = 1;
    public int? PageSize { get; set; } = 10;
}

public class GetShoolsWithPaginationQueryHandler(IApplicationDbContext context, IMapper mapper) : ICommandHandler<GetSchoolsWithPaginationQuery,
    PaginatedList<SchoolItemDto>>
{

    private readonly IApplicationDbContext _context = context;
    private readonly IMapper _mapper = mapper;


    public Task<PaginatedList<SchoolItemDto>> Handle(GetSchoolsWithPaginationQuery request, CancellationToken cancellationToken)
    {
        // var query = _context.Schools.AsQueryable();

        // query = query.ApplySchoolFilters(
        //     request.SchoolName,
        //     request.PhoneNumber,
        //     request.EmailAddress,
        //     request.LogoUrl,
        //     request.Address,
        //     // request.DocumentUrl,
        //     // request.Modules,
        //     request.Subscribed,
        //     request.Status,
        //     request.SubscriptionType);
        var query = _context.Schools.AsQueryable();

        if (!string.IsNullOrEmpty(request.SchoolName))
        {
            query = query.Where(x => x.SchoolName.Contains(request.SchoolName));
        }

        if (!string.IsNullOrEmpty(request.PhoneNumber))
        {
            query = query.Where(x => x.PhoneNumber.Contains(request.PhoneNumber));
        }

        if (!string.IsNullOrEmpty(request.EmailAddress))
        {
            query = query.Where(x => x.EmailAddress.Contains(request.EmailAddress));
        }

        if (!string.IsNullOrEmpty(request.LogoUrl))
        {
            query = query.Where(x => x.LogoUrl.Contains(request.LogoUrl));
        }

        if (!string.IsNullOrEmpty(request.AddressCountry))
        {
            query = query.Where(x => x.Address.Country == request.AddressCountry);
        }

        if (!string.IsNullOrEmpty(request.AddressState))
        {
            query = query.Where(x => x.Address.State == request.AddressState);
        }

        if (!string.IsNullOrEmpty(request.AddressLga))
        {
            query = query.Where(x => x.Address.LGA == request.AddressLga);
        }

        if (!string.IsNullOrEmpty(request.AddressStreetAddress))
        {
            query = query.Where(x => x.Address.StreetAddress == request.AddressStreetAddress);
        }


        if (request.Subscribed.HasValue)
        {
            query = query.Where(x => x.Subscribed == request.Subscribed.Value);
        }

        if (request.Status.HasValue)
        {
            query = query.Where(x => x.Status == request.Status.Value);
        }

        if (request.SubscriptionType.HasValue)
        {
            query = query.Where(x => x.Subscriptions.subscriptionType == request.SubscriptionType.Value);
        }

        return query
            .ProjectTo<SchoolItemDto>(_mapper.ConfigurationProvider)
           .PaginatedListAsync(request.PageNumber ?? 1, request.PageSize ?? 10);
    }
}