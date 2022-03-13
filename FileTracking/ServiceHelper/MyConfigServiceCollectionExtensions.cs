namespace FileTracking.ServiceHelper
{
    public static class MyConfigServiceCollectionExtensions
    {
        public static IServiceCollection AddConfig(
             this IServiceCollection services, IConfiguration config)
        {
            services.Configure<AppSettings>(
                config.GetSection("AppSettings"));
            return services;
        }
    }
}
