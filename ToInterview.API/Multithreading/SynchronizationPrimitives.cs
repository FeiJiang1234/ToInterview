using System.Collections.Concurrent;

namespace ToInterview.API.Services;

/// <summary>
/// 同步原语详解和使用示例
/// </summary>
public class SynchronizationPrimitives
{
    private readonly ILogger<SynchronizationPrimitives> _logger;

    public SynchronizationPrimitives(ILogger<SynchronizationPrimitives> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// 1. lock 语句 - 最常用的同步原语
    /// </summary>
    public class LockExample
    {
        private readonly object _lockObject = new object();
        private int _counter = 0;
        private readonly List<string> _items = new();

        public void IncrementCounter()
        {
            lock (_lockObject)
            {
                _counter++;
                Console.WriteLine("计数器增加到: {Counter}", _counter);
            }
        }

        public void AddItem(string item)
        {
            lock (_lockObject)
            {
                _items.Add(item);
                Console.WriteLine("添加项目: {Item}, 总数: {Count}", item, _items.Count);
            }
        }

        public int GetCounter() => _counter;
        public List<string> GetItems() => new List<string>(_items);
    }

    /// <summary>
    /// 2. Monitor - lock语句的底层实现
    /// </summary>
    public class MonitorExample
    {
        private readonly object _monitorObject = new object();
        private int _value = 0;

        public void SetValue(int newValue)
        {
            Monitor.Enter(_monitorObject);
            try
            {
                _value = newValue;
                Console.WriteLine("设置值为: {Value}", _value);
            }
            finally
            {
                Monitor.Exit(_monitorObject);
            }
        }

        public int GetValue()
        {
            Monitor.Enter(_monitorObject);
            try
            {
                return _value;
            }
            finally
            {
                Monitor.Exit(_monitorObject);
            }
        }

        // 使用TryEnter避免死锁
        public bool TrySetValue(int newValue, int timeoutMs = 1000)
        {
            if (Monitor.TryEnter(_monitorObject, timeoutMs))
            {
                try
                {
                    _value = newValue;
                    Console.WriteLine("成功设置值为: {Value}", _value);
                    return true;
                }
                finally
                {
                    Monitor.Exit(_monitorObject);
                }
            }
            else
            {
                Console.WriteLine("获取锁超时");
                return false;
            }
        }
    }

    /// <summary>
    /// 3. Mutex - 跨进程同步
    /// </summary>
    public class MutexExample
    {
        private readonly Mutex _mutex = new Mutex(false, "GlobalMutexExample");

        public void CriticalSection()
        {
            _mutex.WaitOne();
            try
            {
                Console.WriteLine("进入临界区 - 线程: {ThreadId}", Thread.CurrentThread.ManagedThreadId);
                Thread.Sleep(2000); // 模拟工作
                Console.WriteLine("离开临界区 - 线程: {ThreadId}", Thread.CurrentThread.ManagedThreadId);
            }
            finally
            {
                _mutex.ReleaseMutex();
            }
        }

        public void Dispose()
        {
            _mutex?.Dispose();
        }
    }

    /// <summary>
    /// 4. SemaphoreSlim - 限制并发数量
    /// </summary>
    public class SemaphoreSlimExample
    {
        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(3, 3); // 最多3个并发

        public async Task ProcessItemAsync(int itemId)
        {
            await _semaphore.WaitAsync();
            try
            {
                Console.WriteLine("开始处理项目 {ItemId} - 线程: {ThreadId}", 
                    itemId, Thread.CurrentThread.ManagedThreadId);
                
                await Task.Delay(2000); // 模拟处理时间
                
                Console.WriteLine("完成处理项目 {ItemId}", itemId);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public async Task ProcessMultipleItemsAsync(int[] itemIds)
        {
            var tasks = itemIds.Select(id => ProcessItemAsync(id));
            await Task.WhenAll(tasks);
        }

        public void Dispose()
        {
            _semaphore?.Dispose();
        }
    }

    /// <summary>
    /// 5. AutoResetEvent - 自动重置事件
    /// </summary>
    public class AutoResetEventExample
    {
        private readonly AutoResetEvent _autoResetEvent = new AutoResetEvent(false);
        private readonly List<string> _results = new();

        public async Task ProducerAsync()
        {
            for (int i = 0; i < 5; i++)
            {
                await Task.Delay(1000);
                _results.Add($"数据 {i}");
                Console.WriteLine("生产者产生数据: {Data}", $"数据 {i}");
                _autoResetEvent.Set(); // 通知消费者
            }
        }

        public async Task ConsumerAsync()
        {
            for (int i = 0; i < 5; i++)
            {
                Console.WriteLine("消费者等待数据...");
                _autoResetEvent.WaitOne(); // 等待生产者信号
                
                if (_results.Count > 0)
                {
                    var data = _results[_results.Count - 1];
                    Console.WriteLine("消费者处理数据: {Data}", data);
                }
            }
        }

        public void Dispose()
        {
            _autoResetEvent?.Dispose();
        }
    }

    /// <summary>
    /// 6. ManualResetEventSlim - 手动重置事件
    /// </summary>
    public class ManualResetEventSlimExample
    {
        private readonly ManualResetEventSlim _manualResetEvent = new ManualResetEventSlim(false);
        private readonly List<string> _data = new();

        public async Task DataLoaderAsync()
        {
            Console.WriteLine("开始加载数据...");
            
            for (int i = 0; i < 10; i++)
            {
                await Task.Delay(500);
                _data.Add($"项目 {i}");
            }
            
            Console.WriteLine("数据加载完成，设置事件");
            _manualResetEvent.Set(); // 设置事件，通知所有等待的线程
        }

        public async Task DataProcessorAsync(int processorId)
        {
            Console.WriteLine("处理器 {ProcessorId} 等待数据...", processorId);
            _manualResetEvent.Wait(); // 等待数据加载完成
            
            Console.WriteLine("处理器 {ProcessorId} 开始处理 {Count} 个项目", 
                processorId, _data.Count);
            
            foreach (var item in _data)
            {
                await Task.Delay(100);
                Console.WriteLine("处理器 {ProcessorId} 处理: {Item}", processorId, item);
            }
        }

        public void Dispose()
        {
            _manualResetEvent?.Dispose();
        }
    }

    /// <summary>
    /// 7. CountdownEvent - 倒计时事件
    /// </summary>
    public class CountdownEventExample
    {
        private readonly CountdownEvent _countdownEvent = new CountdownEvent(3);
        private readonly List<string> _results = new();

        public async Task WorkerAsync(int workerId)
        {
            Console.WriteLine("工作者 {WorkerId} 开始工作", workerId);
            
            await Task.Delay(Random.Shared.Next(1000, 3000)); // 模拟工作时间
            
            var result = $"工作者 {workerId} 的结果";
            _results.Add(result);
            
            Console.WriteLine("工作者 {WorkerId} 完成工作", workerId);
            _countdownEvent.Signal(); // 减少计数
        }

        public async Task CoordinatorAsync()
        {
            Console.WriteLine("协调者等待所有工作者完成...");
            _countdownEvent.Wait(); // 等待所有工作者完成
            
            Console.WriteLine("所有工作者完成，结果数量: {Count}", _results.Count);
            foreach (var result in _results)
            {
                Console.WriteLine("结果: {Result}", result);
            }
        }

        public void Dispose()
        {
            _countdownEvent?.Dispose();
        }
    }

    /// <summary>
    /// 8. ReaderWriterLockSlim - 读写锁
    /// </summary>
    public class ReaderWriterLockSlimExample
    {
        private readonly ReaderWriterLockSlim _rwLock = new ReaderWriterLockSlim();
        private readonly Dictionary<string, string> _cache = new();

        public string ReadFromCache(string key)
        {
            _rwLock.EnterReadLock();
            try
            {
                Console.WriteLine("读取缓存: {Key}", key);
                return _cache.TryGetValue(key, out var value) ? value : null;
            }
            finally
            {
                _rwLock.ExitReadLock();
            }
        }

        public void WriteToCache(string key, string value)
        {
            _rwLock.EnterWriteLock();
            try
            {
                Console.WriteLine("写入缓存: {Key} = {Value}", key, value);
                _cache[key] = value;
            }
            finally
            {
                _rwLock.ExitWriteLock();
            }
        }

        public bool TryUpgradeToWriteLock(string key, string value)
        {
            _rwLock.EnterUpgradeableReadLock();
            try
            {
                if (!_cache.ContainsKey(key))
                {
                    _rwLock.EnterWriteLock();
                    try
                    {
                        _cache[key] = value;
                        Console.WriteLine("升级锁并写入: {Key} = {Value}", key, value);
                        return true;
                    }
                    finally
                    {
                        _rwLock.ExitWriteLock();
                    }
                }
                return false;
            }
            finally
            {
                _rwLock.ExitUpgradeableReadLock();
            }
        }

        public void Dispose()
        {
            _rwLock?.Dispose();
        }
    }

    /// <summary>
    /// 9. Barrier - 屏障同步
    /// </summary>
    public class BarrierExample
    {
        private readonly Barrier _barrier = new Barrier(3, barrier => 
        {
            Console.WriteLine("所有参与者到达屏障点 {Phase}", barrier.CurrentPhaseNumber);
        });

        public async Task ParticipantAsync(int participantId)
        {
            for (int phase = 0; phase < 3; phase++)
            {
                Console.WriteLine("参与者 {ParticipantId} 执行阶段 {Phase}", participantId, phase);
                
                await Task.Delay(Random.Shared.Next(1000, 3000)); // 模拟工作
                
                Console.WriteLine("参与者 {ParticipantId} 完成阶段 {Phase}，等待其他参与者", 
                    participantId, phase);
                
                _barrier.SignalAndWait(); // 等待所有参与者到达
            }
        }

        public void Dispose()
        {
            _barrier?.Dispose();
        }
    }

    /// <summary>
    /// 10. 线程安全集合
    /// </summary>
    public class ThreadSafeCollectionsExample
    {
        private readonly ConcurrentQueue<string> _queue = new();
        private readonly ConcurrentStack<string> _stack = new();
        private readonly ConcurrentBag<string> _bag = new();
        private readonly ConcurrentDictionary<string, int> _dictionary = new();

        public void DemonstrateCollections()
        {
            // ConcurrentQueue - 线程安全的队列
            Parallel.For(0, 10, i =>
            {
                _queue.Enqueue($"队列项目 {i}");
            });

            while (_queue.TryDequeue(out var item))
            {
                Console.WriteLine("从队列取出: {Item}", item);
            }

            // ConcurrentStack - 线程安全的栈
            Parallel.For(0, 10, i =>
            {
                _stack.Push($"栈项目 {i}");
            });

            while (_stack.TryPop(out var item))
            {
                Console.WriteLine("从栈取出: {Item}", item);
            }

            // ConcurrentBag - 线程安全的无序集合
            Parallel.For(0, 10, i =>
            {
                _bag.Add($"包项目 {i}");
            });

            foreach (var item in _bag)
            {
                Console.WriteLine("包中的项目: {Item}", item);
            }

            // ConcurrentDictionary - 线程安全的字典
            Parallel.For(0, 10, i =>
            {
                _dictionary.TryAdd($"键{i}", i);
            });

            foreach (var kvp in _dictionary)
            {
                Console.WriteLine("字典项目: {Key} = {Value}", kvp.Key, kvp.Value);
            }
        }
    }

    /// <summary>
    /// 同步原语选择指南
    /// </summary>
    public static class SynchronizationPrimitiveGuide
    {
        /*
         * 选择同步原语的指南:
         * 
         * 1. lock/Monitor:
         *    - 最常用，适合大多数场景
         *    - 简单易用，性能良好
         *    - 适合保护共享资源
         * 
         * 2. Mutex:
         *    - 跨进程同步
         *    - 开销较大，谨慎使用
         *    - 适合系统级资源保护
         * 
         * 3. SemaphoreSlim:
         *    - 限制并发数量
         *    - 支持异步操作
         *    - 适合资源池管理
         * 
         * 4. AutoResetEvent:
         *    - 一对一的信号通知
         *    - 自动重置
         *    - 适合生产者-消费者模式
         * 
         * 5. ManualResetEventSlim:
         *    - 一对多的信号通知
         *    - 手动重置
         *    - 适合广播通知
         * 
         * 6. CountdownEvent:
         *    - 等待多个操作完成
         *    - 倒计时机制
         *    - 适合并行任务协调
         * 
         * 7. ReaderWriterLockSlim:
         *    - 读多写少的场景
         *    - 提高读操作的并发性
         *    - 适合缓存实现
         * 
         * 8. Barrier:
         *    - 同步多个线程的执行阶段
         *    - 适合分阶段并行算法
         *    - 适合迭代计算
         * 
         * 9. 线程安全集合:
         *    - 无需额外同步
         *    - 性能优化
         *    - 适合高并发场景
         */
    }
}
