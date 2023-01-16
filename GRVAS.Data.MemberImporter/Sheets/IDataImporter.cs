using GRVAS.Data.MemberImporter.Models;

namespace GRVAS.Data.MemberImporter.Sheets;

internal interface IDataImporter
{
    Task<List<Member>>? GetTableAsync();
}