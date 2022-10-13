using System.Security.Cryptography.X509Certificates;

namespace O2SmsCli.O2;

public record O2ConnectorConfig(
  string EndpointUri,
  string BaId,
  CertificateConfig ClientCertificate
);
public record CertificateConfig(
  StoreLocation StoreLocation,
  StoreName StoreName,
  string Thumbprint
);
