using GeneratorAttributes.Attributes;

namespace DTO
{
    [GeneratorIgnore]
    public class GithubEmailModel
    {
        public string Email {get; set;}
        public bool Primary {get; set;}
    }
}
