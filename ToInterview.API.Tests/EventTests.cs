using ToInterview.API.Delegates;
using ToInterview.API.Services;
using Xunit;

namespace ToInterview.API.Tests
{
    public class EventTests
    {
        [Fact]
        public async Task TestEvent_UserLoggedIn()
        {
            // Arrange
            var eventService = new EventService();
            object? capturedSender = null;

            UserEventArgs? capturedArgs = null;
            eventService.UserLoggedIn += (sender, e) =>
            {
                capturedSender = sender;
                capturedArgs = e;
            };

            // Act
            await eventService.SimulateUserLogin("TestUser");

            // Assert
            Assert.NotNull(capturedArgs);
            Assert.Equal("TestUser", capturedArgs.UserName);
            Assert.Equal("Login", capturedArgs.Action);

            Assert.Same(eventService, capturedSender);
        }

        [Fact]
        public void TestEvent_MultipleSubscribers()
        {
            // Arrange
            var eventService = new EventService();
            var handler1Called = false;
            var handler2Called = false;

            // Subscribe multiple handlers
            eventService.UserLoggedIn += (sender, e) => handler1Called = true;
            eventService.UserLoggedIn += (sender, e) => handler2Called = true;

            // Act
            eventService.SimulateUserLogin("TestUser").Wait();

            // Assert
            Assert.True(handler1Called);
            Assert.True(handler2Called);
        }

        [Fact]
        public void TestEvent_UnsubscribeHandler()
        {
            // Arrange
            var eventService = new EventService();
            var handler1Called = false;
            var handler2Called = false;

            // Create handler methods
            UserEventHandler handler1 = (sender, e) => handler1Called = true;
            UserEventHandler handler2 = (sender, e) => handler2Called = true;

            // Subscribe both handlers
            eventService.UserLoggedIn += handler1;
            eventService.UserLoggedIn += handler2;

            // Unsubscribe one handler
            eventService.UserLoggedIn -= handler1;

            // Act
            eventService.SimulateUserLogin("TestUser").Wait();

            // Assert
            Assert.False(handler1Called);
            Assert.True(handler2Called);
        }

        [Fact]
        public void TestEvent_SubscribeHandler()
        {
            Cat cat = new Cat();
            var mouse = new Mouse();

            cat.CatShout += mouse.Run;

            cat.Shout();
        }
    }
}
