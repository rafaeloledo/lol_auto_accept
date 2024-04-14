using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace lol_auto_accept.src {
  internal class Accepter {
    public static bool isAutoAcceptOn = true;

    private static bool pickedChamp = false;
    private static bool pickedBan = false;
    private static string lastChatRoom = "";

    public static async Task acceptQueue() {
      while (true) {
        if (isAutoAcceptOn && LeagueClientUpdate.leagueAuth[0] != "") {
          string[] gameSession = LeagueClientUpdate.clientRequest("GET", "lol-gameflow/v1/session");

          if (gameSession[0] == "200" || gameSession[0] == "OK") {
            string phase = gameSession[1].Split(new string[] { "phase" }, StringSplitOptions.None).Last().Split('"')[2];

            switch (phase) {
              case "Lobby":
                await Task.Delay(4000);
                break;
              case "Matchmaking":
                await Task.Delay(1000);
                break;
              case "ReadyCheck":
                LeagueClientUpdate.clientRequest("POST", "lol-matchmaking/v1/ready-check/accept");
                break;
              case "ChampSelect":
                handleChampSelect();
                handlePickOrderSwap();
                break;
              case "InProgress":
                await Task.Delay(9000);
                break;
              case "WaitingForStats":
                await Task.Delay(9000);
                break;
              case "PreEndOfGame":
                await Task.Delay(9000);
                break;
              case "EndOfGame":
                await Task.Delay(5000);
                break;
              default:
                await Task.Delay(1000);
                break;
            }
            if (phase != "ChampSelect") lastChatRoom = "";
          }
          await Task.Delay(50);
        }
        await Task.Delay(800);
      }
    }

    private static void handleQueueRestart() { }
    private static void handleChampSelect() { }
    private static void handleChampSelectChat() { }
    private static void handleChampSelectChatSendMsh() { }
    private static void handleChampSelectActions() { }
    private static void handlePickAction() { }
    private static void handleBanAction() { }
    private static void markPhaseStart() { }
    private static void hoverChampion() { }
    private static void lockChampion(string actId, string championId, string actType) {
      string[] champSelectAction = LeagueClientUpdate
        .clientRequest("PATCH", "lol-champ-select/v1/session/actions/" + actId, "{\"completed\":true,\"championId\":" + championId + "}");

      if (champSelectAction[0] == "204") {
        if (actType == "pick") {
          pickedChamp = true;
        } else if (actType == "ban") {
          pickedBan = true;
        }
      }
    }
    private static void checkLockDelay() { }
    private static void handlePickOrderSwap() { }
    public static void toggle() {
      isAutoAcceptOn = !isAutoAcceptOn;
    }
  }
}
