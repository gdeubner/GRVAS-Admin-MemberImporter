namespace GRVAS.Data.MemberImporter.Processor;

internal class MemberProcessor : IMemberProcessor
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

    public async Task<bool> Process()
    {
        try
        {
            var result = await _dataImporter.GetTableAsync();

            if (result != null && result.Count() > 0)
            {
                _tableManager.CreateMembersTable();
                _dataWriter.BulkInsertMembers(result);

            }
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error processing. Exc: {ex}");
            return false;
        }
    }
}
