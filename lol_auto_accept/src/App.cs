using lol_auto_accept.src;
using lol_auto_accept.util;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace lol_auto_accept {
  internal class App {
    private static void Main(string[] args) {
      Console.Title = "LOL Auto Accept";
      Console.OutputEncoding = Encoding.UTF8;

      var taskAcceptQueue = new Task(Accepter.acceptQueue);
      taskAcceptQueue.Start();
      var taskLeagueIsActive = new Task(LeagueClientUpdate.isOpenTask);
      taskLeagueIsActive.Start();

      var tasks = new[] { taskLeagueIsActive, taskAcceptQueue };
      Task.WaitAll(tasks);
    }
  }
}
