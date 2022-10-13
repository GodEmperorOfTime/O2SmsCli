using System.Reflection.Metadata;
using System.Runtime.Serialization;
using Microsoft.Extensions.Configuration;

namespace O2SmsCli.Configuration;

static class ConfigFactoryHelper
{
  public static TSection GetRequiredSection<TSection>(
    this IConfigurationRoot configuration, string sectionName)
  {
    return configuration.GetSection(sectionName).Get<TSection>()
      ?? throw new ConfigurationException(
        $"V configu nebyla nelezena sekce '{sectionName}'.");
  }

  /// <summary>
  /// Pokud je splnena podminka <paramref name="assert"/>, vrati <paramref name="value"/>, jinka 
  /// vyhodi <see cref="ConfigurationException"/>.
  /// </summary>
  /// <param name="errorMessage">Chybova zprava vyjimky</param>
  /// <exception cref="ConfigurationException"></exception>
  public static T Guard<T>(this T value, Func<T, bool> assert, string errorMessage)
  {
    return assert.Invoke(value)
      ? value
      : throw new ConfigurationException(errorMessage);
  }

  /// <summary>
  /// Vrati otrimovany <paramref name="s"/>. Pokud nic neobsahuje (nebo obsahuje jen whitespace), vyhodi 
  /// <see cref="ConfigurationException"/>.
  /// </summary>
  /// <param name="s">chraneny string</param>
  /// <param name="errorMessage">Chybova zprava vyjimky</param>
  /// <returns>chraneny string</returns>
  /// <exception cref="ConfigurationException"></exception>
  public static string GuardNonEmptyString(this string? s, string errorMessage)
  {
    return string.IsNullOrWhiteSpace(s)
      ? throw new ConfigurationException(errorMessage)
      : s.Trim();
  }

}

class ConfigurationException : Exception
{
  public ConfigurationException()
  {
  }

  public ConfigurationException(string? message) : base(message)
  {
  }

  public ConfigurationException(string? message, Exception? innerException) : base(message, innerException)
  {
  }

  protected ConfigurationException(SerializationInfo info, StreamingContext context) : base(info, context)
  {
  }

}
