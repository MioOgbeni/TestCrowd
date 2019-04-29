using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TestCrowd.DataAccess.Model;

namespace TestCrowd.DataAccess.Interface.Repository
{
    public interface ICountryRepository
    {
        IList<Country> GetAll();
        Task<IList<Country>> GetAllAsync();

        Country GetByCountryCode(string countryCode);
        Task<Country> GetByCountryCodeAsync(string countryCode);

        int GetCount();
        Task<int> GetCountAsync();
    }
}