namespace ToInterview.API.Patterns.Memento;

public class TextMemento
{
    public string Content { get; }
    public DateTime Timestamp { get; }

    public TextMemento(string content)
    {
        Content = content;
        Timestamp = DateTime.Now;
    }
}

public class TextEditor
{
    private string _content = string.Empty;

    public string Content
    {
        get => _content;
        set
        {
            _content = value;
            Console.WriteLine($"内容已更新: {_content}");
        }
    }

    public TextMemento CreateMemento()
    {
        Console.WriteLine($"创建备忘录: {_content}");
        return new TextMemento(_content);
    }

    public void RestoreFromMemento(TextMemento memento)
    {
        _content = memento.Content;
        Console.WriteLine($"从备忘录恢复: {_content} (保存时间: {memento.Timestamp:HH:mm:ss})");
    }

    public void DisplayContent()
    {
        Console.WriteLine($"当前内容: {_content}");
    }
}

// 3. 管理者类 - 管理备忘录
public class Caretaker
{
    private List<TextMemento> _mementos = new();
    private int _currentIndex = -1;

    public void SaveMemento(TextMemento memento)
    {
        // 删除当前位置之后的所有备忘录（撤销后重新编辑的情况）
        if (_currentIndex < _mementos.Count - 1)
        {
            _mementos.RemoveRange(_currentIndex + 1, _mementos.Count - _currentIndex - 1);
        }

        _mementos.Add(memento);
        _currentIndex = _mementos.Count - 1;
        Console.WriteLine($"备忘录已保存 (总数: {_mementos.Count})");
    }

    public TextMemento? GetMemento(int index)
    {
        if (index >= 0 && index < _mementos.Count)
        {
            _currentIndex = index;
            return _mementos[index];
        }
        return null;
    }

    public TextMemento? Undo()
    {
        if (_currentIndex > 0)
        {
            _currentIndex--;
            Console.WriteLine($"撤销到第 {_currentIndex + 1} 个版本");
            return _mementos[_currentIndex];
        }
        Console.WriteLine("无法撤销，已是最早版本");
        return null;
    }

    public TextMemento? Redo()
    {
        if (_currentIndex < _mementos.Count - 1)
        {
            _currentIndex++;
            Console.WriteLine($"重做到第 {_currentIndex + 1} 个版本");
            return _mementos[_currentIndex];
        }
        Console.WriteLine("无法重做，已是最新版本");
        return null;
    }

    public void ListMementos()
    {
        Console.WriteLine("=== 备忘录列表 ===");
        for (int i = 0; i < _mementos.Count; i++)
        {
            var memento = _mementos[i];
            var marker = i == _currentIndex ? " ← 当前" : "";
            Console.WriteLine($"{i + 1}. {memento.Content} ({memento.Timestamp:HH:mm:ss}){marker}");
        }
        Console.WriteLine();
    }

    public int GetMementosCount()
    {
        return _mementos.Count;
    }
}

// 4. 演示类
public class MementoDemo
{
    public static void RunDemo()
    {
        Console.WriteLine("=== 备忘录模式演示 ===\n");

        var editor = new TextEditor();
        var caretaker = new Caretaker();

        // 编辑文本并保存状态
        editor.Content = "Hello";
        caretaker.SaveMemento(editor.CreateMemento());

        editor.Content = "Hello World";
        caretaker.SaveMemento(editor.CreateMemento());

        editor.Content = "Hello World!";
        caretaker.SaveMemento(editor.CreateMemento());

        editor.Content = "Hello World! How are you?";
        caretaker.SaveMemento(editor.CreateMemento());

        // 显示所有备忘录
        caretaker.ListMementos();

        // 撤销操作
        Console.WriteLine("=== 撤销操作 ===");
        var undoMemento = caretaker.Undo();
        if (undoMemento != null)
        {
            editor.RestoreFromMemento(undoMemento);
        }

        // 再次撤销
        undoMemento = caretaker.Undo();
        if (undoMemento != null)
        {
            editor.RestoreFromMemento(undoMemento);
        }

        // 重做操作
        Console.WriteLine("\n=== 重做操作 ===");
        var redoMemento = caretaker.Redo();
        if (redoMemento != null)
        {
            editor.RestoreFromMemento(redoMemento);
        }

        // 显示最终状态
        Console.WriteLine("\n=== 最终状态 ===");
        editor.DisplayContent();
        caretaker.ListMementos();
    }
}
