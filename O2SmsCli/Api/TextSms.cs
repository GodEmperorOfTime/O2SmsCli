using System;

namespace O2SmsCli.Api;

public class TextSms
{
  public TextSms(Guid messageId, TelCislo recipient, string text)
  {
    MessageId = messageId;
    Recipient = recipient ?? throw new ArgumentNullException(nameof(recipient));
    Text = text ?? throw new ArgumentNullException(nameof(text));
  }

  public Guid MessageId { get; }
  public TelCislo Recipient { get; }
  public string Text { get; }

  /// <summary>
  /// Vytvori SMSku s novym <see cref="MessageId"/>.
  /// </summary>
  /// <param name="recipient">Prijemce</param>
  /// <param name="text">text smsky</param>
  /// <returns></returns>
  public static TextSms NewSms(TelCislo recipient, string text)
  {
    if (string.IsNullOrWhiteSpace(text))
      throw new ArgumentException($"'{nameof(text)}' cannot be null or whitespace.", nameof(text));
    return new TextSms(Guid.NewGuid(), recipient, text);
  }

  public static TextSms NewSms(ushort mezinarodniPredvolba, string cislo, string text)
    => NewSms(new TelCislo(mezinarodniPredvolba, cislo), text);

  public override string ToString() => $"[{MessageId}] {Recipient}: {Text}";
}

public class TelCislo : IEquatable<TelCislo?>
{

  public ushort MezinarodniPredvolba { get; }

  public string Cislo { get; }

  public TelCislo(ushort mezinarodniPredvolba, string cislo)
  {
    MezinarodniPredvolba = mezinarodniPredvolba;
    Cislo = cislo ?? throw new ArgumentNullException(nameof(cislo));
  }

  public override bool Equals(object? obj)
  {
    return Equals(obj as TelCislo);
  }

  public bool Equals(TelCislo? other)
  {
    return other is not null &&
           MezinarodniPredvolba == other.MezinarodniPredvolba &&
           Cislo == other.Cislo;
  }

  public override int GetHashCode()
  {
    var hashCode = 569542687;
    hashCode = hashCode * -1521134295 + MezinarodniPredvolba.GetHashCode();
    hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Cislo);
    return hashCode;
  }

  public override string ToString() => $"+{MezinarodniPredvolba}{Cislo}";

  public static bool operator ==(TelCislo? left, TelCislo? right)
  {
    return EqualityComparer<TelCislo>.Default.Equals(left, right);
  }

  public static bool operator !=(TelCislo? left, TelCislo? right)
  {
    return !(left == right);
  }

}

