namespace ToInterview.API.Patterns.Factory;

/// <summary>
/// 工厂模式对比演示 - 使用相同的IAnimal例子
/// </summary>
public class FactoryComparisonDemo
{
    public static void RunComparison()
    {
        Console.WriteLine("=== 工厂模式对比演示 ===\n");

        // 1. 简单工厂模式
        Console.WriteLine("1. 简单工厂模式：");
        Console.WriteLine("   - 一个工厂类创建所有产品");
        Console.WriteLine("   - 通过参数决定创建哪种产品");
        
        var dog1 = SimpleAnimalFactory.CreateAnimal("dog");
        var cat1 = SimpleAnimalFactory.CreateAnimal("cat");
        
        Console.WriteLine("   创建结果：");
        dog1.MakeSound();
        cat1.MakeSound();

        Console.WriteLine("\n" + new string('-', 40) + "\n");

        // 2. 工厂方法模式
        Console.WriteLine("2. 工厂方法模式：");
        Console.WriteLine("   - 每个具体工厂负责创建一种产品");
        Console.WriteLine("   - 通过不同的工厂类创建产品");
        
        AnimalFactory dogFactory = new DogFactory();
        AnimalFactory catFactory = new CatFactory();
        
        Console.WriteLine("   创建结果：");
        dogFactory.ShowAnimal();
        catFactory.ShowAnimal();

        Console.WriteLine("\n=== 关键区别总结 ===");
        Console.WriteLine("简单工厂：SimpleAnimalFactory.CreateAnimal(\"dog\")");
        Console.WriteLine("工厂方法：new DogFactory().CreateAnimal()");
        Console.WriteLine("\n简单工厂：一个工厂管所有产品");
        Console.WriteLine("工厂方法：每个工厂管一种产品");
    }
}
