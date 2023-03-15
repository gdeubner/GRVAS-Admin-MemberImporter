namespace GRVAS.Admin.MemberImporter.Dynamo;

internal class TableDeleter : ITableDeleter
{
    private readonly IAmazonDynamoDB _dynamoDB;
    private readonly ILogger<TableDeleter> _logger;
    private readonly string _tableName;

    public TableDeleter(
        IAmazonDynamoDB dynamoDB,
        ILogger<TableDeleter> logger,
        string tableName)
    {
        _dynamoDB = dynamoDB;
        _logger = logger;
        _tableName = tableName;
    }

    public async Task<bool> DeleteAsync()
    {
        _logger.LogTrace($"Deleting dynamo table {_tableName}");
        try
        {
            var request = new DeleteTableRequest
            {
                TableName = _tableName,
            };

            var response = await _dynamoDB.DeleteTableAsync(request);
            if (response.HttpStatusCode == System.Net.HttpStatusCode.OK)
            {
                _logger.LogInformation($"Successfully started delete process for table: {response.TableDescription.TableName}.");
                await _waitUntilTableDeletedAsync(_tableName);
                return true;
            }
            else
            {
                _logger.LogInformation($"Could not delete table: {_tableName}.");
                return false;
            }
        }
        catch (ResourceNotFoundException)
        {
            _logger.LogInformation($"Dynamo table {_tableName} not found. Continuing.");
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError($"While trying to delete table [{_tableName}] the following exception occurred ex =[{ex}]");
            return false;
        }
    }

    private async Task _waitUntilTableDeletedAsync(string tableName)
    {
        var secondsDelay = 5;
        string status = null;
        _logger.LogInformation($"Waiting for table: {_tableName} to be deleted.");

        do
        {
            _logger.LogInformation($"Sleeping for {secondsDelay} seconds.");
            Thread.Sleep(secondsDelay * 1000); // Wait 5 seconds.
            try
            {
                var res = await _dynamoDB.DescribeTableAsync(new DescribeTableRequest
                {
                    TableName = tableName
                });

                status = res.Table.TableStatus;
            }
            catch (ResourceNotFoundException)
            {
                _logger.LogInformation($"Table: {_tableName} is now deleted.");
            }
        } while (status == "ACTIVE" || status == "DELETING ");
    }
}
