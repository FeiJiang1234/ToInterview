using System.Collections.Concurrent;
using System.Diagnostics;

namespace ToInterview.API.Services;

/// <summary>
/// 多线程编程示例和最佳实践
/// </summary>
public class MultithreadingExamples
{
    private readonly ILogger<MultithreadingExamples> _logger;
    private readonly ConcurrentQueue<string> _taskQueue = new();
    private readonly CancellationTokenSource _cancellationTokenSource = new();
    private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(3, 3); // 最多3个并发任务

    public MultithreadingExamples(ILogger<MultithreadingExamples> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// 示例1: 基本的Task和async/await
    /// </summary>
    public async Task BasicAsyncExample()
    {
        // 串行执行
        var startTime = Stopwatch.StartNew();
        await Task1();
        await Task2();
        await Task3();
        startTime.Stop();
        Console.WriteLine(startTime.ElapsedMilliseconds);

        // 并行执行
        startTime.Restart();
        var tasks = new[] { Task1(), Task2(), Task3() };
        await Task.WhenAll(tasks);
        startTime.Stop();
        Console.WriteLine(startTime.ElapsedMilliseconds);
    }

    private async Task Task1()
    {
        await Task.Delay(1000);
        Console.WriteLine($"Task1 complete - ID: {Environment.CurrentManagedThreadId}");
    }

    private async Task Task2()
    {
        await Task.Delay(1500);
        Console.WriteLine($"Task2 complete - ID: {Environment.CurrentManagedThreadId}");
    }

    private async Task Task3()
    {
        await Task.Delay(800);
        Console.WriteLine($"Task3 complete - ID: {Environment.CurrentManagedThreadId}");
    }

    /// <summary>
    /// 示例2: 使用Parallel.For进行并行循环
    /// </summary>
    public void ParallelForExample()
    {
        var numbers = Enumerable.Range(1, 1000000).ToArray();
        var results = new int[numbers.Length];

        var stopwatch = Stopwatch.StartNew();

        // 并行处理
        Parallel.For(0, numbers.Length, i =>
        {
            results[i] = numbers[i] * numbers[i]; // 计算平方
        });

        stopwatch.Stop();

        Console.WriteLine($"Parallel.For {numbers.Length}");
        Console.WriteLine($"{stopwatch.ElapsedMilliseconds}ms");
    }

    /// <summary>
    /// 示例3: 使用PLINQ进行并行查询
    /// </summary>
    public void PLINQExample()
    {
        var numbers = Enumerable.Range(1, 1000000);

        var stopwatch = Stopwatch.StartNew();

        // 并行LINQ查询
        var evenSquares = numbers
            .AsParallel()
            .Where(x => x % 2 == 0)
            .Select(x => x * x)
            .OrderBy(x => x)
            .Take(100)
            .ToList();

        stopwatch.Stop();
        Console.WriteLine(stopwatch.ElapsedMilliseconds);
        Console.WriteLine(evenSquares.Count);
    }

    /// <summary>
    /// 示例4: 生产者-消费者模式
    /// </summary>
    public async Task ProducerConsumerExample()
    {
        Console.WriteLine("开始生产者-消费者示例");

        var cancellationToken = _cancellationTokenSource.Token;

        // 启动生产者
        var producerTask = Task.Run(() => Producer(cancellationToken));

        // 启动多个消费者
        var consumerTasks = Enumerable.Range(1, 3)
            .Select(i => Task.Run(() => Consumer(i, cancellationToken)))
            .ToArray();

        // 等待一段时间后停止
        await Task.Delay(5000);
        _cancellationTokenSource.Cancel();

        await Task.WhenAll(new[] { producerTask }.Concat(consumerTasks));
        Console.WriteLine("生产者-消费者示例完成");
    }

    private async Task Producer(CancellationToken cancellationToken)
    {
        var counter = 0;
        while (!cancellationToken.IsCancellationRequested)
        {
            var item = $"任务-{++counter}";
            _taskQueue.Enqueue(item);
            Console.WriteLine("生产者生成了: {Item}", item);
            await Task.Delay(100, cancellationToken);
        }
    }

    private async Task Consumer(int consumerId, CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            if (_taskQueue.TryDequeue(out var item))
            {
                Console.WriteLine("消费者 {ConsumerId} 处理: {Item} - 线程ID: {ThreadId}", 
                    consumerId, item, Environment.CurrentManagedThreadId);
                await Task.Delay(200, cancellationToken); // 模拟处理时间
            }
            else
            {
                await Task.Delay(50, cancellationToken); // 队列为空时等待
            }
        }
    }

    /// <summary>
    /// 示例5: 使用信号量控制并发
    /// </summary>
    public async Task SemaphoreExample()
    {
        Console.WriteLine("开始信号量示例");

        var tasks = Enumerable.Range(1, 10)
            .Select(i => ProcessWithSemaphore(i))
            .ToArray();

        await Task.WhenAll(tasks);
        Console.WriteLine("信号量示例完成");
    }

    private async Task ProcessWithSemaphore(int taskId)
    {
        await _semaphore.WaitAsync();
        try
        {
            Console.WriteLine("任务 {TaskId} 开始执行 - 线程ID: {ThreadId}", 
                taskId, Environment.CurrentManagedThreadId);
            await Task.Delay(2000); // 模拟工作
            Console.WriteLine("任务 {TaskId} 完成", taskId);
        }
        finally
        {
            _semaphore.Release();
        }
    }

    /// <summary>
    /// 示例6: 线程安全的集合操作
    /// </summary>
    public void ThreadSafeCollectionsExample()
    {
        Console.WriteLine("开始线程安全集合示例");

        var concurrentDictionary = new ConcurrentDictionary<string, int>();
        var concurrentBag = new ConcurrentBag<string>();
        var concurrentQueue = new ConcurrentQueue<int>();

        // 并行添加数据
        Parallel.For(0, 1000, i =>
        {
            var key = $"key-{i}";
            concurrentDictionary.TryAdd(key, i);
            concurrentBag.Add($"item-{i}");
            concurrentQueue.Enqueue(i);
        });

        Console.WriteLine("ConcurrentDictionary 大小: {Count}", concurrentDictionary.Count);
        Console.WriteLine("ConcurrentBag 大小: {Count}", concurrentBag.Count);
        Console.WriteLine("ConcurrentQueue 大小: {Count}", concurrentQueue.Count);
    }

    /// <summary>
    /// 示例7: 取消令牌的使用
    /// </summary>
    public async Task CancellationTokenExample()
    {
        Console.WriteLine("开始取消令牌示例");

        using var cts = new CancellationTokenSource();
        
        // 5秒后自动取消
        cts.CancelAfter(TimeSpan.FromSeconds(5));

        try
        {
            await LongRunningTask(cts.Token);
        }
        catch (OperationCanceledException)
        {
            Console.WriteLine("任务被取消");
        }
    }

    private async Task LongRunningTask(CancellationToken cancellationToken)
    {
        for (int i = 0; i < 100; i++)
        {
            cancellationToken.ThrowIfCancellationRequested();
            
            Console.WriteLine("执行步骤 {Step}", i);
            await Task.Delay(1000, cancellationToken);
        }
    }

    /// <summary>
    /// 示例8: 异常处理在多线程中的最佳实践
    /// </summary>
    public async Task ExceptionHandlingExample()
    {
        Console.WriteLine("开始异常处理示例");

        var tasks = new[]
        {
            Task.Run(() => { throw new InvalidOperationException("任务1失败"); }),
            Task.Run(() => { throw new ArgumentException("任务2失败"); }),
            Task.Run(() => { Console.WriteLine("任务3成功完成"); })
        };

        try
        {
            await Task.WhenAll(tasks);
        }
        catch (AggregateException ex)
        {
            Console.WriteLine("捕获到聚合异常: {Message}", ex.Message);
            foreach (var innerEx in ex.InnerExceptions)
            {
                Console.WriteLine("内部异常: {Type} - {Message}", 
                    innerEx.GetType().Name, innerEx.Message);
            }
        }
    }

    public void Dispose()
    {
        _cancellationTokenSource?.Dispose();
        _semaphore?.Dispose();
    }
}
