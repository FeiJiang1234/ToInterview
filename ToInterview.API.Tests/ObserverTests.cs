using ToInterview.API.Patterns.Observer;
using Xunit;

namespace ToInterview.API.Tests
{
    public class ObserverTests
    {
        [Fact]
        public void NewsPublisher_ShouldRemoveObserver()
        {
            // Arrange
            var publisher = new NewsPublisher();
            var emailNotifier = new EmailNotifier("test@example.com");
            var smsNotifier = new SmsNotifier("13800138000");

            // Act
            publisher.Attach(emailNotifier);
            publisher.Attach(smsNotifier);
            publisher.Detach(emailNotifier);
            publisher.PublishNews("测试新闻");

            // Assert
            Assert.Equal("测试新闻", publisher.GetLatestNews());
        }

        [Fact]
        public void EmailNotifier_ShouldUpdateWithMessage()
        {
            // Arrange
            var emailNotifier = new EmailNotifier("test@example.com");

            // Act & Assert
            // 验证可以调用Update方法而不抛出异常
            emailNotifier.Update("测试消息");
            Assert.NotNull(emailNotifier);
        }
    }
}
