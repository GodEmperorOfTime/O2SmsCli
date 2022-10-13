using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;
using O2SmsCli.Api;
using O2SmsCli.O2.PPGwPort;

namespace O2SmsCli.O2;

class O2TextSmsSender : ITextSmsSender
{

  private readonly O2ConnectorConfig _config;


  public O2TextSmsSender(O2ConnectorConfig config)
  {
    this._config = config ?? throw new ArgumentNullException(nameof(config));
  }

  public async Task<SenderResponse> SendSafeAsync(Api.TextSms textSms, CancellationToken cancellationToken)
  {
    if (textSms is null) throw new ArgumentNullException(nameof(textSms));
    var convertor = new ApiConvertor(this._config);
    try
    {
      var messageContainer = convertor.CreateMessageContainer(textSms);
      var o2response = await SendUnsafeAsync(messageContainer);
      return convertor.CreateResponse(o2response, textSms);
    }
    catch(Exception ex)
    {
      return convertor.CreateErrorResponse(ex, textSms.MessageId);
    }
  }

  async Task<sendResponse> SendUnsafeAsync(MessageContainer messageContainer)
  {
    var client = CreateClient();
    try
    {      
      var response = await client.sendAsync(null, messageContainer);
      return response;
    }
    finally
    {

      /*
       * Kod ve finally by se mel provest i v pripade, ze je v try blok konci returnem. Takze by vzdy 
       * melo dojit k zavreni.
       * Control is passed to the finally block when control leaves the try block normally -- that is, by 
       * a *return*, goto, break, continue, or simply falling off the end. Control is passed to the finally 
       * block when control leaves the try block via an exception that has been caught by an enclosing 
       * catch block. 
       * https://stackoverflow.com/a/10260233
       * https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/try-finally
       */

      // This will:
      // - be executed if any exception was thrown above in the 'try' (including ThreadAbortException); and
      // - ensure that CloseOrAbortServiceChannel() itself will not be interrupted by a ThreadAbortException
      //   (since it is executing from within a 'finally' block)
      await client.CloseOrAbortServiceChannelAsync();
    }
  }

  private PPGwClient CreateClient()
  {
    var binding = new BasicHttpsBinding();
    binding.Security.Mode = BasicHttpsSecurityMode.Transport;
    binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Certificate;
    var remoteAddress = new EndpointAddress(_config.EndpointUri);
    var client = new PPGwClient(binding, remoteAddress);
    client.ClientCredentials.ClientCertificate.SetCertificate(
      storeLocation: _config.ClientCertificate.StoreLocation,
      storeName: _config.ClientCertificate.StoreName,
      findType: X509FindType.FindByThumbprint,
      findValue: _config.ClientCertificate.Thumbprint);
    return client;
  }


}
