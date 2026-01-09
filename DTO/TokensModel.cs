using GeneratorAttributes.Attributes;

namespace DTO;

public class TokensModel
{
    [CustomFieldGenerationName("jwtToken")]
    public string JWTToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
}
