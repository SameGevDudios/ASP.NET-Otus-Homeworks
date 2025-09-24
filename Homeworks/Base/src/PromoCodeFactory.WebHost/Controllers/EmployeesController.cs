using Microsoft.AspNetCore.Mvc;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain.Administration;
using PromoCodeFactory.DataAccess.Data;
using PromoCodeFactory.WebHost.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace PromoCodeFactory.WebHost.Controllers
{
    /// <summary>
    /// Сотрудники
    /// </summary>
    [ApiController]
    [Route("api/v1/[controller]")]
    public class EmployeesController : ControllerBase
    {
        private readonly IRepository<Employee> _employeeRepository;

        public EmployeesController(IRepository<Employee> employeeRepository)
        {
            _employeeRepository = employeeRepository;
        }

        /// <summary>
        /// Получить данные всех сотрудников
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<List<EmployeeShortResponse>> GetEmployeesAsync()
        {
            var employees = await _employeeRepository.GetAllAsync();

            var employeesModelList = employees.Select(x =>
                new EmployeeShortResponse()
                {
                    Id = x.Id,
                    Email = x.Email,
                    FullName = x.FullName,
                }).ToList();

            return employeesModelList;
        }

        /// <summary>
        /// Получить данные сотрудника по Id
        /// </summary>
        /// <returns></returns>
        [HttpGet("{id:guid}")]
        public async Task<ActionResult<EmployeeResponse>> GetEmployeeByIdAsync(Guid id)
        {
            var employee = await _employeeRepository.GetByIdAsync(id);

            if (employee == null)
                return NotFound();

            var employeeModel = new EmployeeResponse()
            {
                Id = employee.Id,
                Email = employee.Email,
                Roles = employee.Roles.Select(x => new RoleItemResponse()
                {
                    Name = x.Name,
                    Description = x.Description
                }).ToList(),
                FullName = employee.FullName,
                AppliedPromocodesCount = employee.AppliedPromocodesCount
            };

            return employeeModel;
        }

        [HttpPost]
        public async Task<ActionResult> CreateEmployeeAsync(
            [FromQuery] string email, [FromQuery] string firstName, [FromQuery] string lastName, [FromQuery] List<string> roles)
        {
            Employee employee = new Employee()
            {
                Id = Guid.NewGuid(),
                Email = email,
                FirstName = firstName,
                LastName = lastName,
                Roles = FakeDataFactory.Roles.Where(x => roles.Contains(x.Name))
                .DefaultIfEmpty(new Role() { Id = Guid.NewGuid(), Name = "InvalidRole"})
                .ToList(),
            };
            await _employeeRepository.AddAsync(employee);
            return CreatedAtAction("GetEmployeeById", new { id = employee.Id },
                new EmployeeShortResponse() { Email = employee.Email, FullName = employee.FullName, });
        }

        [HttpPut("{id:guid}")]
        public async Task<ActionResult> UpdateEmployeeFullNameAsync(Guid id, [FromQuery] string firstName, [FromQuery] string lastName)
        {
            var employee = await _employeeRepository.GetByIdAsync(id);
            employee.FirstName = firstName;
            employee.LastName = lastName;

            return NoContent();
        }

        [HttpDelete("{id:guid}")]
        public async Task<ActionResult> DeleteEmployeeByIdAsync(Guid id)
        {
            await _employeeRepository.RemoveByIdAsync(id);

            return NoContent();
        }
    }
}