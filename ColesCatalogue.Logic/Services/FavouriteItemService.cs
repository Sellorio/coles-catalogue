using System.Collections.Generic;
using System.Threading.Tasks;

namespace ColesCatalogue.Logic.Services
{
    internal class FavouriteItemService : IFavouriteItemService
    {
        private readonly IStorageService _storageService;

        public FavouriteItemService(IStorageService storageService)
        {
            _storageService = storageService;
        }

        public async Task FavouriteAsync(int itemId)
        {
            IList<int> favourites = await GetFavouritesAsync();

            if (!favourites.Contains(itemId))
            {
                favourites.Add(itemId);
                await _storageService.WriteDataAsync("FavouriteItems", favourites);
            }
        }

        public async Task UnfavouriteAsync(int itemId)
        {
            IList<int> favourites = await GetFavouritesAsync();

            if (favourites.Contains(itemId))
            {
                favourites.Remove(itemId);
                await _storageService.WriteDataAsync("FavouriteItems", favourites);
            }
        }

        public async Task<IList<int>> GetFavouritesAsync()
        {
            return await _storageService.ReadAsync<List<int>>("FavouriteItems") ?? new List<int>();
        }
    }
}
