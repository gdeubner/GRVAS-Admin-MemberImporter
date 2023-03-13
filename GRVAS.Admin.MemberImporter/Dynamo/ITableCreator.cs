namespace GRVAS.Admin.MemberImporter.Dynamo;

internal interface ITableCreator
{
    Task<bool> CreateAsync();
}