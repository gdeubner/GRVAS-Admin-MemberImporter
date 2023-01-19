namespace GRVAS.Data.MemberImporter.Database;

internal class TableManager : ITableManager
{
    private readonly ILogger<TableManager> _logger;
    private readonly string _connectionString;

    public TableManager(
        ILogger<TableManager> logger,
        string connectionString)
    {
        _logger = logger;
        _connectionString = connectionString;
    }

    public bool CreateMembersTable()
    {
        try
        {
            _logger.LogInformation("Creating members table");
            using var conn = new MySqlConnection(_connectionString);

            conn.Open();

            string sql = _getCreateTableCommand();
            MySqlCommand cmd = new MySqlCommand(sql, conn);
            cmd.ExecuteNonQuery();
            _logger.LogInformation("Finished creating members table");
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error creating members table. Exc: {ex}");
            return false;
        }
    }

    public bool Truncate()
    {
        try
        {
            _logger.LogInformation("Truncating members table");
            using var conn = new MySqlConnection(_connectionString);

            conn.Open();

            string sql = $"TRUNCATE TABLE {Constants.MEMBERS_TABLE}";
            MySqlCommand cmd = new MySqlCommand(sql, conn);
            cmd.ExecuteNonQuery();
            _logger.LogInformation("Finished truncating members");
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error truncating members table. Exc: {ex}");
            return false;
        }
    }

   private string _getCreateTableCommand()
    {
        return "create table if not exists members(member_id int not null key AUTO_INCREMENT,first_name varchar(50) not null," +
            "last_name varchar(50) not null,njemt varchar(50),nremt varchar(50),is_driver bool not null,is_emt bool not null,is_cc bool not null," +
            "is_in_training bool not null,is_transport bool not null,in_highschool bool not null,is_fourth bool not null,emt_exp date, cpr_exp date," +
            "status varchar(50),home_phone varchar(20),cell varchar(20),address varchar(100),email varchar(100),schedule_id varchar(50)," +
            "is_active bool not null);";
    }
}
