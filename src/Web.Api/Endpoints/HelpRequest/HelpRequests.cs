// using System;
// using Application.Abstractions.Models;
// using admin_service.Application.HelpRequest.Commands.CreateHelpReqestMessage;
// using admin_service.Application.HelpRequest.Commands.CreateHelpRequest;
// using admin_service.Application.HelpRequest.Commands.UpdateHelpRequestStatus;
// using admin_service.Application.HelpRequest.Queries;
// using admin_service.Application.HelpRequest.Queries.GetHelpRequestWithPagination;
// using Microsoft.AspNetCore.Http.HttpResults;

// namespace admin_service.Web.Endpoints;

// public class HelpRequests : EndpointGroupBase
// {
//     public override void Map(WebApplication app)
//     {
//         app.MapGroup(this)
//         // .RequireAuthorization()
//         .MapPost(AddHelpRequestMessage, "/{id}/messages")
//         .MapPut(UpdateHelpRequestStatus, "/{PublicId}/status")
//         .MapPost(CreateHelpRequest, "/")
//         .MapGet("/", GetHelpRequestsWithPagination);
//     }

//     public async Task<Ok<PaginatedList<HelpRequestItemDto>>> GetHelpRequestsWithPagination(
//         ISender sender,
//         [AsParameters] GetHelpRequestWithPaginationQuery query
//     )
//     {
//         var result = await sender.Send(query);
//         return TypedResults.Ok(result);
//     }

//     // public async Task<Ok<HelpRequestItemDto>> GetHelpRequestById(ISender sender, int request)
//     // {
//     //     var result = await sender.Send(new GetHelpRequestByIdQuery(request));
//     //     return TypedResults.Ok(result);
//     // }

//     public async Task<Ok<HelpRequestItemDto>> CreateHelpRequest(
//         ISender sender,
//         IntiateHelpRequestCommand command
//     )
//     {
//         var result = await sender.Send(command);

//         return TypedResults.Ok(result);
//     }

//     public async Task<Results<Ok<HelpRequestMessageDto>, BadRequest>> AddHelpRequestMessage(
//         ISender sender,
//         int id,
//         AddHelpRequestMessageCommand command
//     )
//     {
//         if (id != command.HelpRequestId)
//             return TypedResults.BadRequest();

//         var result = await sender.Send(command);

//         return TypedResults.Ok(result);
//     }

//     public async Task<Results<Ok<HelpRequestItemDto>, BadRequest>> UpdateHelpRequestStatus(
//         ISender sender,
//         string publicId,
//         UpdateHelpRequestStatusCommand command
//     )
//     {
//         if (publicId != command.PublicId)
//             return TypedResults.BadRequest();

//         var result = await sender.Send(command);

//         return TypedResults.Ok(result);
//     }

// }
