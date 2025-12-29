using System.Text.Json.Serialization;

namespace DTO
{
    public class GoogleTokenResponseModel
    {
        [JsonPropertyName("id_token")] public string IdToken { get; set; } = null!;
    }
}
