using System;

namespace Application.Interfaces;

public interface ITokenService
{
    string GenerateToken(IDictionary<string, object> claims);
}

