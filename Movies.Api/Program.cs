using System.Text;
using Asp.Versioning;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Swashbuckle.AspNetCore.SwaggerGen;

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
        // //only "admin" claim policy
        // options.AddPolicy(AuthConstants.AdminUserPolicyName,
        //     p => p.RequireClaim(AuthConstants.AdminUserClaimName, "true"));

        // mixed policy - either api key & jwt 
        options.AddPolicy(AuthConstants.AdminUserPolicyName,
                p => p.AddRequirements(new AdminAuthRequirement(config["ApiKey"]!))
            );

        //either and "admin" or "trusted_member" policy
        options.AddPolicy(AuthConstants.TrusterMemberPolicyName,
           p => p.RequireAssertion(c =>
            c.User.HasClaim(m => m is { Type: AuthConstants.AdminUserClaimName, Value: "true" }) ||
            c.User.HasClaim(m => m is { Type: AuthConstants.TrusterMemberClaimName, Value: "true" })



             ));
    }
);


// API Key

builder.Services.AddScoped<ApiKeyAuthFilter>();

// Add API Versioning
builder.Services.AddApiVersioning(
    options =>
    {
        options.DefaultApiVersion = new ApiVersion(1.0);
        options.AssumeDefaultVersionWhenUnspecified = true;
        options.ReportApiVersions = true;
    }

).AddMvc();


// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

builder.Services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();



// Add services to the container.
builder.Services.AddControllers();



//Inject MovieApplication with MovieRepository
builder.Services.AddApplication();
builder.Services.AddDatabase(config["Database:ConnectionString"]!);


// Health Checks

builder.Services.AddHealthChecks()
.AddCheck<DatabaseHealthCheck>("Database");


// Response Caching - client caching

//builder.Services.AddResponseCaching();

//Output Caching - server side caching

builder.Services.AddOutputCache(
    x =>
    {
        x.AddBasePolicy(c => c.Cache());
        x.AddPolicy("MovieCache", c => c.Cache()
        .Expire(TimeSpan.FromMinutes(1))
        .SetVaryByQuery(new[] { "title", "year", "sortBy", "page", "pageSize" })
        .Tag("movies"));

    }
);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapHealthChecks("_health");
app.UseHttpsRedirection();


app.UseAuthentication();
app.UseAuthorization();

// Response Caching - client caching technique
//app.UseResponseCaching();

//Output Caching - server side caching
app.UseOutputCache();


app.UseMiddleware<ValidationMapperMiddleware>();
app.MapControllers();

var dbInitializer = app.Services.GetRequiredService<DbInitializer>();
await dbInitializer.InitializeAsync();

app.Run();
