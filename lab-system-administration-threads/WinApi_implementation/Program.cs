// WinApi_implementation of the task, VARIANT - 7

using System;
using System.Globalization;
using WinApi;

struct CalcParams
{
    public double X;
    public double Eps;
    public CalcParams(double x, double eps) { X = x; Eps = eps; }
}

class Program
{
    static volatile bool stopRequested = false;
    static double resultSum = 0.0;

    static void Main(string[] args) {
        System.Threading.Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;

        Console.WriteLine("Обчислення нескінченної суми");
        Console.WriteLine("y(x) = sum_{i=0}^{inf} (-1)^(i+1) * (x^2+1) / (5i+3)");
        Console.WriteLine();

        try
        {
            Console.Write("Введіть x (x != 0): ");
            double x = double.Parse(Console.ReadLine());

            Console.Write("Введіть точність e > 0: ");
            double eps = double.Parse(Console.ReadLine());

            const int S = 18;
            Console.WriteLine($"Ліміт часу S = {S} секунд.");

            if (x == 0)   { Console.WriteLine("Помилка: x != 0."); return; }
            if (eps <= 0) { Console.WriteLine("Помилка: e > 0.");  return; }

            var parameters = new CalcParams(x, eps);

            IntPtr ht = WinApiFuncs.CreateThread(CalculateSum, parameters);

            Console.WriteLine("\nЗапуск обчислень.\n");

            uint waitResult = WinApiFuncs.WaitForSingleObject(ht, (uint)(S * 1000));

            if (waitResult == WinApiFuncs.WAIT_TIMEOUT)
            {
                Console.WriteLine($"\nЛіміт {S} с. вичерпано. Зупинка.");
                stopRequested = true;
                WinApiFuncs.WaitForSingleObject(ht, WinApiFuncs.INFINITE);
                Console.WriteLine("Обчислення перервано достроково.");
            }
            else
            {
                Console.WriteLine("\nОбчислення завершено з необхідною точністю.");
            }

            WinApiFuncs.CloseHandle(ht);

            Console.WriteLine($"\nОстаточний результат: y({x}) = {resultSum:F10}");
        }
        catch (FormatException)
        {
            Console.WriteLine("Помилка: введено не число.");
        }

        Console.WriteLine("\nНатисніть Enter для виходу.");
        Console.ReadLine();
    }

    static void CalculateSum(IntPtr data)
    {
        var p = (CalcParams)WinApiFuncs.ToObject(data);
        double x = p.X;
        double eps = p.Eps;

        double currentSum = 0.0;
        long i = 0;
        double numerator = x * x + 1.0;  
        double sign = -1.0;          
        double term;

        do
        {
            term = sign * numerator / (5.0 * i + 3.0);
            currentSum += term;
            resultSum = currentSum;

            if (i % 10000 == 0)
                Console.WriteLine(
                    $"Ітерація: {i,10:N0} | Поточна сума: {currentSum:F10}");

            i++;
            sign = -sign;

        } while (Math.Abs(term) > eps && !stopRequested);

        if (!stopRequested)
            Console.WriteLine(
                $"  Ітерація: {i - 1,10:N0} | Фінальна сума: {currentSum:F10}");
    }
}
