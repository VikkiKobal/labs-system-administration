using System;
using System.Globalization;
using WinApi;

// WinAPI of the task, variant 1.1 / 2.7
class Program
{
    static volatile bool stopRequested = false;

    static int[] sharedX = null;
    static int[] sharedY = null;

    static IntPtr hEvent1; 
    static IntPtr hEvent2;  

    static readonly object consoleLock = new object();

    static int paramA, paramB;

    static void Main(string[] args)
    {
        System.Threading.Thread.CurrentThread.CurrentCulture =
            CultureInfo.InvariantCulture;

        Console.WriteLine("Синхронізація потоків");
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

        hEvent1 = WinApiFuncs.CreateEvent(IntPtr.Zero, false, false, null);
        hEvent2 = WinApiFuncs.CreateEvent(IntPtr.Zero, false, true,  null);

        Console.WriteLine("\nНатисніть ESC для зупинки.\n");
        Console.WriteLine(new string('-', 60));

        IntPtr ht1 = WinApiFuncs.CreateThread(Thread1Func, null);
        IntPtr ht2 = WinApiFuncs.CreateThread(Thread2Func, null);

        while (Console.ReadKey(intercept: true).Key != ConsoleKey.Escape) { }

        stopRequested = true;
        WinApiFuncs.SetEvent(hEvent1);
        WinApiFuncs.SetEvent(hEvent2);

        WinApiFuncs.WaitForSingleObject(ht1, WinApiFuncs.INFINITE);
        WinApiFuncs.WaitForSingleObject(ht2, WinApiFuncs.INFINITE);

        WinApiFuncs.CloseHandle(ht1);
        WinApiFuncs.CloseHandle(ht2);
        WinApiFuncs.CloseHandle(hEvent1);
        WinApiFuncs.CloseHandle(hEvent2);

        Console.WriteLine("\n" + new string('-', 60));
        Console.WriteLine("Потоки завершено. Натисніть Enter для виходу.");
        Console.ReadLine();
    }

    static void Thread1Func(IntPtr data)
    {
        var rng  = new Random();
        int step = 0;

        while (!stopRequested)
        {
            WinApiFuncs.WaitForSingleObject(hEvent2, WinApiFuncs.INFINITE);
            if (stopRequested) break;

            WinApiFuncs.Sleep(1500);

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
                Console.WriteLine($"\nПотік 1 — масиви розміром {size}:");
                Console.Write("  x = [");
                foreach (var v in x) Console.Write($" {v,4}");
                Console.WriteLine(" ]");
                Console.Write("  y = [");
                foreach (var v in y) Console.Write($" {v,4}");
                Console.WriteLine(" ]");
                Console.ResetColor();
            }

            WinApiFuncs.SetEvent(hEvent1);
        }
    }

    static void Thread2Func(IntPtr data)
    {
        int step = 0;

        while (!stopRequested)
        {
            WinApiFuncs.WaitForSingleObject(hEvent1, WinApiFuncs.INFINITE);
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
                    ? $"  Висновок : додатних більше ({pos} > {neg})"
                    : $"  Висновок : додатних не більше ({pos} <= {neg})");
                Console.ResetColor();
                Console.WriteLine(new string('-', 60));
            }

            WinApiFuncs.SetEvent(hEvent2);
        }
    }
}
