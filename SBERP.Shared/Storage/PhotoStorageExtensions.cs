using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace SBERP.Shared.Storage
{
    public static class PhotoStorageExtensions
    {
        /// <summary>Registers <see cref="IPhotoStorage"/> bound to the "PhotoStorage" configuration section.</summary>
        public static IServiceCollection AddPhotoStorage(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<PhotoStorageSettings>(configuration.GetSection(PhotoStorageSettings.SectionName));
            services.AddSingleton<IPhotoStorage, LocalPhotoStorage>();
            return services;
        }
    }
}
