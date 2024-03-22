using System;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Net.Http;
using System.Net.Http.Headers;

namespace lol_auto_accept.src
{
  internal class LeagueClientUpdate
  {
    private static string[] leagueAuth;
    private static int lcuPid = 0;
    public static bool isLolOpen = false;

    public static void isOpenTask()
    {
      while (true)
      {
        Process client = Process.GetProcessesByName("LeagueClientUx").FirstOrDefault();
        if (client != null)
        {
          leagueAuth = getLeagueAuth(client);
          isLolOpen = true;
          if (lcuPid != client.Id)
          {
            lcuPid = client.Id;
          }
        } else
        {
          isLolOpen = false;
        }
        Thread.Sleep(2000);
        Data.loadChatId();
      }
    }

    public static bool isLeagueClientOpen()
    {
      Process client = Process.GetProcessesByName("LeagueClientUx").FirstOrDefault();
      return client != null ? true : false;
    }

    private static string[] getLeagueAuth(Process client)
    {
      string command = "wmic process where 'Processid=" + client.Id + "' get Commandline";
      ProcessStartInfo psi = new ProcessStartInfo("cmd.exe", "/c " + command);
      psi.RedirectStandardOutput = true;

      Process cmd = new Process();
      cmd.StartInfo = psi;
      cmd.StartInfo.UseShellExecute = false;
      cmd.Start();

      string output = cmd.StandardOutput.ReadToEnd();
      cmd.WaitForExit();

      // Parse the port and auth token into variables
      string port = Regex.Match(output, @"(?<=--app-port=)\d+").Value;
      string authToken = Regex.Match(output, @"(?<=--remoting-auth-token=)[a-zA-Z0-9_-]+").Value;

      // Compute the encoded key
      string auth = "riot:" + authToken;
      string authBase64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(auth));

      // Return content
      return new string[] { authBase64, port };
    }

    public static string[] clientRequest(string method, string url, string body = null)
    {
      var handler = new HttpClientHandler()
      {
        ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
      };

      try
      {
        using (HttpClient client = new HttpClient(handler))
        {
          client.BaseAddress = new Uri("https://127.0.0.1:" + leagueAuth[1] + "/");
          client.DefaultRequestHeaders.Authorization= new AuthenticationHeaderValue("Basic", leagueAuth[0]);

          HttpRequestMessage request = new HttpRequestMessage(new HttpMethod(method), url);

          if (!string.IsNullOrEmpty(body))
          {
            request.Content = new StringContent(body, Encoding.UTF8, "application/json");
          }

          HttpResponseMessage response = client.SendAsync(request).Result;

          if (response == null) {
            return new string[] { "999", "" };
          }

          int statusCode = (int)response.StatusCode;
          string statusString = statusCode.ToString();

          string responseFromServer = response.Content.ReadAsStringAsync().Result;

          response.Dispose();
          return new string[] { statusString, responseFromServer };
        }
      } catch
      {
        return new string[] { "999", "" };
      }

    }

    public static string[] clientRequestUntilSuccess(string method, string url, string body = null)
    {
      string[] request = { "000", "" };

      while (request[0].Substring(0, 1) != "2")
      {
        request = clientRequest(method, url, body);

        if (request[0].Substring(0, 1)  == "2")
        {
          return request;
        } else if (isLeagueClientOpen())
        {
          Thread.Sleep(1000);
        } else
        {
          return request;
        }
      }
      return request;
    }

  }

}
