// using System;
// using System.ComponentModel.DataAnnotations;
// using admin_service.Application.Common.Interfaces;
// using admin_service.Application.Common.Models;
// using Microsoft.Extensions.Logging;

// namespace admin_service.Application.Email;


// public record InitiateEmailCommand : IRequest<EmailRequestDto>
// {
//     public required string Email { get; set; }

//         public required string Title { get; set; }

//         public required string Description { get; set; }
//         public required string ButtonText { get; set; }
//         public required string ButtonLink { get; set; }

//         public bool EmailButton { get; set; }
// }


//  public class EmailRequestDto
//     {
//         public required string Email { get; set; }

//         public required string Title { get; set; }

//         public required string Description { get; set; }
//         public required string ButtonText { get; set; }
//         public required string ButtonLink { get; set; }

//         public bool EmailButton { get; set; }
//     }

// public class InitiateEmailRequestHandler(IEmailService emailService, ILogger logger) : IRequestHandler<InitiateEmailCommand>
// {
//     private readonly IEmailService _emailService = emailService;
//     private readonly ILogger _logger = logger;

//     public async Task Handle(InitiateEmailCommand request, CancellationToken cancellationToken)
//     {
//         var emailMessage = new EmailMessage
//         {
//             Email = request.Email,
//             Title = request.Title,
//             Description = request.Description,
//             ButtonText = request.ButtonText,
//             ButtonLink = request.ButtonLink,
//             EmailButton = request.EmailButton
//         };

//         // Queue the email for processing
//         var messageId = await _emailService.SendEmailAsync(emailMessage);

//         _logger.LogInformation("Email queued with message ID: {MessageId}", messageId);
//     }
// }

using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using admin_service.Application.Common.Interfaces;
using admin_service.Application.Common.Models;

namespace admin_service.Application.Email
{
    public record InitiateEmailCommand : IRequest<EmailRequestDto>
    {
        public required string Email { get; init; }
        public required string SchoolName { get; init; }
        public required string Title { get; init; }
        public required string Description { get; init; }
        public string? ButtonText { get; init; }
        public string? ButtonLink { get; init; }
        public bool EmailButton { get; init; }
    }

    public class EmailRequestDto
    {
        public required string Email { get; init; }
        public required string SchoolName { get; init; }
        public required string Title { get; init; }
        public required string Description { get; init; }
        public string? ButtonText { get; init; }
        public string? ButtonLink { get; init; }
        public bool EmailButton { get; init; }
    }

    public class InitiateEmailRequestHandler
        : IRequestHandler<InitiateEmailCommand, EmailRequestDto>
    {
        private readonly IEmailService _emailService;
        private readonly ILogger<InitiateEmailRequestHandler> _logger;

        public InitiateEmailRequestHandler(
            IEmailService emailService,
            ILogger<InitiateEmailRequestHandler> logger)
        {
            _emailService = emailService;
            _logger = logger;
        }

        public async Task<EmailRequestDto> Handle(
            InitiateEmailCommand request,
            CancellationToken cancellationToken)
        {
            // 1. Map to your domain/email model
            var emailMessage = new EmailMessage
            {
                Email = request.Email,
                Title = request.Title,
                SchoolName = request.SchoolName,
                Description = request.Description,
                ButtonText = request.ButtonText,
                ButtonLink = request.ButtonLink,
                EmailButton = request.EmailButton
            };

            // 2. Fire-and-forget? or get back an ID?
            var messageId = await _emailService.SendEmailAsync(emailMessage);
            _logger.LogInformation("Email queued with message ID: {MessageId}", messageId);

            // 3. Return the DTO (mirror the request or include new data)
            return new EmailRequestDto
            {
                Email = request.Email,
                Title = request.Title,
                SchoolName = request.SchoolName,
                Description = request.Description,
                ButtonText = request.ButtonText,
                ButtonLink = request.ButtonLink,
                EmailButton = request.EmailButton
            };
        }
    }
}
