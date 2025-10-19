namespace ToInterview.API.Patterns.Factory;

// 工厂方法模式 - 使用相同的IAnimal接口

public abstract class AnimalFactory
{
    public abstract IAnimal CreateAnimal();

    public void ShowAnimal()
    {
        Console.WriteLine("展示动物...");
        var animal = CreateAnimal(); // 调用工厂方法
        animal.MakeSound();
        Console.WriteLine("展示完成！\n");
    }
}

public class DogFactory : AnimalFactory
{
    public override IAnimal CreateAnimal()
    {
        Console.WriteLine("正在创建狗狗...");
        return new Dog();
    }
}

public class CatFactory : AnimalFactory
{
    public override IAnimal CreateAnimal()
    {
        Console.WriteLine("正在创建猫咪...");
        return new Cat();
    }
}

// 客户端使用
public class FactoryMethodDemo
{
    public static void RunDemo()
    {
        Console.WriteLine("=== 工厂方法模式演示 ===\n");

        AnimalFactory dogFactory = new DogFactory();
        AnimalFactory catFactory = new CatFactory();

        dogFactory.ShowAnimal();
        catFactory.ShowAnimal();
    }
}
