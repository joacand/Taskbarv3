using System.Text.Json;

namespace Taskbarv3.Infrastructure
{
    internal static class JsonOptions
    {
        internal static readonly JsonSerializerOptions Default = new()
        {
            WriteIndented = true,
            PropertyNameCaseInsensitive = true
        };
    }
}
