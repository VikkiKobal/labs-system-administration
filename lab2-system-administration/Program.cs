/* Створити три процеси. Якщо третій процес завершиться першим, тоді завершити
другий і дочекатися завершення першого. У супротивному випадку дочекатися
завершення активних процесів. Стан процесів відображати у консольному вікні. */

using System;
using System.Diagnostics;

namespace Lab2
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

            Process p1 = null;
            Process p2 = null;
            Process p3 = null;

            try
            {
                p1 = Process.Start(psi);
                p2 = Process.Start(psi);
                p3 = Process.Start(psi);

                if (p1 == null || p2 == null || p3 == null)
                {
                    Console.WriteLine("Не вдалося створити процеси.");
                    return;
                }

                Console.WriteLine($"P1 PID={p1.Id}, P2 PID={p2.Id}, P3 PID={p3.Id}");

                int firstFinished = WaitHandle.WaitAny(new[]
                {
                    p1.WaitHandle,
                    p2.WaitHandle,
                    p3.WaitHandle
                });

                if (firstFinished == 2)
                {
                    Console.WriteLine("Спочатку завершився P3, завершуєм P2, чекаєм P1.");

                    if (!p2.HasExited)
                        p2.Kill();

                    p2.WaitForExit();
                    p1.WaitForExit();
                }
                else
                {
                    Console.WriteLine("Спочатку завершився не P3, чекаєм всі процеси.");

                    p1.WaitForExit();
                    p2.WaitForExit();
                    p3.WaitForExit();
                }

                Console.WriteLine("Стан:");
                PrintExit(1, p1);
                PrintExit(2, p2);
                PrintExit(3, p3);

                Console.WriteLine("Готово.");
                Console.ReadLine();
            }
            finally
            {
                p1?.Dispose();
                p2?.Dispose();
                p3?.Dispose();
            }
        }

        static void PrintExit(int n, Process p)
        {
            string s = p.HasExited ? $"код виходу {p.ExitCode}" : "ще працює";
            Console.WriteLine($"  P{n}: {s}");
        }
    }
}
