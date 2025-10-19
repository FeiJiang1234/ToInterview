# 工厂模式对比 - 使用Animal例子

## 🎯 **三种工厂模式对比**

### **1. 简单工厂模式 (Simple Factory)**

#### **特点**
- 一个工厂类创建所有产品
- 通过参数决定创建哪种产品

#### **代码示例**
```csharp
public class SimpleAnimalFactory
{
    public static IAnimal CreateAnimal(string type)
    {
        return type switch
        {
            "dog" => new Dog(),
            "cat" => new Cat(),
            _ => throw new ArgumentException()
        };
    }
}

// 使用
var dog = SimpleAnimalFactory.CreateAnimal("dog");
var cat = SimpleAnimalFactory.CreateAnimal("cat");
```

#### **适用场景**
- 产品类型固定，很少变化
- 创建过程简单

---

### **2. 工厂方法模式 (Factory Method)**

#### **特点**
- 每个具体工厂负责创建一种产品
- 通过不同的工厂类创建产品

#### **代码示例**
```csharp
public abstract class AnimalFactory
{
    public abstract IAnimal CreateAnimal();
}

public class DogFactory : AnimalFactory
{
    public override IAnimal CreateAnimal()
    {
        return new Dog();
    }
}

public class CatFactory : AnimalFactory
{
    public override IAnimal CreateAnimal()
    {
        return new Cat();
    }
}

// 使用
AnimalFactory dogFactory = new DogFactory();
var dog = dogFactory.CreateAnimal();
```

#### **适用场景**
- 产品类型经常变化
- 需要灵活扩展

---

### **3. 抽象工厂模式 (Abstract Factory)**

#### **特点**
- 每个工厂创建一系列相关产品
- 确保产品族的一致性

#### **代码示例**
```csharp
public interface IAnimalFactory
{
    IAnimal CreateAnimal();
    IAnimalFood CreateFood();
}

public class PetFactory : IAnimalFactory
{
    public IAnimal CreateAnimal() => new Dog();
    public IAnimalFood CreateFood() => new DogFood();
}

public class WildAnimalFactory : IAnimalFactory
{
    public IAnimal CreateAnimal() => new Lion();
    public IAnimalFood CreateFood() => new Meat();
}

// 使用
IAnimalFactory petFactory = new PetFactory();
var animal = petFactory.CreateAnimal();
var food = petFactory.CreateFood();
```

#### **适用场景**
- 需要创建一系列相关产品
- 产品族需要保持一致

---

## 📊 **详细对比表**

| 方面 | 简单工厂 | 工厂方法 | 抽象工厂 |
|------|----------|----------|----------|
| **工厂数量** | 1个工厂类 | 多个具体工厂类 | 多个具体工厂类 |
| **产品数量** | 多种产品 | 一种产品 | 一系列相关产品 |
| **创建方式** | 静态方法 | 实例方法 | 实例方法 |
| **参数** | 类型参数 | 工厂类 | 工厂类 |
| **扩展性** | 难扩展 | 易扩展 | 易扩展 |
| **复杂度** | 简单 | 中等 | 复杂 |
| **一致性** | 无要求 | 无要求 | 产品族一致 |

---

## 🔄 **创建流程对比**

### **简单工厂**
```
输入: "dog"
↓
SimpleAnimalFactory.CreateAnimal("dog")
↓
输出: Dog对象
```

### **工厂方法**
```
输入: DogFactory
↓
new DogFactory().CreateAnimal()
↓
输出: Dog对象
```

### **抽象工厂**
```
输入: PetFactory
↓
petFactory.CreateAnimal() → Dog对象
petFactory.CreateFood() → DogFood对象
↓
输出: 宠物系列 (Dog + DogFood)
```

---

## 💡 **选择建议**

### **使用简单工厂当：**
- 产品类型固定
- 创建过程简单
- 快速原型开发

### **使用工厂方法当：**
- 产品类型经常变化
- 需要灵活扩展
- 关注创建什么对象

### **使用抽象工厂当：**
- 需要创建一系列相关产品
- 产品族需要保持一致
- 关注产品族的一致性

---

## 🎯 **总结**

- **简单工厂** = 一个工厂管所有产品
- **工厂方法** = 每个工厂管一种产品  
- **抽象工厂** = 每个工厂管一系列相关产品

选择哪种模式取决于您的具体需求和复杂度！
