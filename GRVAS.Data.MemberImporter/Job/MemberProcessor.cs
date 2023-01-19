namespace GRVAS.Data.MemberImporter.Job;

internal class MemberProcessor : IHostedService
{
    private readonly IDataImporter _dataImporter;
    private readonly IDataWriter _dataWriter;
    private readonly ITableManager _tableManager;
    private readonly ILogger<MemberProcessor> _logger;

    public MemberProcessor(
        IDataImporter dataImporter,
        IDataWriter dataWriter,
        ITableManager tableManager,
        ILogger<MemberProcessor> logger)
    {
        _dataImporter = dataImporter;
        _dataWriter = dataWriter;
        _tableManager = tableManager;
        _logger = logger;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        var result = _dataImporter.GetTableAsync()?.Result;

        if (result != null && result.Count() > 0)
        {
            _tableManager.CreateMembersTable();
            _dataWriter.BulkInsertMembers(result);

        }

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
