using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain;

namespace PromoCodeFactory.DataAccess.Repositories
{
    public class InMemoryRepository<T>
        : IRepository<T>
        where T : BaseEntity
    {
        protected IEnumerable<T> Data { get; set; }

        public InMemoryRepository(IEnumerable<T> data)
        {
            Data = data;
        }

        public Task<IEnumerable<T>> GetAllAsync()
        {
            return Task.FromResult(Data);
        }

        public Task<T> GetByIdAsync(Guid id)
        {
            return Task.FromResult(Data.FirstOrDefault(x => x.Id == id));
        }

        public Task<T> AddAsync(T item)
        {
            var dataList = Data.ToList();
            dataList.Add(item);
            Data = dataList.AsQueryable();
            return Task.FromResult(item);
        }

        public Task<T> UpdateByIdAsync(Guid id, T item)
        {
            var dataList = Data.ToList();
            var dataItem = Data.FirstOrDefault(x => x.Id == id);
            dataItem = item;
            return Task.FromResult(dataItem);
        }

        public Task<T> RemoveByIdAsync(Guid id)
        {
            var dataList = Data.ToList();
            var item = dataList.FirstOrDefault(x => x.Id == id);
            dataList.Remove(item);
            Data = dataList.AsQueryable();
            return Task.FromResult(item);
        }
    }
}