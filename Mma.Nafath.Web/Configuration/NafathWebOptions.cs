namespace Mma.Nafath.Web.Configuration
{
    public class NafathWebOptions
    {
        public string BaseUrl { get; set; } = string.Empty;
        public string CreateSessionEndpoint { get; set; } = "nafath-web/api/v1/sessions";
        public string FetchJwkEndpoint { get; set; } = "nafath-web/api/v1/jwk/{0}";
        public string BasicAuth { get; set; } = string.Empty;
        public string? CertificatePath { get; set; }
        public string? CertificatePassword { get; set; }
        public bool UseCertificate { get; set; }
    }
}
