using MoneyTransfer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyTransfer.Business.Abstract
{
    public interface ICustomerService
    {
        List<Customer> GetAllCustomers();

        Customer GetCustomerById(int id);

        Customer CreateCustomer(Customer customer);

        Customer UpdateCustomer(Customer customer);

        void DeleteCustomer(int id);

        Customer Login(string username, string password);  // Login metodu
    }
}
