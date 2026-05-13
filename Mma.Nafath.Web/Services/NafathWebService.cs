using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Mma.Nafath.Web.Configuration;
using Mma.Nafath.Web.Models;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace Mma.Nafath.Web.Services
{
    public class NafathWebService : INafathWebService
    {
        private readonly HttpClient _httpClient;
        private readonly NafathWebOptions _options;
        private readonly ILogger<NafathWebService> _logger;

        public NafathWebService(
            HttpClient httpClient,
            IOptions<NafathWebOptions> options,
            ILogger<NafathWebService> logger)
        {
            _httpClient = httpClient;
            _options = options.Value;
            _logger = logger;

            if (!string.IsNullOrEmpty(_options.BaseUrl))
            {
                _httpClient.BaseAddress = new Uri(_options.BaseUrl);
            }
        }

        public async Task<NafathWebCreateSessionResponse?> CreateSessionAsync(string locale = "ar")
        {
            _logger.LogInformation("Creating Nafath web session for locale: {Locale}", locale);

            try
            {
                var requestBody = new NafathWebCreateSessionRequest { Locale = locale };
                var content = new StringContent(JsonConvert.SerializeObject(requestBody), Encoding.UTF8, "application/json");

                using var request = new HttpRequestMessage(HttpMethod.Post, _options.CreateSessionEndpoint);
                request.Content = content;

                if (!string.IsNullOrEmpty(_options.BasicAuth))
                {
                    request.Headers.Authorization = new AuthenticationHeaderValue("Basic", _options.BasicAuth);
                }

                var response = await _httpClient.SendAsync(request);
                response.EnsureSuccessStatusCode();

                var jsonResponse = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<NafathWebCreateSessionResponse>(jsonResponse);

                if (result != null)
                {
                    _logger.LogInformation("Nafath session created successfully. RequestId: {RequestId}", result.RequestId);
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create Nafath web session");
                return null;
            }
        }

        public string DecryptJwe(string jweToken, string base64Key)
        {
            _logger.LogInformation("Starting JWE token decryption");

            try
            {
                string[] parts = jweToken.Split('.');
                if (parts.Length != 5)
                {
                    throw new ArgumentException("Invalid JWE format - expected 5 parts");
                }

                byte[] iv = Base64UrlDecodeBytes(parts[2]);
                byte[] ciphertext = Base64UrlDecodeBytes(parts[3]);
                byte[] tag = Base64UrlDecodeBytes(parts[4]);
                byte[] key = Convert.FromBase64String(base64Key);

                using var aesGcm = new AesGcm(key, 16);
                byte[] plaintext = new byte[ciphertext.Length];
                aesGcm.Decrypt(iv, ciphertext, tag, plaintext, Encoding.UTF8.GetBytes(parts[0]));

                var result = Encoding.UTF8.GetString(plaintext);
                _logger.LogInformation("JWE token decrypted successfully");
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "JWE token decryption failed");
                throw;
            }
        }

        public string? ExtractNationalIdFromJwt(string jwtToken)
        {
            try
            {
                var handler = new JwtSecurityTokenHandler();
                var token = handler.ReadJwtToken(jwtToken);
                return token.Claims.FirstOrDefault(c => c.Type == "userid")?.Value;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to extract National ID from JWT");
                return null;
            }
        }

        public async Task<FetchJwkResponse?> FetchJwkAsync(string keyId)
        {
            _logger.LogInformation("Fetching JWK for KeyId: {KeyId}", keyId);

            try
            {
                var endpoint = string.Format(_options.FetchJwkEndpoint, keyId);
                using var request = new HttpRequestMessage(HttpMethod.Get, endpoint);

                if (!string.IsNullOrEmpty(_options.BasicAuth))
                {
                    request.Headers.Authorization = new AuthenticationHeaderValue("Basic", _options.BasicAuth);
                }

                var response = await _httpClient.SendAsync(request);
                response.EnsureSuccessStatusCode();

                var jsonResponse = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<FetchJwkResponse>(jsonResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to fetch JWK for KeyId: {KeyId}", keyId);
                return null;
            }
        }

        private byte[] Base64UrlDecodeBytes(string input)
        {
            if (string.IsNullOrEmpty(input)) return Array.Empty<byte>();

            string padded = input.Replace('-', '+').Replace('_', '/');
            switch (padded.Length % 4)
            {
                case 2: padded += "=="; break;
                case 3: padded += "="; break;
            }
            return Convert.FromBase64String(padded);
        }
    }
}
