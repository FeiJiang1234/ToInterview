namespace ToInterview.API.Services;

public class UserEventArgs : EventArgs
{
    public string UserName { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
    public string Action { get; set; } = string.Empty;
}

public delegate void UserEventHandler(object sender, UserEventArgs e);

public class EventService
{
    public event UserEventHandler? UserLoggedIn;

    public async Task SimulateUserLogin(string userName)
    {
        await Task.Delay(100);
        
        var args = new UserEventArgs
        {
            UserName = userName,
            Timestamp = DateTime.Now,
            Action = "Login"
        };
        
        OnUserLoggedIn(args);
    }

    protected virtual void OnUserLoggedIn(UserEventArgs e)
    {
        UserLoggedIn?.Invoke(this, e);
    }
}

public class UserEventHandlers
{
    private readonly ILogger<UserEventHandlers> _logger;
    private readonly List<string> _loggedInUsers = new();

    public UserEventHandlers(ILogger<UserEventHandlers> logger)
    {
        _logger = logger;
    }

    public void HandleUserLogin(object? sender, UserEventArgs e)
    {
        _loggedInUsers.Add(e.UserName);
        Console.WriteLine("用户 {UserName} 在 {Timestamp} 登录", e.UserName, e.Timestamp);
    }

    public void HandleUserLogout(object? sender, UserEventArgs e)
    {
        _loggedInUsers.Remove(e.UserName);
        Console.WriteLine("用户 {UserName} 在 {Timestamp} 登出", e.UserName, e.Timestamp);
    }

    public void HandleUserAction(object? sender, UserEventArgs e)
    {
        Console.WriteLine("用户 {UserName} 在 {Timestamp} 执行了操作: {Action}", 
            e.UserName, e.Timestamp, e.Action);
    }

    public List<string> GetLoggedInUsers()
    {
        return new List<string>(_loggedInUsers);
    }
}
