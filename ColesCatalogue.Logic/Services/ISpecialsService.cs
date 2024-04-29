using ColesCatalogue.Logic.Data;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ColesCatalogue.Logic.Services
{
    public interface ISpecialsService
    {
        Task<IList<SpecialsItem>> GetSpecialsAsync(ProgressTracker progressTracker = null);
    }
}
