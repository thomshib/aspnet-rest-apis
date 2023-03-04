

public class TokenGenerationRequest{

    public Guid UserId {get; set;} = default!;
    public string Email {get; set;} = default!;

    public Dictionary<string,object> CustomClaims  {get; set;} = new();


}