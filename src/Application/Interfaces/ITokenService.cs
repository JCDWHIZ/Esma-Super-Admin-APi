using System;

namespace admin_service.Application.Common.Interfaces;

public interface ITokenService
{
    string GenerateToken(IDictionary<string, object> claims);
}

