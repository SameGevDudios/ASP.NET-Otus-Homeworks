using System;
using System.Linq;
using System.Threading.Tasks;
using Grpc.Core;
using Pcf.GivingToCustomer.Core.Abstractions.Repositories;
using Pcf.GivingToCustomer.Core.Domain;

namespace Pcf.GivingToCustomer.WebHost.Grpc
{
    public class CustomersGrpcService : CustomersGrpc.CustomersGrpcBase
    {
        private readonly IRepository<Customer> _customerRepository;

        public CustomersGrpcService(IRepository<Customer> customerRepository)
        {
            _customerRepository = customerRepository;
        }

        public override async Task<CustomerGrpcResponse> GetCustomer(GetCustomerRequest request, ServerCallContext context)
        {
            if (!Guid.TryParse(request.Id, out var id))
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid GUID format"));
            }

            var customer = await _customerRepository.GetByIdAsync(id);

            if (customer == null)
            {
                throw new RpcException(new Status(StatusCode.NotFound, "Customer not found"));
            }

            return MapToResponse(customer);
        }

        public override async Task<CustomersListResponse> GetCustomers(EmptyRequest request, ServerCallContext context)
        {
            var customers = await _customerRepository.GetAllAsync();

            var response = new CustomersListResponse();
            response.Customers.AddRange(customers.Select(MapToResponse));

            return response;
        }

        private static CustomerGrpcResponse MapToResponse(Customer customer)
        {
            return new CustomerGrpcResponse
            {
                Id = customer.Id.ToString(),
                FirstName = customer.FirstName ?? string.Empty,
                LastName = customer.LastName ?? string.Empty,
                Email = customer.Email ?? string.Empty
            };
        }
    }
}