namespace ToInterview.API.IoC;

/// <summary>
/// 控制反转（Inversion of Control，IoC）概念演示
/// IoC是一种设计原则，不是具体的设计模式
/// 核心思想：将对象的创建和依赖关系的管理从对象内部转移到外部容器
/// </summary>
public class IoCConcepts
{
    /// <summary>
    /// 传统方式：控制正转 - 对象自己创建依赖
    /// 问题：紧耦合，难以测试，违反开闭原则
    /// </summary>
    public class TraditionalApproach
    {
        public void ProcessOrder()
        {
            // 对象自己创建依赖 - 紧耦合
            var emailService = new EmailService();
            var databaseService = new DatabaseService();
            var logger = new FileLogger();
            
            logger.Log("开始处理订单");
            databaseService.SaveOrder("订单数据");
            emailService.SendEmail("客户", "订单确认");
            logger.Log("订单处理完成");
        }
    }

    /// <summary>
    /// IoC方式：控制反转 - 依赖由外部注入
    /// 优势：松耦合，易于测试，符合依赖倒置原则
    /// </summary>
    public class IoCApproach
    {
        private readonly IEmailService _emailService;
        private readonly IDatabaseService _databaseService;
        private readonly ILogger _logger;

        // 构造函数注入 - 依赖由外部提供
        public IoCApproach(IEmailService emailService, IDatabaseService databaseService, ILogger logger)
        {
            _emailService = emailService;
            _databaseService = databaseService;
            _logger = logger;
        }

        public void ProcessOrder()
        {
            _logger.Log("开始处理订单");
            _databaseService.SaveOrder("订单数据");
            _emailService.SendEmail("客户", "订单确认");
            _logger.Log("订单处理完成");
        }
    }
}

// 服务接口定义
public interface IEmailService
{
    void SendEmail(string to, string message);
}

public interface IDatabaseService
{
    void SaveOrder(string orderData);
}

public interface ILogger
{
    void Log(string message);
}

// 具体实现
public class EmailService : IEmailService
{
    public void SendEmail(string to, string message)
    {
        Console.WriteLine($"发送邮件给 {to}: {message}");
    }
}

public class DatabaseService : IDatabaseService
{
    public void SaveOrder(string orderData)
    {
        Console.WriteLine($"保存订单到数据库: {orderData}");
    }
}

public class FileLogger : ILogger
{
    public void Log(string message)
    {
        Console.WriteLine($"[文件日志] {DateTime.Now}: {message}");
    }
}

// 其他实现示例
public class ConsoleLogger : ILogger
{
    public void Log(string message)
    {
        Console.WriteLine($"[控制台日志] {DateTime.Now}: {message}");
    }
}