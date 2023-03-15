namespace GRVAS.Admin.MemberImporter.Dynamo;

internal class TableCreator : ITableCreator
{
    private readonly IAmazonDynamoDB _dynamoDB;
    private readonly ILogger<TableCreator> _logger;
    private readonly string _tableName;

    public TableCreator(
        IAmazonDynamoDB dynamoDB,
        ILogger<TableCreator> logger,
        string tableName)
    {
        _dynamoDB = dynamoDB;
        _logger = logger;
        _tableName = tableName;
    }

    public async Task<bool> CreateAsync()
    {
        _logger.LogInformation($"Creating table: {_tableName}");

        try
        {
            var request = new CreateTableRequest
            {
                AttributeDefinitions = new List<AttributeDefinition>()
                {
                    new AttributeDefinition
                    {
                        AttributeName = "LastName",
                        AttributeType = "s"
                    },
                    new AttributeDefinition
                    {
                        AttributeName = "ScheduleId",
                        AttributeType = "s"
                    }
                },
                ProvisionedThroughput = new ProvisionedThroughput
                {
                    ReadCapacityUnits = 5,
                    WriteCapacityUnits = 5
                },
                KeySchema = new List<KeySchemaElement>()
                {
                    new KeySchemaElement()
                    {
                        AttributeName = "ScheduleId",
                        KeyType = "HASH" //Partition key
                    },
                    new KeySchemaElement
                    {
                        AttributeName = "LastName",
                        KeyType = "RANGE" //Sort key
                    }
                },
                TableName = _tableName,
            };

            var response = await _dynamoDB.CreateTableAsync(request);
            _logger.LogInformation($"Successfully initiated creation of table: {_tableName}");

            var tableDescription = response.TableDescription;

            string status = tableDescription.TableStatus;
            Console.WriteLine(_tableName + " - " + status);

            await _waitUntilTableReadyAsync(_tableName);

            return true;
        }
        catch (Exception exc)
        {
            _logger.LogError($"Ann error occured while creating dynamo table [{_tableName}]. Exc: {exc}");
            return false;
        }
    }

    private async Task _waitUntilTableReadyAsync(string tableName)
    {
        var secondsDelay = 5;
        string status = null;
        _logger.LogInformation($"Waiting for table: {_tableName} to become available.");

        do
        {
            _logger.LogInformation($"Sleeping for {secondsDelay} seconds.");
            System.Threading.Thread.Sleep(secondsDelay * 1000); // Wait 5 seconds.
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
                // DescribeTable is eventually consistent. So you might
                // get resource not found. So we handle the potential exception.
            }
        } while (status != "ACTIVE");
        _logger.LogInformation($"Table: {_tableName} is now available.");
    }
}
