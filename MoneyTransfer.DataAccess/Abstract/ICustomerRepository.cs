using MoneyTransfer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyTransfer.DataAccess.Abstract
{
    public interface ICustomerRepository
    {
        List<Customer> GetAllCustomers();

        Customer GetCustomerById(int id);

        Customer CreateCustomer(Customer customer);

        Customer UpdateCustomer(Customer customer);

        void DeleteCustomer(int id);

        bool IsCustomerExists(string tc, string mail, string phoneNumber);

        Customer GetCustomerByUsernameAndPassword(string username, string password);  //Login metodu
    }
}
