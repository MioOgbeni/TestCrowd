using System.Threading.Tasks;
using NHibernate.Criterion;
using TestCrowd.DataAccess.Interface.Repository.Disputes;
using TestCrowd.DataAccess.Model;
using TestCrowd.DataAccess.Model.Disputes;

namespace TestCrowd.DataAccess.Repository.Disputes
{
    public class DisputeRepository : RepositoryBase<Dispute>, IDisputeRepository
    {
        public int GetCountWithCompany(Company currentUser = null)
        {
            if (currentUser != null)
            {
                return Session.CreateCriteria<Dispute>().Add(Restrictions.Eq(nameof(Dispute.Company), currentUser)).SetProjection(Projections.RowCount()).UniqueResult<int>();
            }

            return Session.CreateCriteria<Dispute>().SetProjection(Projections.RowCount()).UniqueResult<int>();
        }

        public async Task<int> GetCountWithCompanyAsync(Company currentUser = null)
        {
            if (currentUser != null)
            {
                return await Session.CreateCriteria<Dispute>().Add(Restrictions.Eq(nameof(Dispute.Company), currentUser)).SetProjection(Projections.RowCount()).UniqueResultAsync<int>();
            }

            return await Session.CreateCriteria<Dispute>().SetProjection(Projections.RowCount()).UniqueResultAsync<int>();
        }

        public int GetCountWithTester(Tester currentUser = null)
        {
            if (currentUser != null)
            {
                return Session.CreateCriteria<Dispute>().Add(Restrictions.Eq(nameof(Dispute.Tester), currentUser)).SetProjection(Projections.RowCount()).UniqueResult<int>();
            }

            return Session.CreateCriteria<Dispute>().SetProjection(Projections.RowCount()).UniqueResult<int>();
        }

        public async Task<int> GetCountWithTesterAsync(Tester currentUser = null)
        {
            if (currentUser != null)
            {
                return await Session.CreateCriteria<Dispute>().Add(Restrictions.Eq(nameof(Dispute.Tester), currentUser)).SetProjection(Projections.RowCount()).UniqueResultAsync<int>();
            }

            return await Session.CreateCriteria<Dispute>().SetProjection(Projections.RowCount()).UniqueResultAsync<int>();
        }

        public int GetCountPanding()
        {
            return Session.CreateCriteria<Dispute>().Add(Restrictions.Eq(nameof(Dispute.Status), Status.Pending)).SetProjection(Projections.RowCount()).UniqueResult<int>();
        }

        public async Task<int> GetCountPandingAsync()
        {
            return await Session.CreateCriteria<Dispute>().Add(Restrictions.Eq(nameof(Dispute.Status), Status.Pending)).SetProjection(Projections.RowCount()).UniqueResultAsync<int>();
        }

        public int GetCountForAdminInProgress(Admin currentUser = null)
        {
            if (currentUser != null)
            {
                return Session.CreateCriteria<Dispute>().Add(Restrictions.Eq(nameof(Dispute.Resolver), currentUser)).Add(Restrictions.Eq(nameof(Dispute.Status), Status.InProgress)).SetProjection(Projections.RowCount()).UniqueResult<int>();
            }

            return Session.CreateCriteria<Dispute>().SetProjection(Projections.RowCount()).UniqueResult<int>();
        }

        public async Task<int> GetCountForAdminInProgressAsync(Admin currentUser = null)
        {
            if (currentUser != null)
            {
                return await Session.CreateCriteria<Dispute>().Add(Restrictions.Eq(nameof(Dispute.Resolver), currentUser)).Add(Restrictions.Eq(nameof(Dispute.Status), Status.InProgress)).SetProjection(Projections.RowCount()).UniqueResultAsync<int>();
            }

            return await Session.CreateCriteria<Dispute>().SetProjection(Projections.RowCount()).UniqueResultAsync<int>();
        }

        public int GetCountForAdminResolved(Admin currentUser = null)
        {
            if (currentUser != null)
            {
                return Session.CreateCriteria<Dispute>().Add(Restrictions.Eq(nameof(Dispute.Resolver), currentUser)).Add(Restrictions.Or(Restrictions.Eq(nameof(Dispute.Status), Status.Rejected), Restrictions.Eq(nameof(Dispute.Status), Status.Approved))).SetProjection(Projections.RowCount()).UniqueResult<int>();
            }

            return Session.CreateCriteria<Dispute>().SetProjection(Projections.RowCount()).UniqueResult<int>();
        }

        public async Task<int> GetCountForAdminResolvedAsync(Admin currentUser = null)
        {
            if (currentUser != null)
            {
                return await Session.CreateCriteria<Dispute>().Add(Restrictions.Eq(nameof(Dispute.Resolver), currentUser)).Add(Restrictions.Or(Restrictions.Eq(nameof(Dispute.Status), Status.Rejected), Restrictions.Eq(nameof(Dispute.Status), Status.Approved))).SetProjection(Projections.RowCount()).UniqueResultAsync<int>();
            }

            return await Session.CreateCriteria<Dispute>().SetProjection(Projections.RowCount()).UniqueResultAsync<int>();
        }
    }
}