using ToInterview.API.Patterns.Decorator;

namespace ToInterview.API.IoC;

/// <summary>
/// 装饰者模式与IoC的关系演示
/// 
/// 关系说明：
/// 1. 装饰者模式通过组合和委托实现功能扩展
/// 2. IoC容器可以管理装饰者的创建和依赖注入
/// 3. 装饰者模式使得IoC容器能够动态组合不同的功能
/// 4. 两者结合可以实现灵活的功能组合和配置
/// </summary>
public class DecoratorWithIoC
{
    /// <summary>
    /// 传统装饰者模式使用方式
    /// 问题：需要手动创建装饰者链，紧耦合
    /// </summary>
    public static void TraditionalDecoratorUsage()
    {
        Console.WriteLine("=== 传统装饰者模式 ===");
        
        // 手动创建装饰者链 - 紧耦合
        ICoffee coffee = new SimpleCoffee();
        coffee = new MilkDecorator(coffee);
        coffee = new SugarDecorator(coffee);
        coffee = new FoamDecorator(coffee);
        
        Console.WriteLine($"描述: {coffee.GetDescription()}");
        Console.WriteLine($"价格: ${coffee.GetCost()}");
    }

    /// <summary>
    /// 使用IoC容器的装饰者模式
    /// 优势：松耦合，可配置，易于测试
    /// </summary>
    public static void IoCWithDecoratorUsage()
    {
        Console.WriteLine("\n=== IoC + 装饰者模式 ===");
        
        // 创建IoC容器
        var container = new SimpleIoCContainer();
        
        // 注册基础服务
        container.Register<ICoffee, SimpleCoffee>();
        
        // 注册装饰者工厂
        container.Register<ICoffeeDecoratorFactory, CoffeeDecoratorFactory>();
        
        // 使用工厂创建装饰者链
        var factory = container.Resolve<ICoffeeDecoratorFactory>();
        var decoratedCoffee = factory.CreateDecoratedCoffee(new[] { "Milk", "Sugar", "Foam" });
        
        Console.WriteLine($"描述: {decoratedCoffee.GetDescription()}");
        Console.WriteLine($"价格: ${decoratedCoffee.GetCost()}");
    }
}

/// <summary>
/// 咖啡装饰者工厂接口
/// 用于通过IoC容器管理装饰者的创建
/// </summary>
public interface ICoffeeDecoratorFactory
{
    ICoffee CreateDecoratedCoffee(string[] decorators);
    ICoffee CreateCustomCoffee(string baseType, string[] decorators);
}

/// <summary>
/// 咖啡装饰者工厂实现
/// 演示如何通过IoC容器管理装饰者
/// </summary>
public class CoffeeDecoratorFactory : ICoffeeDecoratorFactory
{
    private readonly Dictionary<string, Func<ICoffee, ICoffee>> _decoratorCreators;

    public CoffeeDecoratorFactory()
    {
        _decoratorCreators = new Dictionary<string, Func<ICoffee, ICoffee>>
        {
            { "Milk", coffee => new MilkDecorator(coffee) },
            { "Sugar", coffee => new SugarDecorator(coffee) },
            { "Foam", coffee => new FoamDecorator(coffee) },
            { "Vanilla", coffee => new VanillaDecorator(coffee) },
            { "Chocolate", coffee => new ChocolateDecorator(coffee) }
        };
    }

    public ICoffee CreateDecoratedCoffee(string[] decorators)
    {
        // 从IoC容器获取基础咖啡
        var coffee = new SimpleCoffee();
        
        // 应用装饰者
        foreach (var decorator in decorators)
        {
            if (_decoratorCreators.ContainsKey(decorator))
            {
                // coffee = _decoratorCreators[decorator](coffee);
            }
        }
        
        return coffee;
    }

    public ICoffee CreateCustomCoffee(string baseType, string[] decorators)
    {
        // 根据基础类型创建咖啡
        ICoffee coffee = baseType switch
        {
            "Espresso" => new EspressoCoffee(),
            "Latte" => new LatteCoffee(),
            _ => new SimpleCoffee()
        };
        
        // 应用装饰者
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

/// <summary>
/// 扩展的咖啡装饰者
/// </summary>
public class VanillaDecorator : CoffeeDecorator
{
    public VanillaDecorator(ICoffee coffee) : base(coffee) { }

    public override string GetDescription()
    {
        return _coffee.GetDescription() + ", 香草";
    }

    public override decimal GetCost()
    {
        return _coffee.GetCost() + 0.75m;
    }
}

public class ChocolateDecorator : CoffeeDecorator
{
    public ChocolateDecorator(ICoffee coffee) : base(coffee) { }

    public override string GetDescription()
    {
        return _coffee.GetDescription() + ", 巧克力";
    }

    public override decimal GetCost()
    {
        return _coffee.GetCost() + 1.00m;
    }
}

/// <summary>
/// 扩展的咖啡类型
/// </summary>
public class EspressoCoffee : ICoffee
{
    public string GetDescription()
    {
        return "浓缩咖啡";
    }

    public decimal GetCost()
    {
        return 2.50m;
    }
}

public class LatteCoffee : ICoffee
{
    public string GetDescription()
    {
        return "拿铁咖啡";
    }

    public decimal GetCost()
    {
        return 3.50m;
    }
}

/// <summary>
/// 咖啡服务 - 演示业务层如何使用装饰者模式
/// </summary>
public interface ICoffeeService
{
    ICoffee CreateCoffee(string type, string[] decorators);
    decimal CalculateTotalCost(ICoffee coffee);
    string GetCoffeeDescription(ICoffee coffee);
}

public class CoffeeService : ICoffeeService
{
    private readonly ICoffeeDecoratorFactory _decoratorFactory;
    private readonly ILogger _logger;

    public CoffeeService(ICoffeeDecoratorFactory decoratorFactory, ILogger logger)
    {
        _decoratorFactory = decoratorFactory;
        _logger = logger;
    }

    public ICoffee CreateCoffee(string type, string[] decorators)
    {
        _logger.Log($"创建咖啡: {type}, 装饰者: {string.Join(", ", decorators)}");
        
        var coffee = _decoratorFactory.CreateCustomCoffee(type, decorators);
        
        _logger.Log($"咖啡创建完成: {coffee.GetDescription()}, 价格: ${coffee.GetCost()}");
        
        return coffee;
    }

    public decimal CalculateTotalCost(ICoffee coffee)
    {
        var cost = coffee.GetCost();
        _logger.Log($"计算总价: ${cost}");
        return cost;
    }

    public string GetCoffeeDescription(ICoffee coffee)
    {
        var description = coffee.GetDescription();
        _logger.Log($"获取描述: {description}");
        return description;
    }
}
