namespace ToInterview.API.Patterns.Prototype;

public interface ICloneable<T>
{
    T Clone();
}

public class Person : ICloneable<Person>
{
    public string Name { get; set; } = string.Empty;
    public List<string> Hobbies { get; set; } = new();

    public Person(string name)
    {
        Name = name;
    }

    public Person Clone()
    {
        var clonedPerson = new Person(Name);
        clonedPerson.Hobbies = new List<string>(Hobbies);
        
        return clonedPerson;
    }

    public void AddHobby(string hobby)
    {
        Hobbies.Add(hobby);
    }

    public void DisplayInfo()
    {
        Console.WriteLine($"姓名: {Name}");
        Console.WriteLine($"爱好: {string.Join(", ", Hobbies)}");
    }
}

public class PersonPrototypeManager
{
    private readonly Dictionary<string, Person> _prototypes = new();

    public void RegisterPrototype(string key, Person person)
    {
        _prototypes[key] = person;
        Console.WriteLine($"注册原型: {key}");
    }

    public Person CreatePerson(string key)
    {
        if (_prototypes.ContainsKey(key))
        {
            return _prototypes[key].Clone();
        }
        throw new ArgumentException($"未找到原型: {key}");
    }

    public void ListPrototypes()
    {
        Console.WriteLine("已注册的原型:");
        foreach (var kvp in _prototypes)
        {
            Console.WriteLine($"- {kvp.Key}: {kvp.Value.Name}");
        }
    }
}