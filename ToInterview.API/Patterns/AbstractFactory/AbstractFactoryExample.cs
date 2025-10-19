namespace ToInterview.API.Patterns.AbstractFactory;

public interface IAnimal
{
    void MakeSound();
}

public interface IAnimalFood
{
    void Feed();
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

public class DogFood : IAnimalFood
{
    public void Feed()
    {
        Console.WriteLine("喂狗粮");
    }
}

public class CatFood : IAnimalFood
{
    public void Feed()
    {
        Console.WriteLine("喂猫粮");
    }
}

public class Lion : IAnimal
{
    public void MakeSound()
    {
        Console.WriteLine("吼吼吼！");
    }
}

public class Tiger : IAnimal
{
    public void MakeSound()
    {
        Console.WriteLine("嗷嗷嗷！");
    }
}

public class Meat : IAnimalFood
{
    public void Feed()
    {
        Console.WriteLine("喂生肉");
    }
}

public interface IAnimalFactory
{
    IAnimal CreateAnimal();
    IAnimalFood CreateFood();
}

public class PetFactory : IAnimalFactory
{
    public IAnimal CreateAnimal()
    {
        return new Dog();
    }

    public IAnimalFood CreateFood()
    {
        return new DogFood();
    }
}

public class WildAnimalFactory : IAnimalFactory
{
    public IAnimal CreateAnimal()
    {
        return new Lion();
    }

    public IAnimalFood CreateFood()
    {
        return new Meat();
    }
}

// 7. 客户端类
public class AnimalCare
{
    private IAnimal _animal;
    private IAnimalFood _food;

    public AnimalCare(IAnimalFactory factory)
    {
        _animal = factory.CreateAnimal();
        _food = factory.CreateFood();
    }

    public void CareForAnimal()
    {
        Console.WriteLine("=== 照顾动物 ===");
        _animal.MakeSound();
        _food.Feed();
        Console.WriteLine();
    }
}

// 8. 演示类
public class AbstractFactoryDemo
{
    public static void RunDemo()
    {
        Console.WriteLine("=== 抽象工厂模式演示 ===\n");

        // 创建宠物系列
        Console.WriteLine("创建宠物系列:");
        var petFactory = new PetFactory();
        var petCare = new AnimalCare(petFactory);
        petCare.CareForAnimal();

        // 创建野生动物系列
        Console.WriteLine("创建野生动物系列:");
        var wildFactory = new WildAnimalFactory();
        var wildCare = new AnimalCare(wildFactory);
        wildCare.CareForAnimal();
    }
}
