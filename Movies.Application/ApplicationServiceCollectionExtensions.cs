

using Microsoft.Extensions.DependencyInjection;

public static class ApplicationServiceCollectionExtensions{

    public static IServiceCollection  AddApplication( this IServiceCollection services){
        services.AddSingleton<IMovieRepository, MovieRepository>();
        services.AddSingleton<IMovieService, MovieService>();
        return services;
        
    }

      public static IServiceCollection  AddDatabase( this IServiceCollection services,string connectionString){
        
        services.AddSingleton<IDbConnectionFactory>( _ => 
            new NpgsqlConnectionFactory(connectionString));
            
        services.AddSingleton<DbInitializer>();
        return services;
        
    }


}