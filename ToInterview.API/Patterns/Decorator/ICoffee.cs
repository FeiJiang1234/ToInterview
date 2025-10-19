namespace ToInterview.API.Patterns.Decorator;

/// <summary>
/// 咖啡接口 - 定义咖啡的基本操作
/// </summary>
public interface ICoffee
{
    /// <summary>
    /// 获取咖啡描述
    /// </summary>
    string GetDescription();
    
    /// <summary>
    /// 获取咖啡价格
    /// </summary>
    decimal GetCost();
}
