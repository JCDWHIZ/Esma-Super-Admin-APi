using System;

namespace admin_service.Application.Common.Interfaces;

   public interface IMessageProducer
    {
        Task<string> SendMessageAsync<T>(string messageType, T data, string? topic = null);
    }