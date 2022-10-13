using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace O2SmsCli.Api;

/// <summary>
/// Primo komunikuje s branou. Tzn.: neni zamysleno pro nafrontovani zprav.
/// </summary>
public interface ITextSmsSender
{

  /// <summary>
  /// Nedokladne odesle zpravu. Pokud se nepodari odeslat, neodesle chybu, ale nastavi chybovy status.
  /// Presto muze za uritych oklnosti spadnout na vyjimce: pokud neni zadana <paramref name="textSms"/> 
  /// (tj. je null).
  /// </summary>
  /// <param name="textSms">Odesilana textovka (musi byt zadana - nesmi byt null)</param>
  /// <param name="cancellationToken">Pro zruseni odesilani</param>
  /// <exception cref="ArgumentNullException"></exception>
  Task<SenderResponse> SendSafeAsync(TextSms textSms, CancellationToken cancellationToken);

}

public static class TextSmsSenderExtension
{
  /// <summary>
  /// Nedokladne odesle zpravu. Pokud nastane nejaky problem, vyhodi <see cref="SmsSenderException"/>.
  /// </summary>
  /// <param name="sender">Pomoci ceho odesilat</param>
  /// <param name="textSms">Textovka k odeslani</param>
  /// <param name="cancellationToken"></param>
  /// <returns></returns>
  /// <exception cref="ArgumentNullException"></exception>
  /// <exception cref="SmsSenderException">Pokud se nepodarilo odeslat.</exception>
  public static async Task<SenderResponse> SendAsync(
    this ITextSmsSender sender, TextSms textSms, CancellationToken cancellationToken)
  {
    if (sender is null)
      throw new ArgumentNullException(nameof(sender));
    if (textSms is null)
      throw new ArgumentNullException(nameof(textSms));
    var r = await sender.SendSafeAsync(textSms, cancellationToken);
    return r.Status.IsSuccess ? r : throw new SmsSenderException(r.Status);

  }
}

public class SmsSenderException : Exception
{
  public SmsSenderException(ResponseStatus status)
  {
    Status = status ?? throw new ArgumentNullException(nameof(status));
  }

  public ResponseStatus Status { get; }
}

public class SenderResponse
{
  public SenderResponse(Guid messageId, string originSenderDesignation, ResponseStatus status, DateTime timestamp, Bonus bonus)
  {
    MessageId = messageId;
    SenderDesignation = originSenderDesignation ?? throw new ArgumentNullException(nameof(originSenderDesignation));
    Status = status ?? throw new ArgumentNullException(nameof(status));
    Timestamp = timestamp;
    Bonus = bonus ?? throw new ArgumentNullException(nameof(bonus));
  }


  /// <summary>
  /// Identifikator zpravy
  /// </summary>
  public Guid MessageId { get; }

  /// <summary>
  /// Oznaceni odesilace, ktery response vygeneroval
  /// </summary>
  public string SenderDesignation { get; }

  /// <summary>
  /// Status, ktery obsahuje informaci, jestli se podarilo odeslat
  /// </summary>
  public ResponseStatus Status { get; }

  /// <summary>
  /// Kdy bylo odesilano
  /// </summary>
  public DateTime Timestamp { get; }

  /// <summary>
  /// Dalsi neurcita data, ktera pripadne mohou pomoci pri debugu. 
  /// Muze obsahovat data specificka pro pouzity konektor.
  /// </summary>
  public Bonus Bonus { get; }

  public override string ToString()
  {
    return $"[{MessageId}] {Status}";
  }

}

/// <summary>
/// Neurcita data z odopovedi specificka pro pouzity konektor.
/// </summary>
public class Bonus : IReadOnlyDictionary<string, object>
{
  private readonly IReadOnlyDictionary<string, object> d;

  public Bonus(IReadOnlyDictionary<string, object> d)
  {
    this.d = d ?? throw new ArgumentNullException(nameof(d));
  }

  public object this[string key] => d[key];

  public IEnumerable<string> Keys => d.Keys;

  public IEnumerable<object> Values => d.Values;

  public int Count => d.Count;

  public bool ContainsKey(string key)
  {
    return d.ContainsKey(key);
  }

  public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
  {
    return d.GetEnumerator();
  }

  public bool TryGetValue(string key, [MaybeNullWhen(false)] out object value)
  {
    return d.TryGetValue(key, out value);
  }

  IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)d).GetEnumerator();

  public static Bonus Empty { get; } = new Bonus(new Dictionary<string, object>());

  public override string ToString() => this.d.ToString() ?? string.Empty;

}

public class ResponseStatus
{
  private const string UNKNOWN_MESSAGE_ERROR = "Neznama chyba";
  private const string UNKNOWN_MESSAGE_SUCCESS = "Success";
  private const string UNKNOWN_MESSAGE = "Neznama zprava";

  private const string FAIL = "FAIL";
  private const string SUCCESS = "SUCCESS";
  private const string NA = "n/a";

  public ResponseStatus(bool isSuccess, string message)
    : this(isSuccess, message, null)
  { }

  public ResponseStatus(bool isSuccess, string? message, Exception? exception)
  {
    IsSuccess = isSuccess;
    Message = message ?? UNKNOWN_MESSAGE;
    Exception = exception;
  }

  /// <summary>
  /// True. pokud bylo uspesne odeslano
  /// </summary>
  public bool IsSuccess { get; }

  /// <summary>
  /// Nejaka asi nepodstatna informace, pokud bylo odeslano. Pokud nebyo odeslano (<see cref="IsSuccess"/> je false), tak chybovka.
  /// </summary>
  public string Message { get; }

  /// <summary>
  /// Chybovka, pokud je znama (jinak null). Ne vsechny neuspesne vysledky maji nastavenou exception.
  /// </summary>
  public Exception? Exception { get; }

  public static ResponseStatus ForException(Exception ex) => new(
    isSuccess: false,
    message: ex.Message ?? UNKNOWN_MESSAGE_ERROR,
    exception: ex);


  public static ResponseStatus Error(Exception ex, string message) => new(
    isSuccess: false,
    message: message ?? UNKNOWN_MESSAGE_ERROR,
    exception: null);

  public static ResponseStatus Success(string? message) => new(
    isSuccess: true,
    message: message ?? UNKNOWN_MESSAGE_SUCCESS,
    exception: null);

  public override string ToString()
  {
    var s = this.IsSuccess ? SUCCESS : FAIL;
    var ex = Exception?.Message ?? NA;
    return $"{s}: {Message} (Ex: {ex})";
  }

}
