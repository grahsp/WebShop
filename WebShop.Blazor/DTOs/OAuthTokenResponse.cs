using System.Text.Json.Serialization;

namespace WebShop.Blazor.DTOs
{
    public class OAuthTokenResponse
    {
        [JsonRequired]
        [JsonPropertyName("access_token")]
        public required string AccessToken { get; set; }


        [JsonPropertyName("refresh_token")]
        public string? RefreshToken { get; set; }

        [JsonRequired]
        [JsonPropertyName("token_type")]
        public required string TokenType { get; set; }

        [JsonRequired]
        [JsonPropertyName("expires_in")]
        public int ExpiresIn { get; set; }
    }
}
