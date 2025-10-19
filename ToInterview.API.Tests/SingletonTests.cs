using ToInterview.API.Patterns.Singleton;
using Xunit;

namespace ToInterview.API.Tests
{
    public class SingletonTests
    {
        [Fact]
        public void DatabaseConnection_ShouldReturnSameInstance()
        {
            // Act
            var instance1 = DatabaseConnection.GetInstance();
            var instance2 = DatabaseConnection.GetInstance();

            // Assert
            Assert.Same(instance1, instance2);
        }

        [Fact]
        public void DatabaseConnection_ShouldHaveCorrectConnectionString()
        {
            // Arrange
            var dbConnection = DatabaseConnection.GetInstance();

            // Act
            var connectionString = dbConnection.GetConnectionString();

            // Assert
            Assert.Equal("Server=localhost;Database=MyDB;Trusted_Connection=true;", connectionString);
        }

        [Fact]
        public void DatabaseConnection_ShouldExecuteQuery()
        {
            // Arrange
            var dbConnection = DatabaseConnection.GetInstance();

            // Act & Assert
            // 验证可以执行查询而不抛出异常
            dbConnection.ExecuteQuery("SELECT * FROM Users");
            Assert.NotNull(dbConnection);
        }

        [Fact]
        public void Logger_ShouldReturnSameInstance()
        {
            // Act
            var instance1 = Logger.GetInstance();
            var instance2 = Logger.GetInstance();

            // Assert
            Assert.Same(instance1, instance2);
        }

        [Fact]
        public void Logger_ShouldLogMessages()
        {
            // Arrange
            var logger = Logger.GetInstance();
            logger.ClearLogs();

            // Act
            logger.Log("测试消息");

            // Assert
            var logs = logger.GetLogs();
            Assert.Single(logs);
            Assert.Contains("测试消息", logs[0]);
        }

        [Fact]
        public void Logger_ShouldClearLogs()
        {
            // Arrange
            var logger = Logger.GetInstance();
            logger.Log("测试消息1");
            logger.Log("测试消息2");

            // Act
            logger.ClearLogs();

            // Assert
            var logs = logger.GetLogs();
            Assert.Empty(logs);
        }

        [Fact]
        public void ConfigurationManager_ShouldReturnSameInstance()
        {
            // Act
            var instance1 = ConfigurationManager.GetInstance();
            var instance2 = ConfigurationManager.GetInstance();

            // Assert
            Assert.Same(instance1, instance2);
        }

        [Fact]
        public void ConfigurationManager_ShouldGetDefaultConfiguration()
        {
            // Arrange
            var configManager = ConfigurationManager.GetInstance();

            // Act
            var appName = configManager.GetConfiguration("AppName");
            var version = configManager.GetConfiguration("Version");
            var environment = configManager.GetConfiguration("Environment");

            // Assert
            Assert.Equal("MyApplication", appName);
            Assert.Equal("1.0.0", version);
            Assert.Equal("Development", environment);
        }

        [Fact]
        public void ConfigurationManager_ShouldSetAndGetConfiguration()
        {
            // Arrange
            var configManager = ConfigurationManager.GetInstance();

            // Act
            configManager.SetConfiguration("TestKey", "TestValue");
            var value = configManager.GetConfiguration("TestKey");

            // Assert
            Assert.Equal("TestValue", value);
        }

        [Fact]
        public void ConfigurationManager_ShouldGetAllConfigurations()
        {
            // Arrange
            var configManager = ConfigurationManager.GetInstance();

            // Act
            var configurations = configManager.GetAllConfigurations();

            // Assert
            Assert.NotNull(configurations);
            Assert.True(configurations.Count >= 3); // 至少有默认配置
            Assert.Contains("AppName", configurations.Keys);
            Assert.Contains("Version", configurations.Keys);
            Assert.Contains("Environment", configurations.Keys);
        }

        [Fact]
        public void ConfigurationManager_ShouldReturnEmptyStringForNonExistentKey()
        {
            // Arrange
            var configManager = ConfigurationManager.GetInstance();

            // Act
            var value = configManager.GetConfiguration("NonExistentKey");

            // Assert
            Assert.Equal(string.Empty, value);
        }

        [Fact]
        public void ApplicationManager_ShouldUseSingletonInstances()
        {
            // Arrange
            var appManager1 = new ApplicationManager();
            var appManager2 = new ApplicationManager();

            // Act & Assert
            // 验证可以初始化应用程序而不抛出异常
            appManager1.InitializeApplication();
            appManager2.InitializeApplication();
            Assert.NotNull(appManager1);
            Assert.NotNull(appManager2);
        }

        [Fact]
        public void ApplicationManager_ShouldProcessData()
        {
            // Arrange
            var appManager = new ApplicationManager();

            // Act & Assert
            // 验证可以处理数据而不抛出异常
            appManager.ProcessData();
            Assert.NotNull(appManager);
        }

        [Fact]
        public void ApplicationManager_ShouldShutdown()
        {
            // Arrange
            var appManager = new ApplicationManager();

            // Act & Assert
            // 验证可以关闭应用程序而不抛出异常
            appManager.ShutdownApplication();
            Assert.NotNull(appManager);
        }

        [Fact]
        public void MultipleThreads_ShouldReturnSameInstance()
        {
            // Arrange
            var instances = new List<DatabaseConnection>();
            var tasks = new List<Task>();

            // Act
            for (int i = 0; i < 10; i++)
            {
                tasks.Add(Task.Run(() =>
                {
                    instances.Add(DatabaseConnection.GetInstance());
                }));
            }

            Task.WaitAll(tasks.ToArray());

            // Assert
            Assert.Equal(10, instances.Count);
            var firstInstance = instances[0];
            foreach (var instance in instances)
            {
                Assert.Same(firstInstance, instance);
            }
        }
    }
}
