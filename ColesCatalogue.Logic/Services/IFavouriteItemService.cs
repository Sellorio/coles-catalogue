using System.Collections.Generic;
using System.Threading.Tasks;

namespace ColesCatalogue.Logic.Services
{
    public interface IFavouriteItemService
    {
        Task FavouriteAsync(int itemId);
        Task UnfavouriteAsync(int itemId);
        Task<IList<int>> GetFavouritesAsync();
    }
}
