using System;
using Application.Abstractions.Models;
using admin_service.Application.Email;
using admin_service.Application.Subscription.Queries;
using admin_service.Application.Subscription.Queries.GetSubscriptionStats;
using admin_service.Application.Subscription.Queries.GetSubscriptionWithPagination;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace admin_service.Web.Endpoints;

public class Subscription : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup(this)
            // .RequireAuthorization()
            .MapGet(GetSubscriptionWithPagination, "subscriptions")
            .MapPost(CreateEmailTest, "mail")
            .MapGet(GetSubscriptionStats, "subscriptions/stats");
    }

    public async Task<Ok<PaginatedList<SubscriptionDto>>> GetSubscriptionWithPagination(ISender sender,
        [AsParameters] GetSubscriptionWithPaginationQuery query)
    {
        var result = await sender.Send(query);
        return TypedResults.Ok(result);
    }


    // public async Task<Ok<object>> GetSubscriptionStats(ISender sender,
    //    GetSubscriptionStatsQuery query)
    // {
    //     var result = await sender.Send(query);
    //     return TypedResults.Ok(result);
    // }

    public async Task<Ok<SubscriptionStatsDto>> GetSubscriptionStats(ISender sender, CancellationToken cancellationToken)
    {
        var query = new GetSubscriptionStatsQuery();
        var result = await sender.Send(query, cancellationToken);
        return TypedResults.Ok(result);
    }
    public async Task<Ok<EmailRequestDto>> CreateEmailTest(ISender sender, [FromBody] InitiateEmailCommand command)
    {
        var result = await sender.Send(command);
        return TypedResults.Ok(result);
    }


}
