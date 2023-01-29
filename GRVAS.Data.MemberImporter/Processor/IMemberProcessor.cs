namespace GRVAS.Data.MemberImporter.Processor;

internal interface IMemberProcessor
{
    Task<bool> Process();
}