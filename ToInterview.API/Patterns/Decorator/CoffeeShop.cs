namespace ToInterview.API.Patterns.Decorator;

/// <summary>
/// 咖啡店 - 演示装饰模式的使用
/// </summary>
public class CoffeeShop
{
    /// <summary>
    /// 创建各种咖啡组合
    /// </summary>
    public static class CoffeeBuilder
    {
        /// <summary>
        /// 创建拿铁咖啡（浓缩咖啡 + 牛奶 + 泡沫）
        /// </summary>
        public static ICoffee CreateLatte()
        {
            var espresso = new Espresso();
            var withMilk = new MilkDecorator(espresso);
            var withFoam = new FoamDecorator(withMilk);
            return withFoam;
        }
    }
}
