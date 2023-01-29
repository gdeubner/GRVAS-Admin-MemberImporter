using GRVAS.Data.MemberImporter.Processor;

namespace GRVAS.Data.MemberImporter.HostedServices;

internal class HangfireJob : IHostedService
{
    private readonly IRecurringJobManager _recurringJobManager;
    private readonly IMemberProcessor _memberProcessor;
    private ILogger<HangfireJob> _logger;

    public HangfireJob(
        IRecurringJobManager recurringJobManager,
        IMemberProcessor memberProcessor,
        ILogger<HangfireJob> logger)
    {
        _recurringJobManager = recurringJobManager;
        _memberProcessor = memberProcessor;
        _logger = logger;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Starting historical gps average processor");

        _recurringJobManager.AddOrUpdate(
            "Historical Average Processor Task",
            () => _memberProcessor.Process(),
            $"{DateTime.UtcNow.AddMinutes(1).Minute} * * * *", //"0 0 * * *",
            TimeZoneInfo.Utc);

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
