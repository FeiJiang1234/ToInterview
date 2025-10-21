# 控制反转（Inversion of Control，IoC）详解

## 什么是控制反转？

**控制反转（IoC）是一种设计原则，不是具体的设计模式**。它描述了对象如何获得依赖关系的方式。

### 核心概念

- **传统方式（控制正转）**：对象自己创建和管理依赖关系
- **IoC方式（控制反转）**：依赖关系由外部容器管理并注入

## 为什么需要IoC？

### 传统方式的问题

```csharp
public class OrderService
{
    public void ProcessOrder()
    {
        // 问题：紧耦合，难以测试
        var emailService = new EmailService();
        var databaseService = new DatabaseService();
        var logger = new FileLogger();
        
        // 业务逻辑...
    }
}
```

**问题：**
- 紧耦合：OrderService直接依赖具体实现
- 难以测试：无法模拟依赖进行单元测试
- 违反开闭原则：修改依赖需要修改OrderService
- 难以扩展：无法轻松替换不同的实现

### IoC方式的优势

```csharp
public class OrderService
{
    private readonly IEmailService _emailService;
    private readonly IDatabaseService _databaseService;
    private readonly ILogger _logger;

    // 构造函数注入
    public OrderService(IEmailService emailService, IDatabaseService databaseService, ILogger logger)
    {
        _emailService = emailService;
        _databaseService = databaseService;
        _logger = logger;
    }

    public void ProcessOrder()
    {
        // 业务逻辑...
    }
}
```

**优势：**
- 松耦合：依赖抽象接口，不依赖具体实现
- 易于测试：可以注入模拟对象进行单元测试
- 符合开闭原则：可以轻松扩展新的实现
- 易于维护：依赖关系清晰，易于理解和修改

## IoC的实现方式

### 1. 构造函数注入（推荐）

```csharp
public class OrderService
{
    private readonly IEmailService _emailService;
    
    public OrderService(IEmailService emailService)
    {
        _emailService = emailService;
    }
}
```

### 2. 属性注入

```csharp
public class OrderService
{
    public IEmailService EmailService { get; set; }
}
```

### 3. 方法注入

```csharp
public class OrderService
{
    public void ProcessOrder(IEmailService emailService)
    {
        // 使用注入的服务
    }
}
```

### 4. 服务定位器模式

```csharp
public class OrderService
{
    public void ProcessOrder()
    {
        var emailService = ServiceLocator.GetService<IEmailService>();
        // 使用服务
    }
}
```

## 依赖注入（DI）vs 控制反转（IoC）

- **IoC**：设计原则，描述依赖关系的管理方式
- **DI**：IoC的具体实现技术，通过注入的方式实现IoC

## ASP.NET Core中的IoC

ASP.NET Core内置了强大的DI容器：

```csharp
// Program.cs
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IDatabaseService, DatabaseService>();
builder.Services.AddScoped<IOrderService, OrderService>();
```

### 服务生命周期

- **Singleton**：单例，整个应用程序生命周期内只有一个实例
- **Scoped**：作用域，每个HTTP请求一个实例
- **Transient**：瞬态，每次请求都创建新实例

## 测试中的IoC

IoC使得单元测试变得简单：

```csharp
[Fact]
public void OrderService_ShouldProcessOrder_WithInjectedDependencies()
{
    // 创建模拟对象
    var mockEmailService = new Mock<IEmailService>();
    var mockDatabaseService = new Mock<IDatabaseService>();
    var mockLogger = new Mock<ILogger>();

    // 注入模拟依赖
    var orderService = new OrderService(
        mockEmailService.Object,
        mockDatabaseService.Object,
        mockLogger.Object
    );

    // 执行测试
    orderService.ProcessOrder("TEST-001", "测试客户");

    // 验证调用
    mockEmailService.Verify(x => x.SendEmail("测试客户", "您的订单 TEST-001 已确认"), Times.Once);
}
```

## 最佳实践

1. **优先使用构造函数注入**
2. **依赖抽象接口，不依赖具体实现**
3. **保持构造函数简单，避免复杂逻辑**
4. **使用适当的服务生命周期**
5. **避免服务定位器反模式**
6. **编写可测试的代码**

## 相关设计模式

- **依赖注入（DI）**：IoC的具体实现
- **工厂模式**：创建对象的模式
- **抽象工厂模式**：创建相关对象族的模式
- **服务定位器模式**：另一种IoC实现方式

## 总结

控制反转是一种重要的设计原则，它通过将依赖关系的管理从对象内部转移到外部容器，实现了松耦合、高内聚的设计。在ASP.NET Core中，IoC容器被广泛使用，使得应用程序更加模块化、可测试和可维护。
