using System.Security.Cryptography.X509Certificates;
using System.Text;
using Microsoft.Extensions.Primitives;

namespace O2SmsCli.O2;

public record O2ConnectorConfig(
  string EndpointUri,
  string BaId,
  CertificateConfig ClientCertificate
)
{
  public override string ToString()
  {
    return new StringBuilder()
      .Append($"EndpointUri: ").AppendLine(EndpointUri)
      .Append($"BaId: ").AppendLine(BaId)
      .AppendLine($"ClientCertificate:")
      .Append("  StoreLocation: ").AppendLine(ClientCertificate.StoreLocation.ToString())
      .Append("  StoreName: ").AppendLine(ClientCertificate.StoreName.ToString())
      .Append("  Thumbprint: ").Append(ClientCertificate.Thumbprint)
      .ToString();
  }
}

public record CertificateConfig(
  StoreLocation StoreLocation,
  StoreName StoreName,
  string Thumbprint
);
