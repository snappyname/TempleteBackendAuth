using System.Text.Json.Serialization;

namespace DTO
{
    public class GoogleTokenResponse
    {
        [JsonPropertyName("id_token")] public string IdToken { get; set; } = null!;
    }
}
