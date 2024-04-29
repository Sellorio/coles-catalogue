using ColesCatalogue.Logic.Services;
using Microsoft.Extensions.DependencyInjection;

namespace ColesCatalogue.Logic
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddLogic(this IServiceCollection services)
        {
            return services
                .AddHttpClient()
                .AddSingleton<ISpecialsService, SpecialsService>()
                .AddSingleton<IStorageService, StorageService>()
                .AddSingleton<IHiddenItemService, HiddenItemService>()
                .AddSingleton<IFavouriteItemService, FavouriteItemService>();
        }
    }
}
