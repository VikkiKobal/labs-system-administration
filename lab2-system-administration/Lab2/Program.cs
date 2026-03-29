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

            Console.WriteLine("=== Process Manager Started ===");

            ProcessInformation pi1 = WinApiFuncs.CreateProcess(testApp, WinApiFuncs.CREATE_NEW_CONSOLE);
            Console.WriteLine($"[START] Process 1 (PID: {pi1.dwProcessId})");

            ProcessInformation pi2 = WinApiFuncs.CreateProcess(testApp, WinApiFuncs.CREATE_NEW_CONSOLE);
            Console.WriteLine($"[START] Process 2 (PID: {pi2.dwProcessId})");

            ProcessInformation pi3 = WinApiFuncs.CreateProcess(testApp, WinApiFuncs.CREATE_NEW_CONSOLE);
            Console.WriteLine($"[START] Process 3 (PID: {pi3.dwProcessId})");

            if (pi1.hProcess == WinApiFuncs.INVALID_HANDLE_VALUE ||
                pi2.hProcess == WinApiFuncs.INVALID_HANDLE_VALUE ||
                pi3.hProcess == WinApiFuncs.INVALID_HANDLE_VALUE)
            {
                Console.WriteLine("Failed to create one or more processes.");
                return;
            }

            IntPtr[] handles = { pi1.hProcess, pi2.hProcess, pi3.hProcess };

            Console.WriteLine("\nWaiting for the first process to exit...");

            uint waitResult = WinApiFuncs.WaitForMultipleObjects(3, handles, false, WinApiFuncs.INFINITE);

            if (waitResult == WinApiFuncs.WAIT_OBJECT_0 + 2)
            {
                Console.WriteLine("\n>>> Process 3 finished FIRST.");
                Console.WriteLine(">>> Terminating Process 2 and waiting for Process 1...");

                WinApiFuncs.TerminateProcess(pi2.hProcess, 0);

                WinApiFuncs.WaitForSingleObject(pi1.hProcess, WinApiFuncs.INFINITE);
            }
            else
            {
                int firstExitIdx = (int)(waitResult - WinApiFuncs.WAIT_OBJECT_0);
                Console.WriteLine($"\n>>> Process {firstExitIdx + 1} finished first (not P3).");
                Console.WriteLine(">>> Waiting for all remaining processes to finish...");

                WinApiFuncs.WaitForMultipleObjects(3, handles, true, WinApiFuncs.INFINITE);
            }

            Console.WriteLine("\n" + new string('-', 30));
            Console.WriteLine("FINAL PROCESS STATUS:");
            PrintStatus(1, pi1.hProcess);
            PrintStatus(2, pi2.hProcess);
            PrintStatus(3, pi3.hProcess);
            Console.WriteLine(new string('-', 30));

            Cleanup(pi1);
            Cleanup(pi2);
            Cleanup(pi3);

            Console.WriteLine("\nTask complete. Press Enter to close.");
            Console.ReadLine();
        }

        static void PrintStatus(int id, IntPtr hProcess)
        {
            WinApiFuncs.GetExitCodeProcess(hProcess, out uint exitCode);
            string status = (exitCode == WinApiFuncs.STILL_ACTIVE) ? "Running" : $"Finished (Code: {exitCode})";
            Console.WriteLine($"Process {id}: {status}");
        }

        static void Cleanup(ProcessInformation pi)
        {
            if (pi.hThread != IntPtr.Zero && pi.hThread != WinApiFuncs.INVALID_HANDLE_VALUE)
                WinApiFuncs.CloseHandle(pi.hThread);
            if (pi.hProcess != IntPtr.Zero && pi.hProcess != WinApiFuncs.INVALID_HANDLE_VALUE)
                WinApiFuncs.CloseHandle(pi.hProcess);
        }
    }
}
