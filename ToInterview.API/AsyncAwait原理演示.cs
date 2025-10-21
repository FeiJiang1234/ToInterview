using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace ToInterview.API.Multithreading;

/// <summary>
/// async/await 原理演示和深入解析
/// </summary>
public class AsyncAwaitPrincipleDemo
{
    private readonly ILogger<AsyncAwaitPrincipleDemo> _logger;

    public AsyncAwaitPrincipleDemo(ILogger<AsyncAwaitPrincipleDemo> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// 演示1: 基本async/await工作原理
    /// </summary>
    public class BasicAsyncAwaitDemo
    {
        /*
         * async/await 工作原理：
         * 1. 方法被标记为async时，编译器会生成状态机
         * 2. 遇到await时，方法会暂停并返回Task
         * 3. 异步操作完成后，状态机会恢复执行
         * 4. 整个过程不会阻塞调用线程
         */

        // 同步方法 - 会阻塞线程
        public string SyncOperation()
        {
            Console.WriteLine($"[{Thread.CurrentThread.ManagedThreadId}] 同步操作开始");
            Thread.Sleep(2000); // 阻塞2秒
            Console.WriteLine($"[{Thread.CurrentThread.ManagedThreadId}] 同步操作完成");
            return "同步结果";
        }

        // 异步方法 - 不会阻塞线程
        public async Task<string> AsyncOperation()
        {
            Console.WriteLine($"[{Thread.CurrentThread.ManagedThreadId}] 异步操作开始");
            await Task.Delay(2000); // 异步等待2秒，不阻塞线程
            Console.WriteLine($"[{Thread.CurrentThread.ManagedThreadId}] 异步操作完成");
            return "异步结果";
        }

        // 演示同步vs异步的执行差异
        public async Task DemonstrateSyncVsAsync()
        {
            Console.WriteLine("=== 同步 vs 异步执行对比 ===");
            Console.WriteLine($"主线程ID: {Thread.CurrentThread.ManagedThreadId}");

            // 同步执行
            Console.WriteLine("\n--- 同步执行 ---");
            var syncStart = DateTime.Now;
            var syncResult1 = SyncOperation();
            var syncResult2 = SyncOperation();
            var syncTime = DateTime.Now - syncStart;
            Console.WriteLine($"同步执行总耗时: {syncTime.TotalMilliseconds}ms");

            // 异步执行
            Console.WriteLine("\n--- 异步执行 ---");
            var asyncStart = DateTime.Now;
            var asyncTask1 = AsyncOperation();
            var asyncTask2 = AsyncOperation();
            var asyncResult1 = await asyncTask1;
            var asyncResult2 = await asyncTask2;
            var asyncTime = DateTime.Now - asyncStart;
            Console.WriteLine($"异步执行总耗时: {asyncTime.TotalMilliseconds}ms");

            Console.WriteLine($"性能提升: {syncTime.TotalMilliseconds / asyncTime.TotalMilliseconds:F2}x");
        }
    }

    /// <summary>
    /// 演示2: 状态机工作原理
    /// </summary>
    public class StateMachineDemo
    {
        /*
         * 状态机工作原理：
         * 1. 编译器为每个async方法生成状态机类
         * 2. 状态机实现IAsyncStateMachine接口
         * 3. MoveNext方法推进状态
         * 4. 每个await点都是一个状态
         */

        public async Task<string> ComplexAsyncMethod()
        {
            Console.WriteLine($"[{Thread.CurrentThread.ManagedThreadId}] 状态1: 开始");
            
            var result1 = await FirstStepAsync();
            Console.WriteLine($"[{Thread.CurrentThread.ManagedThreadId}] 状态2: 第一步完成 - {result1}");
            
            var result2 = await SecondStepAsync(result1);
            Console.WriteLine($"[{Thread.CurrentThread.ManagedThreadId}] 状态3: 第二步完成 - {result2}");
            
            var result3 = await ThirdStepAsync(result2);
            Console.WriteLine($"[{Thread.CurrentThread.ManagedThreadId}] 状态4: 第三步完成 - {result3}");
            
            return result3;
        }

        private async Task<string> FirstStepAsync()
        {
            Console.WriteLine($"[{Thread.CurrentThread.ManagedThreadId}] 执行第一步");
            await Task.Delay(1000);
            return "第一步结果";
        }

        private async Task<string> SecondStepAsync(string input)
        {
            Console.WriteLine($"[{Thread.CurrentThread.ManagedThreadId}] 执行第二步，输入: {input}");
            await Task.Delay(1500);
            return $"处理后的{input}";
        }

        private async Task<string> ThirdStepAsync(string input)
        {
            Console.WriteLine($"[{Thread.CurrentThread.ManagedThreadId}] 执行第三步，输入: {input}");
            await Task.Delay(800);
            return $"最终{input}";
        }

        // 演示状态机的执行过程
        public async Task DemonstrateStateMachine()
        {
            Console.WriteLine("=== 状态机执行演示 ===");
            Console.WriteLine($"主线程ID: {Thread.CurrentThread.ManagedThreadId}");
            
            var result = await ComplexAsyncMethod();
            Console.WriteLine($"最终结果: {result}");
        }
    }

    /// <summary>
    /// 演示3: ConfigureAwait的作用
    /// </summary>
    public class ConfigureAwaitDemo
    {
        /*
         * ConfigureAwait的作用：
         * 1. ConfigureAwait(false) - 不捕获同步上下文，可能在任何线程继续
         * 2. ConfigureAwait(true) - 默认值，在原始同步上下文中继续
         * 3. 在库代码中使用ConfigureAwait(false)避免死锁
         */

        // 模拟库方法 - 使用ConfigureAwait(false)
        public async Task<string> LibraryMethodAsync()
        {
            Console.WriteLine($"[{Thread.CurrentThread.ManagedThreadId}] 库方法开始");
            
            // 使用ConfigureAwait(false)避免死锁和提高性能
            await Task.Delay(1000).ConfigureAwait(false);
            
            Console.WriteLine($"[{Thread.CurrentThread.ManagedThreadId}] 库方法完成");
            return "库方法结果";
        }

        // 模拟UI方法 - 通常不需要ConfigureAwait(false)
        public async Task<string> UIMethodAsync()
        {
            Console.WriteLine($"[{Thread.CurrentThread.ManagedThreadId}] UI方法开始");
            
            var result = await LibraryMethodAsync(); // 这里会自动回到UI线程
            Console.WriteLine($"[{Thread.CurrentThread.ManagedThreadId}] UI方法完成");
            
            return result;
        }

        // 演示死锁问题
        public void DemonstrateDeadlock()
        {
            Console.WriteLine("=== 死锁演示 ===");
            Console.WriteLine($"主线程ID: {Thread.CurrentThread.ManagedThreadId}");
            
            try
            {
                // 这可能导致死锁
                var result = SomeAsyncMethod().Result; // 危险！
                Console.WriteLine($"结果: {result}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"捕获到异常: {ex.Message}");
            }
        }

        private async Task<string> SomeAsyncMethod()
        {
            Console.WriteLine($"[{Thread.CurrentThread.ManagedThreadId}] 异步方法开始");
            await Task.Delay(1000);
            Console.WriteLine($"[{Thread.CurrentThread.ManagedThreadId}] 异步方法完成");
            return "异步方法结果";
        }

        // 演示ConfigureAwait的效果
        public async Task DemonstrateConfigureAwait()
        {
            Console.WriteLine("=== ConfigureAwait演示 ===");
            Console.WriteLine($"主线程ID: {Thread.CurrentThread.ManagedThreadId}");
            
            await UIMethodAsync();
        }
    }

    /// <summary>
    /// 演示4: 异常处理机制
    /// </summary>
    public class ExceptionHandlingDemo
    {
        /*
         * 异步异常处理：
         * 1. 异常被包装在Task中
         * 2. 只有在await时才会抛出异常
         * 3. 多个异步操作可能产生AggregateException
         */

        public async Task<string> MethodWithExceptionAsync()
        {
            Console.WriteLine($"[{Thread.CurrentThread.ManagedThreadId}] 开始可能抛出异常的操作");
            await Task.Delay(1000);
            throw new InvalidOperationException("异步方法中的异常");
        }

        public async Task HandleSingleExceptionAsync()
        {
            Console.WriteLine("=== 单个异常处理演示 ===");
            
            try
            {
                var result = await MethodWithExceptionAsync();
                Console.WriteLine($"结果: {result}");
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine($"捕获到异常: {ex.Message}");
            }
        }

        // 处理多个异步操作的异常
        public async Task HandleMultipleExceptionsAsync()
        {
            Console.WriteLine("=== 多个异常处理演示 ===");
            
            var tasks = new[]
            {
                Task.Run(() => throw new ArgumentException("任务1异常")),
                Task.Run(() => throw new InvalidOperationException("任务2异常")),
                Task.Run(() => "任务3成功")
            };

            try
            {
                await Task.WhenAll(tasks);
            }
            catch (AggregateException ex)
            {
                Console.WriteLine($"捕获到聚合异常，包含 {ex.InnerExceptions.Count} 个异常");
                foreach (var innerEx in ex.InnerExceptions)
                {
                    Console.WriteLine($"内部异常: {innerEx.GetType().Name} - {innerEx.Message}");
                }
            }
        }
    }

    /// <summary>
    /// 演示5: 取消操作
    /// </summary>
    public class CancellationDemo
    {
        /*
         * CancellationToken的使用：
         * 1. 支持协作式取消
         * 2. 避免强制终止线程
         * 3. 可以链接多个CancellationToken
         */

        public async Task<string> CancellableOperationAsync(CancellationToken cancellationToken = default)
        {
            Console.WriteLine($"[{Thread.CurrentThread.ManagedThreadId}] 开始可取消操作");
            
            for (int i = 0; i < 100; i++)
            {
                cancellationToken.ThrowIfCancellationRequested();
                
                Console.WriteLine($"执行步骤 {i}");
                await Task.Delay(100, cancellationToken);
            }

            Console.WriteLine("操作完成");
            return "操作完成";
        }

        public async Task DemonstrateCancellationAsync()
        {
            Console.WriteLine("=== 取消操作演示 ===");
            
            using var cts = new CancellationTokenSource();
            
            // 3秒后自动取消
            cts.CancelAfter(TimeSpan.FromSeconds(3));

            try
            {
                await CancellableOperationAsync(cts.Token);
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("操作被取消");
            }
        }

        // 链接取消令牌
        public async Task LinkedCancellationAsync(CancellationToken externalToken)
        {
            Console.WriteLine("=== 链接取消令牌演示 ===");
            
            using var cts = CancellationTokenSource.CreateLinkedTokenSource(externalToken);
            cts.CancelAfter(TimeSpan.FromSeconds(2));

            try
            {
                await CancellableOperationAsync(cts.Token);
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("链接操作被取消");
            }
        }
    }

    /// <summary>
    /// 演示6: 异步流
    /// </summary>
    public class AsyncStreamDemo
    {
        /*
         * 异步流的特点：
         * 1. 使用IAsyncEnumerable<T>
         * 2. 使用yield return产生异步数据
         * 3. 使用await foreach消费数据
         */

        public async IAsyncEnumerable<string> GenerateDataAsync()
        {
            for (int i = 0; i < 5; i++)
            {
                Console.WriteLine($"[{Thread.CurrentThread.ManagedThreadId}] 生成数据项 {i}");
                await Task.Delay(500); // 模拟异步操作
                yield return $"数据项 {i}";
            }
        }

        public async Task ProcessAsyncStreamAsync()
        {
            Console.WriteLine("=== 异步流演示 ===");
            Console.WriteLine($"主线程ID: {Thread.CurrentThread.ManagedThreadId}");
            
            await foreach (var item in GenerateDataAsync())
            {
                Console.WriteLine($"[{Thread.CurrentThread.ManagedThreadId}] 处理项目: {item}");
            }
            
            Console.WriteLine("异步流处理完成");
        }
    }

    /// <summary>
    /// 演示7: ValueTask优化
    /// </summary>
    public class ValueTaskDemo
    {
        private readonly Dictionary<string, string> _cache = new();

        /*
         * ValueTask的优势：
         * 1. 对于可能同步完成的操作更高效
         * 2. 减少堆分配
         * 3. 提高性能
         */

        // 使用Task（可能不必要的堆分配）
        public async Task<string> GetDataWithTaskAsync(string key)
        {
            if (_cache.TryGetValue(key, out var cachedValue))
            {
                Console.WriteLine("从缓存返回数据");
                return cachedValue; // 同步返回，但仍会分配Task
            }

            Console.WriteLine("异步获取数据");
            await Task.Delay(1000); // 异步操作
            var value = $"异步获取的数据: {key}";
            _cache[key] = value;
            return value;
        }

        // 使用ValueTask（更高效）
        public async ValueTask<string> GetDataWithValueTaskAsync(string key)
        {
            if (_cache.TryGetValue(key, out var cachedValue))
            {
                Console.WriteLine("从缓存返回数据（ValueTask）");
                return cachedValue; // 同步返回，无堆分配
            }

            Console.WriteLine("异步获取数据（ValueTask）");
            await Task.Delay(1000); // 异步操作
            var value = $"异步获取的数据: {key}";
            _cache[key] = value;
            return value;
        }

        // 性能对比
        public async Task PerformanceComparisonAsync()
        {
            Console.WriteLine("=== ValueTask性能对比 ===");
            
            const int iterations = 1000;
            
            // Task性能测试
            var stopwatch = Stopwatch.StartNew();
            for (int i = 0; i < iterations; i++)
            {
                await GetDataWithTaskAsync("test");
            }
            var taskTime = stopwatch.ElapsedMilliseconds;

            // ValueTask性能测试
            stopwatch.Restart();
            for (int i = 0; i < iterations; i++)
            {
                await GetDataWithValueTaskAsync("test");
            }
            var valueTaskTime = stopwatch.ElapsedMilliseconds;

            Console.WriteLine($"Task: {taskTime}ms, ValueTask: {valueTaskTime}ms");
            Console.WriteLine($"性能提升: {(double)taskTime / valueTaskTime:F2}x");
        }
    }

    /// <summary>
    /// 演示8: 最佳实践
    /// </summary>
    public class BestPracticesDemo
    {
        /*
         * 异步编程最佳实践：
         * 1. 避免async void（除了事件处理程序）
         * 2. 在库代码中使用ConfigureAwait(false)
         * 3. 正确处理异常
         * 4. 使用CancellationToken
         * 5. 避免阻塞异步代码
         */

        // ❌ 错误做法
        public void BadAsyncUsage()
        {
            // 阻塞异步代码
            var result = SomeAsyncMethod().Result; // 可能导致死锁
            
            // 异步void（除了事件处理程序）
            SomeAsyncVoidMethod(); // 异常难以捕获
        }

        // ✅ 正确做法
        public async Task GoodAsyncUsageAsync()
        {
            var result = await SomeAsyncMethod();
            Console.WriteLine($"结果: {result}");
        }

        private async Task<string> SomeAsyncMethod()
        {
            await Task.Delay(1000);
            return "结果";
        }

        private async void SomeAsyncVoidMethod()
        {
            await Task.Delay(1000);
        }

        // 批量异步操作
        public async Task ProcessBatchAsync<T>(IEnumerable<T> items, Func<T, Task> processor, int batchSize = 10)
        {
            var batches = items.Chunk(batchSize);
            
            foreach (var batch in batches)
            {
                var tasks = batch.Select(processor);
                await Task.WhenAll(tasks);
            }
        }

        // 超时处理
        public async Task<string> OperationWithTimeoutAsync()
        {
            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(3));
            
            try
            {
                return await LongRunningOperationAsync(cts.Token);
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("操作超时");
                return "超时";
            }
        }

        private async Task<string> LongRunningOperationAsync(CancellationToken cancellationToken)
        {
            await Task.Delay(10000, cancellationToken); // 10秒
            return "完成";
        }
    }
}
