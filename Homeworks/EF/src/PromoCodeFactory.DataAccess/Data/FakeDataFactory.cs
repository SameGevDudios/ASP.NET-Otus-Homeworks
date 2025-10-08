using System;
using System.Collections.Generic;
using System.Linq;
using PromoCodeFactory.Core.Domain.Administration;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;

namespace PromoCodeFactory.DataAccess.Data
{
    public static class FakeDataFactory
    {
        private static readonly List<Employee> _employees;
        private static readonly List<Role> _roles;
        private static readonly List<Preference> _preferences;
        private static readonly List<CustomerPreference> _customerPreferences;
        private static readonly List<Customer> _customers;
        private static readonly List<PromoCode> _promoCodes;

        private static readonly Guid s_customerId1 = Guid.Parse("a6c8c6b1-4349-45b0-ab31-244740aaf0f0");
        private static readonly Guid s_customerId2 = Guid.Parse("3f8a1e2b-7c4d-4f91-a6b2-9f3c2d1e8b7a");

        static FakeDataFactory()
        {
            _roles = new List<Role>()
            {
                new Role
                {
                    Id = Guid.Parse("53729686-a368-4eeb-8bfa-cc69b6050d02"),
                    Name = "Admin",
                    Description = "Администратор",
                },
                new Role
                {
                    Id = Guid.Parse("b0ae7aac-5493-45cd-ad16-87426a5e7665"),
                    Name = "PartnerManager",
                    Description = "Партнерский менеджер"
                }
            };

            _employees = new List<Employee>()
            {
                new Employee
                {
                    Id = Guid.Parse("451533d5-d8d5-4a11-9c7b-eb9f14e1a32f"),
                    Email = "owner@somemail.ru",
                    FirstName = "Иван",
                    LastName = "Сергеев",
                    Role = _roles.First(x => x.Name == "Admin"),
                    AppliedPromocodesCount = 5
                },
                new Employee
                {
                    Id = Guid.Parse("f766e2bf-340a-46ea-bff3-f1700b435895"),
                    Email = "andreev@somemail.ru",
                    FirstName = "Петр",
                    LastName = "Андреев",
                    Role = _roles.First(x => x.Name == "PartnerManager"),
                    AppliedPromocodesCount = 10
                },
                new Employee
                {
                    Id = Guid.Parse("c2e7a4d9-81f5-4d43-9d8e-73f5c2a9e4b1"),
                    Email = "nikolaev@somemail.ru",
                    FirstName = "Сергей",
                    LastName = "Николаев",
                    Role = _roles.First(x => x.Name == "PartnerManager"),
                    AppliedPromocodesCount = 5
                }
            };

            _preferences = new List<Preference>()
            {
                new Preference
                {
                    Id = Guid.Parse("ef7f299f-92d7-459f-896e-078ed53ef99c"),
                    Name = "Театр",
                },
                new Preference
                {
                    Id = Guid.Parse("c4bda62e-fc74-4256-a956-4760b3858cbd"),
                    Name = "Семья",
                },
                new Preference
                {
                    Id = Guid.Parse("76324c47-68d2-472d-abb8-33cfa8cc0c84"),
                    Name = "Дети",
                }
            };

            _customers = new List<Customer>()
            {
                new Customer
                {
                    Id = s_customerId1,
                    Email = "ivan_sergeev@mail.ru",
                    FirstName = "Иван",
                    LastName = "Петров"
                },
                new Customer
                {
                    Id = s_customerId2,
                    Email = "alex_ivanov@mail.ru",
                    FirstName = "Александр",
                    LastName = "Иванов"
                }
            };

            _promoCodes = new List<PromoCode>()
            {
                new PromoCode
                {
                    Id = Guid.Parse("b7e62b53-3a2c-45cf-8a07-1196b2dbcc5d"),
                    Code = "A7F3-91BC-D204-5E8A",
                    BeginDate = DateTime.Parse("2025-09-30 12:30:00"),
                    EndDate = DateTime.Parse("2025-10-14 12:30:00"),
                    PartnerName = "Ozon",
                    PartnerManager = _employees.First(x => x.Role.Name == "PartnerManager"),
                    Customer = _customers.First(x => x.FirstName == "Иван"),
                    Preference = _preferences.First(x => x.Name == "Театр"),
                    ServiceInfo = "Скидка на билет в Большой театр"
                },
                new PromoCode
                {
                    Id = Guid.Parse("3f8a1e4c-9b27-4c62-af81-2e7b05d74a91"),
                    Code = "4C9E-FF12-7B6A-3D0F",
                    BeginDate = DateTime.Parse("2025-10-05 08:15:00"),
                    EndDate = DateTime.Parse("2025-10-19 08:15:00"),
                    PartnerName = "Wildberries",
                    PartnerManager = _employees.First(x => x.Role.Name == "PartnerManager"),
                    Customer = _customers.First(x => x.FirstName == "Александр"),
                    Preference = _preferences.First(x => x.Name == "Семья"),
                    ServiceInfo = "Скидка на термобельё"
                },
                new PromoCode
                {
                    Id = Guid.Parse("7b3c2f18-4d91-45a7-9c3f-2a67ef09d5e4"),
                    Code = "4C9E-FF12-7B6A-3D0F",
                    BeginDate = DateTime.Parse("2025-09-30 10:00:00"),
                    EndDate = DateTime.Parse("2025-10-07 10:00:00"),
                    PartnerName = "Wildberries",
                    PartnerManager = _employees.First(x => x.Role.Name == "PartnerManager"),
                    Customer = _customers.First(x => x.FirstName == "Александр"),
                    Preference = _preferences.First(x => x.Name == "Дети"),
                    ServiceInfo = "Скидка на детское питание"
                }
            };

            _customerPreferences = new List<CustomerPreference>()
            {
                new CustomerPreference
                {
                    Id = Guid.Parse("a1f2b3c4-5d6e-7f81-9a2b-3c4d5e6f7a8b"),
                    CustomerId = s_customerId1,
                    PreferenceId = _preferences.First(x => x.Name == "Театр").Id
                },
                new CustomerPreference
                {
                    Id = Guid.Parse("3f2504e0-4f89-11d3-9a0c-0305e82c3301"),
                    CustomerId = s_customerId2,
                    PreferenceId = _preferences.First(x => x.Name == "Семья").Id
                },
                new CustomerPreference
                {
                    Id = Guid.Parse("550e8400-e29b-41d4-a716-446655440000"),
                    CustomerId = s_customerId2,
                    PreferenceId = _preferences.First(x => x.Name == "Дети").Id
                }
            };

            // Tie up reverse relations
            foreach (var customer in _customers)
            {
                customer.Promocodes = _promoCodes.Where(pc => pc.Customer.Id == customer.Id).ToList();
                customer.CustomerPreferences = _customerPreferences.Where(cp => cp.CustomerId == customer.Id).ToList();
            }
            foreach (var preference in _preferences)
            {
                preference.CustomerPreferences = _customerPreferences.Where(cp => cp.PreferenceId  == preference.Id).ToList();
            }
            foreach(var employee  in _employees)
            {
                employee.PromoCodes = _promoCodes.Where(p => p.PartnerManager.Id == employee.Id).ToList();
            }
        }

        public static IEnumerable<Employee> Employees => _employees;
        public static IEnumerable<Role> Roles => _roles;
        public static IEnumerable<Preference> Preferences => _preferences;
        public static IEnumerable<CustomerPreference> CustomersPreferences => _customerPreferences;
        public static IEnumerable<Customer> Customers => _customers;
        public static IEnumerable<PromoCode> PromoCodes => _promoCodes;
    }
}
