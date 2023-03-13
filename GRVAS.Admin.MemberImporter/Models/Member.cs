namespace GRVAS.Data.MemberImporter.Models;

internal class Member
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Njemt { get; set; }
    public string? Nremt { get; set; }
    public DateOnly? EmtExpiration { get; set; }
    public DateOnly? CprExpiration { get; set; }
    public string? Status { get; set; }
    public string? HomePhone { get; set; }
    public string? Address { get; set; }
    public string? CellPhone { get; set; }
    public string? Email { get; set; }
    public string? Id { get; set; }
    public bool? IsDriver { get; set; }
    public bool? IsEmt { get; set; }
    public bool? IsCC { get; set; }
    public bool? IsInTraining { get; set; }
    public bool? IsTransport { get; set; }
    public bool? InHighschool { get; set; }
    public bool? IsFourth { get; set; }
    public bool? IsActive { get; set; }
    public string? Schedule_id { get; set; }

}
