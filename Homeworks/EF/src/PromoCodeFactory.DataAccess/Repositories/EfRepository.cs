using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PromoCodeFactory.DataAccess.DbContexts;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain;
using Microsoft.EntityFrameworkCore;

namespace PromoCodeFactory.DataAccess.Repositories
{
    internal class EfRepository<T> : IRepository<T> where T : BaseEntity
    {
        private CustomersDbContext _customerDbContext;

        public EfRepository(CustomersDbContext customerDbContext) { _customerDbContext = customerDbContext; }

        public async Task<T> AddAsync(T item)
        {
            await _customerDbContext.Set<T>().AddAsync(item);
            await _customerDbContext.SaveChangesAsync();
            return item;
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _customerDbContext.Set<T>().ToListAsync();
        }

        public async Task<T> GetByIdAsync(Guid id)
        {
            return await _customerDbContext.Set<T>().FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<T> RemoveByIdAsync(Guid id)
        {
            var row = _customerDbContext.Set<T>().FirstOrDefault(x => x.Id == id);
            if (row != null)
            {
                _customerDbContext.Set<T>().Remove(row);
                await _customerDbContext.SaveChangesAsync();
            }
            return row;
        }

        public async Task UpdateAsync(T item)
        {
            _customerDbContext.Set<T>().Update(item);
            await _customerDbContext.SaveChangesAsync();
        }
    }
}
