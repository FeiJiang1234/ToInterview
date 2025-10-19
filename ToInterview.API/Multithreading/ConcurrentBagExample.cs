using System.Collections.Concurrent;
using System.Diagnostics;

namespace ToInterview.API.Services;

/// <summary>
/// ConcurrentBag 线程安全原理示例
/// </summary>
public class ConcurrentBagExample
{
    /// <summary>
    /// 演示 ConcurrentBag 的线程安全特性
    /// </summary>
    public void DemonstrateThreadSafety()
    {
        Console.WriteLine("=== ConcurrentBag 线程安全演示 ===");

        // 1. 基本线程安全操作
        BasicThreadSafetyExample();

        // 2. 性能对比
        PerformanceComparison();

        // 3. 内部工作原理
        InternalWorkingPrinciple();
    }

    /// <summary>
    /// 基本线程安全操作示例
    /// </summary>
    public void BasicThreadSafetyExample()
    {
        Console.WriteLine("\n--- 基本线程安全操作 ---");

        var concurrentBag = new ConcurrentBag<int>();
        var tasks = new List<Task>();

        // 多个线程同时添加元素
        for (int i = 0; i < 10; i++)
        {
            int threadId = i;
            tasks.Add(Task.Run(() =>
            {
                for (int j = 0; j < 100; j++)
                {
                    concurrentBag.Add(threadId * 100 + j);
                }
                Console.WriteLine($"线程 {threadId} 添加了 100 个元素");
            }));
        }

        Task.WaitAll(tasks.ToArray());
        Console.WriteLine($"总共添加了 {concurrentBag.Count} 个元素");

        // 多个线程同时取出元素
        var takeTasks = new List<Task>();
        var results = new ConcurrentBag<int>();

        for (int i = 0; i < 5; i++)
        {
            int threadId = i;
            takeTasks.Add(Task.Run(() =>
            {
                int count = 0;
                while (concurrentBag.TryTake(out int item))
                {
                    results.Add(item);
                    count++;
                }
                Console.WriteLine($"线程 {threadId} 取出了 {count} 个元素");
            }));
        }

        Task.WaitAll(takeTasks.ToArray());
        Console.WriteLine($"总共取出了 {results.Count} 个元素");
    }

    /// <summary>
    /// 性能对比：ConcurrentBag vs 普通集合 + 锁
    /// </summary>
    public void PerformanceComparison()
    {
        Console.WriteLine("\n--- 性能对比 ---");

        const int iterations = 100000;
        var stopwatch = Stopwatch.StartNew();

        // 1. 使用 ConcurrentBag
        var concurrentBag = new ConcurrentBag<int>();
        stopwatch.Restart();

        Parallel.For(0, iterations, i =>
        {
            concurrentBag.Add(i);
        });

        var concurrentBagTime = stopwatch.ElapsedMilliseconds;
        Console.WriteLine($"ConcurrentBag 添加 {iterations} 个元素: {concurrentBagTime}ms");

        // 2. 使用普通 List + 锁
        var list = new List<int>();
        var lockObject = new object();
        stopwatch.Restart();

        Parallel.For(0, iterations, i =>
        {
            lock (lockObject)
            {
                list.Add(i);
            }
        });

        var listWithLockTime = stopwatch.ElapsedMilliseconds;
        Console.WriteLine($"List + 锁 添加 {iterations} 个元素: {listWithLockTime}ms");

        // 3. 性能提升
        var improvement = (double)listWithLockTime / concurrentBagTime;
        Console.WriteLine($"ConcurrentBag 性能提升: {improvement:F2}x");
    }

    /// <summary>
    /// 内部工作原理演示
    /// </summary>
    public void InternalWorkingPrinciple()
    {
        Console.WriteLine("\n--- 内部工作原理 ---");

        var bag = new ConcurrentBag<string>();

        // 模拟 Thread-Local Storage 行为
        Console.WriteLine("1. Thread-Local Storage (TLS) 原理:");
        Console.WriteLine("   - 每个线程维护自己的本地列表");
        Console.WriteLine("   - 添加操作优先写入本地列表");
        Console.WriteLine("   - 减少线程间竞争");

        // 添加元素
        Parallel.For(0, 10, i =>
        {
            bag.Add($"Thread-{Thread.CurrentThread.ManagedThreadId}-Item-{i}");
        });

        Console.WriteLine($"   添加了 {bag.Count} 个元素");

        // 模拟工作窃取算法
        Console.WriteLine("\n2. 工作窃取算法 (Work-Stealing):");
        Console.WriteLine("   - 每个线程优先从自己的本地列表操作");
        Console.WriteLine("   - 当本地列表为空时，从其他线程窃取元素");
        Console.WriteLine("   - 提高并行效率");

        var takeResults = new ConcurrentBag<string>();
        Parallel.For(0, 5, i =>
        {
            int count = 0;
            while (bag.TryTake(out string item))
            {
                takeResults.Add(item);
                count++;
            }
            Console.WriteLine($"   线程 {Thread.CurrentThread.ManagedThreadId} 取出了 {count} 个元素");
        });

        Console.WriteLine($"   总共取出了 {takeResults.Count} 个元素");
    }

    /// <summary>
    /// 演示 ConcurrentBag 的特殊行为
    /// </summary>
    public void DemonstrateSpecialBehaviors()
    {
        Console.WriteLine("\n--- ConcurrentBag 特殊行为 ---");

        var bag = new ConcurrentBag<int> { 1, 2, 3, 4, 5 };

        // 1. LIFO 行为（后进先出）
        Console.WriteLine("1. LIFO 行为 (后进先出):");
        Console.WriteLine("   添加顺序: 1, 2, 3, 4, 5");
        
        var takeOrder = new List<int>();
        while (bag.TryTake(out int item))
        {
            takeOrder.Add(item);
        }
        
        Console.WriteLine($"   取出顺序: {string.Join(", ", takeOrder)}");
        Console.WriteLine("   注意：每个线程内部是 LIFO，但线程间可能不同");

        // 2. 无序性
        Console.WriteLine("\n2. 无序性:");
        Console.WriteLine("   - 不能保证元素的顺序");
        Console.WriteLine("   - 适合不需要顺序的场景");
        Console.WriteLine("   - 如果需要顺序，使用 ConcurrentQueue 或 ConcurrentStack");
    }

    /// <summary>
    /// 线程安全机制详解
    /// </summary>
    public void ThreadSafetyMechanisms()
    {
        Console.WriteLine("\n--- 线程安全机制详解 ---");

        Console.WriteLine("1. 无锁编程 (Lock-Free Programming):");
        Console.WriteLine("   - 使用原子操作 (Atomic Operations)");
        Console.WriteLine("   - 使用内存屏障 (Memory Barriers)");
        Console.WriteLine("   - 使用 Compare-And-Swap (CAS) 操作");
        Console.WriteLine("   - 避免传统锁的开销");

        Console.WriteLine("\n2. 细粒度锁 (Fine-Grained Locking):");
        Console.WriteLine("   - 只在必要时使用锁");
        Console.WriteLine("   - 使用 Thread-Local 锁");
        Console.WriteLine("   - 减少锁的竞争");

        Console.WriteLine("\n3. 内存模型 (Memory Model):");
        Console.WriteLine("   - 使用 volatile 关键字");
        Console.WriteLine("   - 确保内存可见性");
        Console.WriteLine("   - 防止指令重排序");

        Console.WriteLine("\n4. 工作窃取 (Work-Stealing):");
        Console.WriteLine("   - 每个线程维护本地队列");
        Console.WriteLine("   - 本地队列为空时窃取其他线程的工作");
        Console.WriteLine("   - 提高并行效率");
    }

    /// <summary>
    /// 使用场景和最佳实践
    /// </summary>
    public void UsageScenariosAndBestPractices()
    {
        Console.WriteLine("\n--- 使用场景和最佳实践 ---");

        Console.WriteLine("适用场景:");
        Console.WriteLine("1. 生产者-消费者模式");
        Console.WriteLine("2. 并行计算结果的收集");
        Console.WriteLine("3. 不需要顺序的集合操作");
        Console.WriteLine("4. 高并发的添加和取出操作");

        Console.WriteLine("\n最佳实践:");
        Console.WriteLine("1. 适合读多写少的场景");
        Console.WriteLine("2. 不需要保证元素顺序");
        Console.WriteLine("3. 避免频繁的 Count 操作（性能开销大）");
        Console.WriteLine("4. 使用 TryTake 而不是 Take（避免阻塞）");

        Console.WriteLine("\n不适用场景:");
        Console.WriteLine("1. 需要保证元素顺序");
        Console.WriteLine("2. 需要频繁的 Count 操作");
        Console.WriteLine("3. 需要索引访问");
        Console.WriteLine("4. 需要排序功能");
    }
}
