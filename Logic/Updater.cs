using DoDdns.Data;
using System.Net;
using System.Text;
using System.Text.Json;

namespace DoDdns.Logic;
internal static class Updater
{
    public static async Task<IPAddress?> GetTargetIp(string targetHostName, CancellationToken cancelToken)
    {
        var ipHostEntry = await Dns.GetHostEntryAsync(targetHostName, cancelToken);

        return ipHostEntry.AddressList.FirstOrDefault();
    }

    public static async Task<IPAddress> GetHostIp(HttpClient httpClient, string getPublicIpUrl, CancellationToken cancelToken)
    {
        using var response = await httpClient.GetAsync(getPublicIpUrl, cancelToken);

        response.EnsureSuccessStatusCode();

        var responseText = await response.Content.ReadAsStringAsync(cancelToken);

        return IPAddress.Parse(responseText);
    }

    /// <summary>
    /// https://docs.digitalocean.com/reference/api/api-reference/#operation/domains_list_records
    /// </summary>
    public static async Task<DomainRecord> GetTargetDomainRecord(HttpClient httpClient, string accessToken, string targetDomain, string targetName, CancellationToken cancelToken)
    {
        using HttpRequestMessage request = new(HttpMethod.Get, $"https://api.digitalocean.com/v2/domains/{targetDomain}/records");
        request.Headers.Authorization = new("Bearer", accessToken);
        using var response = await httpClient.SendAsync(request, cancelToken);

        response.EnsureSuccessStatusCode();

        var responseText = await response.Content.ReadAsStringAsync(cancelToken);
        var domainRecordsResult = JsonSerializer.Deserialize<DomainRecordsResponse>(responseText) ?? throw new Exception("Failed to deserialize DomainRecordsResult");

        return domainRecordsResult.DomainRecords.First(x => x.Type == "A" && x.Name == targetName);
    }

    /// <summary>
    /// https://docs.digitalocean.com/reference/api/api-reference/#operation/domains_patch_record
    /// </summary>
    public static async Task UpdateDomainRecord(HttpClient httpClient, string accessToken, DomainRecord domainRecord, IPAddress newIpAddress, CancellationToken cancelToken)
    {

        using HttpRequestMessage request = new(HttpMethod.Patch, $"https://api.digitalocean.com/v2/domains/jstnf.com/records/{domainRecord.Id}");
        request.Headers.Authorization = new("Bearer", accessToken);
        PatchDomainRecordRequest patchDomainRecordRequest = new(newIpAddress.ToString());
        request.Content = new StringContent(JsonSerializer.Serialize(patchDomainRecordRequest), Encoding.UTF8, "application/json");
        using var response = await httpClient.SendAsync(request, cancelToken);

        response.EnsureSuccessStatusCode();

        var responseText = await response.Content.ReadAsStringAsync(cancelToken);

        Console.WriteLine(responseText);
    }
}
