# 多线程编程示例和最佳实践

本项目包含了C#多线程编程的完整示例和最佳实践指南。

## 文件结构

### 核心服务
- `ThreadSafeEventService.cs` - 线程安全的事件服务
- `MultithreadingExamples.cs` - 多线程编程示例集合
- `MultithreadingBestPractices.cs` - 最佳实践指南

### API控制器
- `MultithreadingController.cs` - 多线程功能演示的API端点

## 主要功能

### 1. 线程安全的事件服务
- 支持并发用户登录/登出
- 使用`SemaphoreSlim`控制并发访问
- 使用`ConcurrentDictionary`存储用户会话
- 线程安全的事件处理

### 2. 多线程示例
- **基本异步操作**: async/await vs 串行执行
- **Parallel.For**: 并行循环处理
- **PLINQ**: 并行LINQ查询
- **生产者-消费者模式**: 使用`ConcurrentQueue`
- **信号量控制**: 限制并发数量
- **线程安全集合**: `ConcurrentBag`, `ConcurrentDictionary`
- **取消令牌**: 优雅的任务取消
- **异常处理**: 多线程中的异常管理

### 3. 最佳实践
- async/await的正确使用
- ConfigureAwait的使用场景
- 线程安全数据结构选择
- 异常处理策略
- 死锁预防
- 性能优化技巧
- 内存管理

## API端点

### 基础示例
- `GET /api/multithreading/basic-async` - 基本异步示例
- `GET /api/multithreading/parallel-for` - Parallel.For示例
- `GET /api/multithreading/plinq` - PLINQ示例
- `GET /api/multithreading/semaphore` - 信号量示例

### 高级功能
- `GET /api/multithreading/producer-consumer` - 生产者-消费者模式
- `GET /api/multithreading/thread-safe-collections` - 线程安全集合
- `GET /api/multithreading/cancellation-token` - 取消令牌示例
- `GET /api/multithreading/exception-handling` - 异常处理示例

### 实际应用
- `POST /api/multithreading/simulate-concurrent-logins` - 模拟并发登录
- `POST /api/multithreading/simulate-user-action` - 模拟用户操作
- `GET /api/multithreading/online-users` - 获取在线用户
- `POST /api/multithreading/stress-test` - 压力测试

## 使用示例

### 1. 模拟并发用户登录
```bash
POST /api/multithreading/simulate-concurrent-logins
Content-Type: application/json

["user1", "user2", "user3", "user4", "user5"]
```

### 2. 压力测试
```bash
POST /api/multithreading/stress-test?userCount=100&actionsPerUser=5
```

### 3. 模拟用户操作
```bash
POST /api/multithreading/simulate-user-action
Content-Type: application/json

{
  "userName": "testuser",
  "action": "view_profile"
}
```

## 关键概念

### 线程安全
- 使用`ConcurrentDictionary`而不是`Dictionary` + `lock`
- 使用`SemaphoreSlim`控制并发访问
- 避免共享可变状态

### 异步编程
- 优先使用async/await而不是Task.Run
- 在库代码中使用ConfigureAwait(false)
- 正确处理CancellationToken

### 性能优化
- 使用ValueTask对于可能同步完成的操作
- 批量处理数据
- 避免不必要的线程创建

### 异常处理
- 不要吞掉异常
- 使用AggregateException处理多个异常
- 在适当的地方重新抛出异常

## 注意事项

1. **避免死锁**: 不要在async方法中使用.Result或.Wait()
2. **资源管理**: 正确实现IDisposable和IAsyncDisposable
3. **取消支持**: 长时间运行的操作应该支持取消
4. **异常传播**: 确保异常能够正确传播到调用者
5. **性能监控**: 使用性能计数器监控多线程应用的性能

## 扩展建议

1. 添加更多的性能测试
2. 实现自定义的线程池
3. 添加分布式锁的实现
4. 集成监控和日志记录
5. 添加单元测试覆盖
