using MoneyTransfer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyTransfer.Business.Abstract
{
    public interface IAccountService
    {
        List<Account> GetAllAccount();

        Account GetAccountById(int id);

        Account CreateAccount(Account account);

        Account UpdateAccount(Account account);

        void DeleteAccount(int id);

        List<Account> GetAccountsByCustomerId(int customerId);
    }
}
