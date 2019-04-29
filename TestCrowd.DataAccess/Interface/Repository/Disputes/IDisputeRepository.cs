using System.Threading.Tasks;
using TestCrowd.DataAccess.Model;
using TestCrowd.DataAccess.Model.Disputes;

namespace TestCrowd.DataAccess.Interface.Repository.Disputes
{
    public interface IDisputeRepository:IRepositoryBase<Dispute>
    {
        int GetCountWithCompany(Company currentUser = null);
        Task<int> GetCountWithCompanyAsync(Company currentUser = null);

        int GetCountWithTester(Tester currentUser = null);
        Task<int> GetCountWithTesterAsync(Tester currentUser = null);

        int GetCountPanding();
        Task<int> GetCountPandingAsync();

        int GetCountForAdminResolved(Admin currentUser = null);
        Task<int> GetCountForAdminResolvedAsync(Admin currentUser = null);

        int GetCountForAdminInProgress(Admin currentUser = null);
        Task<int> GetCountForAdminInProgressAsync(Admin currentUser = null);
    }
}