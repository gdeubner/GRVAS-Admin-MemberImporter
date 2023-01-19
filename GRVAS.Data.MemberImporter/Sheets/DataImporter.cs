namespace GRVAS.Data.MemberImporter.Sheets;

internal class DataImporter : IDataImporter
{

    private readonly SpreadsheetsResource.ValuesResource _googleSheetValues;
    private readonly ILogger<DataImporter> _logger;

    private const string SPREADSHEET_ID = "1UvKp2dYshVNZOO41Ac3K6ekj9Hk1079yT-_9qoN8OEs";

    public DataImporter(ICredentialProvider credentialProvider, ILogger<DataImporter> logger)
    {
        _googleSheetValues = credentialProvider.Service.Spreadsheets.Values;
        _logger = logger;
    }

    public async Task<List<Member>>? GetTableAsync()
    {
        try
        {
            var range = "A:K";
            var request = _googleSheetValues.Get(SPREADSHEET_ID, range);
            var response = await request.ExecuteAsync();
            var values = response.Values;

            var members = new List<Member>();
            foreach (var v in values)
            {
                try
                {
                    if (v is null || v.Count() < 10)
                        continue;
                    var nameSplit = v?.Count() >= 1 ? v?[0].ToString()?.Split(",") : null;
                    if(nameSplit?.Length == 2)
                    {
                        members.Add(new Member()
                        {
                            FirstName = nameSplit?[1]?.Trim(),
                            LastName = nameSplit?[0]?.Trim(),
                            Njemt = v?.Count() >= 2 ? v?[1].ToString()?.Trim() : null,
                            Nremt = v?.Count() >= 3 ? v?[2].ToString()?.Trim() : null,
                            EmtExpiration = _formatDate(v?[3].ToString()),
                            CprExpiration = _formatDate(v?[4].ToString()),
                            Status = v?.Count() >= 6 ? v?[5].ToString()?.Trim() : null,
                            Home = v?.Count() >= 7 ? v?[6].ToString()?.Trim() : null,
                            Address = v?.Count() >= 8 ? v?[7].ToString()?.Trim() : null,
                            Cell = v?.Count() >= 9 ? v?[8].ToString()?.Trim() : null,
                            Email = v?.Count() >= 10 ? v?[9].ToString()?.Trim() : null,
                            Schedule_id = v?.Count() >= 11 ? v?[10].ToString()?.Trim() : null,
                            IsDriver = (v?[5]?.ToString()?.ToLower().Contains("driver") ?? false) || (v?[5]?.ToString()?.ToLower().Contains("cc") ?? false),
                            IsEmt = (v?[5]?.ToString()?.ToLower().Contains("emt") ?? false) || (v?[5]?.ToString()?.ToLower().Contains("cc") ?? false),
                            IsCC = v?[5]?.ToString()?.ToLower().Contains("cc") ?? false,
                            IsInTraining = (v?[5]?.ToString() == null) || (v?[5]?.ToString()?.Trim().Length == 0) ||
                                (v?[5]?.ToString()?.ToLower().Contains("i/t") ?? false) ||
                                (v?[5]?.ToString()?.ToLower().Contains("student") ?? false),
                            IsFourth = v?[5]?.ToString()?.ToLower().Contains("4th") ?? false,
                            InHighschool = v?[5]?.ToString()?.ToLower().Contains("hs") ?? false,
                            IsTransport = v?[5]?.ToString()?.ToLower().Contains("transport") ?? false,
                            IsActive = true
                        });
                    }
                   
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Unable to parse member. Exc: {e}");
                }
            }

            return members;
        }
        catch (Exception e)
        {
            Console.WriteLine($"An error occured while retrieving member data from Google Sheets. Exc: {e}");
            return null;
        }
    }

    private string? _formatDate(string? date)
    {
        if (!string.IsNullOrEmpty(date) && !(date?.ToLower().Equals("n/a") ?? false) && !(date?.ToLower().Equals("x")??false) && !(date?.ToLower().Equals("i/t") ?? false))
        {
            if(DateTime.TryParse(date, out var parsedDate))
            {
                return $"{parsedDate.Year}-{parsedDate.Month}-{parsedDate.Day}";
            }
            else
            {
                _logger.LogWarning($"Invalid date entered: [{date}]");
            }
        }
        return null;
    }
}
