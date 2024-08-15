using MoneyTransfer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyTransfer.Business.Abstract
{
    public interface ITransferService
    {
        List<Transfer> GetAllTransfer();

        Transfer GetTransferById(int id);

        Transfer CreateTransfer(Transfer transfer);

        Transfer UpdateTransfer(Transfer transfer);

        void DeleteTransfer(int id);

        List<Transfer> GetAllTransferByAccountId(int accountId);

        public bool TransferMoney(int accountFromId, string toAccountNumber, decimal amount, string description);

        List<Transfer> GetAllTransfersByAccountIdWithCustomer(int accountId);

    }
}
