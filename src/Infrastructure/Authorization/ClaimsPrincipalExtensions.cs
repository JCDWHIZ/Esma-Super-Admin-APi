using System.Text.Json;

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
