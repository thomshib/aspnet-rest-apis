

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

public class ApiKeyAuthFilter : IAuthorizationFilter
{
    private readonly IConfiguration _configuration;
    public ApiKeyAuthFilter(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    public void OnAuthorization(AuthorizationFilterContext context)
    {
        if(!context.HttpContext.Request.Headers.TryGetValue(AuthConstants.ApiKeyHeaderName,
        out var extractedApiKey)){
            context.Result = new UnauthorizedObjectResult("API Key is missing");
            return;
        }
        

        var _apiKey = _configuration["ApiKey"]!;

        if(_apiKey !=  extractedApiKey){
             context.Result = new UnauthorizedObjectResult("Invalid API Key");
            
        }
    }
}