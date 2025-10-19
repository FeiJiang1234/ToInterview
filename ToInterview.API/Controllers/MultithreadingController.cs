using Microsoft.AspNetCore.Mvc;
using ToInterview.API.Services;

namespace ToInterview.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MultithreadingController : ControllerBase
{
    private readonly MultithreadingExamples _multithreadingExamples;
    private readonly ThreadSafeEventService _eventService;
    private readonly ThreadSafeUserEventHandlers _eventHandlers;
    private readonly ILogger<MultithreadingController> _logger;

    public MultithreadingController(
        MultithreadingExamples multithreadingExamples,
        ThreadSafeEventService eventService,
        ThreadSafeUserEventHandlers eventHandlers,
        ILogger<MultithreadingController> logger)
    {
        _multithreadingExamples = multithreadingExamples;
        _eventService = eventService;
        _eventHandlers = eventHandlers;
        _logger = logger;

        // 订阅事件
        _eventService.UserLoggedIn += _eventHandlers.HandleUserLogin;
        _eventService.UserLoggedOut += _eventHandlers.HandleUserLogout;
        _eventService.UserAction += _eventHandlers.HandleUserAction;
    }

    /// <summary>
    /// 基本异步示例
    /// </summary>
    [HttpGet("basic-async")]
    public async Task<IActionResult> BasicAsyncExample()
    {
        await _multithreadingExamples.BasicAsyncExample();
        return Ok();
    }

    /// <summary>
    /// Parallel.For示例
    /// </summary>
    [HttpGet("parallel-for")]
    public IActionResult ParallelForExample()
    {
        _multithreadingExamples.ParallelForExample();
        return Ok();
    }

    /// <summary>
    /// PLINQ示例
    /// </summary>
    [HttpGet("plinq")]
    public IActionResult PLINQExample()
    {
        _multithreadingExamples.PLINQExample();
        return Ok(new { Message = "PLINQ示例完成" });
    }

    /// <summary>
    /// 生产者-消费者示例
    /// </summary>
    [HttpGet("producer-consumer")]
    public async Task<IActionResult> ProducerConsumerExample()
    {
        await _multithreadingExamples.ProducerConsumerExample();
        return Ok(new { Message = "生产者-消费者示例完成" });
    }

    /// <summary>
    /// 信号量示例
    /// </summary>
    [HttpGet("semaphore")]
    public async Task<IActionResult> SemaphoreExample()
    {
        await _multithreadingExamples.SemaphoreExample();
        return Ok(new { Message = "信号量示例完成" });
    }

    /// <summary>
    /// 线程安全集合示例
    /// </summary>
    [HttpGet("thread-safe-collections")]
    public IActionResult ThreadSafeCollectionsExample()
    {
        _multithreadingExamples.ThreadSafeCollectionsExample();
        return Ok(new { Message = "线程安全集合示例完成" });
    }

    /// <summary>
    /// 取消令牌示例
    /// </summary>
    [HttpGet("cancellation-token")]
    public async Task<IActionResult> CancellationTokenExample()
    {
        await _multithreadingExamples.CancellationTokenExample();
        return Ok(new { Message = "取消令牌示例完成" });
    }

    /// <summary>
    /// 异常处理示例
    /// </summary>
    [HttpGet("exception-handling")]
    public async Task<IActionResult> ExceptionHandlingExample()
    {
        await _multithreadingExamples.ExceptionHandlingExample();
        return Ok(new { Message = "异常处理示例完成" });
    }

    /// <summary>
    /// 模拟多个用户并发登录
    /// </summary>
    [HttpPost("simulate-concurrent-logins")]
    public async Task<IActionResult> SimulateConcurrentLogins([FromBody] string[] userNames)
    {
        if (userNames == null || userNames.Length == 0)
        {
            return BadRequest("用户名列表不能为空");
        }

        Console.WriteLine("开始模拟 {Count} 个用户的并发登录", userNames.Length);

        // 并发执行用户登录
        await _eventService.SimulateMultipleUserLoginsAsync(userNames);

        var onlineUsers = _eventService.GetOnlineUsers();
        var loggedInUsers = _eventHandlers.GetLoggedInUsers();

        return Ok(new
        {
            Message = "并发登录模拟完成",
            OnlineUserCount = _eventService.GetOnlineUserCount(),
            OnlineUsers = onlineUsers.Keys.ToArray(),
            LoggedInUsers = loggedInUsers
        });
    }

    /// <summary>
    /// 模拟用户操作
    /// </summary>
    [HttpPost("simulate-user-action")]
    public async Task<IActionResult> SimulateUserAction([FromBody] UserActionRequest request)
    {
        if (string.IsNullOrEmpty(request.UserName) || string.IsNullOrEmpty(request.Action))
        {
            return BadRequest("用户名和操作不能为空");
        }

        await _eventService.SimulateUserActionAsync(request.UserName, request.Action);

        return Ok(new { Message = $"用户 {request.UserName} 执行操作 {request.Action} 完成" });
    }

    /// <summary>
    /// 获取当前在线用户
    /// </summary>
    [HttpGet("online-users")]
    public IActionResult GetOnlineUsers()
    {
        var onlineUsers = _eventService.GetOnlineUsers();
        var loggedInUsers = _eventHandlers.GetLoggedInUsers();

        return Ok(new
        {
            OnlineUserCount = _eventService.GetOnlineUserCount(),
            OnlineUsers = onlineUsers.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.ToString("yyyy-MM-dd HH:mm:ss")),
            LoggedInUsers = loggedInUsers
        });
    }

    /// <summary>
    /// 压力测试 - 大量并发请求
    /// </summary>
    [HttpPost("stress-test")]
    public async Task<IActionResult> StressTest([FromQuery] int userCount = 100, [FromQuery] int actionsPerUser = 5)
    {
        Console.WriteLine("开始压力测试: {UserCount} 用户, 每用户 {ActionsPerUser} 个操作", 
            userCount, actionsPerUser);

        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        // 生成测试用户
        var testUsers = Enumerable.Range(1, userCount)
            .Select(i => $"TestUser{i}")
            .ToArray();

        // 并发登录
        await _eventService.SimulateMultipleUserLoginsAsync(testUsers);

        // 并发执行用户操作
        var actionTasks = testUsers.SelectMany(userName =>
            Enumerable.Range(1, actionsPerUser)
                .Select(actionId => _eventService.SimulateUserActionAsync(userName, $"Action{actionId}")))
            .ToArray();

        await Task.WhenAll(actionTasks);

        stopwatch.Stop();

        var onlineUsers = _eventService.GetOnlineUsers();

        return Ok(new
        {
            Message = "压力测试完成",
            TotalUsers = userCount,
            ActionsPerUser = actionsPerUser,
            TotalActions = userCount * actionsPerUser,
            ElapsedMilliseconds = stopwatch.ElapsedMilliseconds,
            OnlineUserCount = _eventService.GetOnlineUserCount(),
            Throughput = (userCount * actionsPerUser) / (stopwatch.ElapsedMilliseconds / 1000.0) // 操作/秒
        });
    }
}

public class UserActionRequest
{
    public string UserName { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty;
}
