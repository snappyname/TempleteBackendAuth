using GeneratorAttributes.Attributes;
using System.Text.Json.Serialization;

namespace DTO
{
    [GeneratorIgnore]
    public class GithubTokenResponseModel
    {
        [JsonPropertyName("access_token")] public string AccessToken { get; set; }
    }
}
