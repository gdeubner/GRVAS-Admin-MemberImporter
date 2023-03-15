namespace GRVAS.Admin.MemberImporter.Dynamo;

internal interface ITableDeleter
{
    Task<bool> DeleteAsync();
}