/* ВАРІАНТ - 7: Створити процес. Якщо на протязі 10 секунд він завершиться, тоді запустити його знову
через 5 секунд після його завершення і завершити через 3 секунди. Інакше ще через 5
секунд завершити його. Використати можливості C#. */

using System;
using System.Diagnostics;
using System.Threading;

namespace module_system_administration
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string testApp = @"C:\Users\Vision\source\repos\ConsoleTest\ConsoleTest\bin\Debug\net8.0\ConsoleTest.exe";

            var psi = new ProcessStartInfo
            {
                FileName = testApp,
                UseShellExecute = false,
                CreateNoWindow = false
            };

            Process? p1 = null;
            Process? p2 = null;

            try
            {
                p1 = Process.Start(psi);
                if (p1 == null)
                {
                    Console.WriteLine("Не вдалося створити процес.");
                    return;
                }

                Console.WriteLine($"Процес PID={p1.Id}. Очікування завершення до 10 с.");

                bool finishedWithin10 = p1.WaitForExit(10_000);

                if (finishedWithin10)
                {
                    Console.WriteLine("Завершився протягом 10 с. Через 5 с після завершення перезапуск.");

                    Thread.Sleep(5000);

                    p2 = Process.Start(psi);
                    if (p2 == null)
                    {
                        Console.WriteLine("Не вдалося перезапустити процес.");
                        return;
                    }

                    Console.WriteLine($"Перезапущено PID={p2.Id}. Через 3 с примусове завершення.");

                    Thread.Sleep(3000);
                    if (!p2.HasExited)
                        p2.Kill();

                    p2.WaitForExit();
                }
                else
                {
                    Console.WriteLine("Не завершився за 10 с. Ще через 5 с примусове завершення.");

                    Thread.Sleep(5000);

                    if (!p1.HasExited)
                        p1.Kill();

                    p1.WaitForExit();
                }
            }
            finally
            {
                p1?.Dispose();
                p2?.Dispose();
            }

            Console.WriteLine("Готово.");
            Console.ReadLine();
        }
    }
}
