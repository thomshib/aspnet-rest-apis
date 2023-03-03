1. dotnet cli commands

dotnet add Movies.Api/Movies.Api.csproj reference Movies.Application/Movies.Application.csproj

dotnet add Movies.Api/Movies.Api.csproj reference Movies.Contracts/Movies.Contracts.csproj

dotnet new sln --name Movies

dotnet sln add Movies.Api/Movies.Api.csproj

dotnet sln add Movies.Application/Movies.Application.csproj

dotnet sln add Movies.Contracts/Movies.Contracts.csproj

2. Packages

2.1 Movies Application

- dotnet add package Microsoft.Extensions.DependencyInjection.Abstractions --version 7.0.0

- Dapper 
 dotnet add package Dapper --version 2.0.123

 - NpgSQL
 dotnet add package Npgsql --version 7.0.2

 - Fluent Validations
 
 dotnet add package FluentValidation.DependencyInjectionExtensions --version 11.5.1


3. Request Json

{
    "Title": "Misson Impossible",
    "YearOfRelease": 2023,
    "Genres": [
        "Thriller"
    ]
}

4. SQL queries

    delete FROM "genres";
    delete FROM "movies";