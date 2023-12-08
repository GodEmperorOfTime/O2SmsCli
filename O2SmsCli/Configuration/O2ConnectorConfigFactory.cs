using System.Security.Cryptography.X509Certificates;
using Microsoft.Extensions.Configuration;
using O2SmsCli.Configuration.Nodes;
using O2SmsCli.O2;

namespace O2SmsCli.Configuration;

static class O2ConnectorConfigFactory
{

  internal static O2ConnectorConfig GetConfig(IConfigurationRoot config)
  {
    var node = config.GetRequiredSection<O2ConnectorConfigNode>(O2ConnectorConfigNode.SECTION_NAME);
    return new O2ConnectorConfig(
      EndpointUri: node.EndpointUri.GuardNonEmptyString($"{nameof(O2ConnectorConfigNode.EndpointUri)} musi byt zadan"),
      BaId: node.BaId.GuardNonEmptyString($"{nameof(O2ConnectorConfigNode.BaId)} musi byt zadan"),
      NickName: string.IsNullOrWhiteSpace(node.NickName) ? null : node.NickName.Trim(),
      ClientCertificate: GetClientCertificateConfig(node));
  }

  private static CertificateConfig GetClientCertificateConfig(O2ConnectorConfigNode node)
  {
    var certificateNode = node.ClientCertificate
      ?? throw new ConfigurationException($"{nameof(O2ConnectorConfigNode.ClientCertificate)} musi byt uveden");
    return GetCertificateConfig(certificateNode);
  }

  private static CertificateConfig GetCertificateConfig(CertificateConfigNode certificateNode)
  {
    if (certificateNode is null)
      throw new ArgumentNullException(nameof(certificateNode));
    return new CertificateConfig(
      StoreLocation: ParseRquiredEnum<StoreLocation>(certificateNode.StoreLocation, ""),
      StoreName: ParseRquiredEnum<StoreName>(certificateNode.StoreName, ""),
      Thumbprint: certificateNode.Thumbprint.GuardNonEmptyString(""));
  }

  static TEnum ParseRquiredEnum<TEnum>(string? s, string errorMessage) where TEnum : struct
  {
    return Enum.TryParse<TEnum>(s, out var r) ? r : throw new ConfigurationException(errorMessage);
  }

}
