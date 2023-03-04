1. System.IdentityModel.Tokens.Jwt nuget

dotnet add package System.IdentityModel.Tokens.Jwt --version 6.27.0

2. Token Request
{
    "userid": "d347169a-9b03-41a1-89a5-0806aa949dbe",
    "email": "test@test.com",
    "customClaims": {
        "admin": false,
        "trusted_member": true
    }

}
