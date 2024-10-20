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
using System.Windows.Forms;

namespace lol_auto_accept {
  internal class App {
#pragma warning disable
    private static async Task Main(string[] args) {
      Console.Title = "LOL Auto Accept";
      Console.OutputEncoding = Encoding.UTF8;

      var taskLeagueIsActive = Task.Run(async () => { await LeagueClientUpdate.isOpenTask(); });
      var taskAcceptQueue = Task.Run(async () => { await Accepter.acceptQueue(); });
      var taskUpdate = Task.Run(async () => {
        await Task.Delay(1000);
        UI.updateAsync(true);
      });

      UI.render();

      var tasks = new[] { taskLeagueIsActive, taskAcceptQueue, taskUpdate };
      Task.WaitAll(tasks);
    }
  }
#pragma warning enable
}
