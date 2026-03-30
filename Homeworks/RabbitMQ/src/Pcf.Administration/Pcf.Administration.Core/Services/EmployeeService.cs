using Pcf.Administration.Core.Abstractions.Repositories;
using Pcf.Administration.Core.Domain.Administration;
using System;
using System.Threading.Tasks;

namespace Pcf.Administration.Core.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IRepository<Employee> _employeeRepository;

        public EmployeeService(IRepository<Employee> employeeRepository)
        {
            _employeeRepository = employeeRepository;
        }

        public async Task UpdateAppliedPromocodesAsync(Guid id)
        {
            var employee = await _employeeRepository.GetByIdAsync(id);
            if (employee != null)
            {
                employee.AppliedPromocodesCount++;
                await _employeeRepository.UpdateAsync(employee);
            }
        }
    }
}
