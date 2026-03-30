using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Pcf.ReceivingFromPartner.Core.Domain;

namespace Pcf.ReceivingFromPartner.Core.Abstractions.Repositories
{
    public interface IPreferencesCacheRepository
    {
        Task<IEnumerable<Preference>> GetAllAsync();
        Task<Preference> GetByIdAsync(Guid id);
        Task InitializeCacheAsync(IEnumerable<Preference> preferences);
    }
}