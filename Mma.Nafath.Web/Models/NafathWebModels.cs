namespace Mma.Nafath.Web.Models
{
    public class NafathWebCreateSessionRequest
    {
        public string Service { get; set; } = "WEB_REDIRECT";
        public string? Locale { get; set; } = "ar";
    }

    public class NafathWebCreateSessionResponse
    {
        public string RequestId { get; set; } = string.Empty;
        public string RedirectUrl { get; set; } = string.Empty;
        public string Key { get; set; } = string.Empty;
        public string? ExpirationDate { get; set; }
    }

    public class FetchJwkResponse
    {
        public string Kty { get; set; } = string.Empty;
        public string E { get; set; } = string.Empty;
        public string Use { get; set; } = string.Empty;
        public string Kid { get; set; } = string.Empty;
        public string Alg { get; set; } = string.Empty;
        public string N { get; set; } = string.Empty;
    }
}
