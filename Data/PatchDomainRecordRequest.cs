using System.Text.Json.Serialization;

namespace DoDdns.Data;
internal record PatchDomainRecordRequest([property: JsonPropertyName("data")] string Data);
