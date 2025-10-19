namespace ToInterview.API.Patterns.Facade;

public class CPU
{
    public void Start()
    {
        Console.WriteLine("CPU 启动");
    }
    
    public void Shutdown()
    {
        Console.WriteLine("CPU 关闭");
    }
}

public class Memory
{
    public void Load()
    {
        Console.WriteLine("内存 加载");
    }
    
    public void Unload()
    {
        Console.WriteLine("内存 卸载");
    }
}

public class HardDrive
{
    public void Read()
    {
        Console.WriteLine("硬盘 读取");
    }
    
    public void Write()
    {
        Console.WriteLine("硬盘 写入");
    }
}

// 2. 外观类 - 简化复杂子系统的接口
public class ComputerFacade
{
    private readonly CPU _cpu;
    private readonly Memory _memory;
    private readonly HardDrive _hardDrive;

    public ComputerFacade()
    {
        _cpu = new CPU();
        _memory = new Memory();
        _hardDrive = new HardDrive();
    }

    // 简化的启动方法
    public void StartComputer()
    {
        Console.WriteLine("=== 启动电脑 ===");
        _cpu.Start();
        _memory.Load();
        _hardDrive.Read();
        Console.WriteLine("电脑启动完成！\n");
    }

    // 简化的关闭方法
    public void ShutdownComputer()
    {
        Console.WriteLine("=== 关闭电脑 ===");
        _hardDrive.Write();
        _memory.Unload();
        _cpu.Shutdown();
        Console.WriteLine("电脑关闭完成！\n");
    }

    // 简化的运行程序方法
    public void RunProgram(string programName)
    {
        Console.WriteLine($"=== 运行程序: {programName} ===");
        _cpu.Start();
        _memory.Load();
        _hardDrive.Read();
        Console.WriteLine($"程序 {programName} 运行完成！\n");
    }
}

// 3. 客户端使用
public class FacadeDemo
{
    public static void RunDemo()
    {
        Console.WriteLine("=== 外观模式演示 ===\n");

        // 使用外观类简化操作
        var computer = new ComputerFacade();
        
        // 启动电脑 - 一个方法调用完成复杂操作
        computer.StartComputer();
        
        // 运行程序
        computer.RunProgram("游戏");
        
        // 关闭电脑
        computer.ShutdownComputer();
    }
}
