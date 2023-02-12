using GRVAS.Data.MemberImporter.Processor;
using Hangfire.Storage;

namespace GRVAS.Data.MemberImporter.HostedServices;

internal class HangfireJobScheduler : IHostedService
{
    private readonly IRecurringJobManager _recurringJobManager;
    private readonly IMemberProcessor _memberProcessor;
    private ILogger<HangfireJobScheduler> _logger;

    public HangfireJobScheduler(
        IRecurringJobManager recurringJobManager,
        IMemberProcessor memberProcessor,
        ILogger<HangfireJobScheduler> logger)
    {
        _recurringJobManager = recurringJobManager;
        _memberProcessor = memberProcessor;
        _logger = logger;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Starting grvas member importer");

        _recurringJobManager.AddOrUpdate(
            "Member Importer Task",
            () => _memberProcessor.ProcessAsync(),
             $"{DateTime.UtcNow.AddMinutes(1).Minute} * * * *", //"0 8 * * *",
            TimeZoneInfo.Utc);

        var job = JobStorage.Current.GetConnection().GetRecurringJobs().FirstOrDefault().NextExecution;
        _logger.LogInformation($"Next execution: {job}");
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
