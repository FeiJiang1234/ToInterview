using ToInterview.API.Patterns.Prototype;
using Xunit;

namespace ToInterview.API.Tests.Patterns
{
    public class PrototypeTests
    {
        [Fact]
        public void Person_Clone_ShouldCreateIndependentCopy()
        {
            // Arrange
            var originalPerson = new Person("张三");
            originalPerson.AddHobby("编程");
            originalPerson.AddHobby("读书");

            // Act
            var clonedPerson = originalPerson.Clone();
            clonedPerson.Name = "李四";
            clonedPerson.AddHobby("游泳");

            // Assert
            Assert.Equal("张三", originalPerson.Name);
            Assert.Equal(2, originalPerson.Hobbies.Count);
            Assert.Contains("编程", originalPerson.Hobbies);
            Assert.DoesNotContain("游泳", originalPerson.Hobbies);

            Assert.Equal("李四", clonedPerson.Name);
            Assert.Equal(3, clonedPerson.Hobbies.Count);
            Assert.Contains("游泳", clonedPerson.Hobbies);
        }

        [Fact]
        public void PersonPrototypeManager_ShouldRegisterAndCreatePrototypes()
        {
            // Arrange
            var manager = new PersonPrototypeManager();
            var prototype = new Person("模板");
            prototype.AddHobby("模板爱好");

            // Act
            manager.RegisterPrototype("test", prototype);
            var clonedPerson = manager.CreatePerson("test");
            clonedPerson.Name = "新名字";

            // Assert
            Assert.Equal("模板", prototype.Name);
            Assert.Equal("新名字", clonedPerson.Name);
        }
    }
}
