using System.Linq;
using HotChocolate;
using HotChocolate.Data;
using Pcf.GivingToCustomer.Core.Domain;
using Pcf.GivingToCustomer.DataAccess;

namespace Pcf.GivingToCustomer.WebHost.GraphQl
{
    public class CustomerQueries
    {
        [UseProjection]
        [UseFiltering]
        [UseSorting]
        public IQueryable<Customer> GetCustomers([Service] DataContext dbContext)
        {
            return dbContext.Customers;
        }

        [UseSingleOrDefault]
        [UseProjection]
        public IQueryable<Customer> GetCustomerById(System.Guid id, [Service] DataContext dbContext)
        {
            return dbContext.Customers.Where(c => c.Id == id);
        }
    }
}