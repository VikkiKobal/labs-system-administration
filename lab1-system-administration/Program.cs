using System;
using WinApi;

namespace lab1_system_administration
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string testApp = @"C:\Users\Vision\source\repos\ConsoleTest\ConsoleTest\bin\Debug\net8.0\ConsoleTest.exe";
            Console.WriteLine("Starting processes.");
            ProcessInformation pi1 = WinApiFuncs.CreateProcess(testApp, WinApiFuncs.CREATE_NEW_CONSOLE);
            ProcessInformation pi2 = WinApiFuncs.CreateProcess(testApp, WinApiFuncs.CREATE_NEW_CONSOLE);

            if (pi1.hProcess == WinApiFuncs.INVALID_HANDLE_VALUE || pi2.hProcess == WinApiFuncs.INVALID_HANDLE_VALUE)
            {
                Console.WriteLine("Could not create processes");
                return;
            }

            Console.WriteLine("Monitoring process 2 (10s timeout)");
            uint waitResult = WinApiFuncs.WaitForSingleObject(pi2.hProcess, 10000);

            if (waitResult == WinApiFuncs.WAIT_OBJECT_0)
            {
                Console.WriteLine("Process 2 finished. Restarting");
                WinApiFuncs.CloseHandle(pi2.hProcess);
                WinApiFuncs.CloseHandle(pi2.hThread);

                pi2 = WinApiFuncs.CreateProcess(testApp, WinApiFuncs.CREATE_NEW_CONSOLE);

                Console.WriteLine("Process 2 restarted. Waiting 5s before exit");
                WinApiFuncs.Sleep(5000);
            }
            else
            {
                Console.WriteLine("Process 2 is still running.");
            }

            Console.WriteLine("Terminating all processes");
            WinApiFuncs.TerminateProcess(pi1.hProcess, 0);
            WinApiFuncs.TerminateProcess(pi2.hProcess, 0);

            WinApiFuncs.CloseHandle(pi1.hProcess);
            WinApiFuncs.CloseHandle(pi1.hThread);
            WinApiFuncs.CloseHandle(pi2.hProcess);
            WinApiFuncs.CloseHandle(pi2.hThread);

            Console.WriteLine("Done.");
            Console.ReadLine();
        }
    }
}