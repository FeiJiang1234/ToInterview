using System.Diagnostics;
using System.Collections.Concurrent;

namespace ToInterview.API.Services;

/// <summary>
/// async/await 概念深入解析
/// </summary>
public class AsyncAwaitConcepts
{
    private readonly ILogger<AsyncAwaitConcepts> _logger;

    public AsyncAwaitConcepts(ILogger<AsyncAwaitConcepts> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// 概念1: async/await 的本质
    /// </summary>
    public class AsyncAwaitEssence
    {
        /*
         * async/await 的本质:
         * 1. async 关键字将方法标记为异步方法
         * 2. await 关键字暂停方法执行，等待异步操作完成
         * 3. 编译器将异步方法转换为状态机
         * 4. 异步方法返回 Task 或 Task<T>
         * 5. 不会阻塞调用线程
         */

        // 同步方法
        public string SyncMethod()
        {
            Thread.Sleep(1000); // 阻塞线程
            return "同步结果";
        }

        // 异步方法
        public async Task<string> AsyncMethod()
        {
            await Task.Delay(1000); // 不阻塞线程
            return "异步结果";
        }

        // 异步方法返回Task<T>
        public async Task<int> AsyncMethodWithReturn()
        {
            await Task.Delay(1000);
            return 42;
        }

        // 异步void方法（仅用于事件处理程序）
        public async void AsyncVoidMethod()
        {
            await Task.Delay(1000);
            // 注意：异步void方法中的异常难以捕获
        }
    }

    /// <summary>
    /// 概念2: 状态机的工作原理
    /// </summary>
    public class StateMachineExample
    {
        /*
         * 编译器将异步方法转换为状态机:
         * 1. 创建状态机类
         * 2. 实现IAsyncStateMachine接口
         * 3. 使用MoveNext方法推进状态
         * 4. 使用await时保存状态并返回
         * 5. 异步操作完成后恢复执行
         */

        public async Task<string> ComplexAsyncMethod()
        {
            Console.WriteLine("步骤1: 开始");
            
            var result1 = await FirstAsyncOperation();
            Console.WriteLine("步骤2: 第一个操作完成 - {Result}", result1);
            
            var result2 = await SecondAsyncOperation(result1);
            Console.WriteLine("步骤3: 第二个操作完成 - {Result}", result2);
            
            var result3 = await ThirdAsyncOperation(result2);
            Console.WriteLine("步骤4: 第三个操作完成 - {Result}", result3);
            
            return result3;
        }

        private async Task<string> FirstAsyncOperation()
        {
            await Task.Delay(1000);
            return "第一个结果";
        }

        private async Task<string> SecondAsyncOperation(string input)
        {
            await Task.Delay(1500);
            return $"处理后的 {input}";
        }

        private async Task<string> ThirdAsyncOperation(string input)
        {
            await Task.Delay(800);
            return $"最终 {input}";
        }
    }

    /// <summary>
    /// 概念3: ConfigureAwait 的重要性
    /// </summary>
    public class ConfigureAwaitExample
    {
        /*
         * ConfigureAwait(false) 的作用:
         * 1. 避免死锁
         * 2. 提高性能
         * 3. 在库代码中应该使用
         * 4. 在UI代码中通常不需要
         */

        // 库代码 - 应该使用ConfigureAwait(false)
        public async Task<string> LibraryMethodAsync()
        {
            using var httpClient = new HttpClient();
            
            // 使用ConfigureAwait(false)避免死锁和提高性能
            var response = await httpClient.GetStringAsync("https://api.example.com")
                .ConfigureAwait(false);
            
            return response;
        }

        // UI代码 - 通常不需要ConfigureAwait(false)
        public async Task<string> UIMethodAsync()
        {
            var result = await LibraryMethodAsync(); // 这里会自动回到UI线程
            return result;
        }

        // 演示死锁问题
        public void DemonstrateDeadlock()
        {
            // 这可能导致死锁
            var result = SomeAsyncMethod().Result; // 危险！
        }

        private async Task<string> SomeAsyncMethod()
        {
            await Task.Delay(1000);
            return "结果";
        }
    }

    /// <summary>
    /// 概念4: 异常处理
    /// </summary>
    public class ExceptionHandlingExample
    {
        /*
         * 异步方法中的异常处理:
         * 1. 异常会被包装在Task中
         * 2. 只有在await时才会抛出异常
         * 3. 使用try-catch包围await
         * 4. AggregateException可能包含多个异常
         */

        public async Task<string> MethodWithExceptionAsync()
        {
            await Task.Delay(1000);
            throw new InvalidOperationException("异步方法中的异常");
        }

        public async Task HandleExceptionAsync()
        {
            try
            {
                var result = await MethodWithExceptionAsync();
                Console.WriteLine("结果: {Result}", result);
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine("捕获到异常: {Message}", ex.Message);
            }
        }

        // 处理多个异步操作的异常
        public async Task HandleMultipleExceptionsAsync()
        {
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
                Console.WriteLine("捕获到聚合异常，包含 {Count} 个异常", ex.InnerExceptions.Count);
                foreach (var innerEx in ex.InnerExceptions)
                {
                    Console.WriteLine("内部异常: {Type} - {Message}", 
                        innerEx.GetType().Name, innerEx.Message);
                }
            }
        }
    }

    /// <summary>
    /// 概念5: 取消操作
    /// </summary>
    public class CancellationExample
    {
        /*
         * CancellationToken 的使用:
         * 1. 支持协作式取消
         * 2. 避免强制终止线程
         * 3. 在长时间运行的操作中使用
         * 4. 可以链接多个CancellationToken
         */

        public async Task<string> CancellableOperationAsync(CancellationToken cancellationToken = default)
        {
            for (int i = 0; i < 100; i++)
            {
                cancellationToken.ThrowIfCancellationRequested();
                
                Console.WriteLine("执行步骤 {Step}", i);
                await Task.Delay(100, cancellationToken);
            }

            return "操作完成";
        }

        public async Task DemonstrateCancellationAsync()
        {
            using var cts = new CancellationTokenSource();
            
            // 5秒后自动取消
            cts.CancelAfter(TimeSpan.FromSeconds(5));

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
            using var cts = CancellationTokenSource.CreateLinkedTokenSource(externalToken);
            cts.CancelAfter(TimeSpan.FromSeconds(10));

            await CancellableOperationAsync(cts.Token);
        }
    }

    /// <summary>
    /// 概念6: 异步流 (Async Streams)
    /// </summary>
    public class AsyncStreamsExample
    {
        /*
         * 异步流的特点:
         * 1. 使用IAsyncEnumerable<T>
         * 2. 使用yield return产生异步数据
         * 3. 使用await foreach消费数据
         * 4. 支持异步迭代
         */

        public async IAsyncEnumerable<string> GenerateDataAsync()
        {
            for (int i = 0; i < 10; i++)
            {
                await Task.Delay(500); // 模拟异步操作
                yield return $"数据项 {i}";
            }
        }

        public async Task ProcessAsyncStreamAsync()
        {
            Console.WriteLine("开始处理异步流");
            
            await foreach (var item in GenerateDataAsync())
            {
                Console.WriteLine("处理项目: {Item}", item);
            }
            
            Console.WriteLine("异步流处理完成");
        }

        // 带取消令牌的异步流
        public async IAsyncEnumerable<string> GenerateDataWithCancellationAsync(
            [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            for (int i = 0; i < 100; i++)
            {
                cancellationToken.ThrowIfCancellationRequested();
                
                await Task.Delay(100, cancellationToken);
                yield return $"数据项 {i}";
            }
        }
    }

    /// <summary>
    /// 概念7: ValueTask 优化
    /// </summary>
    public class ValueTaskExample
    {
        /*
         * ValueTask 的优势:
         * 1. 对于可能同步完成的操作更高效
         * 2. 减少堆分配
         * 3. 提高性能
         * 4. 适用于缓存场景
         */

        private readonly Dictionary<string, string> _cache = new();

        // 使用Task（可能不必要的堆分配）
        public async Task<string> GetDataWithTaskAsync(string key)
        {
            if (_cache.TryGetValue(key, out var cachedValue))
            {
                return cachedValue; // 同步返回，但仍会分配Task
            }

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
                return cachedValue; // 同步返回，无堆分配
            }

            await Task.Delay(1000); // 异步操作
            var value = $"异步获取的数据: {key}";
            _cache[key] = value;
            return value;
        }
    }

    /// <summary>
    /// 概念8: 异步模式最佳实践
    /// </summary>
    public class AsyncBestPractices
    {
        /*
         * 异步编程最佳实践:
         * 1. 避免async void（除了事件处理程序）
         * 2. 在库代码中使用ConfigureAwait(false)
         * 3. 正确处理异常
         * 4. 使用CancellationToken
         * 5. 避免阻塞异步代码
         * 6. 使用ValueTask优化性能
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
            try
            {
                var result = await SomeAsyncMethod();
                Console.WriteLine("结果: {Result}", result);
            }
            catch (Exception ex)
            {
                Console.WriteLine("操作失败: {Message}", ex.Message);
            }
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
        public async Task ProcessBatchAsync<T>(
            IEnumerable<T> items,
            Func<T, Task> processor,
            int batchSize = 10)
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
            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
            
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

    /// <summary>
    /// 概念9: Parallel.For 并行处理
    /// </summary>
    public class ParallelForExample
    {
        /*
         * Parallel.For 的特点:
         * 1. 用于CPU密集型任务的并行处理
         * 2. 自动管理线程池
         * 3. 适用于数据并行处理
         * 4. 与async/await不同，是同步并行
         * 5. 需要注意线程安全问题
         */

        // 基本用法
        public void BasicParallelFor()
        {
            Console.WriteLine("开始并行处理...");
            
            Parallel.For(0, 10, i =>
            {
                Console.WriteLine($"线程 {Thread.CurrentThread.ManagedThreadId} 处理项目 {i}");
                Thread.Sleep(100); // 模拟CPU密集型工作
            });
            
            Console.WriteLine("并行处理完成");
        }

        // 带选项的Parallel.For
        public void ParallelForWithOptions()
        {
            var options = new ParallelOptions
            {
                MaxDegreeOfParallelism = Environment.ProcessorCount, // 限制并发数
                CancellationToken = CancellationToken.None
            };

            Parallel.For(0, 100, options, i =>
            {
                // 模拟计算密集型任务
                var result = CalculateFibonacci(i % 20);
                Console.WriteLine($"项目 {i}: 结果 = {result}");
            });
        }

        // 并行处理集合
        public void ParallelForEachExample()
        {
            var numbers = Enumerable.Range(1, 100).ToList();
            var results = new List<int>();

            // 注意：需要线程安全的集合操作
            var lockObject = new object();

            Parallel.ForEach(numbers, number =>
            {
                var result = number * number; // 计算平方
                
                lock (lockObject)
                {
                    results.Add(result);
                }
            });

            Console.WriteLine($"处理了 {results.Count} 个项目");
        }

        // 并行处理与异常处理
        public void ParallelForWithExceptionHandling()
        {
            try
            {
                Parallel.For(0, 10, i =>
                {
                    if (i == 5)
                    {
                        throw new InvalidOperationException($"处理项目 {i} 时发生错误");
                    }
                    
                    Console.WriteLine($"成功处理项目 {i}");
                });
            }
            catch (AggregateException ex)
            {
                Console.WriteLine($"捕获到聚合异常，包含 {ex.InnerExceptions.Count} 个异常");
                foreach (var innerEx in ex.InnerExceptions)
                {
                    Console.WriteLine($"内部异常: {innerEx.Message}");
                }
            }
        }

        // 性能比较：串行 vs 并行
        public void PerformanceComparison()
        {
            const int iterations = 1000000;
            var stopwatch = Stopwatch.StartNew();

            // 串行处理
            stopwatch.Restart();
            var serialResults = new List<double>();
            for (int i = 0; i < iterations; i++)
            {
                serialResults.Add(Math.Sqrt(i));
            }
            var serialTime = stopwatch.ElapsedMilliseconds;

            // 并行处理
            stopwatch.Restart();
            var parallelResults = new List<double>();
            var lockObject = new object();
            
            Parallel.For(0, iterations, i =>
            {
                var result = Math.Sqrt(i);
                lock (lockObject)
                {
                    parallelResults.Add(result);
                }
            });
            var parallelTime = stopwatch.ElapsedMilliseconds;

            Console.WriteLine($"串行处理: {serialTime}ms, 并行处理: {parallelTime}ms");
            Console.WriteLine($"性能提升: {((double)serialTime / parallelTime):F2}x");
        }

        // 使用线程安全的集合
        public void ThreadSafeCollectionExample()
        {
            var numbers = Enumerable.Range(1, 1000);
            var results = new ConcurrentBag<int>(); // 线程安全的集合

            Parallel.ForEach(numbers, number =>
            {
                var result = number * number;
                results.Add(result); // 无需锁，ConcurrentBag是线程安全的
            });

            Console.WriteLine($"使用线程安全集合处理了 {results.Count} 个项目");
        }

        // 取消并行操作
        public void CancellableParallelFor()
        {
            using var cts = new CancellationTokenSource();
            cts.CancelAfter(TimeSpan.FromSeconds(2)); // 2秒后取消

            try
            {
                var options = new ParallelOptions
                {
                    CancellationToken = cts.Token
                };

                Parallel.For(0, 1000, options, i =>
                {
                    Thread.Sleep(10); // 模拟工作
                    Console.WriteLine($"处理项目 {i}");
                });
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("并行操作被取消");
            }
        }

        private int CalculateFibonacci(int n)
        {
            if (n <= 1) return n;
            return CalculateFibonacci(n - 1) + CalculateFibonacci(n - 2);
        }
    }

    /// <summary>
    /// 概念10: 异步性能分析
    /// </summary>
    public class AsyncPerformanceAnalysis
    {
        public async Task AnalyzeAsyncPerformanceAsync()
        {
            Console.WriteLine("=== 异步性能分析 ===");

            // 分析串行 vs 并行执行
            await AnalyzeSerialVsParallelAsync();
            
            // 分析Task vs ValueTask性能
            await AnalyzeTaskVsValueTaskAsync();
        }

        private async Task AnalyzeSerialVsParallelAsync()
        {
            var stopwatch = Stopwatch.StartNew();

            // 串行执行
            stopwatch.Restart();
            await Task1();
            await Task2();
            await Task3();
            var serialTime = stopwatch.ElapsedMilliseconds;

            // 并行执行
            stopwatch.Restart();
            await Task.WhenAll(Task1(), Task2(), Task3());
            var parallelTime = stopwatch.ElapsedMilliseconds;

            Console.WriteLine("串行执行: {SerialTime}ms, 并行执行: {ParallelTime}ms", 
                serialTime, parallelTime);
        }

        private async Task AnalyzeTaskVsValueTaskAsync()
        {
            const int iterations = 10000;
            
            // Task性能测试
            var stopwatch = Stopwatch.StartNew();
            for (int i = 0; i < iterations; i++)
            {
                await GetDataWithTaskAsync("key");
            }
            var taskTime = stopwatch.ElapsedMilliseconds;

            // ValueTask性能测试
            stopwatch.Restart();
            for (int i = 0; i < iterations; i++)
            {
                await GetDataWithValueTaskAsync("key");
            }
            var valueTaskTime = stopwatch.ElapsedMilliseconds;

            Console.WriteLine("Task: {TaskTime}ms, ValueTask: {ValueTaskTime}ms", 
                taskTime, valueTaskTime);
        }

        private async Task Task1() => await Task.Delay(1000);
        private async Task Task2() => await Task.Delay(1500);
        private async Task Task3() => await Task.Delay(800);

        private async Task<string> GetDataWithTaskAsync(string key)
        {
            await Task.Delay(1);
            return "数据";
        }

        private async ValueTask<string> GetDataWithValueTaskAsync(string key)
        {
            await Task.Delay(1);
            return "数据";
        }
    }
}
