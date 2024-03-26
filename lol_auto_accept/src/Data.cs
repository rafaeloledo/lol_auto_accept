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
    public bool free { get; set; }
  }

  internal class Data {
    public static List<itemList> champsSortered = new List<itemList>();
    public static List<itemList> spellSortered = new List<itemList>();

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

    public static void loadChampionsList() {
      Console.Clear();

      if (!champsSortered.Any()) {
        loadSummonerId();

        Console.WriteLine("Fetching champions and ownership list");

        List<itemList> champs = new List<itemList>();

        string[] ownedChampions = LeagueClientUpdate
          .clientRequestUntilSuccess("GET", "lol-champions/v1/inventories/" + currentSummonerId + "/champions-minimal");

        Console.Clear();

        string[] champsJSONArray = ownedChampions[1].Split(new string[] { "},{" }, StringSplitOptions.None);

        foreach (var champ in champsJSONArray) {
          string champName = champ.Split(new string[] { "alias\":\"" }, StringSplitOptions.None)[1].Split('"')[0];

          if (champName == "None") {
            continue;
          }

          string champId = champ.Split(new string[] { "id\":" }, StringSplitOptions.None)[1].Split(',')[0];
          string champIsOwned = champ.Split(new string[] { "owned\":" }, StringSplitOptions.None)[1].Split(',')[0];
          string champIsFreeXboxPass = champ.Split(new string[] { "xboxGPReward\":" }, StringSplitOptions.None)[1].Split('}')[0];
          string champIsFree = champ.Split(new string[] { "freeToPlay\":" }, StringSplitOptions.None)[1].Split(',')[0];

          bool isPickable = false;

          if (champIsOwned == "true" || champIsFree == "true" || champIsFreeXboxPass == "true")
            isPickable = true;

          champs.Add(new itemList() { name = champName, id = champId, free = isPickable });
        }

        champsSortered = champs.OrderBy(x => x.name).ToList();
      }
    }
  }
}
