using System.Collections.Concurrent;
using System.Diagnostics;

namespace ToInterview.API.Services;

/// <summary>
/// 多线程编程核心概念详解
/// </summary>
public static class MultithreadingConcepts
{
    /// <summary>
    /// 概念1: 进程 vs 线程 vs 任务
    /// </summary>
    public static class ProcessThreadTask
    {
        /*
         * 进程 (Process):
         * - 操作系统分配资源的基本单位
         * - 拥有独立的内存空间
         * - 进程间通信需要特殊机制（管道、消息队列等）
         * 
         * 线程 (Thread):
         * - 进程内的执行单元
         * - 共享进程的内存空间
         * - 线程间通信相对简单
         * - 创建和销毁开销较大
         * 
         * 任务 (Task):
         * - .NET中的高级抽象
         * - 基于线程池，开销较小
         * - 支持异步编程模式
         * - 更好的异常处理和取消支持
         */

        public static void DemonstrateThreadVsTask()
        {
            Console.WriteLine("=== 线程 vs 任务对比 ===");
            
            // 使用Thread（低级API）
            var thread = new Thread(() =>
            {
                Console.WriteLine($"Thread ID: {Thread.CurrentThread.ManagedThreadId}");
                Thread.Sleep(1000);
                Console.WriteLine("Thread 完成");
            });
            thread.Start();
            thread.Join();

            // 使用Task（高级API）
            var task = Task.Run(() =>
            {
                Console.WriteLine($"Task Thread ID: {Thread.CurrentThread.ManagedThreadId}");
                Thread.Sleep(1000);
                Console.WriteLine("Task 完成");
            });
            task.Wait();
        }
    }

    /// <summary>
    /// 概念2: 同步 vs 异步
    /// </summary>
    public static class SyncVsAsync
    {
        /*
         * 同步 (Synchronous):
         * - 按顺序执行，一个操作完成后才执行下一个
         * - 阻塞调用线程
         * - 简单但效率较低
         * 
         * 异步 (Asynchronous):
         * - 不等待操作完成，继续执行其他代码
         * - 不阻塞调用线程
         * - 复杂但效率高
         */

        // 同步方法
        public static string SyncMethod()
        {
            Thread.Sleep(2000); // 模拟耗时操作
            return "同步结果";
        }

        // 异步方法
        public static async Task<string> AsyncMethod()
        {
            await Task.Delay(2000); // 模拟异步操作
            return "异步结果";
        }

        public static async Task DemonstrateSyncVsAsync()
        {
            Console.WriteLine("=== 同步 vs 异步对比 ===");
            
            // 同步执行
            var startTime = DateTime.Now;
            var syncResult1 = SyncMethod();
            var syncResult2 = SyncMethod();
            var syncTime = DateTime.Now - startTime;
            Console.WriteLine($"同步执行耗时: {syncTime.TotalMilliseconds}ms");

            // 异步执行
            startTime = DateTime.Now;
            var asyncTask1 = AsyncMethod();
            var asyncTask2 = AsyncMethod();
            var asyncResult1 = await asyncTask1;
            var asyncResult2 = await asyncTask2;
            var asyncTime = DateTime.Now - startTime;
            Console.WriteLine($"异步执行耗时: {asyncTime.TotalMilliseconds}ms");
        }
    }

    /// <summary>
    /// 概念3: 并发 vs 并行
    /// </summary>
    public static class ConcurrencyVsParallelism
    {
        /*
         * 并发 (Concurrency):
         * - 多个任务在同一时间段内执行
         * - 可能在同一核心上交替执行
         * - 通过时间片轮转实现
         * 
         * 并行 (Parallelism):
         * - 多个任务在同一时刻执行
         * - 需要多个核心/处理器
         * - 真正的同时执行
         */

        public static void DemonstrateConcurrency()
        {
            Console.WriteLine("=== 并发示例 ===");
            
            // 并发执行多个任务
            var tasks = new[]
            {
                Task.Run(() => { Thread.Sleep(1000); Console.WriteLine("任务1完成"); }),
                Task.Run(() => { Thread.Sleep(1500); Console.WriteLine("任务2完成"); }),
                Task.Run(() => { Thread.Sleep(800); Console.WriteLine("任务3完成"); })
            };

            Task.WaitAll(tasks);
        }

        public static void DemonstrateParallelism()
        {
            Console.WriteLine("=== 并行示例 ===");
            
            var numbers = Enumerable.Range(1, 1000000);
            
            // 并行处理
            Parallel.ForEach(numbers, number =>
            {
                // 模拟CPU密集型工作
                var result = Math.Sqrt(number);
            });
            
            Console.WriteLine("并行处理完成");
        }
    }

    /// <summary>
    /// 概念4: 线程安全
    /// </summary>
    public static class ThreadSafety
    {
        /*
         * 线程安全 (Thread Safety):
         * - 多线程环境下数据的一致性和正确性
         * - 避免竞态条件 (Race Condition)
         * - 确保数据不被破坏
         */

        // 非线程安全的示例
        private static int _unsafeCounter = 0;
        private static readonly object _lockObject = new object();

        public static void UnsafeIncrement()
        {
            _unsafeCounter++; // 非原子操作，可能导致竞态条件
        }

        // 线程安全的示例
        public static void SafeIncrement()
        {
            lock (_lockObject)
            {
                _unsafeCounter++; // 使用锁保护
            }
        }

        // 使用线程安全的集合
        private static readonly ConcurrentDictionary<string, int> _safeDictionary = new();

        public static void SafeDictionaryOperation(string key)
        {
            _safeDictionary.AddOrUpdate(key, 1, (k, v) => v + 1);
        }

        public static void DemonstrateThreadSafety()
        {
            Console.WriteLine("=== 线程安全示例 ===");
            
            var tasks = new List<Task>();
            
            // 创建多个任务同时修改共享数据
            for (int i = 0; i < 1000; i++)
            {
                tasks.Add(Task.Run(SafeIncrement));
            }

            Task.WaitAll(tasks.ToArray());
            Console.WriteLine($"最终计数: {_unsafeCounter}");
        }
    }

    /// <summary>
    /// 概念6: 线程池
    /// </summary>
    public static class ThreadPool
    {
        /*
         * 线程池 (Thread Pool):
         * - 预创建的线程集合
         * - 重用线程，减少创建/销毁开销
         * - 自动管理线程数量
         * - Task默认使用线程池
         */
    }

    /// <summary>
    /// 概念7: 异步编程模式
    /// </summary>
    public static class AsyncPatterns
    {
        /*
         * 异步编程模式:
         * 1. APM (Asynchronous Programming Model) - Begin/End
         * 2. EAP (Event-based Asynchronous Pattern) - 事件
         * 3. TAP (Task-based Asynchronous Pattern) - async/await
         */

        // TAP模式 - 推荐使用
        public static async Task<string> TapPatternAsync()
        {
            await Task.Delay(1000);
            return "TAP模式结果";
        }

        // 异步流 (Async Streams)
        public static async IAsyncEnumerable<int> AsyncStreamExample()
        {
            for (int i = 0; i < 5; i++)
            {
                await Task.Delay(500);
                yield return i;
            }
        }

        // 异步枚举
        public static async Task DemonstrateAsyncStream()
        {
            Console.WriteLine("=== 异步流示例 ===");
            
            await foreach (var item in AsyncStreamExample())
            {
                Console.WriteLine($"异步流项目: {item}");
            }
        }
    }

    /// <summary>
    /// 概念8: 取消令牌
    /// </summary>
    public static class CancellationTokens
    {
        /*
         * CancellationToken:
         * - 用于取消长时间运行的操作
         * - 支持协作式取消
         * - 避免强制终止线程
         */

        public static async Task CancellableOperationAsync(CancellationToken cancellationToken = default)
        {
            for (int i = 0; i < 100; i++)
            {
                cancellationToken.ThrowIfCancellationRequested();
                
                Console.WriteLine($"执行步骤 {i}");
                await Task.Delay(100, cancellationToken);
            }
        }

        public static async Task DemonstrateCancellation()
        {
            Console.WriteLine("=== 取消令牌示例 ===");
            
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
    }

    /// <summary>
    /// 概念10: 死锁和避免
    /// </summary>
    public static class DeadlockPrevention
    {
        /*
         * 死锁条件 (四个必要条件):
         * 1. 互斥条件
         * 2. 请求和保持条件
         * 3. 不剥夺条件
         * 4. 环路等待条件
         */

        private static readonly object _lock1 = new object();
        private static readonly object _lock2 = new object();

        // 可能导致死锁的代码
        public static void PotentialDeadlock()
        {
            var task1 = Task.Run(() =>
            {
                lock (_lock1)
                {
                    Thread.Sleep(100);
                    lock (_lock2) // 可能导致死锁
                    {
                        Console.WriteLine("任务1完成");
                    }
                }
            });

            var task2 = Task.Run(() =>
            {
                lock (_lock2)
                {
                    Thread.Sleep(100);
                    lock (_lock1) // 可能导致死锁
                    {
                        Console.WriteLine("任务2完成");
                    }
                }
            });

            Task.WaitAll(task1, task2);
        }

        // 避免死锁的方法
        public static void AvoidDeadlock()
        {
            var task1 = Task.Run(() =>
            {
                lock (_lock1)
                {
                    Thread.Sleep(100);
                    lock (_lock2)
                    {
                        Console.WriteLine("任务1完成");
                    }
                }
            });

            var task2 = Task.Run(() =>
            {
                lock (_lock1) // 相同的锁顺序
                {
                    Thread.Sleep(100);
                    lock (_lock2)
                    {
                        Console.WriteLine("任务2完成");
                    }
                }
            });

            Task.WaitAll(task1, task2);
        }
    }
}
