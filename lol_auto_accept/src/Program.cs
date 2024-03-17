using System;
using System.Collections.Generic;
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
      Console.WriteLine(parentProcess.Id);
      Console.WriteLine(parentProcess.ProcessName);
    }
  }
}
