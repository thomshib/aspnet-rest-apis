using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

var config = builder.Configuration;

// Add Authentication

builder.Services.AddAuthentication(
    options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    }
).AddJwtBearer(
    tokenOptions =>
    {
        tokenOptions.TokenValidationParameters = new TokenValidationParameters
        {
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Key"]!)),
            ValidateIssuerSigningKey = true,
            ValidateLifetime = true,
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidIssuer = config["Jwt:Issuer"],
            ValidAudience = config["Jwt:Audience"]

        };
    }
);

builder.Services.AddAuthorization(
    options =>
    {
        //only "admin" claim policy
        options.AddPolicy(AuthConstants.AdminUserPolicyName,
            p => p.RequireClaim(AuthConstants.AdminUserClaimName, "true"));

        //either and "admin" or "trusted_member" policy
        options.AddPolicy(AuthConstants.TrusterMemberPolicyName,
           p => p.RequireAssertion(c =>
            c.User.HasClaim( m => m is { Type:AuthConstants.AdminUserClaimName, Value: "true"}) ||
            c.User.HasClaim( m => m is { Type:AuthConstants.TrusterMemberClaimName, Value: "true"})



             ));
    }
);


// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//Inject MovieApplication with MovieRepository
builder.Services.AddApplication();
builder.Services.AddDatabase(config["Database:ConnectionString"]!);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();


app.UseAuthentication();
app.UseAuthorization();


app.UseMiddleware<ValidationMapperMiddleware>();
app.MapControllers();

var dbInitializer = app.Services.GetRequiredService<DbInitializer>();
await dbInitializer.InitializeAsync();

app.Run();
