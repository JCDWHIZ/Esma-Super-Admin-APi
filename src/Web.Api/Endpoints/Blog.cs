using System;
using admin_service.Application.BlogModule.Commands.CreateBlog;
using admin_service.Application.BlogModule.Commands.CreateCommands.PublishBlog;
using admin_service.Application.BlogModule.Commands.CreatePublishedBlog;
using admin_service.Application.BlogModule.Commands.CreateScheduledBlog;
using admin_service.Application.BlogModule.Commands.DeleteCommand;
using admin_service.Application.BlogModule.Commands.EditBlog;
using admin_service.Application.BlogModule.Queries.GetBlogById;
using admin_service.Application.BlogModule.Queries.GetBlogsWithpagination;
using admin_service.Application.Common.Models;
using admin_service.Domain.Constants;
using admin_service.Web.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace admin_service.Web.Endpoints;

public class Blog : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup(this)
            // .RequireAuthorization()
            .MapPost(CreateBlogDraft, "/")
            .MapDelete(DeleteBlogById, "/{publicId}")
            .MapPost(CreatePublishedBlog, "/publish")
            .MapPost(CreateScheduledBlog, "/schedule")
            .MapPut(PublishBlog, "/{publicId}/publish")
            .MapPut(UpdateBlog, "/{publicId}")
            .MapGet(GetBlogById, "/{publicId}")
            .MapGet(GetBlogsWithPagination, "/");
    }

    [Audit("Created Draft Blog")]
    [Authorize(Policy = "ManagerOnly")]
    [Authorize(Policy = "CanCreateBlog")]
    public async Task<Ok<BlogItemDto>> CreateBlogDraft(
        ISender sender,
        IntiateBlogDraftRequestCommand command
    )
    {
        var result = await sender.Send(command);

        return TypedResults.Ok(result);
    }

    public async Task<Ok<BlogItemDto>> CreatePublishedBlog(
        ISender sender,
        InitiateScheduledBlogRequestCommand command
    )
    {
        var result = await sender.Send(command);

        return TypedResults.Ok(result);
    }

    public async Task<Ok<BlogItemDto>> CreateScheduledBlog(
        ISender sender,
        InitiateScheduledBlogRequestCommand command
    )
    {
        var result = await sender.Send(command);

        return TypedResults.Ok(result);
    }
    public async Task<Results<Ok, BadRequest>> UpdateBlog(
        ISender sender,
        string publicId,
        EditBlogRequestCommand command
    )
    {
        if (publicId != command.PublicId)
            return TypedResults.BadRequest();

        await sender.Send(command);

        return TypedResults.Ok();
    }

    public async Task<Results<Ok, BadRequest>> PublishBlog(
        ISender sender,
        string publicId,
        InitiateBlogPublish command
    )
    {
        if (publicId != command.PublicId)
            return TypedResults.BadRequest();

        await sender.Send(command);

        return TypedResults.Ok();
    }
    
    public async Task<Ok<BlogItemDto>> GetBlogById(
        ISender sender,
        string publicId
    )
    {
        var result = await sender.Send(new GetBlogByIdQuery(publicId));
        return TypedResults.Ok(result);
    }
    public async Task<Ok<PaginatedList<BlogItemDto>>> GetBlogsWithPagination(
        ISender sender,
        [AsParameters] GetBlogWithPaginationQuery query
    )
    {
        var result = await sender.Send(query);
        return TypedResults.Ok(result);
    }
    [Authorize(Policy = "CanDeleteBlog")]
    public async Task<Results<Ok, NotFound>>  DeleteBlogById(
    ISender sender,
    string publicId
    )
    {
        await sender.Send(new DeleteBlogRequestCommand(publicId));
        return TypedResults.Ok();
    }
}
