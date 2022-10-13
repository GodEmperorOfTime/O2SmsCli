namespace O2SmsCli.Configuration.Nodes;

class O2ConnectorConfigNode
{
  public const string SECTION_NAME = "O2ConnectorConfig";

  public string? EndpointUri { get; set; }
  public string? BaId { get; set; }
  public CertificateConfigNode? ClientCertificate { get; set; }
}

class CertificateConfigNode
{
  public string? StoreLocation { get; set; }
  public string? StoreName { get; set; }
  public string? Thumbprint { get; set; }
}
