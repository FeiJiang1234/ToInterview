using System.Collections.Concurrent;

namespace ToInterview.API.Services;

/// <summary>
/// 并发编程模式详解
/// </summary>
public class ConcurrencyPatterns
{
    private readonly ILogger<ConcurrencyPatterns> _logger;

    public ConcurrencyPatterns(ILogger<ConcurrencyPatterns> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// 模式1: 生产者-消费者模式
    /// </summary>
    public class ProducerConsumerPattern
    {
        private readonly BlockingCollection<string> _queue = new();
        private readonly CancellationTokenSource _cancellationTokenSource = new();

        public async Task StartProducerConsumerAsync()
        {
            Console.WriteLine("=== 生产者-消费者模式 ===");

            // 启动生产者
            var producerTask = Task.Run(() => Producer(_cancellationTokenSource.Token));

            // 启动多个消费者
            var consumerTasks = Enumerable.Range(1, 3)
                .Select(i => Task.Run(() => Consumer(i, _cancellationTokenSource.Token)))
                .ToArray();

            // 运行一段时间后停止
            await Task.Delay(10000);
            _cancellationTokenSource.Cancel();

            await Task.WhenAll(new[] { producerTask }.Concat(consumerTasks));
        }

        private async Task Producer(CancellationToken cancellationToken)
        {
            var counter = 0;
            while (!cancellationToken.IsCancellationRequested)
            {
                var item = $"产品-{++counter}";
                _queue.Add(item);
                Console.WriteLine("生产者生产: {Item}", item);
                await Task.Delay(500, cancellationToken);
            }
            
            _queue.CompleteAdding();
        }

        private async Task Consumer(int consumerId, CancellationToken cancellationToken)
        {
            try
            {
                foreach (var item in _queue.GetConsumingEnumerable(cancellationToken))
                {
                    Console.WriteLine("消费者 {ConsumerId} 消费: {Item}", consumerId, item);
                    await Task.Delay(1000, cancellationToken); // 模拟处理时间
                }
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("消费者 {ConsumerId} 停止", consumerId);
            }
        }

        public void Dispose()
        {
            _cancellationTokenSource?.Dispose();
            _queue?.Dispose();
        }
    }

    /// <summary>
    /// 模式2: 工作窃取模式 (Work Stealing)
    /// </summary>
    public class WorkStealingPattern
    {
        private readonly ConcurrentQueue<WorkItem> _globalQueue = new();
        private readonly ThreadLocal<ConcurrentQueue<WorkItem>> _localQueues = 
            new(() => new ConcurrentQueue<WorkItem>());

        public async Task StartWorkStealingAsync()
        {
            Console.WriteLine("=== 工作窃取模式 ===");

            // 添加工作项到全局队列
            for (int i = 0; i < 100; i++)
            {
                _globalQueue.Enqueue(new WorkItem { Id = i, Data = $"工作项-{i}" });
            }

            // 启动多个工作线程
            var workerTasks = Enumerable.Range(1, 4)
                .Select(workerId => Task.Run(() => Worker(workerId)))
                .ToArray();

            await Task.WhenAll(workerTasks);
        }

        private async Task Worker(int workerId)
        {
            var processedCount = 0;
            
            while (true)
            {
                WorkItem item = null;

                // 首先尝试从本地队列获取工作
                if (_localQueues.Value.TryDequeue(out item))
                {
                    Console.WriteLine("工作者 {WorkerId} 从本地队列获取: {Item}", 
                        workerId, item.Data);
                }
                // 如果本地队列为空，尝试从全局队列获取
                else if (_globalQueue.TryDequeue(out item))
                {
                    Console.WriteLine("工作者 {WorkerId} 从全局队列获取: {Item}", 
                        workerId, item.Data);
                }
                // 如果都为空，尝试从其他工作者的本地队列窃取
                else
                {
                    item = TryStealWork(workerId);
                    if (item != null)
                    {
                        Console.WriteLine("工作者 {WorkerId} 窃取工作: {Item}", 
                            workerId, item.Data);
                    }
                }

                if (item == null)
                {
                    break; // 没有更多工作
                }

                // 处理工作项
                await ProcessWorkItem(item);
                processedCount++;

                // 将新生成的工作项添加到本地队列
                if (processedCount % 5 == 0)
                {
                    var newItem = new WorkItem 
                    { 
                        Id = processedCount * 1000, 
                        Data = $"新工作项-{processedCount}" 
                    };
                    _localQueues.Value.Enqueue(newItem);
                }
            }

            Console.WriteLine("工作者 {WorkerId} 完成，处理了 {Count} 个项目", 
                workerId, processedCount);
        }

        private WorkItem TryStealWork(int currentWorkerId)
        {
            // 简化的窃取逻辑：从其他线程的本地队列窃取
            foreach (var kvp in _localQueues.Values)
            {
                if (kvp.TryDequeue(out var item))
                {
                    return item;
                }
            }
            return null;
        }

        private async Task ProcessWorkItem(WorkItem item)
        {
            await Task.Delay(Random.Shared.Next(100, 500)); // 模拟处理时间
        }

        private class WorkItem
        {
            public int Id { get; set; }
            public string Data { get; set; } = string.Empty;
        }
    }

    /// <summary>
    /// 模式3: 管道模式 (Pipeline Pattern)
    /// </summary>
    public class PipelinePattern
    {
        public async Task StartPipelineAsync()
        {
            Console.WriteLine("=== 管道模式 ===");

            var input = Enumerable.Range(1, 10).Select(i => $"数据-{i}");

            // 创建管道阶段
            var stage1 = new TransformStage<string, int>("解析", ParseData);
            var stage2 = new TransformStage<int, double>("计算", CalculateData);
            var stage3 = new TransformStage<double, string>("格式化", FormatData);

            // 连接管道
            var pipeline = input
                .AsParallel()
                .Select(stage1.Process)
                .Select(stage2.Process)
                .Select(stage3.Process);

            // 处理结果
            foreach (var result in pipeline)
            {
                Console.WriteLine("管道输出: {Result}", result);
            }
        }

        private int ParseData(string input)
        {
            Thread.Sleep(100); // 模拟处理时间
            return input.Split('-')[1].Length;
        }

        private double CalculateData(int input)
        {
            Thread.Sleep(150); // 模拟处理时间
            return input * Math.PI;
        }

        private string FormatData(double input)
        {
            Thread.Sleep(50); // 模拟处理时间
            return $"结果: {input:F2}";
        }

        private class TransformStage<TInput, TOutput>
        {
            private readonly string _name;
            private readonly Func<TInput, TOutput> _transform;

            public TransformStage(string name, Func<TInput, TOutput> transform)
            {
                _name = name;
                _transform = transform;
            }

            public TOutput Process(TInput input)
            {
                var result = _transform(input);
                return result;
            }
        }
    }

    /// <summary>
    /// 模式4: 扇出-扇入模式 (Fan-out/Fan-in)
    /// </summary>
    public class FanOutFanInPattern
    {
        public async Task StartFanOutFanInAsync()
        {
            Console.WriteLine("=== 扇出-扇入模式 ===");

            var inputData = Enumerable.Range(1, 20).ToArray();

            // 扇出：将数据分发到多个处理器
            var processingTasks = inputData
                .Select(data => ProcessDataAsync(data))
                .ToArray();

            // 扇入：收集所有结果
            var results = await Task.WhenAll(processingTasks);

            // 聚合结果
            var finalResult = results.Sum();
            Console.WriteLine("最终结果: {Result}", finalResult);
        }

        private async Task<int> ProcessDataAsync(int data)
        {
            await Task.Delay(Random.Shared.Next(500, 1500)); // 模拟处理时间
            var result = data * data;
            Console.WriteLine("处理数据 {Data} -> {Result}", data, result);
            return result;
        }
    }

    /// <summary>
    /// 模式5: 发布-订阅模式 (Publisher-Subscriber)
    /// </summary>
    public class PublisherSubscriberPattern
    {
        private readonly ConcurrentDictionary<string, List<Func<object, Task>>> _subscribers = new();
        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);

        public async Task StartPublisherSubscriberAsync()
        {
            Console.WriteLine("=== 发布-订阅模式 ===");

            // 订阅事件
            Subscribe("user.login", HandleUserLogin);
            Subscribe("user.logout", HandleUserLogout);
            Subscribe("user.action", HandleUserAction);

            // 发布事件
            await PublishAsync("user.login", new { UserId = 1, UserName = "张三" });
            await Task.Delay(1000);
            
            await PublishAsync("user.action", new { UserId = 1, Action = "查看资料" });
            await Task.Delay(1000);
            
            await PublishAsync("user.logout", new { UserId = 1, UserName = "张三" });
        }

        public void Subscribe(string eventName, Func<object, Task> handler)
        {
            _subscribers.AddOrUpdate(
                eventName,
                new List<Func<object, Task>> { handler },
                (key, existing) => { existing.Add(handler); return existing; });
        }

        public async Task PublishAsync(string eventName, object data)
        {
            if (_subscribers.TryGetValue(eventName, out var handlers))
            {
                Console.WriteLine("发布事件: {EventName}", eventName);
                
                var tasks = handlers.Select(handler => handler(data));
                await Task.WhenAll(tasks);
            }
        }

        private async Task HandleUserLogin(object data)
        {
            await Task.Delay(200);
            Console.WriteLine("处理用户登录事件: {Data}", data);
        }

        private async Task HandleUserLogout(object data)
        {
            await Task.Delay(150);
            Console.WriteLine("处理用户登出事件: {Data}", data);
        }

        private async Task HandleUserAction(object data)
        {
            await Task.Delay(100);
            Console.WriteLine("处理用户操作事件: {Data}", data);
        }
    }

    /// <summary>
    /// 模式6: 读写锁模式 (Reader-Writer Lock)
    /// </summary>
    public class ReaderWriterLockPattern
    {
        private readonly ReaderWriterLockSlim _rwLock = new ReaderWriterLockSlim();
        private readonly Dictionary<string, string> _cache = new();
        private int _readCount = 0;
        private int _writeCount = 0;

        public async Task StartReaderWriterLockAsync()
        {
            Console.WriteLine("=== 读写锁模式 ===");

            // 启动多个读取任务
            var readTasks = Enumerable.Range(1, 5)
                .Select(i => Task.Run(() => Reader(i)))
                .ToArray();

            // 启动写入任务
            var writeTasks = Enumerable.Range(1, 2)
                .Select(i => Task.Run(() => Writer(i)))
                .ToArray();

            await Task.WhenAll(readTasks.Concat(writeTasks));
        }

        private async Task Reader(int readerId)
        {
            for (int i = 0; i < 10; i++)
            {
                _rwLock.EnterReadLock();
                try
                {
                    Interlocked.Increment(ref _readCount);
                    Console.WriteLine("读者 {ReaderId} 读取缓存，当前读取次数: {ReadCount}", 
                        readerId, _readCount);
                    
                    // 模拟读取操作
                    await Task.Delay(100);
                }
                finally
                {
                    _rwLock.ExitReadLock();
                }
                
                await Task.Delay(200);
            }
        }

        private async Task Writer(int writerId)
        {
            for (int i = 0; i < 5; i++)
            {
                _rwLock.EnterWriteLock();
                try
                {
                    Interlocked.Increment(ref _writeCount);
                    var key = $"key-{writerId}-{i}";
                    var value = $"value-{writerId}-{i}";
                    _cache[key] = value;
                    
                    Console.WriteLine("写者 {WriterId} 写入缓存: {Key} = {Value}, 当前写入次数: {WriteCount}", 
                        writerId, key, value, _writeCount);
                    
                    // 模拟写入操作
                    await Task.Delay(300);
                }
                finally
                {
                    _rwLock.ExitWriteLock();
                }
                
                await Task.Delay(500);
            }
        }

        public void Dispose()
        {
            _rwLock?.Dispose();
        }
    }

    /// <summary>
    /// 模式7: 屏障模式 (Barrier Pattern)
    /// </summary>
    public class BarrierPattern
    {
        private readonly Barrier _barrier = new Barrier(3, barrier =>
        {
            Console.WriteLine("所有参与者到达屏障点 {Phase}", barrier.CurrentPhaseNumber);
        });

        public async Task StartBarrierAsync()
        {
            Console.WriteLine("=== 屏障模式 ===");

            var tasks = new[]
            {
                Task.Run(() => Participant("参与者1")),
                Task.Run(() => Participant("参与者2")),
                Task.Run(() => Participant("参与者3"))
            };

            await Task.WhenAll(tasks);
        }

        private async Task Participant(string name)
        {
            for (int phase = 0; phase < 3; phase++)
            {
                Console.WriteLine("{Name} 开始阶段 {Phase}", name, phase);
                
                // 模拟工作
                await Task.Delay(Random.Shared.Next(1000, 3000));
                
                Console.WriteLine("{Name} 完成阶段 {Phase}，等待其他参与者", name, phase);
                
                // 等待所有参与者到达
                _barrier.SignalAndWait();
            }
            
            Console.WriteLine("{Name} 完成所有阶段", name);
        }

        public void Dispose()
        {
            _barrier?.Dispose();
        }
    }

    /// <summary>
    /// 模式8: 信号量模式 (Semaphore Pattern)
    /// </summary>
    public class SemaphorePattern
    {
        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(3, 3); // 最多3个并发

        public async Task StartSemaphoreAsync()
        {
            Console.WriteLine("=== 信号量模式 ===");

            var tasks = Enumerable.Range(1, 10)
                .Select(i => Task.Run(() => Worker(i)))
                .ToArray();

            await Task.WhenAll(tasks);
        }

        private async Task Worker(int workerId)
        {
            Console.WriteLine("工作者 {WorkerId} 请求资源", workerId);
            
            await _semaphore.WaitAsync();
            try
            {
                Console.WriteLine("工作者 {WorkerId} 获得资源，开始工作", workerId);
                
                // 模拟工作
                await Task.Delay(Random.Shared.Next(2000, 5000));
                
                Console.WriteLine("工作者 {WorkerId} 完成工作，释放资源", workerId);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public void Dispose()
        {
            _semaphore?.Dispose();
        }
    }

    /// <summary>
    /// 模式9: 线程池模式 (Thread Pool Pattern)
    /// </summary>
    public class ThreadPoolPattern
    {
        public async Task StartThreadPoolAsync()
        {
            Console.WriteLine("=== 线程池模式 ===");

            // 获取线程池信息
            ThreadPool.GetMinThreads(out int minWorkerThreads, out int minCompletionPortThreads);
            ThreadPool.GetMaxThreads(out int maxWorkerThreads, out int maxCompletionPortThreads);
            
            Console.WriteLine("线程池配置 - 最小工作线程: {MinWorker}, 最大工作线程: {MaxWorker}", 
                minWorkerThreads, maxWorkerThreads);

            // 使用线程池执行任务
            var tasks = new List<Task>();
            for (int i = 0; i < 20; i++)
            {
                var taskId = i;
                var task = Task.Run(() => ThreadPoolWorker(taskId));
                tasks.Add(task);
            }

            await Task.WhenAll(tasks);
        }

        private async Task ThreadPoolWorker(int taskId)
        {
            Console.WriteLine("线程池任务 {TaskId} 开始 - 线程ID: {ThreadId}", 
                taskId, Thread.CurrentThread.ManagedThreadId);
            
            await Task.Delay(Random.Shared.Next(1000, 3000));
            
            Console.WriteLine("线程池任务 {TaskId} 完成", taskId);
        }
    }

    /// <summary>
    /// 模式10: 反应器模式 (Reactor Pattern)
    /// </summary>
    public class ReactorPattern
    {
        private readonly ConcurrentQueue<Event> _eventQueue = new();
        private readonly CancellationTokenSource _cancellationTokenSource = new();

        public async Task StartReactorAsync()
        {
            Console.WriteLine("=== 反应器模式 ===");

            // 启动事件循环
            var reactorTask = Task.Run(() => EventLoop(_cancellationTokenSource.Token));

            // 生成事件
            var eventGeneratorTask = Task.Run(() => GenerateEvents(_cancellationTokenSource.Token));

            // 运行一段时间后停止
            await Task.Delay(10000);
            _cancellationTokenSource.Cancel();

            await Task.WhenAll(reactorTask, eventGeneratorTask);
        }

        private async Task EventLoop(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                if (_eventQueue.TryDequeue(out var eventItem))
                {
                    await HandleEvent(eventItem);
                }
                else
                {
                    await Task.Delay(10, cancellationToken); // 避免忙等待
                }
            }
        }

        private async Task GenerateEvents(CancellationToken cancellationToken)
        {
            var eventTypes = new[] { "user.login", "user.logout", "user.action", "system.alert" };
            var counter = 0;

            while (!cancellationToken.IsCancellationRequested)
            {
                var eventType = eventTypes[Random.Shared.Next(eventTypes.Length)];
                var eventItem = new Event
                {
                    Id = ++counter,
                    Type = eventType,
                    Data = $"事件数据-{counter}",
                    Timestamp = DateTime.Now
                };

                _eventQueue.Enqueue(eventItem);
                Console.WriteLine("生成事件: {Type} - {Id}", eventType, counter);

                await Task.Delay(Random.Shared.Next(100, 500), cancellationToken);
            }
        }

        private async Task HandleEvent(Event eventItem)
        {
            Console.WriteLine("处理事件: {Type} - {Id} - {Data}", 
                eventItem.Type, eventItem.Id, eventItem.Data);

            // 根据事件类型进行不同处理
            switch (eventItem.Type)
            {
                case "user.login":
                    await HandleUserLogin(eventItem);
                    break;
                case "user.logout":
                    await HandleUserLogout(eventItem);
                    break;
                case "user.action":
                    await HandleUserAction(eventItem);
                    break;
                case "system.alert":
                    await HandleSystemAlert(eventItem);
                    break;
            }
        }

        private async Task HandleUserLogin(Event eventItem)
        {
            await Task.Delay(200);
            Console.WriteLine("处理用户登录: {Data}", eventItem.Data);
        }

        private async Task HandleUserLogout(Event eventItem)
        {
            await Task.Delay(150);
            Console.WriteLine("处理用户登出: {Data}", eventItem.Data);
        }

        private async Task HandleUserAction(Event eventItem)
        {
            await Task.Delay(100);
            Console.WriteLine("处理用户操作: {Data}", eventItem.Data);
        }

        private async Task HandleSystemAlert(Event eventItem)
        {
            await Task.Delay(300);
            Console.WriteLine("处理系统警报: {Data}", eventItem.Data);
        }

        private class Event
        {
            public int Id { get; set; }
            public string Type { get; set; } = string.Empty;
            public string Data { get; set; } = string.Empty;
            public DateTime Timestamp { get; set; }
        }

        public void Dispose()
        {
            _cancellationTokenSource?.Dispose();
        }
    }
}
