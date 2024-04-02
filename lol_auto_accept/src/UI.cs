using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Net.Http;

namespace lol_auto_accept.src {
  internal class UI {
    public static string currentWindow = "";

    public static void render() {
      Console.Clear();
      Console.Write("==== League Auto Accepter ==== (ESC: globally reset UI)\n");
      renderClientStatus();
      Menu();
      handleReload();
    }

    private static void renderClientStatus() {
      if (LeagueClientUpdate.isLolOpen) {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("LOL IS OPEN");
      } else {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine("WARNING: OPEN THE CLIENT");
      }
      Console.ResetColor();
    }

    private static void renderAutoAccepterStatus() {
      if (Accepter.isAutoAcceptOn) {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("AUTO ACCEPT: ON");
      } else {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine("WARNING: AUTO ACCEPT OFF");
      }
      Console.ResetColor();

    }

    private static void renderInstalockPickStatus() {
      if (Settings.instalockPick) {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("INSTALOCK PICK ON");
      } else {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine("WARNING: INSTALOCK PICK OFF");
      }
      Console.ResetColor();

    }

    private static void renderInstalockBanStatus() {
      if (Settings.instalockBan) {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("INSTALOCK BAN ON");
      } else {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine("WARNING: INSTALOCK BAN OFF");
      }
      Console.ResetColor();

    }

    private static void Menu() {
      if (currentWindow == "")
        currentWindow = "menu";
      Console.WriteLine("\n-- Current Settings --");
      Console.WriteLine("Current auto selection champion..." + Settings.currentChamp[0]);
      Console.WriteLine("Current auto selection ban..." + Settings.currentBan[0]);
      renderAutoAccepterStatus();
      renderInstalockPickStatus();
      renderInstalockBanStatus();
      Console.WriteLine("\n-- Menu --");
      Console.WriteLine("1 - Select Champion Pick");
      Console.WriteLine("2 - Select Champion Ban");
      Console.WriteLine("3 - Toggle Auto Accepter");
      Console.WriteLine("4 - Toggle Instalock Pick");
      Console.WriteLine("5 - Toggle Instalock Ban");
      handleUserInput();
    }

    private static void handleReload(string windowToReload = "none") {
      if (windowToReload == "none") {
        return;
      }
    }

    private static void handleUserInput() {
      bool control = true;
      Console.CursorVisible = true;
      Console.Write("\nChoose an option above: ");
      while (control) {
        ConsoleKeyInfo keyInfo = Console.ReadKey(true);

        switch (keyInfo.Key) {
          case ConsoleKey.Escape:
            render();
            break;
          case ConsoleKey.D1:
            handleCurrentChampion();
            break;
          case ConsoleKey.D2:
            handleCurrentBan();
            break;
          case ConsoleKey.D3:
            handleToggle();
            break;
          case ConsoleKey.D4:
            handleToggleInstalockPick();
            break;
          case ConsoleKey.D5:
            handleToggleInstalockBan();
            break;
          default:
            break;
        }
      }
    }

    public static void handleCurrentChampion() {
      if (!LeagueClientUpdate.isLolOpen) {
        MessageBoxIcon icon = MessageBoxIcon.Error;
        MessageBox.Show("LEAGUE CLIENT IS NOT OPEN", "ERROR", MessageBoxButtons.OK, icon);
        return;
      }

      string query = "";
      string currentChamp = "";
      render("Query champion (ENTER: confirm): ", true);
      Console.CursorVisible = false;
      Console.SetCursorPosition(33, 0);

      while (true) {
        ConsoleKeyInfo cki = Console.ReadKey(true);

        switch (cki.Key) {
          case ConsoleKey.Escape:
            render();
            return;
          case ConsoleKey.Enter:
            Settings.currentChamp[0] = currentChamp;
            render();
            return;
          default:
            query += cki.Key;
            Console.WriteLine();
            render($"Query champion (ENTER: confirm): {(query != null ? query.ToString().ToLower() : "")}", true);
            for (int i = 0; i < Data.champsSortered.Count; i++) {
              if (Data.champsSortered[i].name.IndexOf(query, StringComparison.CurrentCultureIgnoreCase) >= 0) {
                string champ = Data.champsSortered[i].name;
                Console.WriteLine(champ);
                currentChamp = champ;
              }
            }
            Console.SetCursorPosition(33, 0);
            break;
        }
      }
    }

    private static void render(string text, bool clear) {
      if (clear)
        Console.Clear();

      Console.WriteLine(text);
    }

    private static void handleToggle() {
      Accepter.toggle();
      render();
    }
    private static void handleToggleInstalockBan() {
      Settings.instalockBan = !Settings.instalockBan;
      render();
    }

    private static void handleToggleInstalockPick() {
      Settings.instalockPick = !Settings.instalockPick;
      render();
    }

    private static void handleCurrentBan() {
      string query = "";
      string currentBan = "";

      Console.Clear();
      Console.Write("Query ban (ENTER: confirm): ");

      while (true) {
        ConsoleKeyInfo cki = Console.ReadKey();

        switch (cki.Key) {
          case ConsoleKey.Escape:
            render();
            return;
          case ConsoleKey.Enter:
            Settings.currentBan[0] = currentBan;
            render();
            return;
          default:
            query += cki.Key;
            Console.WriteLine();
            for (int i = 0; i < Data.champsSortered.Count; i++) {
              if (Data.champsSortered[i].name.StartsWith(query, true, null)) {
                string champ = Data.champsSortered[i].name;
                Console.WriteLine(champ);
                currentBan = champ;
              }
            }
            break;
        }
      }
    }

  }
}
