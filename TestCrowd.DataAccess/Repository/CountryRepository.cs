using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NHibernate;
using NHibernate.Criterion;
using TestCrowd.DataAccess.Interface.Repository;
using TestCrowd.DataAccess.Model;

namespace TestCrowd.DataAccess.Repository
{
    public class CountryRepository :ICountryRepository
    {
        protected ISession Session;

        public CountryRepository()
        {
            Session = NHibernateHelper.Session;
        }

        public IList<Country> GetAll()
        {
            return Session.QueryOver<Country>().List();
        }

        public async Task<IList<Country>> GetAllAsync()
        {
            return await Session.QueryOver<Country>().ListAsync();
        }

        public Country GetByCountryCode(string countryCode)
        {
            return Session.CreateCriteria<Country>().Add(Restrictions.Eq(nameof(Country.IdentificationCode), countryCode)).UniqueResult<Country>();
        }

        public async Task<Country> GetByCountryCodeAsync(string countryCode)
        {
            return await Session.CreateCriteria<Country>().Add(Restrictions.Eq(nameof(Country.IdentificationCode), countryCode)).UniqueResultAsync<Country>();
        }

        public int GetCount()
        {
            return Session.CreateCriteria<Country>().SetProjection(Projections.RowCount()).UniqueResult<int>();
        }

        public async Task<int> GetCountAsync()
        {
            return await Session.CreateCriteria<Country>().SetProjection(Projections.RowCount()).UniqueResultAsync<int>();
        }
    }
}