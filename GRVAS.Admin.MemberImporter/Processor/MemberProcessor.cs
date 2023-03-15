using GRVAS.Admin.MemberImporter.Dynamo;
using GRVAS.Data.MemberImporter.Sheets;
using Newtonsoft.Json;
using System.Diagnostics;

namespace GRVAS.Admin.MemberImporter.Processor;

internal class MemberProcessor : IMemberProcessor
{
    private readonly IDataImporter _dataImporter;
    private readonly ITableDeleter _tableDeleter;
    private readonly ITableCreator _tableCreator;
    private readonly IMemberInserter _memberInserter;
    private readonly ILogger<MemberProcessor> _logger;

    public MemberProcessor(
        IDataImporter dataImporter,
        ITableDeleter tableDeleter,
        ITableCreator tableCreator,
        IMemberInserter memberInserter,
        ILogger<MemberProcessor> logger)
    {
        _dataImporter = dataImporter;
        _tableDeleter = tableDeleter;
        _tableCreator = tableCreator;
        _memberInserter = memberInserter;
        _logger = logger;
    }

    public async Task<bool> ProcessAsync()
    {
        var stopwatch = new Stopwatch();
        try
        {
            stopwatch.Start();
            List<Member> members = null;
            var counter = 0;
            while(members == null)
            {
                members = await _dataImporter.GetMembersFromGoogleSheetsAsync();
                counter++;
                if(counter >= 10) //try 10 times max
                {
                    _logger.LogError("Unable to retrieve member data from google sheets after 10 attempts. Ending process.");
                    return false;
                }
            }
            var deleteResult = await _tableDeleter.DeleteAsync();
            var createResult = await _tableCreator.CreateAsync();
            var insertResult = await _memberInserter.InsertAsync(members);

            stopwatch.Stop();
            _logger.LogInformation($"Member importer finished with processing time: {stopwatch.Elapsed}");
            return true;
        }
        catch(Exception ex)
        {
            _logger.LogError($"Failed to process member import due to exception. Exc: {ex}");
            stopwatch.Stop();
            _logger.LogInformation($"Member importer finished with processing time: {stopwatch.Elapsed}");

            return false;
        }
    }
}
