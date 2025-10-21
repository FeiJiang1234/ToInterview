namespace ToInterview.API.Practice
{
    public class DatabaseConnection
    {
        private static DatabaseConnection? _instance;
        private static readonly object _lock = new object();
        private string _connectionString;

        private DatabaseConnection()
        {
            _connectionString = "Server=localhost;Database=MyDB;Trusted_Connection=true;";
        }

        public static DatabaseConnection GetInstance()
        {
            if (_instance == null)
            {
                lock (_lock)
                {
                    if (_instance == null)
                    {
                        _instance = new DatabaseConnection();
                    }
                }
            }
            return _instance;
        }

        public string GetConnectionString()
        {
            return _connectionString;
        }
    }
}
