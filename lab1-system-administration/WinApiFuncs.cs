using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Policy;
using System.Threading;

namespace WinApi
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct STARTUPINFOA
    {
        public int cb;
        public string lpReserved;
        public string lpDesktop;
        public string lpTitle;
        public uint dwX;
        public uint dwY;
        public uint dwXSize;
        public uint dwYSize;
        public uint dwXCountChars;
        public uint dwYCountChars;
        public uint dwFillAttribute;
        public uint dwFlags;
        public ushort wShowWindow;
        public ushort cbReserved2;
        public IntPtr lpReserved2;
        public IntPtr hStdInput;
        public IntPtr hStdOutput;
        public IntPtr hStdError;
    }
    [StructLayout(LayoutKind.Sequential)]
    public struct ProcessInformation
    {
        public IntPtr hProcess;
        public IntPtr hThread;
        public int dwProcessId;
        public int dwThreadId;
    }
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct CRITICAL_SECTION
    {
        IntPtr DebugInfo;
        long LockCount;
        long RecursionCount;
        IntPtr OwningThread;
        IntPtr LockSemaphore;
        ulong SpinCount;
    }
    [Flags]
    public enum FileMapProtection : uint
    {
        PageReadonly = 0x02,
        PageReadWrite = 0x04,
        PageWriteCopy = 0x08,
        PageExecuteRead = 0x20,
        PageExecuteReadWrite = 0x40,
        SectionCommit = 0x8000000,
        SectionImage = 0x1000000,
        SectionNoCache = 0x10000000,
        SectionReserve = 0x4000000,
    }
    public enum SE_OBJECT_TYPE
    {
        SE_UNKNOWN_OBJECT_TYPE,
        SE_FILE_OBJECT,
        SE_SERVICE,
        SE_PRINTER,
        SE_REGISTRY_KEY,
        SE_LMSHARE,
        SE_KERNEL_OBJECT,
        SE_WINDOW_OBJECT,
        SE_DS_OBJECT,
        SE_DS_OBJECT_ALL,
        SE_PROVIDER_DEFINED_OBJECT,
        SE_WMIGUID_OBJECT,
        SE_REGISTRY_WOW64_32KEY
    }
    public enum SECURITY_INFORMATION
    {
        OWNER_SECURITY_INFORMATION = 1,
        GROUP_SECURITY_INFORMATION = 2,
        DACL_SECURITY_INFORMATION = 4,
        SACL_SECURITY_INFORMATION = 8,
    }
    [StructLayout(LayoutKind.Sequential)]
    public struct COORD
    {
        public short X;
        public short Y;
    }
    [StructLayout(LayoutKind.Sequential)]
    public struct SMALL_RECT
    {
        public short Left;
        public short Top;
        public short Right;
        public short Bottom;
    }
    [StructLayout(LayoutKind.Sequential)]
    public struct COLORTABLE
    {
        public uint color0;
        public uint color1;
        public uint color2;
        public uint color3;
        public uint color4;
        public uint color5;
        public uint color6;
        public uint color7;
        public uint color8;
        public uint color9;
        public uint colorA;
        public uint colorB;
        public uint colorC;
        public uint colorD;
        public uint colorE;
        public uint colorF;
    }
    [StructLayout(LayoutKind.Sequential)]
    public struct CONSOLE_SCREEN_BUFFER_INFO
    {
        public COORD dwSize;
        public COORD dwCursorPosition;
        public ushort wAttributes;
        public SMALL_RECT srWindow;
        public COORD dwMaximumWindowSize;
    }
    [StructLayout(LayoutKind.Sequential)]
    public struct CONSOLE_SCREEN_BUFFER_INFOEX
    {
        public int cbSize;
        public COORD dwSize;
        public COORD dwCursorPosition;
        public ushort wAttributes;
        public SMALL_RECT srWindow;
        public COORD dwMaximumWindowSize;
        public ushort wPopupAttributes;
        public bool bFullscreenSupported;
        //[MarshalAs(UnmanagedType.ByValArray, SizeConst=16)]
        //public uint[] ColorTable;
        public COLORTABLE ColorTable;
        //public CONSOLE_SCREEN_BUFFER_INFOEX(int p) 
        //{
        //    cbSize = 0;
        //    dwSize.X = 0;
        //    dwSize.Y = 0;
        //    dwCursorPosition.X = 0;
        //    dwCursorPosition.Y = 0;
        //    dwMaximumWindowSize.X = 0;
        //    dwMaximumWindowSize.Y = 0;
        //    wAttributes = 0;
        //    wPopupAttributes = 0;
        //    bFullscreenSupported = false;
        //    srWindow.Top = 0;
        //    srWindow.Left = 0;
        //    srWindow.Bottom = 0;
        //    srWindow.Right = 0;
        //    ColorTable = new uint[16];
        //}
    }
    public class WinApiFuncs
    {
        public const uint INFINITE = 0xFFFFFFFF;
        public const uint WAIT_ABANDONED = 0x00000080;
        public const uint WAIT_OBJECT_0 = 0x00000000;
        public const uint WAIT_TIMEOUT = 0x00000102;
        public static IntPtr INVALID_HANDLE_VALUE = new IntPtr(-1);
        public const uint CREATE_SUSPENDED = 4;
        public const uint CREATE_NEW_CONSOLE = 0x00000010;
        public const uint CREATE_NO_WINDOW = 0x08000000;
        public const uint STILL_ACTIVE = 0x00000103;

        public const int STARTF_USESHOWWINDOW = 0x00000001;
        public const int STARTF_USECOUNTCHARS = 0x00000008;
        public const int STARTF_USEFILLATTRIBUTE = 0x00000010;
        public const int STARTF_USEPOSITION = 0x00000004;
        public const int STARTF_USESIZE = 0x00000002;
        public const int STARTF_USESTDHANDLES = 0x00000100;
        public const int STARTF_RUNFULLSCREEN = 0x00000020;

        public const short SW_SHOWNORMAL = 1;
        public const short SW_SHOWMINIMIZED = 2;
        public const short SW_SHOWMAXIMIZED = 3;

        public const uint STANDARD_RIGHTS_REQUIRED = 0x000F0000;
        public const uint SYNCHRONIZE = 0x00100000;
        public const uint EVENT_ALL_ACCESS = (STANDARD_RIGHTS_REQUIRED | SYNCHRONIZE | 0x3);
        public const uint EVENT_MODIFY_STATE = 0x0002;
        public const uint ERROR_ALREADY_EXISTS = 183;

        public const UInt32 MUTEX_ALL_ACCESS = 0x1F0001;
        public const UInt32 MUTEX_MODIFY_STATE = 0x0001;

        public const uint TIMER_ALL_ACCESS = (STANDARD_RIGHTS_REQUIRED | SYNCHRONIZE | 0x3);
        public const uint TIMER_MODIFY_STATE = 0x0002;

        public const uint SECTION_QUERY = 0x0001;
        public const uint SECTION_MAP_WRITE = 0x0002;
        public const uint SECTION_MAP_READ = 0x0004;
        public const uint SECTION_MAP_EXECUTE = 0x0008;
        public const uint SECTION_EXTEND_SIZE = 0x0010;
        public const uint SECTION_ALL_ACCESS = (STANDARD_RIGHTS_REQUIRED | SECTION_QUERY |
            SECTION_MAP_WRITE |
            SECTION_MAP_READ |
            SECTION_MAP_EXECUTE |
            SECTION_EXTEND_SIZE);
        public const uint FILE_MAP_ALL_ACCESS = SECTION_ALL_ACCESS;

        public const int STD_INPUT_HANDLE = -10;
        public const int STD_OUTPUT_HANDLE = -11;
        public const int STD_ERROR_HANDLE = -12;

        [DllImport("kernel32.dll", EntryPoint = "CreateProcessA", SetLastError = true)]
        public static extern bool CreateProcess(string lpApplicationName, string lpCommandLine,
            IntPtr lpProcessAttributes, IntPtr lpThreadAttributes, bool bInheritHandles,
            uint dwCreationFlags, IntPtr lpEnvironment, string lpCurrentDirectory,
            [In] ref STARTUPINFOA lpStartupInfo, out ProcessInformation lpProcessInformation);
        public static ProcessInformation CreateProcess(string lpCommandLine, uint dwCreationFlags = 0)
        {
            ProcessInformation pi;
            var si = new STARTUPINFOA();
            si.cb = Marshal.SizeOf(si);
            var ret = CreateProcess(null, lpCommandLine, IntPtr.Zero, IntPtr.Zero, false, dwCreationFlags,
                                    IntPtr.Zero, null, ref si, out pi);
            return ret ? pi : new ProcessInformation { hProcess = INVALID_HANDLE_VALUE, hThread = INVALID_HANDLE_VALUE };

        }
        public static ProcessInformation CreateProcess(string lpCommandLine, STARTUPINFOA si, uint dwCreationFlags = CREATE_NEW_CONSOLE)
        {
            ProcessInformation pi;
            var ret = CreateProcess(null, lpCommandLine, IntPtr.Zero, IntPtr.Zero, true, dwCreationFlags,
                                    IntPtr.Zero, null, ref si, out pi);
            return ret ? pi : new ProcessInformation { hProcess = INVALID_HANDLE_VALUE, hThread = INVALID_HANDLE_VALUE };

        }
        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetExitCodeProcess(IntPtr hObject, out uint code);
        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool CloseHandle(IntPtr hObject);
        [DllImport("kernel32.dll")]
        public static extern void ExitProcess(uint uExitCode);
        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool TerminateProcess(IntPtr hProcess, uint uExitCode);
        public static int GetLastError()
        {
            return Marshal.GetLastWin32Error();
        }
        [DllImport("kernel32.dll", EntryPoint = "GetStartupInfoA", SetLastError = true)]
        public static extern void GetStartupInfo(out STARTUPINFOA si);
        // Wait functions
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern uint WaitForSingleObject(IntPtr hHandle, uint dwMilliseconds);
        [DllImport("kernel32.dll")]
        public static extern uint WaitForMultipleObjects(uint nCount, IntPtr[] lpHandles, bool bWaitAll, uint dwMilliseconds);

        //  Threads
        public delegate void ThreadMethod(IntPtr data);
        //System.Threading.ThreadStart

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr CreateThread(IntPtr securityAttributes, uint stackSize,
            ThreadMethod startFunction, IntPtr threadParameter, uint creationFlags, out uint threadId);
        //public static IntPtr CreateThread(ThreadMethod startFunction, IntPtr threadParameter, uint creationFlags = 0)
        //{
        //    uint dw;
        //    return CreateThread(IntPtr.Zero, 0, startFunction, threadParameter, creationFlags, out dw);
        //}
        public static IntPtr CreateThread(ThreadMethod startFunction, object threadParameter, uint creationFlags = 0)
        {
            if (threadParameter == null)
            {
                return CreateThread(IntPtr.Zero, 0, startFunction, IntPtr.Zero, creationFlags, out _);
            }
            var gptr = GCHandle.Alloc(threadParameter);
            var ptr = GCHandle.ToIntPtr(gptr);
            return CreateThread(IntPtr.Zero, 0, startFunction, ptr, creationFlags, out _);
        }
        public static object ToObject(IntPtr ptr)
        {
            if (ptr.Equals(IntPtr.Zero)) return null;
            GCHandle gch = GCHandle.FromIntPtr(ptr);
            var obj = gch.Target;
            gch.Free();
            return obj;
        }
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern uint ResumeThread(IntPtr hThread);
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern int SuspendThread(IntPtr hThread);
        [DllImport("kernel32.dll")]
        public static extern void ExitThread(uint dwExitCode);
        [DllImport("kernel32.dll")]
        public static extern bool TerminateThread(IntPtr hThread, uint dwExitCode);
        // EVENTS
        [DllImport("kernel32.dll")]
        public static extern IntPtr CreateEvent(IntPtr lpEventAttributes, bool bManualReset, bool bInitialState, string lpName);
        [DllImport("kernel32.dll")]
        public static extern bool SetEvent(IntPtr hEvent);
        [DllImport("kernel32.dll")]
        public static extern bool ResetEvent(IntPtr hEvent);
        [DllImport("Kernel32.dll", SetLastError = true)]
        public static extern IntPtr OpenEvent(uint dwDesiredAccess, bool bInheritHandle, string lpName);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr CreateMutex(IntPtr lpMutexAttributes, bool bInitialOwner, string lpName);
        [DllImport("kernel32.dll")]
        public static extern bool ReleaseMutex(IntPtr hMutex);
        [DllImport("kernel32.dll")]
        public static extern IntPtr OpenMutex(uint dwDesiredAccess, bool bInheritHandle, string lpName);

        // Waitable Timers

        public const long NanoToSec = -10000000;

        public delegate void TimerApcProc(IntPtr arg, uint lowTime, uint highTime);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr CreateWaitableTimer(IntPtr lpTimerAttributes, bool bManualReset, string lpTimerName);
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool SetWaitableTimer(IntPtr hTimer, [In] ref long ft, int lPeriod,
            TimerApcProc pfnCompletionRoutine, IntPtr pArgToCompletionRoutine, bool fResume);
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool CancelWaitableTimer(IntPtr hTimer);
        [DllImport("Kernel32.dll", SetLastError = true)]
        public static extern IntPtr OpenWaitableTimer(uint dwDesiredAccess, bool bInheritHandle, string lpName);
        public static void Sleep(int timeMs)
        {
            Thread.Sleep(timeMs);
        }
        public static void Exit()
        {
            Console.WriteLine("Press <Enter> to exit.");
            Console.ReadLine();
            ExitProcess(0);
        }
        //Critical Section
        [DllImport("kernel32.dll")]
        public static extern bool InitializeCriticalSectionAndSpinCount(ref CRITICAL_SECTION lpCriticalSection, uint dwSpinCount);
        [DllImport("kernel32.dll")]
        public static extern void InitializeCriticalSection(ref CRITICAL_SECTION lpCriticalSection);
        [DllImport("kernel32.dll")]
        public static extern void DeleteCriticalSection(ref CRITICAL_SECTION lpCriticalSection);
        [DllImport("kernel32.dll")]
        public static extern void EnterCriticalSection(ref CRITICAL_SECTION lpCriticalSection);
        [DllImport("kernel32.dll")]
        public static extern bool TryEnterCriticalSection(ref CRITICAL_SECTION lpCriticalSection);
        [DllImport("kernel32.dll")]
        public static extern void LeaveCriticalSection(ref CRITICAL_SECTION lpCriticalSection);
        // File Mapping
        [DllImport("kernel32.dll", SetLastError = true, EntryPoint = "CreateFileMappingA", CharSet = CharSet.Ansi)]
        public static extern IntPtr CreateFileMapping(IntPtr handle, IntPtr securityAttributes, FileMapProtection access, int sizeHigh, int sizeLow, string name);
        public static IntPtr CreateFileMapping(int sizeLow, string name)
        {
            return CreateFileMapping(WinApiFuncs.INVALID_HANDLE_VALUE, IntPtr.Zero, FileMapProtection.PageReadWrite, 0, sizeLow, name);
        }
        [DllImport("kernel32.dll")]
        public static extern IntPtr MapViewOfFile(IntPtr hFileMappingObject, uint dwAccess, uint dwFileOffsetHigh, uint dwFileOffsetLow, int dwNumberOfBytesToMap);
        public static IntPtr MapViewOfFile(IntPtr hFileMappingObject, int dwNumberOfBytesToMap)
        {
            return MapViewOfFile(hFileMappingObject, FILE_MAP_ALL_ACCESS, 0, 0, dwNumberOfBytesToMap);
        }
        [DllImport("kernel32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool UnmapViewOfFile(IntPtr hFileMappingObject);
        // Security
        [DllImport("advapi32.dll", SetLastError = true)]
        public static extern int GetSecurityInfo(IntPtr handle, SE_OBJECT_TYPE objectType, SECURITY_INFORMATION securityInfo,
        out IntPtr sidOwner,
        out IntPtr sidGroup,
        out IntPtr dacl,
        out IntPtr sacl,
        out IntPtr securityDescriptor);
        public static int GetSecurityInfo(IntPtr handle, out IntPtr securityDescriptor)
        {
            IntPtr ownerSid;
            IntPtr groupSid;
            IntPtr dacl;
            IntPtr sacl;
            var sec = SECURITY_INFORMATION.OWNER_SECURITY_INFORMATION |
                SECURITY_INFORMATION.DACL_SECURITY_INFORMATION;
            //| SECURITY_INFORMATION.SACL_SECURITY_INFORMATION;
            //IntPtr securityDescriptor = IntPtr.Zero;
            int returnValue = GetSecurityInfo(handle, SE_OBJECT_TYPE.SE_KERNEL_OBJECT, sec,
                out ownerSid, out groupSid, out dacl, out sacl, out securityDescriptor);
            return returnValue;
        }
        [DllImport("advapi32.dll", EntryPoint = "ConvertSecurityDescriptorToStringSecurityDescriptorA", CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern bool ConvertSecurityDescriptorToStringSecurityDescriptor(IntPtr security, uint revision, SECURITY_INFORMATION sinfo,
            out IntPtr strInfo, out ulong len);
        public static bool ConvertSecurityDescriptorToStringSecurityDescriptor(IntPtr security, out IntPtr strInfo, out ulong len)
        {
            var sec = SECURITY_INFORMATION.OWNER_SECURITY_INFORMATION |
                SECURITY_INFORMATION.DACL_SECURITY_INFORMATION;
            //| SECURITY_INFORMATION.SACL_SECURITY_INFORMATION;
            return ConvertSecurityDescriptorToStringSecurityDescriptor(security, 1, sec, out strInfo, out len);
        }
        [DllImport("advapi32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern bool ConvertSidToStringSid(IntPtr sid, out IntPtr sidString);
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr LocalFree(IntPtr handle);
        //Console
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr GetStdHandle(int nStdHandle);
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool GetConsoleScreenBufferInfoEx(IntPtr handle, ref CONSOLE_SCREEN_BUFFER_INFOEX buffer);
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool SetConsoleScreenBufferInfoEx(IntPtr handle, ref CONSOLE_SCREEN_BUFFER_INFOEX buffer);
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool GetConsoleScreenBufferInfo(IntPtr handle, out CONSOLE_SCREEN_BUFFER_INFO buffer);
        public static COLORTABLE ColorsScale(int t = 0)
        {
            var ct = new COLORTABLE();
            byte c = 0;
            int[] ca = new int[16];
            for (int i = 0; i < 16; i++)
            {
                int cs = 0;
                switch (t)
                {
                    case 1:
                        cs = c;
                        break;
                    case 2:
                        cs = c << 8;
                        break;
                    case 3:
                        cs = c << 16;
                        break;
                    default:
                        cs = (c | (c << 8) | (c << 16));
                        break;
                }
                ca[i] = cs;
                c += 16;
            }
            IntPtr ptr = Marshal.AllocHGlobal(16 * 4);
            Marshal.Copy(ca, 0, ptr, 16);
            ct = (COLORTABLE)Marshal.PtrToStructure(ptr, ct.GetType());
            Marshal.FreeHGlobal(ptr);
            return ct;
        }
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool SetConsoleTextAttribute(IntPtr handle, ushort color);
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool FreeConsole();
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool AttachConsole(int pid);
        public static COLORTABLE GetColors(int pid = 0)
        {
            if (pid != 0)
            {
                FreeConsole();
                AttachConsole(pid);
            }
            var sbiex = new CONSOLE_SCREEN_BUFFER_INFOEX();
            sbiex.cbSize = Marshal.SizeOf(sbiex);
            var hout = GetStdHandle(STD_OUTPUT_HANDLE);
            var rrr = GetConsoleScreenBufferInfoEx(hout, ref sbiex);
            COLORTABLE colors = sbiex.ColorTable;
            if (pid != 0)
            {
                FreeConsole();
                AttachConsole(-1);
            }
            return colors;
        }
        public static void SetColors(int scheme, int pid = 0)
        {
            var sbiex = new CONSOLE_SCREEN_BUFFER_INFOEX();
            sbiex.cbSize = Marshal.SizeOf(sbiex);
            if (pid != 0)
            {
                FreeConsole();
                AttachConsole(pid);
            }
            var hout = GetStdHandle(STD_OUTPUT_HANDLE);
            var rrr = GetConsoleScreenBufferInfoEx(hout, ref sbiex);
            COLORTABLE gray = ColorsScale(scheme);
            var e = Marshal.GetLastWin32Error();
            sbiex.ColorTable = gray;
            rrr = SetConsoleScreenBufferInfoEx(hout, ref sbiex);
            e = Marshal.GetLastWin32Error();
            if (pid != 0)
            {
                FreeConsole();
                AttachConsole(-1);
                ResetInOut();
            }
        }
        public static void SetColors(COLORTABLE colors, int pid = 0)
        {
            var sbiex = new CONSOLE_SCREEN_BUFFER_INFOEX();
            sbiex.cbSize = Marshal.SizeOf(sbiex);
            if (pid != 0)
            {
                FreeConsole();
                AttachConsole(pid);
            }
            var hout = GetStdHandle(STD_OUTPUT_HANDLE);
            var rrr = GetConsoleScreenBufferInfoEx(hout, ref sbiex);
            var e = Marshal.GetLastWin32Error();
            sbiex.ColorTable = colors;
            rrr = SetConsoleScreenBufferInfoEx(hout, ref sbiex);
            e = Marshal.GetLastWin32Error();
            if (pid != 0)
            {
                FreeConsole();
                AttachConsole(-1);
                ResetInOut();
            }
        }
        public static void ResetInOut()
        {
            var standardOutput = new StreamWriter(Console.OpenStandardOutput());
            standardOutput.AutoFlush = true;
            Console.SetOut(standardOutput);
            var standardInput = new StreamReader(Console.OpenStandardInput());
            Console.SetIn(standardInput);
            var standardError = new StreamWriter(Console.OpenStandardError());
            standardError.AutoFlush = true;
            Console.SetOut(standardError);
        }
        [DllImport("user32.dll", SetLastError = true)]
        public static extern uint WaitForInputIdle(IntPtr h, uint time = INFINITE);
    }
}