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
        private IRepository<Preference> _preferenceRepository;

        public CustomersController(
            IRepository<Customer> customerRepository, 
            IRepository<CustomerPreference> customerPreferenceRepository,
            IRepository<Preference> preferenceRepository
            )
        {
            _customerRepository = customerRepository; 
            _customerPreferenceRepository = customerPreferenceRepository;
            _preferenceRepository = preferenceRepository;
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
        [HttpGet("{id}")]
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
            // Create a new customer
            var customer = new Customer()
            {
                Id = Guid.NewGuid(),
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email
            };

            var customerPreferences = await CreateCustomerPreferencesAsync(request.PreferenceIds, customer);
            // Assign customer preferences
            customer.CustomerPreferences = customerPreferences;

            // Save a new customer
            await _customerRepository.AddAsync(customer);

            // Return 201 response code
            return CreatedAtAction("GetCustomer", new { customer.Id },
                new CustomerShortResponse
                {
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
                CustomerPreferences = await CreateCustomerPreferencesAsync(request.PreferenceIds, customer),
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

        private async Task<List<CustomerPreference>> CreateCustomerPreferencesAsync(List<Guid> preferenceIds, Customer customer)
        {
            // Get relevant preferences to request
            var preferenceTasks = preferenceIds
                .Select(_preferenceRepository.GetByIdAsync);
            var preferences = (await Task.WhenAll(preferenceTasks)).ToList();

            // Create new customer preferences
            var customerPreferenceTasks = preferences.Select(async preference =>
                await _customerPreferenceRepository.AddAsync(
                    new CustomerPreference()
                    {
                        Id = Guid.NewGuid(),
                        Customer = customer,
                        Preference = preference
                    }));
            var customerPreferences = (await Task.WhenAll(customerPreferenceTasks)).ToList();
            return customerPreferences;
        }
    }
}