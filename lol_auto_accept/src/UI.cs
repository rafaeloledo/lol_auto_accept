using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Net.Http;
using System.Runtime.InteropServices;
using lol_auto_accept.util;

namespace lol_auto_accept.src {
  internal class UI {
    private static readonly int MAX_COLUMNS_LENGTH = 38;
    private static readonly int MAX_ROWS_LENGTH = 17;
    private static readonly int SYNC_POS = 20;

    public static string currentWindow = "starting";

    public static async Task render() {
      Console.Clear();
      Console.Write("==== League Auto Accepter ==== \n");
      renderClientStatus();
      Menu();
      handleUserInput();
    }

    public static async Task updateAsync(bool loop) {
      while (true) {
        if (currentWindow == "menu") {
          // Wait for printing sync
          await Task.Delay(250);
          Console.CursorVisible = false;
          Console.SetCursorPosition(0, SYNC_POS);
          Console.ForegroundColor = ConsoleColor.DarkYellow;
          Console.Write("Syncing...");
          Console.ForegroundColor = ConsoleColor.White;
          
          // Update
          //Console.CursorVisible = false;
          renderClientStatus();
          renderCurrentSettings();
          
          // Wait for remove syncing... mesage
          await Task.Delay(1500);
          Console.SetCursorPosition(0, SYNC_POS);
          Console.Write("          ");

          // Back to normal state waiting
          Console.SetCursorPosition(24, 17);
          Console.CursorVisible = true;
          await Task.Delay(5000);
        }
        if (!loop) return; // ends the loop if desired
      }
    }

    private static void renderClientStatus() {
      Console.SetCursorPosition(0, 1);
      if (LeagueClientUpdate.isLolOpen) {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.Write("LOL IS OPEN             ");
      } else {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.Write("WARNING: OPEN THE CLIENT");
      }
      Console.ResetColor();
    }

    private static void renderAutoAccepterStatus() {
      if (Accepter.isAutoAcceptOn) {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.Write("AUTO ACCEPT: ON         ");
      } else {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.Write("WARNING: AUTO ACCEPT OFF");
      }
      Console.ResetColor();
    }

    private static void renderInstalockPickStatus() {
      if (Settings.instalockPick) {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.Write("INSTALOCK PICK ON          ");
      } else {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.Write("WARNING: INSTALOCK PICK OFF");
      }
      Console.ResetColor();
    }

    private static void renderInstalockBanStatus() {
      if (Settings.instalockBan) {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.Write("INSTALOCK BAN ON          ");
      } else {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.Write("WARNING: INSTALOCK BAN OFF");
      }
      Console.ResetColor();
    }

    private static void renderCurrentSettings() {
      Console.WriteLine();
      Console.SetCursorPosition(0, 3);
      Console.Write("-- Current Settings --");
      Console.SetCursorPosition(0, 4);
      Console.Write("Current auto selection champion..." + Settings.currentChamp[0]);
      Console.SetCursorPosition(0, 5);
      Console.Write("Current auto selection ban..." + Settings.currentBan[0]);
      Console.SetCursorPosition(0, 6);
      renderAutoAccepterStatus();
      Console.SetCursorPosition(0, 7);
      renderInstalockPickStatus();
      Console.SetCursorPosition(0, 8);
      renderInstalockBanStatus();
      Console.WriteLine();
    }

    private static void Menu() {
      currentWindow = "menu";
      renderCurrentSettings();
      Console.WriteLine("\n-- Menu --");
      Console.ForegroundColor = ConsoleColor.DarkCyan;
      Console.WriteLine("1 - Select Champion Pick");
      Console.WriteLine("2 - Select Champion Ban");
      Console.WriteLine("3 - Toggle Auto Accept");
      Console.WriteLine("4 - Toggle Instalock Pick");
      Console.WriteLine("5 - Toggle Instalock Ban");
      Console.ForegroundColor = ConsoleColor.White;
    }

    private static void handleUserInput() {
      bool control = true;
      //Console.CursorVisible = true;
      Console.Write("\nChoose an option above: ");
      while (control) {
        ConsoleKeyInfo keyInfo = Console.ReadKey(true);

        switch (keyInfo.Key) {
          case ConsoleKey.D1:
            Settings.currentChamp[0] = handleCurrentChampion();
            break;
          case ConsoleKey.D2:
            Settings.currentBan[0] = handleCurrentChampion();
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

    public static string handleCurrentChampion() {
      currentWindow = "query";
      if (!LeagueClientUpdate.isLolOpen) {
        MessageBoxIcon icon = MessageBoxIcon.Error;
        MessageBox.Show("LEAGUE CLIENT IS NOT OPEN", "ERROR", MessageBoxButtons.OK, icon);
        return "";
      }

      StringBuilder query = new StringBuilder("");
      string currentChamp = "";
      int lastQueryLength = 0;
      renderInput("Query champion (ENTER: confirm): ");
      //Console.CursorVisible = false;
      Console.SetCursorPosition(33, 0);

      while (true) {
        ConsoleKeyInfo cki = Console.ReadKey(true);

        switch (cki.Key) {
          case ConsoleKey.D1:
          case ConsoleKey.D2:
          case ConsoleKey.D3:
          case ConsoleKey.D4:
          case ConsoleKey.D5:
          case ConsoleKey.D6:
          case ConsoleKey.D7:
          case ConsoleKey.D8:
          case ConsoleKey.D9:
            break;
          case ConsoleKey.Backspace:
            if (query.Length > 0) {
              query.Length--;
              renderInput($"Query champion (ENTER: confirm): {(query != null ? query.ToString().ToLower() : "")}");
            }
            break;
          case ConsoleKey.Escape:
            if (currentWindow != "menu")
              _ = render();
            return "";
          case ConsoleKey.Enter:
            _ = render();
            return currentChamp;
          default:
            query.Append(cki.Key);
            renderInput($"Query champion (ENTER: confirm): {(query != null ? query.ToString().ToLower() : "")}");
            Console.WriteLine();

            if (lastQueryLength > 0)
              clearLastQueryOutput(lastQueryLength);

            lastQueryLength = 0;

            for (int i = 0; i < Data.champsSortered.Count; i++) {
              if (Data.champsSortered[i].name.IndexOf(query.ToString(), StringComparison.CurrentCultureIgnoreCase) >= 0) {
                string champ = Data.champsSortered[i].name;
                Console.WriteLine(champ);
                lastQueryLength++;
                currentChamp = champ;
              }
            }

            Console.SetCursorPosition(33 + query.Length, 0);
            break;
        }
      }
    }

    private static void clearLastQueryOutput(int lastQueryLength) {
      for (int i = 1; i < lastQueryLength + 1; i++) {
        Console.SetCursorPosition(0, i);
        Console.Write("                  ");
      }
      Console.SetCursorPosition(0, 1);
    }

    private static void renderInput(string output) {
      clearMenu();
      Console.SetCursorPosition(0, 0);
      Console.Write(output);
    }

    private static void clearMenu() {
      for (int i = 0; i < MAX_ROWS_LENGTH + 1; i++) {
        Console.SetCursorPosition(0, i);
        for (int j = 0; j < MAX_COLUMNS_LENGTH; j++) {
          Console.Write(" ");
        }
      }
    }

    private static void handleToggle() {
      Accepter.toggle();
      _ = updateAsync(false);
    }
    private static void handleToggleInstalockBan() {
      Settings.instalockBan = !Settings.instalockBan;
      _ = updateAsync(false);
    }

    private static void handleToggleInstalockPick() {
      Settings.instalockPick = !Settings.instalockPick;
      _ = updateAsync(false);
    }
  }
}
