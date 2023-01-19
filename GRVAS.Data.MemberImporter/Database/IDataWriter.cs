using GRVAS.Data.MemberImporter.Models;

namespace GRVAS.Data.MemberImporter.Database
{
    internal interface IDataWriter
    {
        bool BulkInsertMembers(List<Member> members);
    }
}