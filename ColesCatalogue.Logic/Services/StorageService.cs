using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace ColesCatalogue.Logic.Services
{
    internal class StorageService : IStorageService
    {
        private static readonly string _storagePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "ColesCatalogue", "data.json");

        private Dictionary<string, string> _dataStore;

        public async Task<TData> ReadAsync<TData>(string name)
        {
            await EnsureDataStoreAsync();

            if (!_dataStore.TryGetValue(name, out var data))
            {
                return default;
            }

            return JsonSerializer.Deserialize<TData>(data);
        }

        public async Task WriteDataAsync<TData>(string name, TData data)
        {
            await EnsureDataStoreAsync();

            _dataStore[name] = JsonSerializer.Serialize(data);

            await SaveAsync();
        }

        private async Task EnsureDataStoreAsync()
        {
            if (_dataStore == null)
            {
                if (File.Exists(_storagePath))
                {
                    _dataStore =
                        JsonSerializer.Deserialize<Dictionary<string, string>>(
                            await File.ReadAllTextAsync(_storagePath));
                }
                else
                {
                    _dataStore = new Dictionary<string, string>();
                }
            }
        }

        private async Task SaveAsync()
        {
            Directory.CreateDirectory(Path.GetDirectoryName(_storagePath));
            await File.WriteAllTextAsync(_storagePath, JsonSerializer.Serialize(_dataStore));
        }
    }
}
