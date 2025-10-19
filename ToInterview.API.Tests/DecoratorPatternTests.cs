using ToInterview.API.Patterns.Decorator;
using Xunit;

namespace ToInterview.API.Tests
{
    public class DecoratorPatternTests
    {
        [Fact]
        public void MultipleDecorators_ShouldChainCorrectly()
        {
            // Arrange
            var coffee = new SimpleCoffee();
            var coffeeWithMilk = new MilkDecorator(coffee);
            var coffeeWithMilkAndSugar = new SugarDecorator(coffeeWithMilk);

            // Act & Assert
            Assert.Equal("简单咖啡, 牛奶, 糖", coffeeWithMilkAndSugar.GetDescription());
            Assert.Equal(2.75m, coffeeWithMilkAndSugar.GetCost());
        }
    }
}
