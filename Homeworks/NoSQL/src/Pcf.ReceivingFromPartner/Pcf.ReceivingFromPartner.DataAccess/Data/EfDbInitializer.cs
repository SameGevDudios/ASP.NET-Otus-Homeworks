using Pcf.ReceivingFromPartner.Core.Abstractions.Repositories;

namespace Pcf.ReceivingFromPartner.DataAccess.Data
{
    public class EfDbInitializer
        : IDbInitializer
    {
        private readonly DataContext _dataContext;
        private readonly IPreferencesCacheRepository _cacheRepository;

        public EfDbInitializer(DataContext dataContext, IPreferencesCacheRepository cacheRepository)
        {
            _dataContext = dataContext;
            _cacheRepository = cacheRepository;
        }
        
        public void InitializeDb()
        {
            _dataContext.Database.EnsureDeleted();
            _dataContext.Database.EnsureCreated();

            _cacheRepository.InitializeCacheAsync(FakeDataFactory.Preferences).GetAwaiter().GetResult();
            
            _dataContext.AddRange(FakeDataFactory.Partners);
            _dataContext.SaveChanges();
        }
    }
}