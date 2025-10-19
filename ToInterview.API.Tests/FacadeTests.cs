using ToInterview.API.Patterns.Facade;
using Xunit;

namespace ToInterview.API.Tests
{
    public class FacadeTests
    {
        [Fact]
        public void ComputerFacade_ShouldStartComputer()
        {
            // Arrange
            var computer = new ComputerFacade();

            // Act & Assert
            // 验证可以调用启动方法而不抛出异常
            computer.StartComputer();
            Assert.NotNull(computer);
        }

        [Fact]
        public void ComputerFacade_ShouldShutdownComputer()
        {
            // Arrange
            var computer = new ComputerFacade();

            // Act & Assert
            // 验证可以调用关闭方法而不抛出异常
            computer.ShutdownComputer();
            Assert.NotNull(computer);
        }

        [Fact]
        public void ComputerFacade_ShouldRunProgram()
        {
            // Arrange
            var computer = new ComputerFacade();
            var programName = "测试程序";

            // Act & Assert
            // 验证可以调用运行程序方法而不抛出异常
            computer.RunProgram(programName);
            Assert.NotNull(computer);
        }

        [Fact]
        public void ComputerFacade_ShouldBeInstantiable()
        {
            // Arrange & Act
            var computer = new ComputerFacade();

            // Assert
            Assert.NotNull(computer);
        }

        [Fact]
        public void SubsystemComponents_ShouldBeInstantiable()
        {
            // Arrange & Act
            var cpu = new CPU();
            var memory = new Memory();
            var hardDrive = new HardDrive();
            var graphicsCard = new GraphicsCard();

            // Assert
            Assert.NotNull(cpu);
            Assert.NotNull(memory);
            Assert.NotNull(hardDrive);
            Assert.NotNull(graphicsCard);
        }

        [Fact]
        public void CPU_ShouldHaveStartAndShutdownMethods()
        {
            // Arrange
            var cpu = new CPU();

            // Act & Assert
            // 验证方法存在且可调用
            cpu.Start();
            cpu.Shutdown();
            Assert.NotNull(cpu);
        }

        [Fact]
        public void Memory_ShouldHaveLoadAndUnloadMethods()
        {
            // Arrange
            var memory = new Memory();

            // Act & Assert
            // 验证方法存在且可调用
            memory.Load();
            memory.Unload();
            Assert.NotNull(memory);
        }

        [Fact]
        public void HardDrive_ShouldHaveReadAndWriteMethods()
        {
            // Arrange
            var hardDrive = new HardDrive();

            // Act & Assert
            // 验证方法存在且可调用
            hardDrive.Read();
            hardDrive.Write();
            Assert.NotNull(hardDrive);
        }

        [Fact]
        public void GraphicsCard_ShouldHaveInitializeAndRenderMethods()
        {
            // Arrange
            var graphicsCard = new GraphicsCard();

            // Act & Assert
            // 验证方法存在且可调用
            graphicsCard.Initialize();
            graphicsCard.Render();
            Assert.NotNull(graphicsCard);
        }
    }
}
