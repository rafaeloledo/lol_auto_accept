using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace lol_auto_accept.util {
  internal class MyTimer {
    static Stopwatch stopwatch = new Stopwatch();

    public static void start() {
      stopwatch.Start();
    }

    public static void stop() {
      stopwatch.Stop();
      Console.WriteLine("Time spent=" + stopwatch.ElapsedMilliseconds + "ms");
    }
  }
}
