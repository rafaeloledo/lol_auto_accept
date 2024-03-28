using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace lol_auto_accept.src {
  internal class Accepter {
    public static bool isAutoAcceptOn = true;

    private static bool pickedChamp = false;
    private static bool pickedBan = false;

    public static void acceptQueue() {
      while (true) {
        if (isAutoAcceptOn) {
          string[] gameSession = LeagueClientUpdate.clientRequest("GET", "lol-gameflow/v1/session");

          if (gameSession[0] == "200" || gameSession[0] == "OK") {
            Console.WriteLine(gameSession[1]);
            string phase = gameSession[1].Split(new string[] { "phase" }, StringSplitOptions.None).Last().Split('"')[2];

            switch (phase) {
              case "Lobby":
                //Console.WriteLine("Lobby");
                Thread.Sleep(5000);
                break;
            }

          }
        }
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

  }
}
