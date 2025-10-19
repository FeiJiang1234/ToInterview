namespace ToInterview.API.Patterns.Factory;

public interface IAnimal
{
    void MakeSound();
}

public class Dog : IAnimal
{
    public void MakeSound()
    {
        Console.WriteLine("汪汪汪！");
    }
}

public class Cat : IAnimal
{
    public void MakeSound()
    {
        Console.WriteLine("喵喵喵！");
    }
}

public class SimpleAnimalFactory
{
    public static IAnimal CreateAnimal(string type)
    {
        return type.ToLower() switch
        {
            "dog" => new Dog(),
            "cat" => new Cat(),
            _ => throw new ArgumentException($"不支持的动物类型: {type}")
        };
    }
}