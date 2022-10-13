using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using O2SmsCli.Api;

namespace O2SmsCli.O2;

public static class O2Factory
{

  public static ITextSmsSender CreateTextSmsSender(O2ConnectorConfig config)
  {    
    return new O2TextSmsSender(config);
  }

}
