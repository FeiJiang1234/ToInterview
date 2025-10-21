using Microsoft.Extensions.Logging;
using Moq;
using ToInterview.API.IoC;
using ToInterview.API.Patterns.Decorator;
using Xunit;
using ILogger = ToInterview.API.IoC.ILogger;

namespace ToInterview.API.Tests.IoC;

/// <summary>
/// 装饰者模式与IoC关系测试
/// 演示两者结合使用的优势
/// </summary>
public class DecoratorIoCTests
{
    [Fact]
    public void TraditionalDecorator_ShouldWork_ButIsHardToTest()
    {
        // 传统装饰者模式：手动创建装饰者链
        ICoffee coffee = new SimpleCoffee();
        coffee = new MilkDecorator(coffee);
        coffee = new SugarDecorator(coffee);

        // 验证结果
        Assert.Equal("简单咖啡, 牛奶, 糖", coffee.GetDescription());
        Assert.Equal(2.75m, coffee.GetCost()); // 2.00 + 0.50 + 0.25
    }

    [Fact]
    public void IoCWithDecorator_ShouldBeEasyToTest_WithMockedDependencies()
    {
        // IoC + 装饰者模式：通过依赖注入进行测试
        
        // 创建模拟对象
        var mockFactory = new Mock<ICoffeeDecoratorFactory>();
        var mockLogger = new Mock<ILogger>();

        // 设置模拟行为
        var expectedCoffee = new SimpleCoffee();
        //expectedCoffee = new MilkDecorator(expectedCoffee);
        //expectedCoffee = new SugarDecorator(expectedCoffee);
        
        mockFactory.Setup(f => f.CreateCustomCoffee("Espresso", new[] { "Milk", "Sugar" }))
                  .Returns(expectedCoffee);

        // 创建被测试对象
        var coffeeService = new CoffeeService(mockFactory.Object, mockLogger.Object);

        // 执行测试
        var result = coffeeService.CreateCoffee("Espresso", new[] { "Milk", "Sugar" });

        // 验证结果
        Assert.NotNull(result);
        Assert.Equal("简单咖啡, 牛奶, 糖", result.GetDescription());
        Assert.Equal(2.75m, result.GetCost());

        // 验证模拟对象被正确调用
        mockFactory.Verify(f => f.CreateCustomCoffee("Espresso", new[] { "Milk", "Sugar" }), Times.Once);
        mockLogger.Verify(l => l.Log(It.IsAny<string>()), Times.AtLeast(2));
    }

    [Fact]
    public void CoffeeDecoratorFactory_ShouldCreateCorrectDecoratorChain()
    {
        // 测试装饰者工厂
        var factory = new CoffeeDecoratorFactory();

        // 测试不同的装饰者组合
        var coffee1 = factory.CreateDecoratedCoffee(new[] { "Milk" });
        Assert.Equal("简单咖啡, 牛奶", coffee1.GetDescription());
        Assert.Equal(2.50m, coffee1.GetCost());

        var coffee2 = factory.CreateDecoratedCoffee(new[] { "Milk", "Sugar", "Foam" });
        Assert.Equal("简单咖啡, 牛奶, 糖, 泡沫", coffee2.GetDescription());
        Assert.Equal(3.05m, coffee2.GetCost()); // 2.00 + 0.50 + 0.25 + 0.30
    }

    [Fact]
    public void CoffeeDecoratorFactory_ShouldHandleCustomBaseTypes()
    {
        // 测试自定义基础类型
        var factory = new CoffeeDecoratorFactory();

        var espresso = factory.CreateCustomCoffee("Espresso", new[] { "Milk" });
        Assert.Equal("浓缩咖啡, 牛奶", espresso.GetDescription());
        Assert.Equal(3.00m, espresso.GetCost()); // 2.50 + 0.50

        var latte = factory.CreateCustomCoffee("Latte", new[] { "Sugar", "Foam" });
        Assert.Equal("拿铁咖啡, 糖, 泡沫", latte.GetDescription());
        Assert.Equal(4.05m, latte.GetCost()); // 3.50 + 0.25 + 0.30
    }

    [Fact]
    public void CoffeeService_ShouldLogOperations()
    {
        // 测试日志记录
        var mockFactory = new Mock<ICoffeeDecoratorFactory>();
        var mockLogger = new Mock<ILogger>();

        var testCoffee = new SimpleCoffee();
        mockFactory.Setup(f => f.CreateCustomCoffee(It.IsAny<string>(), It.IsAny<string[]>()))
                  .Returns(testCoffee);

        var coffeeService = new CoffeeService(mockFactory.Object, mockLogger.Object);

        // 执行操作
        coffeeService.CreateCoffee("Espresso", new[] { "Milk" });
        coffeeService.CalculateTotalCost(testCoffee);
        coffeeService.GetCoffeeDescription(testCoffee);

        // 验证日志调用
        mockLogger.Verify(l => l.Log(It.Is<string>(s => s.Contains("创建咖啡"))), Times.Once);
        mockLogger.Verify(l => l.Log(It.Is<string>(s => s.Contains("计算总价"))), Times.Once);
        mockLogger.Verify(l => l.Log(It.Is<string>(s => s.Contains("获取描述"))), Times.Once);
    }

    [Fact]
    public void DecoratorChain_ShouldBeComposable()
    {
        // 测试装饰者链的组合性
        var factory = new CoffeeDecoratorFactory();

        // 测试不同的装饰者顺序
        var coffee1 = factory.CreateDecoratedCoffee(new[] { "Milk", "Sugar" });
        var coffee2 = factory.CreateDecoratedCoffee(new[] { "Sugar", "Milk" });

        // 结果应该相同（装饰者顺序不影响最终结果）
        Assert.Equal(coffee1.GetDescription(), coffee2.GetDescription());
        Assert.Equal(coffee1.GetCost(), coffee2.GetCost());
    }

    [Fact]
    public void IoCContainer_ShouldResolveDecoratorServices()
    {
        // 测试IoC容器解析装饰者相关服务
        var container = new SimpleIoCContainer();

        // 注册服务
        container.Register<ICoffeeDecoratorFactory, CoffeeDecoratorFactory>();
        container.Register<ICoffeeService, CoffeeService>();
        container.RegisterSingleton<ILogger>(new ConsoleLogger());

        // 解析服务
        var factory = container.Resolve<ICoffeeDecoratorFactory>();
        var coffeeService = container.Resolve<ICoffeeService>();

        // 验证服务被正确解析
        Assert.NotNull(factory);
        Assert.NotNull(coffeeService);
        Assert.IsType<CoffeeDecoratorFactory>(factory);
        Assert.IsType<CoffeeService>(coffeeService);
    }

    [Fact]
    public void ExtendedDecorators_ShouldWorkCorrectly()
    {
        // 测试扩展的装饰者
        var factory = new CoffeeDecoratorFactory();

        var vanillaCoffee = factory.CreateDecoratedCoffee(new[] { "Vanilla" });
        Assert.Equal("简单咖啡, 香草", vanillaCoffee.GetDescription());
        Assert.Equal(2.75m, vanillaCoffee.GetCost());

        var chocolateCoffee = factory.CreateDecoratedCoffee(new[] { "Chocolate" });
        Assert.Equal("简单咖啡, 巧克力", chocolateCoffee.GetDescription());
        Assert.Equal(3.00m, chocolateCoffee.GetCost());

        var luxuryCoffee = factory.CreateDecoratedCoffee(new[] { "Milk", "Sugar", "Vanilla", "Chocolate" });
        Assert.Equal("简单咖啡, 牛奶, 糖, 香草, 巧克力", luxuryCoffee.GetDescription());
        Assert.Equal(4.50m, luxuryCoffee.GetCost()); // 2.00 + 0.50 + 0.25 + 0.75 + 1.00
    }
}
