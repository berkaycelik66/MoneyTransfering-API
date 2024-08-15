using MoneyTransfer.Business.Abstract;
using MoneyTransfer.DataAccess.Abstract;
using MoneyTransfer.DataAccess.Concrete;
using MoneyTransfer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyTransfer.Business.Concrete
{
    public class CustomerManager : ICustomerService
    {
        private ICustomerRepository _customerRepository;
        private IAccountRepository _accountRepository;
        public CustomerManager(ICustomerRepository customerRepository, IAccountRepository accountRepository)
        {
            _customerRepository = customerRepository;
            _accountRepository = accountRepository;
        }

        public void DeleteCustomer(int id)
        {
            _customerRepository.DeleteCustomer(id);
        }

        public List<Customer> GetAllCustomers()
        {
            return _customerRepository.GetAllCustomers();
        }

        public Customer GetCustomerById(int id)
        {
            return _customerRepository.GetCustomerById(id);
        }

        public Customer UpdateCustomer(Customer customer)
        {
            return _customerRepository.UpdateCustomer(customer);
        }

        public Customer Login(string username, string password)
        {
            return _customerRepository.GetCustomerByUsernameAndPassword(username, password);
        }

        public Customer CreateCustomer(Customer customer)
        {
            // TC, Mail ve Telefon Numarasının daha önce kullanılıp kullanılmadığını kontrol eder
            if (_customerRepository.IsCustomerExists(customer.TC, customer.Mail!, customer.PhoneNumber!))
            {
                throw new Exception("TC, Mail veya Telefon Numarası zaten kullanılıyor.");
            }

            var createdCustomer = _customerRepository.CreateCustomer(customer);

            //Benzersiz bir hesap numarası oluşturur.
            string accountNumber = GenerateRandomAccountNumber();
            while (_accountRepository.IsAccountNumberExists(accountNumber))
            {
                accountNumber = GenerateRandomAccountNumber();
            }

            var account = new Account
            {
                AccountNumber = accountNumber,
                Balance = 0,
                AccountCurrency = "TRY",
                CustomerId = createdCustomer.Id,
                Customer = createdCustomer
            };
             _accountRepository.CreateAccount(account);

            return createdCustomer;
        }

        private string GenerateRandomAccountNumber()
        {
            var random = new Random();
            return new string(Enumerable.Repeat("0123456789", 16)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}
