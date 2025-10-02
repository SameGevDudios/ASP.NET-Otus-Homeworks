using Microsoft.AspNetCore.Mvc;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using PromoCodeFactory.WebHost.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PromoCodeFactory.WebHost.Controllers
{
    /// <summary>
    /// Клиенты
    /// </summary>
    [ApiController]
    [Route("api/v1/[controller]")]
    public class CustomersController
        : ControllerBase
    {

        private IRepository<Customer> _customerRepository;
        private IRepository<CustomerPreference> _customerPreferenceRepository;

        public CustomersController(
            IRepository<Customer> customerRepository, 
            IRepository<CustomerPreference> customerPreferenceRepository
            )
        {
            _customerRepository = customerRepository; 
            _customerPreferenceRepository = customerPreferenceRepository;
        }

        /// <summary>
        /// Получить все записи о покупателях
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<List<CustomerShortResponse>> GetCustomersAsync()
        {
            var customerList = await _customerRepository.GetAllAsync();
            var customerResponseList = customerList.Select(x =>
                new CustomerShortResponse
                {
                    Id = x.Id,
                    Email = x.Email,
                    FirstName = x.FirstName,
                    LastName = x.LastName
                }).ToList();
            return customerResponseList;
        }

        /// <summary>
        /// Получить покупателя по ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id:guid}", Name = "GetCustomer")]
        public async Task<ActionResult<CustomerResponse>> GetCustomerAsync(Guid id)
        {
            var customer = await _customerRepository.GetByIdAsync(id);
            return new CustomerResponse()
            {
                Id = customer.Id,
                Email = customer.Email,
                FirstName = customer.FirstName,
                LastName = customer.LastName,
                PromoCodes = customer.Promocodes.Select(x =>
                    new PromoCodeShortResponse()
                    {
                        Id = x.Id,
                        BeginDate = x.BeginDate.ToString(),
                        EndDate = x.EndDate.ToString(),
                        Code = x.Code,
                        PartnerName = x.PartnerName,
                        ServiceInfo = x.ServiceInfo,
                    }).ToList()
            };
        }

        /// <summary>
        /// Создать нового покупателя
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> CreateCustomerAsync(CreateOrEditCustomerRequest request)
        {
            Guid id = Guid.NewGuid();
            // Create a new customer
            var customer = new Customer()
            {
                Id = id,
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email
            };
            
            // Create parent entity
            await _customerRepository.AddAsync(customer);

            // Create child entity
            var customerPreferences = await CreateCustomerPreferencesAsync(request.PreferenceIds, customer.Id);
            // Tie up reverse relations
            customer.CustomerPreferences = customerPreferences;

            await _customerRepository.UpdateAsync(customer);

            // Return 201 response code
            return CreatedAtRoute(
            routeName: "GetCustomer",
            routeValues: new { id = customer.Id },
            value: new CustomerShortResponse
            {
                Id = customer.Id,
                FirstName = customer.FirstName,
                LastName = customer.LastName,
                Email = customer.Email
            });
        }

        /// <summary>
        /// Изменить существующего покупателя по ID
        /// </summary>
        /// <param name="id"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> EditCustomersAsync(Guid id, CreateOrEditCustomerRequest request)
        {
            var customer = await _customerRepository.GetByIdAsync(id);

            customer.CustomerPreferences.Select(async x => await _customerPreferenceRepository.RemoveByIdAsync(x.Id)).ToList();

            var updatedCustomer = new Customer()
            {
                Id = customer.Id,
                FirstName = customer.FirstName,
                LastName = customer.LastName,
                Email = customer.Email,
                CustomerPreferences = await CreateCustomerPreferencesAsync(request.PreferenceIds, customer.Id),
                Promocodes = customer.Promocodes
            };

            await _customerRepository.UpdateAsync(updatedCustomer);
            return NoContent();
        }

        /// <summary>
        /// Удалить покупателя по ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete]
        public async Task<IActionResult> DeleteCustomer(Guid id)
        {
            await _customerRepository.RemoveByIdAsync(id);
            return NoContent();
        }

        private async Task<List<CustomerPreference>> CreateCustomerPreferencesAsync(List<Guid> preferenceIds, Guid customerId)
        {
            // Create new customer preferences
            var customerPreferenceTasks = preferenceIds.Select(async preference =>
                await _customerPreferenceRepository.AddAsync(
                    new CustomerPreference()
                    {
                        Id = Guid.NewGuid(),
                        CustomerId = customerId,
                        PreferenceId = preference
                    }));
            var customerPreferences = (await Task.WhenAll(customerPreferenceTasks)).ToList();
            return customerPreferences;
        }
    }
}