using System.Threading.Tasks;

namespace ColesCatalogue.Logic.Services
{
    public interface IStorageService
    {
        Task<TData> ReadAsync<TData>(string name);
        Task WriteDataAsync<TData>(string name, TData data);
    }
}
