namespace ToInterview.API.Patterns.Decorator;

/// <summary>
/// 简单咖啡 - 装饰模式中的具体组件
/// </summary>
public class SimpleCoffee : ICoffee
{
    public string GetDescription()
    {
        return "简单咖啡";
    }

    public decimal GetCost()
    {
        return 2.00m;
    }
}

/// <summary>
/// 浓缩咖啡 - 另一种具体组件
/// </summary>
public class Espresso : ICoffee
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
