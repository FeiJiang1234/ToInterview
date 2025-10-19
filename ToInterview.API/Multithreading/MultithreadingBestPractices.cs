using System.Collections.Concurrent;

namespace ToInterview.API.Multithreading;

/// <summary>
/// 多线程编程最佳实践指南
/// </summary>
public static class MultithreadingBestPractices
{
    /// <summary>
    /// 最佳实践1: 使用async/await而不是Task.Run进行I/O密集型操作
    /// </summary>
    public static class AsyncAwaitBestPractices
    {
        // ❌ 错误做法 - 不必要的线程池线程
        public static async Task<string> BadExampleAsync(string url)
        {
            return await Task.Run(async () =>
            {
                using var httpClient = new HttpClient();
                return await httpClient.GetStringAsync(url);
            });
        }

        // ✅ 正确做法 - 直接使用async/await
        public static async Task<string> GoodExampleAsync(string url)
        {
            using var httpClient = new HttpClient();
            return await httpClient.GetStringAsync(url);
        }
    }

    /// <summary>
    /// 最佳实践2: 正确使用ConfigureAwait
    /// </summary>
    public static class ConfigureAwaitBestPractices
    {
        // ✅ 在库代码中使用ConfigureAwait(false)

        // 1. 避免死锁
        // 2. 提高性能
        public static async Task<string> LibraryMethodAsync()
        {
            using var httpClient = new HttpClient();
            var result = await httpClient.GetStringAsync("https://api.example.com")
                .ConfigureAwait(false); // 避免死锁，提高性能

            return result;
        }

        // ✅ 在UI代码中通常不需要ConfigureAwait(false)
        public static async Task<string> UIMethodAsync()
        {
            var result = await LibraryMethodAsync(); // 这里会自动回到UI线程
            return result;
        }
    }

    /// <summary>
    /// 最佳实践3: 线程安全的数据结构
    /// </summary>
    public static class ThreadSafeDataStructures
    {
        // ❌ 错误做法 - 非线程安全的集合
        private static readonly List<string> _unsafeList = new();
        private static readonly object _lock = new();

        public static void UnsafeAdd(string item)
        {
            lock (_lock)
            {
                _unsafeList.Add(item);
            }
        }

        // ✅ 正确做法 - 使用线程安全的集合
        private static readonly ConcurrentBag<string> _safeBag = new();

        public static void SafeAdd(string item)
        {
            _safeBag.Add(item); // 线程安全，无需锁
        }

        // ✅ 正确做法 - 使用ConcurrentDictionary
        private static readonly ConcurrentDictionary<string, int> _safeDictionary = new();

        public static void SafeDictionaryOperation(string key, int value)
        {
            _safeDictionary.AddOrUpdate(key, value, (k, v) => v + value);
        }
    }

    /// <summary>
    /// 最佳实践4: 正确的异常处理
    /// </summary>
    public static class ExceptionHandlingBestPractices
    {
        // ❌ 错误做法 - 吞掉异常
        public static async Task BadExceptionHandlingAsync()
        {
            try
            {
                await SomeAsyncOperation();
            }
            catch
            {
                // 吞掉异常 - 非常危险！
            }
        }

        // ✅ 正确做法 - 适当的异常处理, 重新抛出或处理
        public static async Task GoodExceptionHandlingAsync()
        {
            try
            {
                await SomeAsyncOperation();
            }
            catch (HttpRequestException ex)
            {
                // 记录特定异常
                Console.WriteLine($"HTTP请求失败: {ex.Message}");
                throw; // 重新抛出或处理
            }
            catch (Exception ex)
            {
                // 记录未预期的异常
                Console.WriteLine($"未预期的异常: {ex.Message}");
                throw;
            }
        }

        private static async Task SomeAsyncOperation()
        {
            await Task.Delay(100);
            throw new HttpRequestException("模拟HTTP错误");
        }
    }

    /// <summary>
    /// 最佳实践5: 使用CancellationToken
    /// </summary>
    public static class CancellationTokenBestPractices
    {
        // ✅ 正确做法 - 支持取消操作
        public static async Task<string> CancellableOperationAsync(
            CancellationToken cancellationToken = default)
        {
            for (int i = 0; i < 100; i++)
            {
                cancellationToken.ThrowIfCancellationRequested();

                await Task.Delay(100, cancellationToken);

                // 执行一些工作
                Console.WriteLine($"处理步骤 {i}");
            }

            return "操作完成";
        }

        // ✅ 正确做法 - 传递CancellationToken
        public static async Task ProcessDataAsync(
            IEnumerable<string> data,
            CancellationToken cancellationToken = default)
        {
            foreach (var item in data)
            {
                cancellationToken.ThrowIfCancellationRequested();

                await ProcessItemAsync(item, cancellationToken);
            }
        }

        private static async Task ProcessItemAsync(string item, CancellationToken cancellationToken)
        {
            await Task.Delay(50, cancellationToken);
            Console.WriteLine($"处理项目: {item}");
        }
    }

    /// <summary>
    /// 最佳实践6: 避免死锁
    /// </summary>
    public static class DeadlockPrevention
    {
        // ✅ 正确做法 - 使用async/await
        public static async Task<string> SafeAsyncMethodAsync()
        {
            var result = await SomeAsyncMethod();
            return result;
        }

        private static async Task<string> SomeAsyncMethod()
        {
            await Task.Delay(100);
            return "结果";
        }
    }

    /// <summary>
    /// 最佳实践7: 性能优化
    /// </summary>
    public static class PerformanceOptimization
    {
        // ✅ 使用TaskCompletionSource进行自定义异步操作
        public static Task<string> CustomAsyncOperationAsync()
        {
            var tcs = new TaskCompletionSource<string>();

            // 模拟异步操作
            Task.Run(async () =>
            {
                await Task.Delay(1000);
                tcs.SetResult("自定义操作完成");
            });

            return tcs.Task;
        }

        // ✅ 使用ValueTask提高性能（对于可能同步完成的操作）
        public static async ValueTask<string> OptimizedAsyncMethodAsync(bool useCache)
        {
            if (useCache)
            {
                return "缓存结果"; // 同步返回
            }

            await Task.Delay(100); // 异步操作
            return "异步结果";
        }

        // ✅ 批量操作优化
        public static async Task ProcessBatchAsync<T>(
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
    }

    /// <summary>
    /// 最佳实践8: 内存管理
    /// </summary>
    public static class MemoryManagement
    {
        // ✅ 正确使用using语句
        public static async Task<string> ProperResourceManagementAsync()
        {
            using var httpClient = new HttpClient();
            using var response = await httpClient.GetAsync("https://api.example.com");
            return await response.Content.ReadAsStringAsync();
        }

        // ✅ 实现IDisposable
        public class AsyncResourceManager : IAsyncDisposable
        {
            private readonly SemaphoreSlim _semaphore = new(1, 1);
            private bool _disposed = false;

            public async Task<string> DoWorkAsync()
            {
                await _semaphore.WaitAsync();
                try
                {
                    await Task.Delay(100);
                    return "工作完成";
                }
                finally
                {
                    _semaphore.Release();
                }
            }

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
    }
}
