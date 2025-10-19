using ToInterview.API.Patterns.AbstractFactory;
using Xunit;

namespace ToInterview.API.Tests
{
    public class AbstractFactoryTests
    {
        [Fact]
        public void ProductFamily_ShouldBeConsistent()
        {
            // Arrange
            var petFactory = new PetFactory();
            var wildFactory = new WildAnimalFactory();

            // Act
            var petAnimal = petFactory.CreateAnimal();
            var petFood = petFactory.CreateFood();
            var wildAnimal = wildFactory.CreateAnimal();
            var wildFood = wildFactory.CreateFood();

            // Assert
            // 验证同一工厂创建的产品属于同一系列
            Assert.IsType<Dog>(petAnimal);
            Assert.IsType<DogFood>(petFood);
            Assert.IsType<Lion>(wildAnimal);
            Assert.IsType<Meat>(wildFood);
        }
    }
}
