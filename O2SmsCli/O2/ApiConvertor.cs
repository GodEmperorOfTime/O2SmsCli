using System.Diagnostics;
using O2SmsCli.Api;
using O2SmsCli.O2.PPGwPort;

namespace O2SmsCli.O2;

/// <summary>
/// Konvertujer mezi entitama pro O2 connector a <see cref="ITextSmsSender"/>
/// </summary>
class ApiConvertor
{

  const string SENDER_DESIGNATION = nameof(O2TextSmsSender);

  private readonly O2ConnectorConfig _config;

  public ApiConvertor(O2ConnectorConfig config)
  {
    _config = config ?? throw new ArgumentNullException(nameof(config));
  }

  public SenderResponse CreateErrorResponse(Exception ex, Guid messageId)
  {
    return new SenderResponse(
      messageId: messageId,
      originSenderDesignation: SENDER_DESIGNATION,
      status: ResponseStatus.ForException(ex),
      timestamp: DateTime.Now,
      bonus: Bonus.Empty);
  }

  public SenderResponse CreateResponse(sendResponse o2response, Api.TextSms textSms)
  {
    var r = o2response.result;
    Debug.Assert(
      condition: Guid.TryParse(r.refMsgID, out var refMsgID) && refMsgID == textSms.MessageId,
      message: "Za jakych okolnosti guid z vracene zpravy neni totozny z guidem odesilane zpravy?");
    var status = new ResponseStatus(
      isSuccess: string.Equals(r.type, O2ResponseType.SUCCESS, StringComparison.InvariantCultureIgnoreCase),
      message: r.description);
    return new SenderResponse(
      messageId: textSms.MessageId,
      originSenderDesignation: SENDER_DESIGNATION,
      status: status,
      timestamp: DateTime.Now,
      bonus: CreateBonus(r)
    );
  }

  private static Bonus CreateBonus(Response r)
  {
    var d = new Dictionary<string, object>()
    {
      { nameof(r.msgID), r.msgID } ,
      { nameof(r.refMsgID), r.refMsgID } ,
      { nameof(r.code), r.code } ,
      { nameof(r.baID), r.baID } ,
      { nameof(r.timestamp), r.timestamp ?? DateTime.MinValue } ,
    };
    return new Bonus(d);

  }

  public MessageContainer CreateMessageContainer(Api.TextSms textSms)
  {
    var o2sms = new PPGwPort.TextSms()
    {
      baID = _config.BaId,
      msgID = textSms.MessageId.ToString(),
      toNumber = textSms.Recipient.ToString(),
      validityPeriod = 0, // Tohle je vychozi hodnota. Asi by teoreticky nemuselo byt explicitne
                          // nastaveno.
      multiPart = true, // Smsky s textem delsim nez 160 znaku se poslou na vice casti. Kazda cast je
                        // uctovana zvlast jako samostatna smska. Maximalni delka je pak 900 znaku.
      text = TextSanitizer.SanitizeText(textSms.Text ?? string.Empty),
    };
    if (!string.IsNullOrEmpty(_config.NickName))
    {
      o2sms.fromNumber = _config.NickName;
    }
    return new MessageContainer()
    {
      selector = MessageTypeSelector.TextSms,
      textSms = o2sms
    };
  }

}
