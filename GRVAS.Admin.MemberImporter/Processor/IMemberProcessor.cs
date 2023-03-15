namespace GRVAS.Admin.MemberImporter.Processor;

internal interface IMemberProcessor
{
    Task<bool> ProcessAsync();
}