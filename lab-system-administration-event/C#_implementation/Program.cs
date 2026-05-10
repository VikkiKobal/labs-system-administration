using System;
using System.Threading;
using System.Globalization;

// C#_implementation of the task, variant 1.1 / 2.7
class Program
{
    static volatile bool stopRequested = false;

    static int[] sharedX = null;
    static int[] sharedY = null;

    static AutoResetEvent event1 = new AutoResetEvent(false);
    static AutoResetEvent event2 = new AutoResetEvent(true);

    static readonly object consoleLock = new object();

    static int paramA, paramB;

    static void Main(string[] args)
    {
        Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;

        Console.WriteLine("Синхронізація потоків через EVENT");
        Console.WriteLine();

        try
        {
            Console.Write("Введіть a (a < 0): ");
            paramA = int.Parse(Console.ReadLine());
            if (paramA >= 0) { Console.WriteLine("Помилка: a має бути < 0."); return; }

            Console.Write("Введіть b (b > a): ");
            paramB = int.Parse(Console.ReadLine());
            if (paramB <= paramA) { Console.WriteLine("Помилка: b > a."); return; }
        }
        catch (FormatException)
        {
            Console.WriteLine("Помилка: введено не ціле число."); return;
        }

        Console.WriteLine("\nНатисніть ESC для зупинки.\n");
        Console.WriteLine(new string('-', 60));

        Thread t1 = new Thread(Thread1Func) { Name = "Generator" };
        Thread t2 = new Thread(Thread2Func) { Name = "Analyzer"  };
        t1.Start();
        t2.Start();

        while (Console.ReadKey(intercept: true).Key != ConsoleKey.Escape) { }

        stopRequested = true;
        event1.Set();
        event2.Set();

        t1.Join();
        t2.Join();

        Console.WriteLine("\n" + new string('-', 60));
        Console.WriteLine("Потоки завершено. Натисніть Enter для виходу.");
        Console.ReadLine();
    }
    static void Thread1Func()
    {
        var rng  = new Random();
        int step = 0;

        while (!stopRequested)
        {
            event2.WaitOne();
            if (stopRequested) break;

            Thread.Sleep(1500); 

            step++;
            int size = rng.Next(10, 21);
            var x = new int[size];
            var y = new int[size];

            for (int i = 0; i < size; i++)
            {
                x[i] = rng.Next(paramA, paramB + 1);
                y[i] = rng.Next(paramA, paramB + 1);
            }

            sharedX = x;
            sharedY = y;

            lock (consoleLock)
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.Write("  x = [");
                foreach (var v in x) Console.Write($" {v,4}");
                Console.WriteLine(" ]");
                Console.Write("  y = [");
                foreach (var v in y) Console.Write($" {v,4}");
                Console.WriteLine(" ]");
                Console.ResetColor();
            }

            event1.Set();
        }
    }

    static void Thread2Func()
    {
        int step = 0;

        while (!stopRequested)
        {
            event1.WaitOne();
            if (stopRequested) break;

            step++;
            int[] x = sharedX;
            int[] y = sharedY;

            int pos = 0, neg = 0;
            foreach (int v in x) { if (v > 0) pos++; else if (v < 0) neg++; }
            foreach (int v in y) { if (v > 0) pos++; else if (v < 0) neg++; }

            bool morePositive = pos > neg;

            lock (consoleLock)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"  Додатних : {pos}");
                Console.WriteLine($"  Від'ємних: {neg}");
                Console.ForegroundColor = morePositive ? ConsoleColor.Green : ConsoleColor.Red;
                Console.WriteLine(morePositive
                    ? $"  Висновок : додатних БІЛЬШЕ ({pos} > {neg})"
                    : $"  Висновок : додатних НЕ БІЛЬШЕ ({pos} <= {neg})");
                Console.ResetColor();
                Console.WriteLine(new string('-', 60));
            }

            event2.Set();
        }
    }
}
