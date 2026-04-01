using System;
class PrimeNumberProgram
{
    static void Main()
    {
        Console.Write("请输入下限：");
        int lower = int.Parse(Console.ReadLine());

        Console.Write("请输入上限：");
        int upper = int.Parse(Console.ReadLine());

        Console.WriteLine($"\n{lower} 到 {upper} 之间的所有素数（10个/行）：");

        int count = 0;

        for (int num = lower; num <= upper; num++)
        {
            if (IsPrime(num))
            {
                Console.Write(num + "\t"); 
                count++;

         
                if (count % 10 == 0)
                {
                    Console.WriteLine();
                }
            }
        }
        if (count % 10 != 0)
        {
            Console.WriteLine();
        }

        Console.WriteLine($"\n共找到 {count} 个素数");
        Console.WriteLine("\n程序执行完毕，按任意键退出...");
        Console.ReadKey(); // 这行是关键：它会等待你按下键盘上的任何一个键
    }

    static bool IsPrime(int n)
    {
        if (n < 2) return false;

        for (int i = 2; i * i <= n; i++)
        {
            if (n % i == 0)
                return false;
        }
        return true;
    }
}