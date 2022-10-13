using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using O2SmsCli.Api;

namespace O2SmsCli.O2;

public static class O2Factory
{

  public static ITextSmsSender CreateTextSmsSender(
    ILoggerFactory loggerFactory, O2ConnectorConfig config)
  {
    var logger = loggerFactory.CreateLogger<O2.O2TextSmsSender>();
    return new O2TextSmsSender(config, logger);
  }

}
