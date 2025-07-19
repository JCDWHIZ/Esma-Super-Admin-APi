// using System;
// using admin_service.Application.Exports.Command;
// using admin_service.Application.Exports.Queries;
// using admin_service.Domain.Enums;
// using Microsoft.AspNetCore.Http.HttpResults;
// using Microsoft.AspNetCore.Mvc;

// namespace admin_service.Web.Endpoints;

// public class Exports : EndpointGroupBase
// {
//      public override void Map(WebApplication app)
//     {
//         app.MapGroup(this)
//         // .RequireAuthorization()
//         .MapGet("/", GetExports);
//     }


//    public async  Task<IResult> GetExports(
//     ISender sender,
//     [FromQuery] AdminModule module, 
//     [FromQuery] ExportType exportType)
//     {
//         var result = await sender.Send(new ExportDataCommand(module, exportType));
//         return Results.File(
//         fileContents: result.FileBytes, 
//         contentType: result.ContentType, 
//         fileDownloadName: result.FileName
//     );
//     }
// }
 