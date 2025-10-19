namespace ToInterview.API.Patterns.Singleton;

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

    public void ExecuteQuery(string query)
    {
        Console.WriteLine($"执行查询: {query}");
    }
}

public class Logger
{
    private static Logger? _instance;
    private static readonly object _lock = new object();
    private List<string> _logs = new();

    private Logger()
    {
    }

    public static Logger GetInstance()
    {
        if (_instance == null)
        {
            lock (_lock)
            {
                if (_instance == null)
                {
                    _instance = new Logger();
                }
            }
        }
        return _instance;
    }

    public void Log(string message)
    {
        var logEntry = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {message}";
        _logs.Add(logEntry);
        Console.WriteLine(logEntry);
    }

    public List<string> GetLogs()
    {
        return new List<string>(_logs);
    }

    public void ClearLogs()
    {
        _logs.Clear();
    }
}

public class ConfigurationManager
{
    private static ConfigurationManager? _instance;
    private static readonly object _lock = new object();
    private Dictionary<string, string> _configurations = new();

    private ConfigurationManager()
    {
        LoadDefaultConfigurations();
    }

    public static ConfigurationManager GetInstance()
    {
        if (_instance == null)
        {
            lock (_lock)
            {
                if (_instance == null)
                {
                    _instance = new ConfigurationManager();
                }
            }
        }
        return _instance;
    }

    private void LoadDefaultConfigurations()
    {
        _configurations["AppName"] = "MyApplication";
        _configurations["Version"] = "1.0.0";
        _configurations["Environment"] = "Development";
    }

    public string GetConfiguration(string key)
    {
        return _configurations.TryGetValue(key, out var value) ? value : string.Empty;
    }

    public void SetConfiguration(string key, string value)
    {
        _configurations[key] = value;
    }

    public Dictionary<string, string> GetAllConfigurations()
    {
        return new Dictionary<string, string>(_configurations);
    }
}

public class ApplicationManager
{
    private DatabaseConnection _dbConnection;
    private Logger _logger;
    private ConfigurationManager _configManager;

    public ApplicationManager()
    {
        _dbConnection = DatabaseConnection.GetInstance();
        _logger = Logger.GetInstance();
        _configManager = ConfigurationManager.GetInstance();
    }

    public void InitializeApplication()
    {
        _logger.Log("应用程序启动");
        _logger.Log($"数据库连接: {_dbConnection.GetConnectionString()}");
        _logger.Log($"应用名称: {_configManager.GetConfiguration("AppName")}");
        _logger.Log($"版本: {_configManager.GetConfiguration("Version")}");
    }

    public void ProcessData()
    {
        _logger.Log("开始处理数据");
        _dbConnection.ExecuteQuery("SELECT * FROM Users");
        _logger.Log("数据处理完成");
    }

    public void ShutdownApplication()
    {
        _logger.Log("应用程序关闭");
    }
}
