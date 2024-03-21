using lol_auto_accept.src;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lol_auto_accept
{
  internal class Program
  {
    private static void Main(string[] args)
    {
      var parentProcess = ParentProcessUtil.getParentProcess();
      var isDebugMode = Debugger.IsAttached;

      if (!isDebugMode && parentProcess.ProcessName != "conhost")
      {
        Process.Start(new ProcessStartInfo
        {
          FileName = "conhost",
          WorkingDirectory = Directory.GetCurrentDirectory(),
          Arguments = Process.GetCurrentProcess().MainModule.FileName,
        });
      }

      Console.Title = "LOL Auto Accept";

      LeagueClientUx.isOpenTask();

      Console.ReadKey();
    }
  }
}
