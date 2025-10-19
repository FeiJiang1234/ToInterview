namespace ToInterview.API.Patterns.SimpleProxy;

public interface IImage
{
    void Display();
}

public class RealImage : IImage
{
    private string _filename;

    public RealImage(string filename)
    {
        _filename = filename;
        LoadFromDisk(); // 模拟从磁盘加载图片（耗时操作）
    }

    private void LoadFromDisk()
    {
        Console.WriteLine($"正在从磁盘加载图片: {_filename}");
        Thread.Sleep(2000); // 模拟加载时间
        Console.WriteLine($"图片 {_filename} 加载完成");
    }

    public void Display()
    {
        Console.WriteLine($"显示图片: {_filename}");
    }
}

public class ImageProxy : IImage
{
    private RealImage? _realImage; // 延迟创建真实对象
    private string _filename;

    public ImageProxy(string filename)
    {
        _filename = filename;
        Console.WriteLine($"创建图片代理: {_filename}");
    }

    public void Display()
    {
        // 只有在需要显示时才创建真实对象（延迟加载）
        if (_realImage == null)
        {
            _realImage = new RealImage(_filename);
        }
        _realImage.Display();
    }
}

// 4. 客户端使用
public class SimpleProxyDemo
{
    public static void RunDemo()
    {
        Console.WriteLine("=== 简单代理模式演示 ===\n");

        // 创建代理对象（不会立即加载图片）
        Console.WriteLine("1. 创建图片代理...");
        var imageProxy = new ImageProxy("photo.jpg");
        
        Console.WriteLine("\n2. 代理已创建，但图片还未加载");
        Console.WriteLine("3. 现在调用显示方法...\n");
        
        // 第一次调用 - 会触发真实对象的创建和加载
        imageProxy.Display();
        
        Console.WriteLine("\n4. 再次调用显示方法...");
        // 第二次调用 - 直接使用已创建的真实对象
        imageProxy.Display();
    }
}
