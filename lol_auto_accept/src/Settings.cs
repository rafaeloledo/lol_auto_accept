using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lol_auto_accept.src {
  internal class Settings {
    public static string[] currentChamp = { "N/A", "0" };
    public static string[] currentBan = { "N/A", "0" };

    public static bool instalockBan = false;
    public static bool instalockPick = false;
  }
}
