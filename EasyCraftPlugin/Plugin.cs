using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

namespace EasyCraftPlugin
{
    public class Plugin
    {
        //下方修改成你的插件信息
        public static string id = "top.easycraft.plugin.bdshooker"; //插件ID 请尽量唯一
        public static string name = "BDS 插件注入"; //插件名称
        public static string author = "Kengwang @ EasyCraft Team"; //作者名称
        public static string description = "可以注入 BDS DLL插件"; //插件简介
        public static string version = "1.0.0"; //插件版本
        public static string link = "https://www.easycraft.top"; //插件地址
        //上方修改成你的插件信息

// 此处为你想要Hook的点位,如果不在此处填写将会无法收到相关事件
        private static Dictionary<string, int> Hooks = new Dictionary<string, int>()
        {
            // {"Hook名称","优先级"}
            // 优先级数字越大越先调用 ,请取 1 - 10 以内的数字,综合考虑,不要太极端,避免和其他插件造成冲突
            {"ServerWillStart", 5} //MC服务器准备开始运行 - 可拦截
        };
        // 上方为你想要Hook的点位,如果不在此处填写将会无法收到相关事件

        //此处为你想要申请的权限,如果未在此处填写但是却尝试执行相关操作将会忽略
        private static string[] Auth =
        {
        };
        //上方为你想要申请的权限,如果未在此处填写但是却尝试执行相关操作将会忽略

        /// <summary>
        /// 当开始加载插件时调用,会返回插件信息,请不要阻塞此方法或改变返回格式
        /// 请注意! 请不要再此方法下调用 EasyCraft API,所有在此方法下调用的操作都将被忽略
        /// </summary>
        /// <returns>插件信息</returns>
        public static object Initialize(Type PluginBack, string key)
        {
            Settings.PluginCallback = PluginBack;
            Settings.key = key;
            return (object) new PluginInfo
            {
                id = id,
                name = name,
                author = author,
                description = description,
                link = link,
                version = version,
                hooks = Hooks,
                auth = Auth
            };
        }

        /// <summary>
        /// 当插件被启用时返回
        /// </summary>
        public static void OnEnable()
        {
            //我们建议您使用 FastConsole 指令,此指令会将输出内容定向到 EasyCraft 日志程序,从而使用户更好的反馈错误
            FastConsole.PrintInfo("成功启用插件!");
        }

        /// <summary>
        /// 服务器将要开启
        /// </summary>
        public static bool ServerWillStart(int sid, Process p)
        {
            p.StartInfo.StandardInputEncoding = Encoding.UTF8;
            p.StartInfo.StandardOutputEncoding = Encoding.UTF8;
            p.StartInfo.StandardErrorEncoding = Encoding.UTF8;
            try
            {
                foreach (string file in Directory.EnumerateFiles(
                    Path.GetDirectoryName(p.StartInfo.FileName) + "/plugin", "*.dll"))
                {
                    FastConsole.PrintInfo("正在尝试Hook: Plugin Path: " + file);
                    p.Start();
                    p.BeginErrorReadLine();
                    p.BeginOutputReadLine();
                    Hook(p, file);
                }
            }
            catch (Exception e)
            {
                FastConsole.PrintInfo(e.ToString());
            }

            return false;
        }

        /// <summary>
        /// 服务器已经被开启
        /// </summary>
        public static bool ServerStarted(int sid)
        {
            FastConsole.PrintInfo("监听到服务器已经开启事件: " + sid);
            return true;
        }

        /******************* Hooker Start ***********************
        [DllImport("kernel32.dll")]
        public static extern IntPtr OpenProcess(int dwDesiredAccess, bool bInheritHandle, int dwProcessId);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr GetModuleHandle(string lpModuleName);

        [DllImport("kernel32", CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = true)]
        static extern IntPtr GetProcAddress(IntPtr hModule, string procName);

        [DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)]
        static extern IntPtr VirtualAllocEx(IntPtr hProcess, IntPtr lpAddress,
            IntPtr dwSize, uint flAllocationType, uint flProtect);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern int WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] buffer, uint size,
            int lpNumberOfBytesWritten);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern int GetLastError();

        [DllImport("kernel32.dll")]
        public static extern IntPtr GetModuleHandleA(string name);

        [DllImport("kernel32.dll")]
        static extern IntPtr CreateRemoteThread(IntPtr hProcess,
            IntPtr lpThreadAttributes, uint dwStackSize, IntPtr lpStartAddress, IntPtr lpParameter,
            uint dwCreationFlags, IntPtr lpThreadId);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern int CloseHandle(IntPtr hObject);

        // privileges
        const int PROCESS_CREATE_THREAD = 0x0002;
        const int PROCESS_QUERY_INFORMATION = 0x0400;
        const int PROCESS_VM_OPERATION = 0x0008;
        const int PROCESS_VM_WRITE = 0x0020;
        const int PROCESS_VM_READ = 0x0010;

        // used for memory allocation
        const uint MEM_COMMIT = 0x00001000;
        const uint MEM_RESERVE = 0x00002000;
        const uint PAGE_READWRITE = 4;

        public static bool Hook(Process name, string dllname)
        {
            int ok1;
            //int ok2;
            //int hwnd;
            IntPtr baseaddress;
            IntPtr hack;
            IntPtr yan;
            //Console.WriteLine("Welcome To Use KWRunner DLL Injecter! Powered by Kengwang (github@kengwang)");
            uint dlllength;
            dlllength = (uint) ((dllname.Length + 1) * Marshal.SizeOf(typeof(char)));

            //Console.WriteLine("First Let's set the charset");
            IntPtr lpSetConsoleCP = GetProcAddress(GetModuleHandleA("Kernel32.dll"), "SetConsoleCP");
            IntPtr lpSetConsoleOutputCP = GetProcAddress(GetModuleHandleA("Kernel32.dll"), "SetConsoleOutputCP");
            if (CreateRemoteThread(name.Handle, IntPtr.Zero, 0, lpSetConsoleCP, (IntPtr) 65001, 0, IntPtr.Zero) ==
                IntPtr.Zero)
            {
                Console.WriteLine("Charset Error! Code: " + GetLastError().ToString());
            }

            if (CreateRemoteThread(name.Handle, IntPtr.Zero, 0, lpSetConsoleOutputCP, (IntPtr) 65001, 0, IntPtr.Zero) ==
                IntPtr.Zero)
            {
                Console.WriteLine("Charset 2 Error! Code: " + GetLastError().ToString());
            }

            IntPtr procHandle =
                OpenProcess((0x2 | 0x8 | 0x10 | 0x20 | 0x400), true, name.Id);
            if (procHandle == IntPtr.Zero)
            {
                Console.WriteLine("Open Process Failed: Code: " + GetLastError().ToString());
                return false;
            }

            //IntPtr procHandle = name.Handle;
            baseaddress =
                VirtualAllocEx(procHandle, IntPtr.Zero, (IntPtr) dllname.Length, (0x1000 | 0x2000), 0x40); //申请内存空间
            if (baseaddress == IntPtr.Zero) //返回0则操作失败，下面都是
            {
                Console.WriteLine("Request for RAM Space Failed! Code: " + GetLastError().ToString());
                return false;
            }

            byte[] bytes = Encoding.ASCII.GetBytes(dllname);
            ok1 = WriteProcessMemory(procHandle, baseaddress, bytes, (uint) bytes.Length, 0); //写内存
            if (ok1 == 0)
            {
                Console.WriteLine("Writing RAM Failed! Code: " + GetLastError().ToString());
                return false;
            }

            hack = GetProcAddress(GetModuleHandleA("Kernel32.dll"), "LoadLibraryA"); //取得loadlibarary在kernek32.dll地址

            if (hack == IntPtr.Zero)
            {
                Console.WriteLine("Getting LoadLibraryA Failed! Code: " + GetLastError().ToString());
                return false;
            }

            yan = CreateRemoteThread(procHandle, IntPtr.Zero, 0, hack, baseaddress, 0, IntPtr.Zero);
            if (yan == IntPtr.Zero)
            {
                //VirtualFreeEx(name.Handle, baseaddress, 0, 0x00008000);
                Console.WriteLine("Creating Remote Thread Failed! Code: " + GetLastError().ToString());
                return false;
            }
            else
            {
                CloseHandle(procHandle);
                Console.WriteLine("Successfully Inject Dll!");
                return true;
            }
        }*/
        
        static readonly IntPtr INTPTR_ZERO = (IntPtr)0;

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern IntPtr OpenProcess(uint dwDesiredAccess, int bInheritHandle, uint dwProcessId);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern int CloseHandle(IntPtr hObject);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern IntPtr GetProcAddress(IntPtr hModule, string lpProcName);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern IntPtr GetModuleHandle(string lpModuleName);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern IntPtr VirtualAllocEx(IntPtr hProcess, IntPtr lpAddress, IntPtr dwSize, uint flAllocationType, uint flProtect);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern int WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] buffer, uint size, int lpNumberOfBytesWritten);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern IntPtr CreateRemoteThread(IntPtr hProcess, IntPtr lpThreadAttribute, IntPtr dwStackSize, IntPtr lpStartAddress,
            IntPtr lpParameter, uint dwCreationFlags, IntPtr lpThreadId);
        
        public static bool Hook(Process p, string sDllPath)
        {
            uint pToBeInjected = (uint)p.Id;
            IntPtr hndProc = OpenProcess((0x2 | 0x8 | 0x10 | 0x20 | 0x400), 1, pToBeInjected);

            if (hndProc == INTPTR_ZERO)
            {
                return false;
            }

            IntPtr lpLLAddress = GetProcAddress(GetModuleHandle("kernel32.dll"), "LoadLibraryA");

            if (lpLLAddress == INTPTR_ZERO)
            {
                return false;
            }

            IntPtr lpAddress = VirtualAllocEx(hndProc, (IntPtr)null, (IntPtr)sDllPath.Length, (0x1000 | 0x2000), 0X40);

            if (lpAddress == INTPTR_ZERO)
            {
                return false;
            }

            byte[] bytes = Encoding.ASCII.GetBytes(sDllPath);

            if (WriteProcessMemory(hndProc, lpAddress, bytes, (uint)bytes.Length, 0) == 0)
            {
                return false;
            }

            if (CreateRemoteThread(hndProc, (IntPtr)null, INTPTR_ZERO, lpLLAddress, lpAddress, 0, (IntPtr)null) == INTPTR_ZERO)
            {
                return false;
            }

            CloseHandle(hndProc);

            return true;
        }
    }
}