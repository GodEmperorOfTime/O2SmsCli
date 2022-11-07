using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace O2SmsCli.O2;

/// <summary>
/// Osetri text, aby ho bylo mozne odeslat v SMSce (konektor ma problemy s ceskyma znamakam).
/// </summary>
static class TextSanitizer
{
  /// <summary>
  /// Znak, ktery se pouzije, pokud je v textu neznamy non-ASCII znak.
  /// </summary>
  public const char NON_ASCII_PLACEHOLDER = '_';

  /// <summary>
  /// Znaky s diakritikou nahradi jeji verzi bez diakritiky. Pokud obsahuje enznamy none-ascii znak, 
  /// vrati na jeho pozici <see cref="NON_ASCII_PLACEHOLDER"/>.
  /// </summary>
  /// <param name="s">string, ktery je treba osetreit. Nesmi byt null.</param>
  public static string SanitizeText(string s)
  {
    if (s is null)    
      throw new ArgumentNullException(nameof(s));
    var resultChars = new char[s.Length];
    foreach (var i in Enumerable.Range(0, s.Length))
    {
      resultChars[i] = SanitizeChar(s[i]);
    }
    return new string(resultChars);
  }

  static char SanitizeChar(char c)
  {
    return c switch
    {
      // Ceske znaky
      'ě' => 'e',
      'š' => 's',
      'č' => 'c',
      'ř' => 'r',
      'ž' => 'z',
      'ý' => 'y',
      'á' => 'a',
      'í' => 'i',
      'é' => 'e',
      'ú' => 'u',
      'ů' => 'u',
      'ó' => 'o',
      'ď' => 'd',
      'ň' => 'n',
      'ť' => 't',
      'Ě' => 'E',
      'Š' => 'S',
      'Č' => 'C',
      'Ř' => 'R',
      'Ž' => 'Z',
      'Ý' => 'Y',
      'Á' => 'A',
      'Í' => 'I',
      'É' => 'E',
      'Ú' => 'U',
      'Ů' => 'U',
      'Ó' => 'O',
      'Ď' => 'D',
      'Ň' => 'N',
      'Ť' => 'T',
      'Ä' => 'A',
      // Nemecke znaky
      'ä' => 'a',
      'Ö' => 'O',
      'ö' => 'o',
      'Ü' => 'U',
      'ü' => 'u',
      'ß' => 'B',     
      _ => GetPlaceholderForNonAsciiChar(c)
    };
  }

  static char GetPlaceholderForNonAsciiChar(char c)
  {
    return c < 128 ? c : NON_ASCII_PLACEHOLDER; // https://stackoverflow.com/a/18596294
  }

}
