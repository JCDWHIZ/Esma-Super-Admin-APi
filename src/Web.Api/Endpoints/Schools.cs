using System;
using Application.Interfaces;
using Application.Abstractions.Models;
using admin_service.Application.School.Commands.ApproveSchool;
using admin_service.Application.School.Commands.DeleteSchool;
using admin_service.Application.School.Commands.EditSchool;
using admin_service.Application.School.Commands.RestoreDeletedSchool;
using admin_service.Application.School.Commands.SoftDeleteSchool;
using admin_service.Application.School.Queries;
using admin_service.Application.School.Queries.GetDeletedSchoolWithPagination;
using admin_service.Application.School.Queries.GetSchoolStats;
using admin_service.Application.School.Queries.GetSchoolsWithById;
using admin_service.Application.School.Queries.GetSchoolsWithPagination;
using admin_service.Application.SoftDelete.Commands;
using admin_service.Application.SoftDelete.Commands.RestoreDeleteCommand;
using admin_service.Domain.Entities;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection.School.Commands;

namespace admin_service.Web.Endpoints;

public class Schools : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup(this)
        // .RequireAuthorization()
            .MapGet(GetSchoolById, "{publicId}")
            .MapGet(GetSchoolStats, "stats")
            .MapGet(GetSchoolDashBoard, "dashboard/stats")
            .MapPut(UpdateSchool, "{publicId}")
             .MapPost(ApproveSchool, "approve/{Id}")
            .MapPost(CreateSchool, "/")
            // .MapDelete(DeleteSchool, "{publicId}")
            .MapGet(GetDeletedSchools, "deleted")
            .MapDelete(SoftDeleteSchool, "{publicId}")
            .MapGet(GetSchoolWithPagination, "/");
    }

    public async Task<Ok<SchoolItemDto>> CreateSchool(
        ISender sender,
        [FromBody] InitiateSchoolRequestCommand command)
    {
        var result = await sender.Send(command);
        return TypedResults.Ok(result);
    }

    public async Task<Results<Ok, BadRequest>> ApproveSchool(
        ISender sender,
        int id,
        ApproveSchoolCommand command)
    {
        if (id != command.Id)
            return TypedResults.BadRequest();
        await sender.Send(command);
        return TypedResults.Ok();
    }

    public async Task<Results<Ok<SchoolItemDto>, NotFound>> GetSchoolById(
        ISender sender,
        string publicId)
    {
        var result = await sender.Send(new GetSchoolByIdQuery(publicId));
        return TypedResults.Ok(result);
    }

    public async Task<Results<Ok, BadRequest>> UpdateSchool(
        ISender sender,
        string publicId,
        [FromBody] IntiateEditSchoolRequestCommand command)
    {
        if (publicId != command.PublicId)
            return TypedResults.BadRequest();
        await sender.Send(command);
        return TypedResults.Ok();
    }

    // public async Task<Ok> DeleteSchool(
    //     ISender sender, 
    //     string publicId)
    // {
    //     await sender.Send(new DeleteSchoolCommand(publicId));
    //     return TypedResults.Ok();
    // }

    public async Task<Ok<SchoolStatsCountDto>> GetSchoolStats(
        ISender sender,
        CancellationToken cancellationToken)
    {
        var query = new GetSchoolStatsQuery();
        var result = await sender.Send(query, cancellationToken);
        return TypedResults.Ok(result);
    }

    public async Task<Ok<List<YearlyOverviewDto>>> GetSchoolDashBoard(
        ISender sender,
        CancellationToken cancellationToken)
    {
        var query = new GetSchoolDashboardQuery();
        var result = await sender.Send(query, cancellationToken);
        return TypedResults.Ok(result);
    }

    public async Task<Results<Ok, NotFound>> SoftDeleteSchool(
        ISender sender,
        string publicId)
    {
        await sender.Send(new SoftDeleteSchoolCommand(publicId));
        return TypedResults.Ok();
    }

    public async Task<Results<Ok, NotFound>> RestoreSchool(
        ISender sender,
        string publicId)
    {
        await sender.Send(new RestoreDeletedSchoolCommand(publicId));
        return TypedResults.Ok();
    }

    public async Task<Ok<PaginatedList<SchoolItemDto>>> GetDeletedSchools(
        ISender sender,
        [AsParameters] GetDeletedSchoolsWithPagination query)
    {
        var result = await sender.Send(query);
        return TypedResults.Ok(result);
    }

    public async Task<Ok<PaginatedList<SchoolItemDto>>> GetSchoolWithPagination(
        ISender sender,
        [AsParameters] GetSchoolsWithPaginationQuery query)
    {
        var result = await sender.Send(query);
        return TypedResults.Ok(result);
    }
}