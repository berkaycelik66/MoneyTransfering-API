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
    public class AccountRepository : IAccountRepository
    {
        private readonly ProjectDbContext _projectDbContext;

        public AccountRepository(ProjectDbContext projectDbContext)
        {
            _projectDbContext = projectDbContext;
        }

        public Account CreateAccount(Account account)
        {
            _projectDbContext.Accounts.Add(account);
            _projectDbContext.SaveChanges();
            return account;
        }

        public void DeleteAccount(int id)
        {
            var deleteAccount = GetAccountById(id);
            _projectDbContext.Accounts.Remove(deleteAccount);
            _projectDbContext.SaveChanges();
        }

        public List<Account> GetAllAccount()
        {
            return _projectDbContext.Accounts.ToList();
        }

        public Account GetAccountById(int id)
        {
            return _projectDbContext.Accounts.Find(id)!;
        }

        public Account UpdateAccount(Account account)
        {
            _projectDbContext.Accounts.Update(account);
            _projectDbContext.SaveChanges();
            return account;
        }

        public Account GetAccountByAccountNumber(string toAccountNumber)
        {
            return _projectDbContext.Accounts.FirstOrDefault(a => a.AccountNumber!.Equals(toAccountNumber))!;
        }

        public List<Account> GetAccountsByCustomerId(int customerId)
        {
            return _projectDbContext.Accounts.Where(a => a.CustomerId == customerId).ToList();
        }

        public bool IsAccountNumberExists(string accountNumber)
        {
            return _projectDbContext.Accounts.Any(a => a.AccountNumber == accountNumber);
        }
    }
}
