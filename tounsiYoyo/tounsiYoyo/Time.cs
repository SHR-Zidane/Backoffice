using System.Text.Json;

namespace tounsiYoyo;

public sealed class Time
{
    public long UnixTimeStamp { get; set; }

    public DateTimeOffset ToDateTimeOffset()
    {
        return DateTimeOffset.FromUnixTimeSeconds(UnixTimeStamp);
    }

    public static Time LoadFromJson(string path)
    {
        string json = File.ReadAllText(path);
        return JsonSerializer.Deserialize<Time>(json)
            ?? throw new JsonException($"Aucune heure n'a été trouvée dans '{path}'.");
    }
}