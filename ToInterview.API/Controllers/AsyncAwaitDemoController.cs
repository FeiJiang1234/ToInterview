using Microsoft.AspNetCore.Mvc;
using ToInterview.API.Multithreading;

namespace ToInterview.API.Controllers;

/// <summary>
/// async/await 原理演示控制器
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class AsyncAwaitDemoController : ControllerBase
{
    private readonly AsyncAwaitPrincipleDemo _demo;
    private readonly ILogger<AsyncAwaitDemoController> _logger;

    public AsyncAwaitDemoController(
        AsyncAwaitPrincipleDemo demo,
        ILogger<AsyncAwaitDemoController> logger)
    {
        _demo = demo;
        _logger = logger;
    }

    /// <summary>
    /// 运行所有async/await原理演示
    /// </summary>
    [HttpGet("run-all-demos")]
    public async Task<IActionResult> RunAllDemos()
    {
        try
        {
            await _demo.RunAllDemosAsync();
            return Ok("所有演示运行完成，请查看控制台输出");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "运行演示时发生错误");
            return StatusCode(500, $"运行演示时发生错误: {ex.Message}");
        }
    }

    /// <summary>
    /// 基本async/await演示
    /// </summary>
    [HttpGet("basic-demo")]
    public async Task<IActionResult> BasicDemo()
    {
        try
        {
            var demo = new AsyncAwaitPrincipleDemo.BasicAsyncAwaitDemo();
            await demo.DemonstrateSyncVsAsync();
            return Ok("基本演示完成，请查看控制台输出");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "基本演示时发生错误");
            return StatusCode(500, $"基本演示时发生错误: {ex.Message}");
        }
    }

    /// <summary>
    /// 状态机演示
    /// </summary>
    [HttpGet("state-machine-demo")]
    public async Task<IActionResult> StateMachineDemo()
    {
        try
        {
            var demo = new AsyncAwaitPrincipleDemo.StateMachineDemo();
            await demo.DemonstrateStateMachine();
            return Ok("状态机演示完成，请查看控制台输出");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "状态机演示时发生错误");
            return StatusCode(500, $"状态机演示时发生错误: {ex.Message}");
        }
    }

    /// <summary>
    /// ConfigureAwait演示
    /// </summary>
    [HttpGet("configure-await-demo")]
    public async Task<IActionResult> ConfigureAwaitDemo()
    {
        try
        {
            var demo = new AsyncAwaitPrincipleDemo.ConfigureAwaitDemo();
            await demo.DemonstrateConfigureAwait();
            return Ok("ConfigureAwait演示完成，请查看控制台输出");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "ConfigureAwait演示时发生错误");
            return StatusCode(500, $"ConfigureAwait演示时发生错误: {ex.Message}");
        }
    }

    /// <summary>
    /// 异常处理演示
    /// </summary>
    [HttpGet("exception-handling-demo")]
    public async Task<IActionResult> ExceptionHandlingDemo()
    {
        try
        {
            var demo = new AsyncAwaitPrincipleDemo.ExceptionHandlingDemo();
            await demo.HandleMultipleExceptionsAsync();
            return Ok("异常处理演示完成，请查看控制台输出");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "异常处理演示时发生错误");
            return StatusCode(500, $"异常处理演示时发生错误: {ex.Message}");
        }
    }

    /// <summary>
    /// 取消操作演示
    /// </summary>
    [HttpGet("cancellation-demo")]
    public async Task<IActionResult> CancellationDemo()
    {
        try
        {
            var demo = new AsyncAwaitPrincipleDemo.CancellationDemo();
            await demo.DemonstrateCancellationAsync();
            return Ok("取消操作演示完成，请查看控制台输出");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "取消操作演示时发生错误");
            return StatusCode(500, $"取消操作演示时发生错误: {ex.Message}");
        }
    }

    /// <summary>
    /// 异步流演示
    /// </summary>
    [HttpGet("async-stream-demo")]
    public async Task<IActionResult> AsyncStreamDemo()
    {
        try
        {
            var demo = new AsyncAwaitPrincipleDemo.AsyncStreamDemo();
            await demo.ProcessAsyncStreamAsync();
            return Ok("异步流演示完成，请查看控制台输出");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "异步流演示时发生错误");
            return StatusCode(500, $"异步流演示时发生错误: {ex.Message}");
        }
    }

    /// <summary>
    /// ValueTask性能演示
    /// </summary>
    [HttpGet("value-task-demo")]
    public async Task<IActionResult> ValueTaskDemo()
    {
        try
        {
            var demo = new AsyncAwaitPrincipleDemo.ValueTaskDemo();
            await demo.PerformanceComparisonAsync();
            return Ok("ValueTask性能演示完成，请查看控制台输出");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "ValueTask性能演示时发生错误");
            return StatusCode(500, $"ValueTask性能演示时发生错误: {ex.Message}");
        }
    }

    /// <summary>
    /// 获取async/await原理说明
    /// </summary>
    [HttpGet("principles")]
    public IActionResult GetPrinciples()
    {
        var principles = new
        {
            Title = "async/await 核心原理",
            Concepts = new[]
            {
                new
                {
                    Name = "基本概念",
                    Description = "async/await是C#中用于异步编程的语法糖，让异步代码看起来像同步代码",
                    KeyPoints = new[]
                    {
                        "async关键字将方法标记为异步方法",
                        "await关键字暂停方法执行，等待异步操作完成",
                        "编译器将异步方法转换为状态机",
                        "异步方法返回Task或Task<T>",
                        "不会阻塞调用线程"
                    }
                },
                new
                {
                    Name = "状态机工作原理",
                    Description = "编译器将异步方法转换为状态机来管理异步执行流程",
                    KeyPoints = new[]
                    {
                        "创建状态机类实现IAsyncStateMachine接口",
                        "使用MoveNext方法推进状态",
                        "每个await点都是一个状态",
                        "异步操作完成后恢复执行"
                    }
                },
                new
                {
                    Name = "ConfigureAwait",
                    Description = "控制异步操作完成后的执行上下文",
                    KeyPoints = new[]
                    {
                        "ConfigureAwait(false)避免死锁和提高性能",
                        "在库代码中应该使用ConfigureAwait(false)",
                        "在UI代码中通常不需要ConfigureAwait(false)"
                    }
                },
                new
                {
                    Name = "异常处理",
                    Description = "异步方法中的异常处理机制",
                    KeyPoints = new[]
                    {
                        "异常被包装在Task中",
                        "只有在await时才会抛出异常",
                        "多个异步操作可能产生AggregateException"
                    }
                },
                new
                {
                    Name = "取消操作",
                    Description = "使用CancellationToken支持协作式取消",
                    KeyPoints = new[]
                    {
                        "支持协作式取消，避免强制终止线程",
                        "可以链接多个CancellationToken",
                        "取消信号会传播到所有相关的异步操作"
                    }
                },
                new
                {
                    Name = "性能优化",
                    Description = "使用ValueTask等优化异步性能",
                    KeyPoints = new[]
                    {
                        "ValueTask减少不必要的堆分配",
                        "适用于可能同步完成的操作",
                        "提高缓存场景的性能"
                    }
                }
            },
            BestPractices = new[]
            {
                "避免async void（除了事件处理程序）",
                "在库代码中使用ConfigureAwait(false)",
                "正确处理异常",
                "使用CancellationToken",
                "避免阻塞异步代码",
                "使用ValueTask优化性能"
            }
        };

        return Ok(principles);
    }
}
