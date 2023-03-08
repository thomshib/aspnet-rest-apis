
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

public class AdminAuthRequirement : IAuthorizationHandler,IAuthorizationRequirement
{
    public readonly string _apiKey;
    public AdminAuthRequirement(string apiKey)
    {
            _apiKey = apiKey;
    }

    public Task HandleAsync(AuthorizationHandlerContext context){
        if(context.User.HasClaim(AuthConstants.AdminUserClaimName, "true")){
            context.Succeed(this);
            return Task.CompletedTask;
        }

        var httpContext = context.Resource as HttpContext;

        if(httpContext is null){
             return Task.CompletedTask;
        }

        if(!httpContext.Request.Headers.TryGetValue(AuthConstants.ApiKeyHeaderName,
        out var extractedApiKey)){
            context.Fail();
            return Task.CompletedTask;
        }        
    
        if(_apiKey !=  extractedApiKey){
              context.Fail();
            return Task.CompletedTask;            
        }

        // Add  a user identity in case of APIKey Auth
        var identity = (ClaimsIdentity) httpContext.User.Identity!;
        identity.AddClaim(new Claim("userid",Guid.Parse("12bf4740-870b-4d21-bf78-799f53a1556e").ToString()));
        context.Succeed(this);
        return Task.CompletedTask;




    }
}