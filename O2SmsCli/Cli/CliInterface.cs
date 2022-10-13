using System;
using System.Collections.Generic;
using System.Data.Odbc;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using O2SmsCli.Api;
using O2SmsCli.Configuration;
using O2SmsCli.Misc;
using O2SmsCli.O2;

namespace O2SmsCli.Cli;

class CliInterface
{

  readonly IConfigurationRoot configuration;

  public CliInterface(IConfigurationRoot configuration)
  {
    this.configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
  }

  public async Task RunInteractiveAsync()
  {
    var o2config = O2ConnectorConfigFactory.GetConfig(this.configuration);

    Console.WriteLine("Nacteny config O2 Connectoru:");
    Console.WriteLine(o2config);

    var o2Sender = O2Factory.CreateTextSmsSender(o2config);
    do
    {
      Console.WriteLine();
      Console.WriteLine("Odesilani SMSky (CTRL+C pro zruseni)");
      var sms = GetSms();
      var r = await o2Sender.SendSafeAsync(sms, default);
      Console.WriteLine($"Odeslano: {r}");
    } while (AskUserIfRepeat());
  }

  TextSms GetSms()
  {
    var recipient = GetTelCislo();
    var text = GetUserInput("Text SMSky");
    return TextSms.NewSms(recipient, text);
  }

  TelCislo GetTelCislo()
  {
    var telCisloString = GetUserInput("Tel. cislo");
    return TelCisloParser.TryParse(telCisloString, out var r) ? r : GetTelCislo();
  }

  string GetUserInput(string title) => TryGetUserInput(title, out var r) ? r : GetUserInput(title);

  bool TryGetUserInput(string title, out string userInput)
  {
    Console.Write($"{title}: ");
    userInput = Console.ReadLine() ?? string.Empty;
    return !string.IsNullOrWhiteSpace(userInput);
  }

  bool AskUserIfRepeat()
  {
    var a = GetUserInput("Znova? N - Ukoncit, cokoliv jineho - pokracovat");
    return !string.Equals(a, "N", StringComparison.CurrentCultureIgnoreCase);
  }


}
