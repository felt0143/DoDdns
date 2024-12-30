using System.Text.Json.Serialization;

namespace DoDdns.Data;
internal record DomainRecordsResponse([property: JsonPropertyName("domain_records")] List<DomainRecord> DomainRecords);
