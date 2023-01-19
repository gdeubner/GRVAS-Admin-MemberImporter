namespace GRVAS.Data.MemberImporter.Database;

internal class DataWriter : IDataWriter
{
    private readonly ILogger<DataWriter> _logger;
    private readonly string _connectionString;

    public DataWriter(
        ILogger<DataWriter> logger,
        string connectionString)
    {
        _logger = logger;
        _connectionString = connectionString;
    }

    public bool BulkInsertMembers(List<Member> members)
    {
        try
        {
            _logger.LogInformation("Inserting member data to MySQL");
            using var conn = new MySqlConnection(_connectionString);

            conn.Open();

            string sql = _formQueryString(members);
            MySqlCommand cmd = new MySqlCommand(sql, conn);
            cmd.ExecuteNonQuery();
            _logger.LogInformation("Finished inserting member data to MySQL");
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error inserting member data to MySQL. Exc: {ex}");
            return false;
        }
    }

    private string _formQueryString(List<Member> members)
    {
        var rows = string.Join(',', members.Select(m => $"(\"{m.FirstName}\", \"{m.LastName}\",'{m.Njemt}','{m.Nremt}',{m.IsDriver},{m.IsEmt}," +
            $"{m.IsCC},{m.IsInTraining},{m.InHighschool},{m.IsFourth},{m.IsTransport},{(m.EmtExpiration == null ? "NULL" : "\'"+m.EmtExpiration+"\'")}," +
            $"{(m.EmtExpiration == null ? "NULL" : "\'" + m.CprExpiration + "\'")},'{m.Status}','{m.Home}'," +
            $"'{m.Cell}','{m.Address}','{m.Email}',{m.IsActive},\"{m.Schedule_id}\")"));

        return $"INSERT INTO members (first_name,last_name,njemt,nremt,is_driver,is_emt,is_cc,is_in_training," +
            $"in_highschool,is_fourth,is_transport,emt_exp,cpr_exp,status,home_phone,cell,address,email,is_active,schedule_id) VALUES {rows};";
    }
}
