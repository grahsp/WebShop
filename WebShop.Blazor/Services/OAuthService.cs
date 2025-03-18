using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;
using WebShop.Blazor.Configuration;
using WebShop.Blazor.DTOs;

namespace WebShop.Blazor.Services
{
    public class OAuthService
    {
        private OAuthTokenResponse? _tokenResponse;
        private DateTime _tokenExpirationTime;

        private readonly OAuthConfig _config;
        private readonly IHttpClientFactory _httpClientFactory;

        public OAuthService(IHttpClientFactory httpClientFactory, IOptions<OAuthConfig> config)
        {
            _config = config.Value;
            _httpClientFactory = httpClientFactory;
        }

        public async Task<string> GetTokenAsync()
        {
            if (_tokenResponse != null && !IsTokenExpired())
            {
                return _tokenResponse.AccessToken;
            }

            using HttpClient client = _httpClientFactory.CreateClient();

            var requestBody = JsonSerializer.Serialize(new
            {
                client_id = _config.ClientId,
                client_secret = _config.ClientSecret,
                audience = _config.Audience,
                grant_type = "client_credentials"
            });

            var content = new StringContent(requestBody, Encoding.UTF8, "application/json");

            var url = new UriBuilder(_config.Domain)
            {
                Path = "oauth/token",
            }.ToString();

            var request = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = content
            };

            try
            {
                var response = await client.SendAsync(request).ConfigureAwait(false);
                response.EnsureSuccessStatusCode();

                var responseContent = await response.Content.ReadAsStringAsync();
                var tokenResponse = JsonSerializer.Deserialize<OAuthTokenResponse>(responseContent)
                    ?? throw new Exception("Failed to deserialize token response.");


                SetTokenExpirationTime(tokenResponse.ExpiresIn);
                _tokenResponse = tokenResponse;


                if (string.IsNullOrEmpty(tokenResponse.AccessToken))
                {
                    throw new Exception("Token response did not contain an access token.");
                }

                return tokenResponse.AccessToken;
            }
            catch (HttpRequestException ex)
            {
                throw new Exception("Network error occurred while requesting token.", ex);
            }
            catch (Exception ex)
            {
                throw new Exception("Error retrieving token: " + ex.Message, ex);
            }
        }

        public bool IsTokenExpired()
        {
            return DateTime.UtcNow > _tokenExpirationTime;
        }

        private void SetTokenExpirationTime(int expiresIn)
        {
            _tokenExpirationTime = DateTime.UtcNow.AddSeconds(expiresIn);
        }
    }
}
