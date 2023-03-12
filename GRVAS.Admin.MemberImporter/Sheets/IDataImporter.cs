namespace GRVAS.Data.MemberImporter.Sheets;

internal interface IDataImporter
{
    Task<List<Member>>? GetMembersFromGoogleSheetsAsync();
}