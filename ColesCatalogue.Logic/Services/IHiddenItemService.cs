using System.Collections.Generic;
using System.Threading.Tasks;

namespace ColesCatalogue.Logic.Services
{
    public interface IHiddenItemService
    {
        Task HideAsync(int itemId);
        Task UnhideAsync(int itemId);
        Task<IList<int>> GetHiddenItemsAsync();
    }
}
