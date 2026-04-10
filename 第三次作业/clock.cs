using System;
using System.Threading;

// 闹钟类：发布事件
class Clock
{
    // 1. 定义委托类型
    public delegate void TickEventHandler(object sender, EventArgs e);
    public delegate void AlarmEventHandler(object sender, EventArgs e);

    // 2. 定义事件
    public event TickEventHandler Tick;   // 嘀嗒事件
    public event AlarmEventHandler Alarm; // 响铃事件

    // 目标响铃时间（秒）
    private int _alarmSecond;

    public Clock(int alarmSecond)
    {
        _alarmSecond = alarmSecond;
    }

    // 启动闹钟
    public void Start()
    {
        Console.WriteLine("闹钟开始运行");

        for (int i = 1; ; i++)
        {
            // 每秒触发一次 Tick
            OnTick(EventArgs.Empty);

            // 到达设定秒数触发 Alarm
            if (i == _alarmSecond)
            {
                OnAlarm(EventArgs.Empty);
                break;
            }

            Thread.Sleep(1000); // 延时1秒
        }
    }

    // 触发 Tick 事件
    protected virtual void OnTick(EventArgs e)
    {
        Tick?.Invoke(this, e);
    }

    // 触发 Alarm 事件
    protected virtual void OnAlarm(EventArgs e)
    {
        Alarm?.Invoke(this, e);
    }
}

// 主程序：订阅事件
class Program
{
    static void Main(string[] args)
    {
        Console.Write("请输入响铃时间（秒）：");
        int sec = int.Parse(Console.ReadLine());

        Clock clock = new Clock(sec);

        // 订阅 Tick 事件（嘀嗒）
        clock.Tick += Clock_Tick;

        // 订阅 Alarm 事件（响铃）
        clock.Alarm += Clock_Alarm;

        // 启动闹钟
        clock.Start();
    }

    // Tick 事件处理
    private static void Clock_Tick(object sender, EventArgs e)
    {
        Console.WriteLine("嘀嗒... Tick");
    }

    // Alarm 事件处理
    private static void Clock_Alarm(object sender, EventArgs e)
    {
        Console.WriteLine("\n 响铃！！！Alarm！！！");
    }
}