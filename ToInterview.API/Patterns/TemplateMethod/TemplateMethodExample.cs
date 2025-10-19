namespace ToInterview.API.Patterns.TemplateMethod;

public abstract class BeverageMaker
{
    public void MakeBeverage()
    {
        Console.WriteLine("开始制作饮料...");
        
        BoilWater();        // 固定步骤
        Brew();            // 抽象步骤 - 由子类实现
        PourInCup();       // 固定步骤
        AddCondiments();   // 抽象步骤 - 由子类实现
        
        Console.WriteLine("饮料制作完成！\n");
    }

    private void BoilWater()
    {
        Console.WriteLine("1. 烧开水");
    }

    private void PourInCup()
    {
        Console.WriteLine("3. 倒入杯子");
    }

    // 抽象步骤 - 由子类实现
    protected abstract void Brew();
    protected abstract void AddCondiments();
}

public class CoffeeMaker : BeverageMaker
{
    protected override void Brew()
    {
        Console.WriteLine("2. 冲泡咖啡粉");
    }

    protected override void AddCondiments()
    {
        Console.WriteLine("4. 加入糖和牛奶");
    }
}

public class TeaMaker : BeverageMaker
{
    protected override void Brew()
    {
        Console.WriteLine("2. 浸泡茶叶");
    }

    protected override void AddCondiments()
    {
        Console.WriteLine("4. 加入柠檬");
    }
}
