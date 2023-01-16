using GRVAS.Data.MemberImporter.Sheets;
using Microsoft.Extensions.Hosting;

namespace GRVAS.Data.MemberImporter.Job;

internal class MemberProcessor : IHostedService
{
    private readonly IDataImporter _dataImporter;

    public MemberProcessor(IDataImporter dataImporter)
    {
        _dataImporter = dataImporter;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        var result = _dataImporter.GetTableAsync()?.Result;
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
