using System;

// 1. 定义接口：规范形状必须实现的功能
public interface IShape
{
    double GetArea();
    bool IsValid();
}

// 2. 抽象基类：实现通用逻辑，继承接口
public abstract class Shape : IShape
{
    // 抽象方法：子类必须实现
    public abstract double GetArea();
    public abstract bool IsValid();
}

// 3. 长方形类
public class Rectangle : Shape
{
    // 属性：长、宽
    public double Length { get; set; }
    public double Width { get; set; }

    // 构造函数
    public Rectangle(double length, double width)
    {
        Length = length;
        Width = width;
    }

    public override double GetArea()
    {
        return Length * Width;
    }

    public override bool IsValid()
    {
        return Length > 0 && Width > 0;
    }
}

// 4. 正方形类
public class Square : Shape
{
    public double Side { get; set; }

    public Square(double side)
    {
        Side = side;
    }

    public override double GetArea()
    {
        return Side * Side;
    }

    public override bool IsValid()
    {
        return Side > 0;
    }
}

// 5. 圆形类
public class Circle : Shape
{
    public double Radius { get; set; }

    public Circle(double radius)
    {
        Radius = radius;
    }

    public override double GetArea()
    {
        return Math.PI * Radius * Radius;
    }

    public override bool IsValid()
    {
        return Radius > 0;
    }
}

class Program
{
    static void Main(string[] args)
    {
        // 随机数对象
        Random random = new Random();
        // 用于存储10个形状对象
        Shape[] shapes = new Shape[10];

        Console.WriteLine("随机创建10个形状对象");
        for (int i = 0; i < 10; i++)
        {
            // 随机生成 0、1、2 三种形状
            int type = random.Next(0, 3);          
            switch (type)
            {
                case 0:
                    // 长方形：随机长/宽 1~10
                    double l = random.NextDouble() * 9 + 1;
                    double w = random.NextDouble() * 9 + 1;
                    shapes[i] = new Rectangle(l, w);
                    Console.WriteLine($"第{i+1}个：长方形，长={l:F2}，宽={w:F2}");
                    break;
                case 1:
                    // 正方形：随机边长 1~10
                    double s = random.NextDouble() * 9 + 1;
                    shapes[i] = new Square(s);
                    Console.WriteLine($"第{i+1}个：正方形，边长={s:F2}");
                    break;
                case 2:
                    // 圆形：随机半径 1~10
                    double r = random.NextDouble() * 9 + 1;
                    shapes[i] = new Circle(r);
                    Console.WriteLine($"第{i+1}个：圆形，半径={r:F2}");
                    break;
            }
        }
        double totalArea = 0;
        foreach (var shape in shapes)
        {
            if (shape.IsValid())
            {
                totalArea += shape.GetArea();
            }
        }

        Console.WriteLine("\n 计算结果");
        Console.WriteLine($"10个随机形状的总面积为 {totalArea:F2}");
    }
}