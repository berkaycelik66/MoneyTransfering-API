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
    public class AccountManager : IAccountService
    {
        private IAccountRepository _accountRepository;

        public AccountManager(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }

        public Account CreateAccount(Account account)
        {
            return _accountRepository.CreateAccount(account);
        }

        public void DeleteAccount(int id)
        {
            _accountRepository.DeleteAccount(id);
        }

        public Account GetAccountById(int id)
        {
            return _accountRepository.GetAccountById(id);
        }

        public List<Account> GetAllAccount()
        {
            return _accountRepository.GetAllAccount();
        }

        public Account UpdateAccount(Account account)
        {
            return _accountRepository.UpdateAccount(account);
        }

        public List<Account> GetAccountsByCustomerId(int customerId)
        {
            return _accountRepository.GetAccountsByCustomerId(customerId);
        }
    }
}
