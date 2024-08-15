using Microsoft.EntityFrameworkCore;
using MoneyTransfer.DataAccess.Abstract;
using MoneyTransfer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyTransfer.DataAccess.Concrete
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly ProjectDbContext _projectDbContext;

        public CustomerRepository(ProjectDbContext projectDbContext)
        {
            _projectDbContext = projectDbContext;
        }
        public Customer CreateCustomer(Customer customer)
        {
            _projectDbContext.Customers.Add(customer);
            _projectDbContext.SaveChanges();
            return customer;
        }

        public void DeleteCustomer(int id)
        {
            var deleteCustomer = GetCustomerById(id);
            _projectDbContext.Customers.Remove(deleteCustomer);
            _projectDbContext.SaveChanges();
        }

        public List<Customer> GetAllCustomers()
        {
            return _projectDbContext.Customers.ToList();
        }

        public Customer GetCustomerById(int id)
        {
            return _projectDbContext.Customers.Find(id)!;

        }

        public Customer UpdateCustomer(Customer customer)
        {
            _projectDbContext.Customers.Update(customer);
            _projectDbContext.SaveChanges();
            return customer;
        }

        public bool IsCustomerExists(string tc, string mail, string phoneNumber)
        {
            return _projectDbContext.Customers.Any(c => c.TC == tc || c.Mail == mail || c.PhoneNumber == phoneNumber);
        }

        public Customer GetCustomerByUsernameAndPassword(string username, string password)
        {
            return _projectDbContext.Customers.SingleOrDefault(c => (c.Mail == username || c.PhoneNumber == username || c.TC == username) && c.Password == password)!;
        }
    }
}
