using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace O2SmsCli.Configuration;

static class ConfigReader
{

  public static IConfigurationRoot ReadConfig<T>(string[] args) where T : class
  {
    return new ConfigurationBuilder()
      .AddJsonFile("appsettings.json", false)
      .AddUserSecrets<T>()
      .AddEnvironmentVariables()
      .AddCommandLine(args)
      .Build();
  }

}
