using Amazon.DynamoDBv2.DataModel;

namespace GRVAS.Admin.MemberImporter.Dynamo;

internal class MemberInserter : IMemberInserter
{
    private readonly IAmazonDynamoDB _dynamoDB;
    private readonly ILogger<TableCreator> _logger;
    private readonly string _tableName;

    private const int BATCH_SIZE = 25;

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
        var batchList = new List<List<Member>>();
        var batch = new List<Member>();
        for (int i = 0; i < members.Count; i++)
        {
            if ((i + 1) % BATCH_SIZE != 0)
            {
                batch.Add(members[i]);
            }
            else
            {
                _logger.LogInformation($"Inserting member batch {(i+1)/25}");
                batchList.Add(batch);
                batch = new List<Member>();
            }
        }
        await Task.WhenAll(batchList.Select(b => _batchInsertAsync(b)));
        return true;
    }

    private async Task<bool> _batchInsertAsync(List<Member> members)
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

            var tableConsumedCapacities = response.ConsumedCapacity;
            var unprocessed = response.UnprocessedItems;

            // For the next iteration, the request will have unprocessed items.
            request.RequestItems = unprocessed;
        } while (response.UnprocessedItems.Count > 0);
        _logger.LogInformation($"Finished inserting batch.");
        return true;
    }

    private BatchWriteItemRequest _createBatchRequest(List<Member> members)
    {
        var writeRequest = new List<WriteRequest>();

        foreach (var member in members)
        {
            writeRequest.Add(new WriteRequest
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
                        },
                        { "FirstName", new AttributeValue {
                                S = member.FirstName
                            }
                        },
                        { "Njemt", new AttributeValue {
                                S = member.Njemt
                            }
                        },
                        { "Nremt", new AttributeValue {
                                S = member.Nremt
                            }
                        },
                        { "EmtExpiration", new AttributeValue {
                                S = member.EmtExpiration.ToString()
                            }
                        },
                        { "CprExpiration", new AttributeValue {
                                S = member.CprExpiration.ToString()
                            }
                        },
                        { "Email", new AttributeValue {
                                S = member.Email
                            }
                        },
                        { "IsDriver", new AttributeValue {
                                BOOL = member.IsDriver ?? false
                            }
                        },
                        { "IsEmt", new AttributeValue {
                                BOOL = member.IsEmt ?? false
                            }
                        },
                        { "IsInTraining", new AttributeValue {
                                BOOL = member.IsInTraining ?? false
                            }
                        },
                        { "IsTransport", new AttributeValue {
                                BOOL = member.IsTransport ?? false
                            }
                        },
                        { "InHighschool", new AttributeValue {
                                BOOL = member.InHighschool ?? false
                            }
                        },
                        { "IsFourth", new AttributeValue {
                                BOOL = member.IsFourth ?? false
                            }
                        },
                        { "IsActive", new AttributeValue {
                                BOOL = member.IsActive ?? false
                            }
                        }

                    }
                }
            });
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
