using GRVAS.Data.MemberImporter.Sheets;

namespace GRVAS.Admin.MemberImporter.Processor;

internal class MemberProcessor : IMemberProcessor
{
    private readonly IDataImporter _dataImporter;

    public MemberProcessor(IDataImporter dataImporter)
    {
        _dataImporter = dataImporter;
    }

    public async Task<bool> ProcessAsync()
    {
        try
        {
            var members = await _dataImporter.GetMembersFromGoogleSheetsAsync();

            return true;
        }
        catch(Exception ex)
        {

            return false;
        }
    }
}
