using Microsoft.Extensions.Logging;

namespace O2SmsCli;

/// <summary>
/// Application-wide idecka eventu
/// </summary>
public static class Event
{

  /// <summary>
  /// Nastaveni clienta pro odesilani nactena z configu
  /// </summary>
  public static EventId ClientConfig { get; } = new(23606, nameof(ClientConfig));
}
