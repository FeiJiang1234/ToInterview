namespace ToInterview.API.Patterns.Builder;

public class Computer
{
    public string CPU { get; set; } = string.Empty;
    public string Memory { get; set; } = string.Empty;

    public void DisplaySpecs()
    {
        Console.WriteLine("=== 电脑配置 ===");
        Console.WriteLine($"CPU: {CPU}");
        Console.WriteLine($"内存: {Memory}");
        Console.WriteLine();
    }
}

public abstract class ComputerBuilder
{
    protected Computer _computer = new();

    public abstract void BuildCPU();
    public abstract void BuildMemory();

    public Computer GetComputer()
    {
        return _computer;
    }
}

public class GamingComputerBuilder : ComputerBuilder
{
    public override void BuildCPU()
    {
        _computer.CPU = "Intel i9-13900K";
        Console.WriteLine("安装游戏级CPU: Intel i9-13900K");
    }

    public override void BuildMemory()
    {
        _computer.Memory = "32GB DDR5";
        Console.WriteLine("安装游戏级内存: 32GB DDR5");
    }
}

// 4. 具体建造者 - 办公电脑
public class OfficeComputerBuilder : ComputerBuilder
{
    public override void BuildCPU()
    {
        _computer.CPU = "Intel i5-13400";
        Console.WriteLine("安装办公级CPU: Intel i5-13400");
    }

    public override void BuildMemory()
    {
        _computer.Memory = "16GB DDR4";
        Console.WriteLine("安装办公级内存: 16GB DDR4");
    }
}

// 5. 指挥者 - 控制构建过程
public class ComputerDirector
{
    private ComputerBuilder _builder;

    public void SetBuilder(ComputerBuilder builder)
    {
        _builder = builder;
    }

    public Computer BuildComputer()
    {
        Console.WriteLine("开始构建电脑...");
        
        _builder.BuildCPU();
        _builder.BuildMemory();
        
        Console.WriteLine("电脑构建完成！\n");
        
        return _builder.GetComputer();
    }
}