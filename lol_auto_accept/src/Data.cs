using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace lol_auto_accept.src {
  public class itemList {
    public string name { get; set; }
    public string id { get; set; }
    public string free { get; set; }
  }

  internal class Data {
    public static List<itemList> champsSortered = new List<itemList>();
    public static List<itemList> spellSortered  = new List<itemList>();

    public static string currentSummonerId = "";
    public static string currentChatId = "";

    public static void loadSummonerId() {
      string[] currentSummoner = LeagueClientUpdate.clientRequestUntilSuccess("GET", "lol-summoner/v1/current-summoner");
      currentSummonerId = Regex.Match(currentSummoner[1], @"(?<=""summonerId"":)\d+").Value;
      // Console.WriteLine(currentSummoner[1]);
    }

    public static void loadChatId() {
      string[] myChatProfile = LeagueClientUpdate.clientRequest("GET", "lol-chat/v1/me");
      currentChatId = Regex.Match(myChatProfile[1], @"(?<=""id"":)""(.*?)""").Value.Replace("\"", "");
      //Console.WriteLine(currentChatId);
    }

    public static void loadChampionsList () {
      Console.Clear();

      if(!champsSortered.Any()) {
        loadSummonerId();
      }
    }
  }
}
