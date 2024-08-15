using MoneyTransfer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyTransfer.DataAccess.Abstract
{
    public interface IAccountRepository
    {
        List<Account> GetAllAccount();

        Account GetAccountById(int id);

        Account CreateAccount(Account account);

        Account UpdateAccount(Account account);

        void DeleteAccount(int id);

        public Account GetAccountByAccountNumber(string ToAccountNumber);

        List<Account> GetAccountsByCustomerId(int customerId);

        public bool IsAccountNumberExists(string accountNumber);
    }
}
