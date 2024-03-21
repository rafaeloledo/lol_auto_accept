using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;

[StructLayout(LayoutKind.Sequential)]
public struct ParentProcessUtil
{
  private IntPtr ExitStatus;
  private IntPtr PebBaseAddress;
  private IntPtr AffinityMask;
  private IntPtr BasePriority;
  private IntPtr UniqueProcessId;
  private IntPtr InheritedFromUniqueProcessId;

  [DllImport("ntdll.dll")]
  private static extern int NtQueryInformationProcess(IntPtr processHandle, int processInformationClass,
    ref ParentProcessUtil processInformation, int processInformationLength, out int returnLength);

  public static Process getParentProcess()
  {
    return getParentProcess(Process.GetCurrentProcess().Handle);
  }

  public static Process getParentProcess(int id)
  {
    Process process = Process.GetProcessById(id);
    return getParentProcess(process.Handle);
  }

  public static Process getParentProcess(IntPtr handle)
  {
    ParentProcessUtil ppu = new ParentProcessUtil();

    int o_Length;

    int status = NtQueryInformationProcess(handle, 0, ref ppu, Marshal.SizeOf(ppu), out o_Length);

    if (status != 0)
      throw new Win32Exception();

    try
    {
      return Process.GetProcessById(ppu.InheritedFromUniqueProcessId.ToInt32());
    }
    catch
    {
      return null;
    }
  }
}
