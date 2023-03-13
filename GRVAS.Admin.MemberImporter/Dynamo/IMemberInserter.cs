namespace GRVAS.Admin.MemberImporter.Dynamo;

internal interface IMemberInserter
{
    Task<bool> InsertAsync(List<Member> members);
}