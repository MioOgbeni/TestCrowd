using System.Threading.Tasks;
using TestCrowd.DataAccess.Model;

namespace TestCrowd.DataAccess.Interface.Repository
{
    public interface IApplicationRoleRepository :IRepositoryBase<ApplicationRole>
    {
        ApplicationRole GetByName(string name);
        Task<ApplicationRole> GetByNameAsync(string name);
    }
}