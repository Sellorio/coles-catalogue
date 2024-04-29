using System.Collections.Generic;
using System.Threading.Tasks;

namespace ColesCatalogue.Logic.Services
{
    internal class HiddenItemService : IHiddenItemService
    {
        private readonly IStorageService _storageService;

        public HiddenItemService(IStorageService storageService)
        {
            _storageService = storageService;
        }

        public async Task HideAsync(int itemId)
        {
            IList<int> hiddenItems = await GetHiddenItemsAsync();

            if (!hiddenItems.Contains(itemId))
            {
                hiddenItems.Add(itemId);
                await _storageService.WriteDataAsync("HiddenItems", hiddenItems);
            }
        }

        public async Task UnhideAsync(int itemId)
        {
            IList<int> hiddenItems = await GetHiddenItemsAsync();

            if (hiddenItems.Contains(itemId))
            {
                hiddenItems.Remove(itemId);
                await _storageService.WriteDataAsync("HiddenItems", hiddenItems);
            }
        }

        public async Task<IList<int>> GetHiddenItemsAsync()
        {
            return await _storageService.ReadAsync<List<int>>("HiddenItems") ?? new List<int>();
        }
    }
}
