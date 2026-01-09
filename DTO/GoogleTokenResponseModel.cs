using GeneratorAttributes.Attributes;
using System.Text.Json.Serialization;

namespace DTO
{
    [GeneratorIgnore]
    public class GoogleTokenResponseModel
    {
        [JsonPropertyName("id_token")] public string IdToken { get; set; } = null!;
    }
}
