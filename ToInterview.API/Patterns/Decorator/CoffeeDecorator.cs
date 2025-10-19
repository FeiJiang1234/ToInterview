namespace ToInterview.API.Patterns.Decorator;

/// <summary>
/// 咖啡装饰器基类 - 装饰模式中的抽象装饰器
/// </summary>
public abstract class CoffeeDecorator : ICoffee
{
    protected ICoffee _coffee;

    public CoffeeDecorator(ICoffee coffee)
    {
        _coffee = coffee ?? throw new ArgumentNullException(nameof(coffee));
    }

    public virtual string GetDescription()
    {
        return _coffee.GetDescription();
    }

    public virtual decimal GetCost()
    {
        return _coffee.GetCost();
    }
}

/// <summary>
/// 牛奶装饰器
/// </summary>
public class MilkDecorator : CoffeeDecorator
{
    public MilkDecorator(ICoffee coffee) : base(coffee) { }

    public override string GetDescription()
    {
        return _coffee.GetDescription() + ", 牛奶";
    }

    public override decimal GetCost()
    {
        return _coffee.GetCost() + 0.50m;
    }
}

/// <summary>
/// 糖装饰器
/// </summary>
public class SugarDecorator : CoffeeDecorator
{
    public SugarDecorator(ICoffee coffee) : base(coffee) { }

    public override string GetDescription()
    {
        return _coffee.GetDescription() + ", 糖";
    }

    public override decimal GetCost()
    {
        return _coffee.GetCost() + 0.25m;
    }
}

/// <summary>
/// 泡沫装饰器
/// </summary>
public class FoamDecorator : CoffeeDecorator
{
    public FoamDecorator(ICoffee coffee) : base(coffee) { }

    public override string GetDescription()
    {
        return _coffee.GetDescription() + ", 泡沫";
    }

    public override decimal GetCost()
    {
        return _coffee.GetCost() + 0.30m;
    }
}
