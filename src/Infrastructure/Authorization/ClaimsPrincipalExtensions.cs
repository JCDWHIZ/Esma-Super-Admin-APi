using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Infrastructure.Authorization;

public static class ClaimsPrincipalExtensions
{
    public static T? DeserializeFromJson<T>(this string json)
    {
        try
        {
            return JsonSerializer.Deserialize<T>(json);
        }
        catch
        {
            return default;
        }
    }
}
