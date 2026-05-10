using System;
using System.Threading;

// C# implementation of the task, variant - 7

class Program
{
    static double Sum = 0.0;
    static readonly Mutex mutex = new Mutex();
    static readonly object sync = new object();
    static volatile bool stop = false;
    static double eps;

    struct ThreadData
    {
        public double X;
        public bool IsEven;
    }

    static void ThreadFunc(object obj)
    {
        ThreadData d = (ThreadData)obj;
        double x = d.X;
        long i = d.IsEven ? 0L : 1L;
        double term = 0.0;
        long iter = 0L;

        do
        {
            double sign = ((i + 1) % 2 == 0) ? 1.0 : -1.0;
            term = sign * (x * x + 1.0) / (5.0 * i + 3.0);

            mutex.WaitOne();
            Sum += term;
            mutex.ReleaseMutex();

            iter++;

            if (iter % 10000 == 0)
            {
                lock (sync)
                {
                    Console.WriteLine(
                        $"  [{(d.IsEven ? "Парний " : "Непарний")} потік] " +
                        $"i={i,10}  ітер={iter,8}  Sum={Sum:F12}");
                }
            }

            i += 2;
        }
        while (Math.Abs(term) > eps && !stop);

        lock (sync)
        {
            Console.WriteLine(
                $"  [{(d.IsEven ? "Парний " : "Непарний")} потік] " +
                $"завершено. i={i - 2}");
        }
    }

    static void Main()
    {
        double x;
        do
        {
            Console.Write("Введіть x (x ≠ 0): ");
        }
        while (!double.TryParse(
                   Console.ReadLine()?.Replace(',', '.'),
                   System.Globalization.NumberStyles.Float,
                   System.Globalization.CultureInfo.InvariantCulture,
                   out x) || x == 0);

        double epsInput;
        do
        {
            Console.Write("Введіть ε: ");
        }
        while (!double.TryParse(
                   Console.ReadLine()?.Replace(',', '.'),
                   System.Globalization.NumberStyles.Float,
                   System.Globalization.CultureInfo.InvariantCulture,
                   out epsInput) || epsInput <= 0);

        eps = epsInput;
        const int S = 18;

        Console.WriteLine($"\nx = {x},  ε = {eps},  S = {S} с\n");
        Console.WriteLine("Запуск потоків.\n");

        ThreadData evenData = new ThreadData { X = x, IsEven = true };
        ThreadData oddData = new ThreadData { X = x, IsEven = false };

        Thread t1 = new Thread(ThreadFunc) { Name = "EvenThread" };
        Thread t2 = new Thread(ThreadFunc) { Name = "OddThread" };

        t1.Start(evenData);
        t2.Start(oddData);

        bool done = t1.Join(S * 1000);
        if (!done)
        {
            stop = true;
            Console.WriteLine($"\nЛіміт часу {S} с вичерпано, зупиняємо потоки.");
        }
        t2.Join(2000);

        Console.WriteLine($"\n{new string('=', 50)}");
        Console.WriteLine($"  Результат:  y({x}) = {Sum}");
        Console.WriteLine(new string('=', 50));
    }
}
