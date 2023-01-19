namespace GRVAS.Data.MemberImporter.Database;

internal interface ITableManager
{
    bool CreateMembersTable();
    bool Truncate();
}