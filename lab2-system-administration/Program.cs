/* Створити три процеси. Якщо третій процес завершиться першим, тоді завершити
другий і дочекатися завершення першого. У супротивному випадку дочекатися
завершення активних процесів. Стан процесів відображати у консольному вікні. */

using System;
using WinApi;

namespace Lab2
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string testApp = @"C:\Users\Vision\source\repos\ConsoleTest\ConsoleTest\bin\Debug\net8.0\ConsoleTest.exe";

            ProcessInformation pi1 = WinApiFuncs.CreateProcess(testApp, WinApiFuncs.CREATE_NEW_CONSOLE);
            ProcessInformation pi2 = WinApiFuncs.CreateProcess(testApp, WinApiFuncs.CREATE_NEW_CONSOLE);
            ProcessInformation pi3 = WinApiFuncs.CreateProcess(testApp, WinApiFuncs.CREATE_NEW_CONSOLE);

            if (pi1.hProcess == WinApiFuncs.INVALID_HANDLE_VALUE ||
                pi2.hProcess == WinApiFuncs.INVALID_HANDLE_VALUE ||
                pi3.hProcess == WinApiFuncs.INVALID_HANDLE_VALUE)
            {
                Console.WriteLine("Не вдалося створити процеси.");
                return;
            }

            Console.WriteLine($"P1 PID={pi1.dwProcessId}, P2 PID={pi2.dwProcessId}, P3 PID={pi3.dwProcessId}");

            IntPtr[] h = { pi1.hProcess, pi2.hProcess, pi3.hProcess };

            uint r = WinApiFuncs.WaitForMultipleObjects(3, h, false, WinApiFuncs.INFINITE);

            if (r == WinApiFuncs.WAIT_OBJECT_0 + 2)
            {
                Console.WriteLine("Спочатку завершився P3, завершуєм P2, чекаєм P1.");
                WinApiFuncs.TerminateProcess(pi2.hProcess, 0);
                WinApiFuncs.WaitForSingleObject(pi1.hProcess, WinApiFuncs.INFINITE);
            }
            else
            {
                Console.WriteLine("Спочатку завершився не P3, чекаєм всі процеси.");
                WinApiFuncs.WaitForMultipleObjects(3, h, true, WinApiFuncs.INFINITE);
            }

            Console.WriteLine("Стан:");
            PrintExit(1, pi1.hProcess);
            PrintExit(2, pi2.hProcess);
            PrintExit(3, pi3.hProcess);

            Close(pi1);
            Close(pi2);
            Close(pi3);
            Console.WriteLine("Готово.");
            Console.ReadLine();
        }

        static void PrintExit(int n, IntPtr hp)
        {
            WinApiFuncs.GetExitCodeProcess(hp, out uint code);
            string s = code == WinApiFuncs.STILL_ACTIVE ? "ще працює" : $"код виходу {code}";
            Console.WriteLine($"  P{n}: {s}");
        }

        static void Close(ProcessInformation pi)
        {
            WinApiFuncs.CloseHandle(pi.hThread);
            WinApiFuncs.CloseHandle(pi.hProcess);
        }
    }
}
