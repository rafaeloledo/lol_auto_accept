using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace lol_auto_accept.src
{
  internal class LeagueClientUx
  {
    private static int lcuPid = 0;
    private static string[] leagueAuth;
    public static bool isLolOpen = false;

    public static void isOpenTask()
    {
      while (true)
      {
        Process client = Process.GetProcessesByName("LeagueClientUx").FirstOrDefault();
        if (client != null)
        {
          leagueAuth = getLeagueAuth(client);
        }
      }
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
      Console.WriteLine(output);
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
  }
}
