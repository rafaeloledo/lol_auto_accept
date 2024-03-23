using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace lol_auto_accept.src {
  internal class Data {
    public static string currentSummonerId = "";
    public static string currentChatId = "";

    public static void loadSummonerId() {
      string[] currentSummoner = LeagueClientUpdate.clientRequestUntilSuccess("GET", "lol-summoner/v1/current-summoner");
      Console.WriteLine(currentSummoner[0]);
      currentSummonerId = Regex.Match(currentSummoner[1], @"(?<=""summonerId"":)\d+").Value;
    }

    public static void loadChatId() {
      string[] myChatProfile = LeagueClientUpdate.clientRequest("GET", "lol-chat/v1/me");
      currentChatId = Regex.Match(myChatProfile[1], @"(?<=""id"":)""(.*?)""").Value.Replace("\"", "");
      Console.WriteLine(currentChatId);
    }
  }
}
