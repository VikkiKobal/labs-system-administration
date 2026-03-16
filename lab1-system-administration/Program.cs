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
            Console.WriteLine("Process 1 started.");

            ProcessInformation pi2 = WinApiFuncs.CreateProcess(testApp, WinApiFuncs.CREATE_NEW_CONSOLE);
            Console.WriteLine("Process 2 started.");

            if (pi1.hProcess == WinApiFuncs.INVALID_HANDLE_VALUE || pi2.hProcess == WinApiFuncs.INVALID_HANDLE_VALUE)
            {
                Console.WriteLine("Could not create processes");
                return;
            }

            Console.WriteLine("Waiting up to 10 seconds to see which process finishes first.");
            uint waitResult = WinApiFuncs.WaitForMultipleObjects(2, new[] { pi1.hProcess, pi2.hProcess }, false, 10000);

            if (waitResult == WinApiFuncs.WAIT_OBJECT_0 + 1)
            {
                Console.WriteLine("Process 2 finished first within 10 seconds. Restarting it.");
                WinApiFuncs.CloseHandle(pi2.hProcess);
                WinApiFuncs.CloseHandle(pi2.hThread);

                pi2 = WinApiFuncs.CreateProcess(testApp, WinApiFuncs.CREATE_NEW_CONSOLE);
                Console.WriteLine("Process 2 restarted.");
                Console.WriteLine("Waiting 5 seconds, then terminating all processes.");
                WinApiFuncs.Sleep(5000);
            }
            else if (waitResult == WinApiFuncs.WAIT_OBJECT_0)
            {
                Console.WriteLine("Process 1 finished first. Terminating all processes without restart.");
            }
            else if (waitResult == WinApiFuncs.WAIT_TIMEOUT)
            {
                Console.WriteLine("No process finished within 10 seconds. Terminating all processes.");
            }
            else
            {
                Console.WriteLine(" Terminating all processes.");
            }

            Console.WriteLine("Terminating all processes");
            if (WinApiFuncs.WaitForSingleObject(pi1.hProcess, 0) != WinApiFuncs.WAIT_OBJECT_0) {
                WinApiFuncs.TerminateProcess(pi1.hProcess, 0);
            }
            if (WinApiFuncs.WaitForSingleObject(pi2.hProcess, 0) != WinApiFuncs.WAIT_OBJECT_0) {
                WinApiFuncs.TerminateProcess(pi2.hProcess, 0);
            }
            WinApiFuncs.CloseHandle(pi1.hProcess);
            WinApiFuncs.CloseHandle(pi1.hThread);
            WinApiFuncs.CloseHandle(pi2.hProcess);
            WinApiFuncs.CloseHandle(pi2.hThread);

            Console.WriteLine("Done.");
            Console.ReadLine();
        }
    }
}
