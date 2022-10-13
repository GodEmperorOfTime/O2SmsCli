using System.Diagnostics.CodeAnalysis;
using O2SmsCli.Api;

namespace O2SmsCli.Misc;

static class TelCisloParser
{
  public static bool TryParse(string? s, [NotNullWhen(true)] out TelCislo? result)
  {
    const string nums = "0123456789";
    var onlyNums = string.IsNullOrWhiteSpace(s)
      ? string.Empty
      : new string(s.Where(nums.Contains).ToArray());
    return onlyNums.Length switch
    {
      9 => NineDigitNum(onlyNums, out result),
      12 => TwelveDigitNum(onlyNums, out result),
      _ => Fail(out result)
    };
  }

  private static bool NineDigitNum(string onlyNums, out TelCislo result)
  {
    result = new TelCislo(420, onlyNums);
    return true;
  }

  private static bool TwelveDigitNum(string onlyNums, out TelCislo result)
  {
    var predvolba = ushort.Parse(onlyNums[..3]);
    var cislo = onlyNums[4..];
    result = new TelCislo(predvolba, cislo);
    return true;
  }

  private static bool Fail([NotNullWhen(true)] out TelCislo? result)
  {
    result = null;
    return false;
  }
}
