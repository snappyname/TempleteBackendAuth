using System.Text.Json.Serialization;

namespace DTO
{
    public class GithubTokenResponseModel
    {
        [JsonPropertyName("access_token")] public string AccessToken { get; set; }
    }
}
