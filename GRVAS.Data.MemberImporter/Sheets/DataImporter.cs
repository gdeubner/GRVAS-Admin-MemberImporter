using Google.Apis.Sheets.v4;
using GRVAS.Data.MemberImporter.Models;

namespace GRVAS.Data.MemberImporter.Sheets;

internal class DataImporter : IDataImporter
{

    private readonly SpreadsheetsResource.ValuesResource _googleSheetValues;

    private const string SPREADSHEET_ID = "1UvKp2dYshVNZOO41Ac3K6ekj9Hk1079yT-_9qoN8OEs";

    public DataImporter(ICredentialProvider credentialProvider)
    {
        _googleSheetValues = credentialProvider.Service.Spreadsheets.Values;
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
                    var nameSplit = v?.Count() >= 1 ? v?[0].ToString()?.Split(", ") : null;
                    if(nameSplit?.Length == 2)
                    {
                        members.Add(new Member()
                        {
                            FirstName = nameSplit?[0],
                            LastName = nameSplit?[1],
                            Njemt = v?.Count() >= 2 ? v?[1].ToString() : null,
                            Nremt = v?.Count() >= 3 ? v?[2].ToString() : null,
                            EmtExpiration = v?.Count() >= 4 ? v?[3].ToString() : null,
                            CprExpiration = v?.Count() >= 5 ? v?[4].ToString() : null,
                            Status = v?.Count() >= 6 ? v?[5].ToString() : null,
                            Home = v?.Count() >= 7 ? v?[6].ToString() : null,
                            Address = v?.Count() >= 8 ? v?[7].ToString() : null,
                            Cell = v?.Count() >= 9 ? v?[8].ToString() : null,
                            Email = v?.Count() >= 10 ? v?[9].ToString() : null,
                            Id = v?.Count() >= 11 ? v?[10].ToString() : null
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
}
