using System.Text.Json.Serialization;

namespace DTO
{
    public class GithubUserResponse
    {
        [JsonPropertyName("login")] public string Login {get; set;}
        [JsonPropertyName("id")] public long Id {get; set;}
    }
}
