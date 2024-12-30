using DoDdns.Logic;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace DoDdns.Services;
internal class UpdaterService(HttpClient httpClient, ILogger<UpdaterService> logger, IConfiguration configuration) : BackgroundService
{
    private const string TargetDomain = "TargetDomain";
    private const string TargetHostName = "TargetHostName";
    private const string AccessToken = "DigitalOceanAccessToken";
    private const string GetPublicIpUrl = "GetPublicIpUrl";
    private const string UpdatePeriodMinutes = "UpdatePeriodMinutes";

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var targetDomain = configuration[TargetDomain] ?? throw new Exception($"{TargetDomain} configuration required");
        var targetHostName = configuration[TargetHostName] ?? throw new Exception($"{TargetHostName} configuration required");
        var accessToken = configuration[AccessToken] ?? throw new Exception($"{AccessToken} configuration required");
        var getPublicIpUrl = configuration[GetPublicIpUrl] ?? throw new Exception($"{GetPublicIpUrl} configuration required");

        if (!double.TryParse(configuration[UpdatePeriodMinutes], out var updatePeriodMinutes))
        {
            throw new Exception($"{UpdatePeriodMinutes} configuration required");
        }

        var updatePeriod = TimeSpan.FromMinutes(updatePeriodMinutes);
        var targetName = targetHostName.Replace(targetDomain, string.Empty).TrimEnd('.');

        while (!stoppingToken.IsCancellationRequested)
        {
            var delayTask = Task.Delay(updatePeriod, stoppingToken);

            try
            {
                await Update(stoppingToken);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to update DNS record, will retry");
            }

            await delayTask;
        }

        async Task Update(CancellationToken stoppingToken)
        {
            var targetIp = await Updater.GetTargetIp(targetHostName, stoppingToken);
            var hostIp = await Updater.GetHostIp(httpClient, getPublicIpUrl, stoppingToken);

            logger.LogInformation("The target's public IP is {targetPublicIpAddress}", targetIp);
            logger.LogInformation("The host's public IP is {hostPublicIpAddress}", hostIp);

            if (targetIp == hostIp)
            {
                return;
            }

            var targetDomainRecord = await Updater.GetTargetDomainRecord(httpClient, accessToken, targetDomain, targetName, stoppingToken);

            await Updater.UpdateDomainRecord(httpClient, accessToken, targetDomainRecord, hostIp, stoppingToken);
        }
    }
}
