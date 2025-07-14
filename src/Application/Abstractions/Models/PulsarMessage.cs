using System;

namespace admin_service.Application.Common.Models;

public class PulsarMessage<T>
    {
        public required string MessageType { get; set; }
        public required T Data { get; set; }
    }