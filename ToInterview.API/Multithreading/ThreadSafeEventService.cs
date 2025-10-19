using System.Collections.Concurrent;
using System.Collections.ObjectModel;

namespace ToInterview.API.Services;

public class ThreadSafeUserEventArgs : EventArgs
{
    public string UserName { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
    public string Action { get; set; } = string.Empty;
    public int ThreadId { get; set; }
}

public delegate void ThreadSafeUserEventHandler(object sender, ThreadSafeUserEventArgs e);

/// <summary>
/// 线程安全的事件服务，支持多线程并发操作
/// </summary>
public class ThreadSafeEventService
{
    private readonly object _lockObject = new object();
    private readonly ConcurrentDictionary<string, DateTime> _userSessions = new();
    private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);

    public event ThreadSafeUserEventHandler? UserLoggedIn;
    public event ThreadSafeUserEventHandler? UserLoggedOut;
    public event ThreadSafeUserEventHandler? UserAction;

    /// <summary>
    /// 模拟用户登录 - 支持并发
    /// </summary>
    public async Task SimulateUserLoginAsync(string userName)
    {
        await _semaphore.WaitAsync();
        try
        {
            await Task.Delay(Random.Shared.Next(50, 200)); // 模拟网络延迟
            
            var args = new ThreadSafeUserEventArgs
            {
                UserName = userName,
                Timestamp = DateTime.Now,
                Action = "Login",
                ThreadId = Environment.CurrentManagedThreadId
            };

            // 线程安全地添加用户会话
            _userSessions.TryAdd(userName, args.Timestamp);
            
            OnUserLoggedIn(args);
        }
        finally
        {
            _semaphore.Release();
        }
    }

    /// <summary>
    /// 模拟用户登出 - 支持并发
    /// </summary>
    public async Task SimulateUserLogoutAsync(string userName)
    {
        await _semaphore.WaitAsync();
        try
        {
            await Task.Delay(Random.Shared.Next(30, 100));
            
            var args = new ThreadSafeUserEventArgs
            {
                UserName = userName,
                Timestamp = DateTime.Now,
                Action = "Logout",
                ThreadId = Environment.CurrentManagedThreadId
            };

            // 线程安全地移除用户会话
            _userSessions.TryRemove(userName, out _);
            
            OnUserLoggedOut(args);
        }
        finally
        {
            _semaphore.Release();
        }
    }

    /// <summary>
    /// 模拟用户操作 - 支持并发
    /// </summary>
    public async Task SimulateUserActionAsync(string userName, string action)
    {
        await Task.Delay(Random.Shared.Next(10, 50));
        
        var args = new ThreadSafeUserEventArgs
        {
            UserName = userName,
            Timestamp = DateTime.Now,
            Action = action,
            ThreadId = Environment.CurrentManagedThreadId
        };
        
        OnUserAction(args);
    }

    /// <summary>
    /// 批量处理用户登录 - 并行执行
    /// </summary>
    public async Task SimulateMultipleUserLoginsAsync(IEnumerable<string> userNames)
    {
        var tasks = userNames.Select(userName => SimulateUserLoginAsync(userName));
        await Task.WhenAll(tasks);
    }

    /// <summary>
    /// 获取当前在线用户 - 线程安全
    /// </summary>
    public ReadOnlyDictionary<string, DateTime> GetOnlineUsers()
    {
        return new ReadOnlyDictionary<string, DateTime>(_userSessions.ToDictionary(kvp => kvp.Key, kvp => kvp.Value));
    }

    /// <summary>
    /// 获取在线用户数量 - 线程安全
    /// </summary>
    public int GetOnlineUserCount()
    {
        return _userSessions.Count;
    }

    protected virtual void OnUserLoggedIn(ThreadSafeUserEventArgs e)
    {
        UserLoggedIn?.Invoke(this, e);
    }

    protected virtual void OnUserLoggedOut(ThreadSafeUserEventArgs e)
    {
        UserLoggedOut?.Invoke(this, e);
    }

    protected virtual void OnUserAction(ThreadSafeUserEventArgs e)
    {
        UserAction?.Invoke(this, e);
    }

    public void Dispose()
    {
        _semaphore?.Dispose();
    }
}

/// <summary>
/// 线程安全的事件处理器
/// </summary>
public class ThreadSafeUserEventHandlers
{
    private readonly ILogger<ThreadSafeUserEventHandlers> _logger;
    private readonly ConcurrentBag<string> _loggedInUsers = new();
    private readonly object _lockObject = new object();

    public ThreadSafeUserEventHandlers(ILogger<ThreadSafeUserEventHandlers> logger)
    {
        _logger = logger;
    }

    public void HandleUserLogin(object? sender, ThreadSafeUserEventArgs e)
    {
        _loggedInUsers.Add(e.UserName);
        Console.WriteLine("线程 {ThreadId}: 用户 {UserName} 在 {Timestamp} 登录", 
            e.ThreadId, e.UserName, e.Timestamp);
    }

    public void HandleUserLogout(object? sender, ThreadSafeUserEventArgs e)
    {
        // 注意：ConcurrentBag 不支持直接移除特定元素
        // 这里只是记录日志，实际应用中可能需要更复杂的逻辑
        Console.WriteLine("线程 {ThreadId}: 用户 {UserName} 在 {Timestamp} 登出", 
            e.ThreadId, e.UserName, e.Timestamp);
    }

    public void HandleUserAction(object? sender, ThreadSafeUserEventArgs e)
    {
        Console.WriteLine("线程 {ThreadId}: 用户 {UserName} 在 {Timestamp} 执行了操作: {Action}", 
            e.ThreadId, e.UserName, e.Timestamp, e.Action);
    }

    public IReadOnlyList<string> GetLoggedInUsers()
    {
        return _loggedInUsers.ToList().AsReadOnly();
    }
}
