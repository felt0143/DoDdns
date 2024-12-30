using System.Text.Json.Serialization;

namespace DoDdns.Data;
internal record DomainRecord([property: JsonPropertyName("id")] int Id, [property: JsonPropertyName("type")] string Type, [property: JsonPropertyName("name")] string Name);
