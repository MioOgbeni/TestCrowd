using System.Threading.Tasks;
using NHibernate.Criterion;
using TestCrowd.DataAccess.Interface.Repository;
using TestCrowd.DataAccess.Model;

namespace TestCrowd.DataAccess.Repository
{
    public class ApplicationRoleRepository :RepositoryBase<ApplicationRole>, IApplicationRoleRepository
    {
        public ApplicationRole GetByName(string name)
        {
            return Session.CreateCriteria<ApplicationRole>().Add(Restrictions.Eq(nameof(ApplicationRole.Name), name)).UniqueResult<ApplicationRole>();
        }

        public async Task<ApplicationRole> GetByNameAsync(string name)
        {
            return await Session.CreateCriteria<ApplicationRole>().Add(Restrictions.Eq(nameof(ApplicationRole.Name), name)).UniqueResultAsync<ApplicationRole>();
        }
    }
}