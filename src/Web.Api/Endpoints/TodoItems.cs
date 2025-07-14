using admin_service.Application.Common.Models;
using admin_service.Application.TodoItems.Commands.CreateTodoItem;
using admin_service.Application.TodoItems.Commands.DeleteTodoItem;
using admin_service.Application.TodoItems.Commands.UpdateTodoItem;
using admin_service.Application.TodoItems.Commands.UpdateTodoItemDetail;
using admin_service.Application.TodoItems.Queries;
using admin_service.Application.TodoItems.Queries.GetTodoItemsById;
using admin_service.Application.TodoItems.Queries.GetTodoItemsWithPagination;
using Microsoft.AspNetCore.Http.HttpResults;

namespace admin_service.Web.Endpoints;

public class TodoItems : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup(this)
            .RequireAuthorization()
            .MapGet(GetTodoItemsWithPagination, "tod")
            .MapPost(CreateTodoItem)
            .MapGet(GetTodoItemById, "{id}")
            .MapPut(UpdateTodoItem, "{id}")
            .MapPut(UpdateTodoItemDetail, "UpdateDetail/{id}")
            .MapDelete(DeleteTodoItem, "{id}");


    }

    public async Task<NoContent> GetTodoItemById(ISender sender, int request)
    {
        await sender.Send(new GetTodoItemByIdQuery(request));

        return TypedResults.NoContent();
    }
    public async Task<Ok<PaginatedList<TodoItemBriefDto>>> GetTodoItemsWithPagination(ISender sender,
        [AsParameters] GetTodoItemsWithPaginationQuery query)
    {
        var result = await sender.Send(query);

        return TypedResults.Ok(result);
    }

    public async Task<Created<int>> CreateTodoItem(ISender sender, CreateTodoItemCommand command)
    {
        var id = await sender.Send(command);

        return TypedResults.Created($"/{nameof(TodoItems)}/{id}", id);
    }

    public async Task<Results<NoContent, BadRequest>> UpdateTodoItem(ISender sender, int id,
        UpdateTodoItemCommand command)
    {
        if (id != command.Id) return TypedResults.BadRequest();

        await sender.Send(command);

        return TypedResults.NoContent();
    }

    public async Task<Results<NoContent, BadRequest>> UpdateTodoItemDetail(ISender sender, int id,
        UpdateTodoItemDetailCommand command)
    {
        if (id != command.Id) return TypedResults.BadRequest();

        await sender.Send(command);

        return TypedResults.NoContent();
    }

    public async Task<NoContent> DeleteTodoItem(ISender sender, int id)
    {
        await sender.Send(new DeleteTodoItemCommand(id));

        return TypedResults.NoContent();
    }
}
