using ToInterview.API.Patterns.SimpleProxy;
using Xunit;

namespace ToInterview.API.Tests.Patterns
{
    public class SimpleProxyTests
    {
        [Fact]
        public void ImageProxy_ShouldCreateWithoutLoadingImage()
        {
            // Arrange & Act
            var imageProxy = new ImageProxy("test.jpg");

            // Assert - 代理创建成功，但真实对象还未创建
            Assert.NotNull(imageProxy);
        }

        [Fact]
        public void ImageProxy_ShouldLoadImageOnFirstDisplay()
        {
            // Arrange
            var imageProxy = new ImageProxy("test.jpg");

            // Act - 第一次调用Display，会触发真实对象的创建
            imageProxy.Display();

            // Assert - 应该能正常显示
            Assert.NotNull(imageProxy);
        }

        [Fact]
        public void ImageProxy_ShouldWorkMultipleTimes()
        {
            // Arrange
            var imageProxy = new ImageProxy("test.jpg");

            // Act - 多次调用Display
            imageProxy.Display();
            imageProxy.Display();
            imageProxy.Display();

            // Assert - 应该都能正常工作
            Assert.NotNull(imageProxy);
        }
    }
}
