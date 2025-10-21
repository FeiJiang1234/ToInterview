namespace ToInterview.API.IoC;

public class SimpleIoCContainer
{
    private readonly Dictionary<Type, Type> _registrations = new();
    private readonly Dictionary<Type, object> _instances = new();

    public void Register<TInterface, TImplementation>() where TImplementation : class, TInterface
    {
        _registrations[typeof(TInterface)] = typeof(TImplementation);
    }

    public void RegisterSingleton<TInterface>(TInterface instance)
    {
        _instances[typeof(TInterface)] = instance;
    }

    public TInterface Resolve<TInterface>()
    {
        return (TInterface)Resolve(typeof(TInterface));
    }

    public object Resolve(Type type)
    {
        if (_instances.ContainsKey(type))
        {
            return _instances[type];
        }

        if (!_registrations.ContainsKey(type))
        {
            throw new InvalidOperationException($"类型 {type.Name} 未注册");
        }

        var implementationType = _registrations[type];
        return CreateInstance(implementationType);
    }

    private object CreateInstance(Type type)
    {
        var constructors = type.GetConstructors();
        
        var constructor = constructors.OrderByDescending(c => c.GetParameters().Length).First();
        var parameters = constructor.GetParameters();
        
        var args = new object[parameters.Length];
        
        for (int i = 0; i < parameters.Length; i++)
        {
            var paramType = parameters[i].ParameterType;
            args[i] = Resolve(paramType);
        }
        
        return Activator.CreateInstance(type, args);
    }
}

public class IoCContainerExample
{
    public static void DemonstrateIoC()
    {
        Console.WriteLine("=== IoC容器演示 ===");
        
        // 创建容器
        var container = new SimpleIoCContainer();
        
        // 注册服务
        container.Register<IEmailService, EmailService>();
        container.Register<IDatabaseService, DatabaseService>();
        container.RegisterSingleton<ILogger>(new ConsoleLogger());
        
        // 注册业务服务
        container.Register<IOrderService, OrderService>();
        
        // 解析服务
        var orderService = container.Resolve<IOrderService>();
        
        // 使用服务
        orderService.ProcessOrder("订单123", "客户A");
        
        Console.WriteLine("\n=== 依赖注入演示 ===");
        
        // 演示不同的日志实现
        var fileLogger = new FileLogger();
        container.RegisterSingleton<ILogger>(fileLogger);
        
        var orderServiceWithFileLog = container.Resolve<IOrderService>();
        orderServiceWithFileLog.ProcessOrder("订单456", "客户B");
    }
}

// 业务服务接口
public interface IOrderService
{
    void ProcessOrder(string orderId, string customerName);
}

// 业务服务实现
public class OrderService : IOrderService
{
    private readonly IEmailService _emailService;
    private readonly IDatabaseService _databaseService;
    private readonly ILogger _logger;

    public OrderService(IEmailService emailService, IDatabaseService databaseService, ILogger logger)
    {
        _emailService = emailService;
        _databaseService = databaseService;
        _logger = logger;
    }

    public void ProcessOrder(string orderId, string customerName)
    {
        _logger.Log($"开始处理订单: {orderId}");
        
        try
        {
            _databaseService.SaveOrder($"订单ID: {orderId}, 客户: {customerName}");
            _emailService.SendEmail(customerName, $"您的订单 {orderId} 已确认");
            _logger.Log($"订单 {orderId} 处理成功");
        }
        catch (Exception ex)
        {
            _logger.Log($"订单 {orderId} 处理失败: {ex.Message}");
        }
    }
}
