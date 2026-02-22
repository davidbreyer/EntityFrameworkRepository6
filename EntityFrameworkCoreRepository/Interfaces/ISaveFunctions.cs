using System.Threading.Tasks;

namespace EntityFramework.Repository.Core.Interfaces
{
    public interface ISaveFunctions<T>
            where T : class
    {
        Task<int> SaveAsync();
        int Save();
    }
}
