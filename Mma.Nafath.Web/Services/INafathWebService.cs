using Mma.Nafath.Web.Models;

namespace Mma.Nafath.Web.Services
{
    public interface INafathWebService
    {
        Task<NafathWebCreateSessionResponse?> CreateSessionAsync(string locale = "ar");
        string DecryptJwe(string jweToken, string base64Key);
        string? ExtractNationalIdFromJwt(string jwtToken);
        Task<FetchJwkResponse?> FetchJwkAsync(string keyId);
    }
}
