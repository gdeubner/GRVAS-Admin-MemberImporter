namespace GRVAS.Admin.MemberImporter.Dynamo;

internal class MemberInserter : IMemberInserter
{
    private readonly IAmazonDynamoDB _dynamoDB;
    private readonly ILogger<TableCreator> _logger;
    private readonly string _tableName;

    public MemberInserter(
        IAmazonDynamoDB dynamoDB,
        ILogger<TableCreator> logger,
        string tableName)
    {
        _dynamoDB = dynamoDB;
        _logger = logger;
        _tableName = tableName;
    }

    public async Task<bool> InsertAsync(List<Member> members)
    {
        if (members == null || !members.Any())
        {
            throw new ArgumentNullException(nameof(members));
        }

        var request = _createBatchRequest(members);

        BatchWriteItemResponse response;

        int callCount = 1;
        do
        {
            _logger.LogInformation($"Making post request number {callCount}");
            response = await _dynamoDB.BatchWriteItemAsync(request);
            callCount++;

            // Check the response.

            var tableConsumedCapacities = response.ConsumedCapacity;
            var unprocessed = response.UnprocessedItems;

            Console.WriteLine("Per-table consumed capacity");
            foreach (var tableConsumedCapacity in tableConsumedCapacities)
            {
                Console.WriteLine("{0} - {1}", tableConsumedCapacity.TableName, tableConsumedCapacity.CapacityUnits);
            }

            Console.WriteLine("Unprocessed");
            foreach (var unp in unprocessed)
            {
                Console.WriteLine("{0} - {1}", unp.Key, unp.Value.Count);
            }
            Console.WriteLine();

            // For the next iteration, the request will have unprocessed items.
            request.RequestItems = unprocessed;
        } while (response.UnprocessedItems.Count > 0);
        return true;
    }

    private BatchWriteItemRequest _createBatchRequest(List<Member> members)
    {
        var writeRequest = new List<WriteRequest>();

        foreach (var member in members)
        {
            var temp = new WriteRequest
            {
                PutRequest = new PutRequest
                {
                    Item = new Dictionary<string, AttributeValue>
                    {
                        { "ScheduleId", new AttributeValue {
                                S = member.Schedule_id
                            }
                        },
                        { "LastName", new AttributeValue {
                                S = member.LastName
                            }
                        }
                    }
                }
            };
        }

        var request = new BatchWriteItemRequest
        {
            ReturnConsumedCapacity = "TOTAL",
            RequestItems = new Dictionary<string, List<WriteRequest>>
            {
                {
                    _tableName, writeRequest
                }
            }
        };

        return request;
    }
}
