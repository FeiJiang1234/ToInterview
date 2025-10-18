using ToInterview.API.Delegates;
using Xunit;

namespace ToInterview.API.Tests
{
    public class DelegateTests
    {
        [Fact]
        public void TestBasicDelegates_Add_ShouldReturnCorrectResult()
        {
            // Arrange
            MathOperation add = DelegateExamples.Add;
            
            // Act
            //int result = add(10, 20);
            int result = add.Invoke(10, 20);

            // Assert
            Assert.Equal(30, result);
        }

        [Fact]
        public void TestGenericDelegates_IntMax_ShouldReturnCorrectResult()
        {
            // Arrange
            GenericOperation<int> intMax = DelegateExamples.Max;
            
            // Act
            int result = intMax(15, 25);
            
            // Assert
            Assert.Equal(25, result);
        }

        [Fact]
        public void TestGenericDelegates_StringMax_ShouldReturnCorrectResult()
        {
            // Arrange
            GenericOperation<string> stringMax = DelegateExamples.Max;
            
            // Act
            string result = stringMax("apple", "banana");
            
            // Assert
            Assert.Equal("banana", result);
        }

        [Fact]
        public void TestMulticastDelegates_AddMultipleHandlers_ShouldExecuteAll()
        {
            // Arrange
            var output = new StringWriter();
            Console.SetOut(output);
            
            NotificationHandler notification = DelegateExamples.LogToConsole;
            notification += DelegateExamples.LogToFile;
            notification += DelegateExamples.SendEmail;
            notification += DelegateExamples.LogToConsole;
            notification -= DelegateExamples.LogToConsole;
            
            // Act
            notification("测试消息");
            
            // Assert
            string result = output.ToString();
            Assert.Contains("[Console]", result);
            Assert.Contains("[File]", result);
            Assert.Contains("[Email]", result);
            Assert.Contains("测试消息", result);
            
            // Reset console output
            Console.SetOut(new StreamWriter(Console.OpenStandardOutput()) { AutoFlush = true });
        }

        [Fact]
        public void TestFuncAndAction_FuncAdd_ShouldReturnCorrectResult()
        {
            // Arrange
            Func<int, int, int> addFunc = (x, y) => x + y;
            
            // Act
            int result = addFunc(5, 3);
            
            // Assert
            Assert.Equal(8, result);
        }

        [Fact]
        public void TestFuncAndAction_FuncToUpper_ShouldReturnCorrectResult()
        {
            // Arrange
            Func<string, string> upperFunc = s => s.ToUpper();
            
            // Act
            string result = upperFunc("hello");
            
            // Assert
            Assert.Equal("HELLO", result);
        }

        [Fact]
        public void TestFuncAndAction_ActionPrint_ShouldExecuteCorrectly()
        {
            // Arrange
            var output = new StringWriter();
            Console.SetOut(output);
            Action<string> printAction = s => Console.WriteLine($"Action输出: {s}");
            
            // Act
            printAction("Hello World");
            
            // Assert
            string result = output.ToString();
            Assert.Contains("Action输出: Hello World", result);
            
            // Reset console output
            Console.SetOut(new StreamWriter(Console.OpenStandardOutput()) { AutoFlush = true });
        }

        [Fact]
        public void TestPredicate_IsGreaterThan5_ShouldReturnCorrectResults()
        {
            // Arrange
            int[] numbers = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
            Predicate<int> isGreaterThan5 = x => x > 5;
            
            // Act
            var largeNumbers = Array.FindAll(numbers, isGreaterThan5);
            
            // Assert
            Assert.Equal(new int[] { 6, 7, 8, 9, 10 }, largeNumbers);
        }

        [Fact]
        public void TestCallbackPattern_SquareTransform_ShouldProcessCorrectly()
        {
            // Arrange
            int[] numbers = { 1, 2, 3, 4, 5 };
            var output = new StringWriter();
            Console.SetOut(output);
            
            // Act
            DelegateExamples.ProcessWithCallback(
                numbers,
                x => x * x, // 平方处理
                result => Console.Write($"{result} ")
            );
            
            // Assert
            string result = output.ToString();
            Assert.Contains("1 4 9 16 25", result);
            
            // Reset console output
            Console.SetOut(new StreamWriter(Console.OpenStandardOutput()) { AutoFlush = true });
        }
    }
}
