# 装饰者模式与IoC的关系

## 概述

装饰者模式（Decorator Pattern）与控制反转（IoC）有着密切的关系，两者结合使用可以创造出更加灵活、可维护和可测试的代码架构。

## 装饰者模式回顾

装饰者模式是一种结构型设计模式，它允许向一个现有的对象添加新的功能，同时又不改变其结构。装饰者模式通过创建一个包装对象来包装真实对象。

### 装饰者模式的特点：
- 动态地给一个对象添加一些额外的职责
- 比生成子类更为灵活
- 通过组合和委托实现功能扩展

## IoC与装饰者模式的关系

### 1. 依赖管理关系

**传统装饰者模式的问题：**
```csharp
// 紧耦合：需要手动创建装饰者链
ICoffee coffee = new SimpleCoffee();
coffee = new MilkDecorator(coffee);
coffee = new SugarDecorator(coffee);
coffee = new FoamDecorator(coffee);
```

**IoC + 装饰者模式的解决方案：**
```csharp
// 松耦合：通过IoC容器管理装饰者
public class CoffeeService
{
    private readonly ICoffeeDecoratorFactory _decoratorFactory;
    
    public CoffeeService(ICoffeeDecoratorFactory decoratorFactory)
    {
        _decoratorFactory = decoratorFactory; // 依赖注入
    }
    
    public ICoffee CreateCoffee(string[] decorators)
    {
        return _decoratorFactory.CreateDecoratedCoffee(decorators);
    }
}
```

### 2. 功能组合关系

IoC容器可以管理装饰者的创建和组合，使得功能组合更加灵活：

```csharp
public class CoffeeDecoratorFactory : ICoffeeDecoratorFactory
{
    private readonly Dictionary<string, Func<ICoffee, ICoffee>> _decoratorCreators;

    public CoffeeDecoratorFactory()
    {
        _decoratorCreators = new Dictionary<string, Func<ICoffee, ICoffee>>
        {
            { "Milk", coffee => new MilkDecorator(coffee) },
            { "Sugar", coffee => new SugarDecorator(coffee) },
            { "Foam", coffee => new FoamDecorator(coffee) }
        };
    }

    public ICoffee CreateDecoratedCoffee(string[] decorators)
    {
        var coffee = new SimpleCoffee();
        foreach (var decorator in decorators)
        {
            if (_decoratorCreators.ContainsKey(decorator))
            {
                coffee = _decoratorCreators[decorator](coffee);
            }
        }
        return coffee;
    }
}
```

### 3. 配置驱动关系

通过IoC容器，装饰者模式可以变得配置驱动：

```csharp
// 在Program.cs中配置
builder.Services.AddScoped<ICoffeeDecoratorFactory, CoffeeDecoratorFactory>();
builder.Services.AddScoped<ICoffeeService, CoffeeService>();

// 可以通过配置文件控制装饰者的行为
var decorators = configuration.GetSection("CoffeeDecorators").Get<string[]>();
```

## 两者结合的优势

### 1. 松耦合设计
- 装饰者通过接口定义，不依赖具体实现
- IoC容器管理依赖关系，降低耦合度

### 2. 动态功能组合
- 可以根据配置动态创建装饰者链
- 支持运行时决定使用哪些装饰者

### 3. 易于测试
- 可以注入模拟对象进行单元测试
- 每个装饰者都可以独立测试

### 4. 易于扩展
- 添加新的装饰者不需要修改现有代码
- 通过IoC容器注册新的装饰者即可

### 5. 配置驱动
- 可以通过配置文件控制装饰者的行为
- 支持不同环境使用不同的装饰者组合

## 实际应用场景

### 1. 日志记录
```csharp
public interface ILogger
{
    void Log(string message);
}

public class ConsoleLogger : ILogger { }
public class FileLogger : ILogger { }
public class DatabaseLogger : ILogger { }

// 通过装饰者模式组合不同的日志记录方式
public class CompositeLogger : ILogger
{
    private readonly ILogger[] _loggers;
    
    public CompositeLogger(ILogger[] loggers)
    {
        _loggers = loggers;
    }
    
    public void Log(string message)
    {
        foreach (var logger in _loggers)
        {
            logger.Log(message);
        }
    }
}
```

### 2. 缓存装饰
```csharp
public interface IDataService
{
    Task<string> GetDataAsync(string key);
}

public class DatabaseDataService : IDataService { }

public class CachedDataService : IDataService
{
    private readonly IDataService _dataService;
    private readonly ICache _cache;
    
    public CachedDataService(IDataService dataService, ICache cache)
    {
        _dataService = dataService;
        _cache = cache;
    }
    
    public async Task<string> GetDataAsync(string key)
    {
        var cached = _cache.Get<string>(key);
        if (cached != null) return cached;
        
        var data = await _dataService.GetDataAsync(key);
        _cache.Set(key, data);
        return data;
    }
}
```

### 3. 安全装饰
```csharp
public interface IUserService
{
    User GetUser(int id);
}

public class UserService : IUserService { }

public class SecureUserService : IUserService
{
    private readonly IUserService _userService;
    private readonly ISecurityService _securityService;
    
    public SecureUserService(IUserService userService, ISecurityService securityService)
    {
        _userService = userService;
        _securityService = securityService;
    }
    
    public User GetUser(int id)
    {
        if (!_securityService.HasPermission("ReadUser"))
            throw new UnauthorizedAccessException();
            
        return _userService.GetUser(id);
    }
}
```

## 最佳实践

### 1. 接口设计
- 装饰者和被装饰者都应该实现相同的接口
- 接口应该设计得足够灵活，支持装饰者的扩展

### 2. 依赖注入
- 使用构造函数注入，避免属性注入
- 通过IoC容器管理装饰者的创建

### 3. 工厂模式
- 使用工厂模式创建装饰者链
- 工厂可以通过配置决定创建哪些装饰者

### 4. 测试策略
- 为每个装饰者编写独立的单元测试
- 使用模拟对象测试装饰者链的组合

### 5. 配置管理
- 通过配置文件控制装饰者的行为
- 支持不同环境使用不同的装饰者组合

## 总结

装饰者模式与IoC的结合使用，创造了一种强大而灵活的设计模式组合：

- **IoC提供了依赖管理的能力**，使得装饰者模式更加松耦合
- **装饰者模式提供了功能扩展的能力**，使得IoC容器能够动态组合功能
- **两者结合实现了配置驱动的功能组合**，提高了系统的灵活性和可维护性

这种组合在现代软件开发中非常常见，特别是在需要动态功能组合和配置驱动的场景中。
