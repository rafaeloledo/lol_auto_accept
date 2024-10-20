using System;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;

namespace lol_auto_accept.src {
  internal class LeagueClientUpdate {
    public static string[] leagueAuth = { "", "" };
    private static int lcuPid = 0;
    public static bool isLolOpen = false;

    public static async Task isOpenTask() {
      while (true) {
        //Console.WriteLine("isOpenTask");
        Process client = Process.GetProcessesByName("LeagueClientUx").FirstOrDefault();
        if (client != null) {
          leagueAuth = getLeagueAuth(client);
          isLolOpen = true;
          if (lcuPid != client.Id) {
            lcuPid = client.Id;
            if (Data.champsSortered.Count == 0) {
              Data.loadChampionsList();
            }
          }
        } else {
          isLolOpen = false;
          Data.champsSortered.Clear();
          Data.spellSortered.Clear();
          Data.currentSummonerId = "";
        }
        await Task.Delay(1000);
      }
    }

    public static bool isLeagueClientOpen() {
      Process client = Process.GetProcessesByName("LeagueClientUx").FirstOrDefault();
      return client != null ? true : false;
    }

    private static string[] getLeagueAuth(Process client) {
      ProcessStartInfo psi = new ProcessStartInfo(
        "powershell.exe",
        "-Command \"Get-Process -Id " + client.Id + " | Select-Object -ExpandProperty CommandLine\""
      );

      psi.RedirectStandardOutput = true;

      Process cmd = new Process();
      cmd.StartInfo = psi;
      cmd.StartInfo.UseShellExecute = false;
      cmd.Start();

      string output = cmd.StandardOutput.ReadToEnd();
      cmd.WaitForExit();

      string auth = "riot:" + Regex.Match(output, @"(?<=--remoting-auth-token=)[a-zA-Z0-9_-]+").Value;

      return new string[] {
        Convert.ToBase64String(Encoding.UTF8.GetBytes(auth)),
        Regex.Match(output, @"(?<=--app-port=)\d+").Value
      };
    }

    public static string[] clientRequest(string method, string url, string body = null) {
      var handler = new HttpClientHandler() {
        ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
      };

      try {
        using (HttpClient client = new HttpClient(handler)) {
          client.BaseAddress = new Uri("https://127.0.0.1:" + leagueAuth[1] + "/");
          client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", leagueAuth[0]);

          HttpRequestMessage request = new HttpRequestMessage(new HttpMethod(method), url);

          if (!string.IsNullOrEmpty(body))
            request.Content = new StringContent(body, Encoding.UTF8, "application/json");

          HttpResponseMessage response = client.SendAsync(request).Result;

          if (response == null) return new string[] { "999", "" };

          string[] output = new string[] {
            ((int)response.StatusCode).ToString(),
            response.Content.ReadAsStringAsync().Result
          };

          response.Dispose();

          return output;
        }
      } catch {
        return new string[] { "999", "" };
      }
    }

    public static string[] clientRequestUntilSuccess(string method, string url, string body = null) {
      string[] response = { "000", "" };

      while (response[0][0] != '2' && response[0] != "OK") {
        response = clientRequest(method, url, body);
        if (isLeagueClientOpen()) Task.Delay(1000);
      };

      return response;
    }
  }
}
