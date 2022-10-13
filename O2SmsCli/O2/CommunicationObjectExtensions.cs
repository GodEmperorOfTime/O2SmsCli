using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace O2SmsCli.O2;


static class CommunicationObjectExtensions
{
  public static Task CloseAsync(this ICommunicationObject communicationObject)
  {
    if (communicationObject is null)    
      throw new ArgumentNullException(nameof(communicationObject));
    // Kod zkopirovanej z vygenerovanyho kodu stubu pro CloseAsync() 
    return Task.Factory.FromAsync(
      asyncResult: communicationObject.BeginClose(null, null),
      endMethod: new Action<IAsyncResult>(communicationObject.EndClose));
  }

  /// <summary>
  /// Melo by zavrit clienta. Pokud pri zavirani doslo k vyjimce, abortne clienta a vyhodi vyjimku 
  /// (nastalou pri uzavirani).
  /// </summary>
  /// <param name="client">Nesmi byt null</param>
  /// <param name="logger">Logger, ktery se pouzije pro zapis vyjimky, ktera nastala pri abortovani</param>
  /// <returns></returns>
  public static async Task CloseOrAbortServiceChannelAsync(
    this ICommunicationObject client, ILogger? logger = null)
  {
    if (client is null)    
      throw new ArgumentNullException(nameof(client));
    
    // Zalozeno na: https://devzone.channeladam.com/articles/2014/07/how-to-call-wcf-service-properly/
    bool isClosed = client.State == CommunicationState.Closed;

    /*
     * Mezi bloky try a finnaly zamerne nezachytavam vyjimky. Chci, aby probublaly az nahoru.
     */
    try
    {
      if (!isClosed && client.State != CommunicationState.Faulted)
      {
        await client.CloseAsync();
        isClosed = true;
        Debug.WriteLine($"Client {client.GetType()} byl uspesne zavren");
      }
    }
    finally
    {
      // If State was Faulted or any exception occurred while doing the Close(), then do an Abort()
      if (!isClosed)
      {
        AbortServiceChannel(client, logger);
      }
    }
  }

  private static void AbortServiceChannel(ICommunicationObject client, ILogger? logger)
  {
    try
    {
      client.Abort();
      Debug.WriteLine($"Client {client.GetType()} byl uspesne abortovan");
    }
    catch (Exception ex)
    {
      logger?.LogError(ex, "Nastala pri abortovani clienta pote, co nepodarilo zavrit clienta");
      // An unexpected exception that we don't know how to handle.
      // If we are in this situation:
      // - we should NOT retry the Abort() because it has already failed and there is nothing to suggest it could be successful next time
      // - the abort may have partially succeeded
      // - the actual service call may have been successful
      //
      // The only thing we can do is hope that the channel's resources have been released.
      // Do not rethrow this exception because the actual service operation call might have succeeded
      // and an exception closing the channel should not stop the client doing whatever it does next.
      //
      // Perhaps write this exception out to log file for gathering statistics and support purposes...
    }
  }


}
