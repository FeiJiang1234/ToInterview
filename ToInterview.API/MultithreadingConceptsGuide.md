# 多线程编程核心概念详解

## 📚 目录

1. [基础概念](#基础概念)
2. [同步原语](#同步原语)
3. [异步编程](#异步编程)
4. [并发模式](#并发模式)
5. [最佳实践](#最佳实践)
6. [性能优化](#性能优化)

## 🔧 基础概念

### 进程 vs 线程 vs 任务

| 概念 | 定义 | 特点 | 使用场景 |
|------|------|------|----------|
| **进程** | 操作系统分配资源的基本单位 | 独立内存空间，进程间通信复杂 | 系统级应用，需要隔离 |
| **线程** | 进程内的执行单元 | 共享内存空间，创建开销大 | 需要精确控制的场景 |
| **任务** | .NET中的高级抽象 | 基于线程池，开销小，支持异步 | 现代.NET应用的首选 |

### 同步 vs 异步

```csharp
// 同步 - 阻塞执行
public string SyncMethod()
{
    Thread.Sleep(2000); // 阻塞线程
    return "结果";
}

// 异步 - 非阻塞执行
public async Task<string> AsyncMethod()
{
    await Task.Delay(2000); // 不阻塞线程
    return "结果";
}
```

### 并发 vs 并行

- **并发 (Concurrency)**: 多个任务在同一时间段内执行，可能交替进行
- **并行 (Parallelism)**: 多个任务在同一时刻执行，需要多个核心

## 🔒 同步原语

### 1. lock 语句
```csharp
private readonly object _lockObject = new object();
private int _counter = 0;

public void Increment()
{
    lock (_lockObject)
    {
        _counter++; // 线程安全
    }
}
```

### 2. SemaphoreSlim
```csharp
private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(3, 3);

public async Task ProcessAsync()
{
    await _semaphore.WaitAsync();
    try
    {
        // 最多3个并发执行
        await DoWorkAsync();
    }
    finally
    {
        _semaphore.Release();
    }
}
```

### 3. ReaderWriterLockSlim
```csharp
private readonly ReaderWriterLockSlim _rwLock = new ReaderWriterLockSlim();

public string ReadData()
{
    _rwLock.EnterReadLock();
    try
    {
        return _data; // 多个读者可以同时访问
    }
    finally
    {
        _rwLock.ExitReadLock();
    }
}

public void WriteData(string value)
{
    _rwLock.EnterWriteLock();
    try
    {
        _data = value; // 写者独占访问
    }
    finally
    {
        _rwLock.ExitWriteLock();
    }
}
```

### 4. 线程安全集合
```csharp
// 使用线程安全集合，无需额外同步
private readonly ConcurrentDictionary<string, int> _cache = new();
private readonly ConcurrentQueue<string> _queue = new();
private readonly ConcurrentBag<string> _bag = new();
```

## ⚡ 异步编程

### async/await 核心概念

```csharp
public async Task<string> AsyncMethod()
{
    // 1. async 标记方法为异步
    // 2. await 暂停执行，等待异步操作完成
    // 3. 编译器生成状态机
    var result = await SomeAsyncOperation();
    return result;
}
```

### ConfigureAwait 的重要性

```csharp
// 库代码 - 使用 ConfigureAwait(false)
public async Task<string> LibraryMethodAsync()
{
    var result = await httpClient.GetStringAsync(url)
        .ConfigureAwait(false); // 避免死锁，提高性能
    return result;
}

// UI代码 - 通常不需要 ConfigureAwait(false)
public async Task<string> UIMethodAsync()
{
    var result = await LibraryMethodAsync(); // 自动回到UI线程
    return result;
}
```

### 异常处理

```csharp
public async Task HandleExceptionsAsync()
{
    try
    {
        await SomeAsyncOperation();
    }
    catch (HttpRequestException ex)
    {
        // 处理特定异常
        Console.WriteLine("HTTP错误: {Message}", ex.Message);
    }
    catch (Exception ex)
    {
        // 处理其他异常
        Console.WriteLine("未预期错误: {Message}", ex.Message);
    }
}
```

### 取消操作

```csharp
public async Task CancellableOperationAsync(CancellationToken cancellationToken)
{
    for (int i = 0; i < 100; i++)
    {
        cancellationToken.ThrowIfCancellationRequested();
        await Task.Delay(100, cancellationToken);
    }
}
```

## 🎯 并发模式

### 1. 生产者-消费者模式
```csharp
private readonly BlockingCollection<string> _queue = new();

// 生产者
public async Task ProducerAsync()
{
    while (true)
    {
        var item = GenerateItem();
        _queue.Add(item);
        await Task.Delay(100);
    }
}

// 消费者
public async Task ConsumerAsync()
{
    foreach (var item in _queue.GetConsumingEnumerable())
    {
        await ProcessItem(item);
    }
}
```

### 2. 发布-订阅模式
```csharp
private readonly ConcurrentDictionary<string, List<Func<object, Task>>> _subscribers = new();

public void Subscribe(string eventName, Func<object, Task> handler)
{
    _subscribers.AddOrUpdate(eventName, 
        new List<Func<object, Task>> { handler },
        (key, existing) => { existing.Add(handler); return existing; });
}

public async Task PublishAsync(string eventName, object data)
{
    if (_subscribers.TryGetValue(eventName, out var handlers))
    {
        var tasks = handlers.Select(handler => handler(data));
        await Task.WhenAll(tasks);
    }
}
```

### 3. 管道模式
```csharp
public async Task PipelineAsync()
{
    var input = Enumerable.Range(1, 100);
    
    var results = input
        .AsParallel()
        .Select(ParseData)      // 阶段1: 解析
        .Select(ProcessData)    // 阶段2: 处理
        .Select(FormatData);    // 阶段3: 格式化
    
    foreach (var result in results)
    {
        Console.WriteLine(result);
    }
}
```

### 4. 扇出-扇入模式
```csharp
public async Task FanOutFanInAsync()
{
    var inputData = Enumerable.Range(1, 100);
    
    // 扇出：分发到多个处理器
    var tasks = inputData.Select(ProcessDataAsync);
    
    // 扇入：收集结果
    var results = await Task.WhenAll(tasks);
    
    // 聚合结果
    var sum = results.Sum();
}
```

## ✅ 最佳实践

### 1. 避免常见陷阱

```csharp
// ❌ 错误：阻塞异步代码
var result = SomeAsyncMethod().Result; // 可能导致死锁

// ✅ 正确：使用 async/await
var result = await SomeAsyncMethod();

// ❌ 错误：异步void（除了事件处理程序）
public async void BadMethod() { }

// ✅ 正确：返回Task
public async Task GoodMethod() { }
```

### 2. 性能优化

```csharp
// 使用ValueTask优化可能同步完成的操作
public async ValueTask<string> OptimizedMethodAsync()
{
    if (_cache.TryGetValue(key, out var value))
    {
        return value; // 同步返回，无堆分配
    }
    
    await Task.Delay(1000); // 异步操作
    return "异步结果";
}

// 批量处理
public async Task ProcessBatchAsync<T>(IEnumerable<T> items, Func<T, Task> processor)
{
    var batches = items.Chunk(10);
    foreach (var batch in batches)
    {
        var tasks = batch.Select(processor);
        await Task.WhenAll(tasks);
    }
}
```

### 3. 资源管理

```csharp
public class AsyncResourceManager : IAsyncDisposable
{
    private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);
    private bool _disposed = false;

    public async ValueTask DisposeAsync()
    {
        if (!_disposed)
        {
            _semaphore?.Dispose();
            _disposed = true;
        }
        await Task.CompletedTask;
    }
}
```

## 🚀 性能优化

### 1. 选择合适的同步原语

| 场景 | 推荐原语 | 原因 |
|------|----------|------|
| 一般同步 | lock | 简单易用，性能良好 |
| 限制并发 | SemaphoreSlim | 支持异步，灵活控制 |
| 读多写少 | ReaderWriterLockSlim | 提高读操作并发性 |
| 跨进程 | Mutex | 支持跨进程同步 |
| 信号通知 | AutoResetEvent/ManualResetEventSlim | 适合事件驱动场景 |

### 2. 线程安全集合选择

| 需求 | 推荐集合 | 特点 |
|------|----------|------|
| 队列操作 | ConcurrentQueue | FIFO，线程安全 |
| 栈操作 | ConcurrentStack | LIFO，线程安全 |
| 无序集合 | ConcurrentBag | 高性能，线程安全 |
| 键值对 | ConcurrentDictionary | 字典操作，线程安全 |

### 3. 异步性能优化

```csharp
// 并行执行独立任务
var task1 = Operation1Async();
var task2 = Operation2Async();
var task3 = Operation3Async();

var results = await Task.WhenAll(task1, task2, task3);

// 使用ConfigureAwait(false)在库代码中
public async Task<string> LibraryMethodAsync()
{
    var result = await SomeAsyncOperation()
        .ConfigureAwait(false);
    return result;
}
```

## 📊 性能监控

### 关键指标

1. **吞吐量**: 单位时间内处理的操作数
2. **延迟**: 单个操作的处理时间
3. **并发度**: 同时执行的操作数
4. **资源利用率**: CPU、内存使用情况

### 监控工具

```csharp
public class PerformanceMonitor
{
    private readonly Stopwatch _stopwatch = new Stopwatch();
    private long _operationCount = 0;
    private long _totalTime = 0;

    public void StartOperation()
    {
        _stopwatch.Restart();
    }

    public void EndOperation()
    {
        _stopwatch.Stop();
        Interlocked.Increment(ref _operationCount);
        Interlocked.Add(ref _totalTime, _stopwatch.ElapsedMilliseconds);
    }

    public double GetAverageLatency()
    {
        var count = Interlocked.Read(ref _operationCount);
        var total = Interlocked.Read(ref _totalTime);
        return count > 0 ? (double)total / count : 0;
    }
}
```

## 🎓 学习建议

1. **从基础开始**: 理解进程、线程、任务的基本概念
2. **实践同步原语**: 掌握各种同步机制的使用场景
3. **深入异步编程**: 理解async/await的工作原理
4. **学习并发模式**: 掌握常见的并发设计模式
5. **性能调优**: 学会分析和优化多线程应用性能
6. **避免陷阱**: 了解常见的多线程编程陷阱和解决方案

## 🔗 相关资源

- [.NET 异步编程文档](https://docs.microsoft.com/en-us/dotnet/csharp/async)
- [并发编程指南](https://docs.microsoft.com/en-us/dotnet/standard/parallel-programming/)
- [线程安全集合](https://docs.microsoft.com/en-us/dotnet/standard/collections/thread-safe/)
- [性能优化最佳实践](https://docs.microsoft.com/en-us/dotnet/framework/performance/)

---

*这个指南涵盖了多线程编程的核心概念，从基础理论到实际应用，帮助您掌握现代.NET应用中的并发编程技术。*
