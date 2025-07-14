using System;
using admin_service.Application.Common.Interfaces;
using admin_service.Application.Exports.Queries;
using admin_service.Domain.Enums;

namespace admin_service.Application.Exports.Command;

public record ExportDataCommand(AdminModule Module, ExportType ExportType) : IRequest<ExportDataResultDto>;


 public class ExportDataCommandHandler : IRequestHandler<ExportDataCommand, ExportDataResultDto>
    {
        private readonly IExportService _exportService;

        public ExportDataCommandHandler(IExportService exportService)
        {
            _exportService = exportService;
        }

        public async Task<ExportDataResultDto> Handle(ExportDataCommand request, CancellationToken cancellationToken)
        {
            return await _exportService.ExportDataAsync(request.Module, request.ExportType);
        }
    }
