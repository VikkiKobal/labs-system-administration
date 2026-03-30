/* ВАРІАНТ - 7: Створити процес. Якщо протягом 10 секунд він завершиться, тоді запустити його знову
через 5 секунд після його завершення і завершити через 3 секунди. Інакше ще через 5 секунд завершити його. */

using System;
using WinApi;

namespace module_system_administration
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string testApp = @"C:\Users\Vision\source\repos\ConsoleTest\ConsoleTest\bin\Debug\net8.0\ConsoleTest.exe";

            ProcessInformation pi = WinApiFuncs.CreateProcess(testApp, WinApiFuncs.CREATE_NEW_CONSOLE);
            if (pi.hProcess == WinApiFuncs.INVALID_HANDLE_VALUE)
            {
                Console.WriteLine("Не вдалося створити процес.");
                return;
            }

            Console.WriteLine($"Процес PID={pi.dwProcessId}. Очікування завершення до 10 с.");

            uint w = WinApiFuncs.WaitForSingleObject(pi.hProcess, 10000);

            if (w == WinApiFuncs.WAIT_OBJECT_0)
            {
                Console.WriteLine("Завершився протягом 10 с. Через 5 с після завершення перезапуск.");
                WinApiFuncs.Sleep(5000);
                Close(pi);

                pi = WinApiFuncs.CreateProcess(testApp, WinApiFuncs.CREATE_NEW_CONSOLE);
                if (pi.hProcess == WinApiFuncs.INVALID_HANDLE_VALUE)
                {
                    Console.WriteLine("Не вдалося перезапустити процес.");
                    return;
                }

                Console.WriteLine($"Перезапущено PID={pi.dwProcessId}. Через 3 с примусове завершення.");
                WinApiFuncs.Sleep(3000);

                if (WinApiFuncs.WaitForSingleObject(pi.hProcess, 0) != WinApiFuncs.WAIT_OBJECT_0)
                    WinApiFuncs.TerminateProcess(pi.hProcess, 0);

                WinApiFuncs.WaitForSingleObject(pi.hProcess, WinApiFuncs.INFINITE);
                Close(pi);
            }
            else if (w == WinApiFuncs.WAIT_TIMEOUT)
            {
                Console.WriteLine("Не завершився за 10 с. Ще через 5 с примусове завершення.");
                WinApiFuncs.Sleep(5000);

                if (WinApiFuncs.WaitForSingleObject(pi.hProcess, 0) != WinApiFuncs.WAIT_OBJECT_0)
                    WinApiFuncs.TerminateProcess(pi.hProcess, 0);

                WinApiFuncs.WaitForSingleObject(pi.hProcess, WinApiFuncs.INFINITE);
                Close(pi);
            }
            else
            {
                Close(pi);
            }

            Console.WriteLine("Готово.");
            Console.ReadLine();
        }

        static void Close(ProcessInformation pi)
        {
            WinApiFuncs.CloseHandle(pi.hThread);
            WinApiFuncs.CloseHandle(pi.hProcess);
        }
    }
}
