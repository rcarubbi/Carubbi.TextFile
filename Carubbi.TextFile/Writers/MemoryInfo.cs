using System.Runtime.InteropServices;

namespace Carubbi.TextFile.Writers;

internal class MemoryInfo
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    private class MEMORYSTATUSEX
    {
        public uint dwLength;
        public uint dwMemoryLoad;
        public ulong ullTotalPhys;
        public ulong ullAvailPhys;
        public ulong ullTotalPageFile;
        public ulong ullAvailPageFile;
        public ulong ullTotalVirtual;
        public ulong ullAvailVirtual;
        public ulong ullAvailExtendedVirtual;

        public MEMORYSTATUSEX()
        {
            dwLength = (uint)Marshal.SizeOf(typeof(MEMORYSTATUSEX));
        }
    }

    [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool GlobalMemoryStatusEx([In, Out] MEMORYSTATUSEX lpBuffer);

    public static void GetMemoryStatus(out double totalMemoryMB, out double availableMemoryMB)
    {
        var memoryStatus = new MEMORYSTATUSEX();
        if (GlobalMemoryStatusEx(memoryStatus))
        {
            totalMemoryMB = ConvertBytesToMB(memoryStatus.ullTotalPhys);
            availableMemoryMB = ConvertBytesToMB(memoryStatus.ullAvailPhys);
        }
        else
        {
            throw new InvalidOperationException("Failed to retrieve memory status.");
        }
    }
    private static double ConvertBytesToMB(ulong bytes)
    {
        return bytes / (1024.0 * 1024.0);
    }
}
