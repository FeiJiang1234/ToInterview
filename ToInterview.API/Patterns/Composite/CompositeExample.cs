namespace ToInterview.API.Patterns.Composite;

public abstract class FileSystemComponent
{
    public string Name { get; set; } = string.Empty;

    public abstract void Display(int depth = 0);
    public abstract int GetSize();
    public virtual void Add(FileSystemComponent component)
    {
        throw new NotSupportedException("叶子节点不支持添加操作");
    }
    public virtual void Remove(FileSystemComponent component)
    {
        throw new NotSupportedException("叶子节点不支持删除操作");
    }
}

public class File : FileSystemComponent
{
    private int _size;

    public File(string name, int size)
    {
        Name = name;
        _size = size;
    }

    public override void Display(int depth = 0)
    {
        var indent = new string(' ', depth * 2);
        Console.WriteLine($"{indent}📄 {Name} ({_size} bytes)");
    }

    public override int GetSize()
    {
        return _size;
    }
}

public class Folder : FileSystemComponent
{
    private List<FileSystemComponent> _children = new();

    public Folder(string name)
    {
        Name = name;
    }

    public override void Display(int depth = 0)
    {
        var indent = new string(' ', depth * 2);
        Console.WriteLine($"{indent}📁 {Name}/");
        
        foreach (var child in _children)
        {
            child.Display(depth + 1);
        }
    }

    public override int GetSize()
    {
        int totalSize = 0;
        foreach (var child in _children)
        {
            totalSize += child.GetSize();
        }
        return totalSize;
    }

    public override void Add(FileSystemComponent component)
    {
        _children.Add(component);
    }

    public override void Remove(FileSystemComponent component)
    {
        _children.Remove(component);
    }

    public List<FileSystemComponent> GetChildren()
    {
        return new List<FileSystemComponent>(_children);
    }
}

public class FileSystemManager
{
    private FileSystemComponent _root;

    public FileSystemManager(FileSystemComponent root)
    {
        _root = root;
    }

    public void DisplayFileSystem()
    {
        Console.WriteLine("=== 文件系统结构 ===");
        _root.Display();
        Console.WriteLine($"\n总大小: {_root.GetSize()} bytes");
        Console.WriteLine();
    }

    public void AddComponent(FileSystemComponent parent, FileSystemComponent child)
    {
        try
        {
            parent.Add(child);
            Console.WriteLine($"已添加 {child.Name} 到 {parent.Name}");
        }
        catch (NotSupportedException ex)
        {
            Console.WriteLine($"错误: {ex.Message}");
        }
    }

    public void RemoveComponent(FileSystemComponent parent, FileSystemComponent child)
    {
        try
        {
            parent.Remove(child);
            Console.WriteLine($"已从 {parent.Name} 中删除 {child.Name}");
        }
        catch (NotSupportedException ex)
        {
            Console.WriteLine($"错误: {ex.Message}");
        }
    }
}
