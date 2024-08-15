using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using MoneyTransfer.Business.Abstract;
using MoneyTransfer.DataAccess;
using MoneyTransfer.DataAccess.Abstract;
using MoneyTransfer.DataAccess.Concrete;
using MoneyTransfer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace MoneyTransfer.Business.Concrete
{
    public class TransferManager : ITransferService
    {
        private ITransferRepository _transferRepository;
        private IAccountRepository _accountRepository;
        private ProjectDbContext _dbContext;

        public TransferManager(ITransferRepository transferRepository, IAccountRepository accountRepository, ProjectDbContext dbContext)
        {
            _transferRepository = transferRepository;
            _accountRepository = accountRepository;
            _dbContext = dbContext;
        }

        public Transfer CreateTransfer(Transfer transfer)
        {
            return _transferRepository.CreateTransfer(transfer);
        }

        public void DeleteTransfer(int id)
        {
            _transferRepository.DeleteTransfer(id);
        }

        public List<Transfer> GetAllTransfer()
        {
            return _transferRepository.GetAllTransfer();
        }

        public Transfer GetTransferById(int id)
        {
            return _transferRepository.GetTransferById(id);
        }

        public Transfer UpdateTransfer(Transfer transfer)
        {
            return _transferRepository.UpdateTransfer(transfer);
        }

        public List<Transfer> GetAllTransferByAccountId(int accountId) 
        {
            return _transferRepository.GetAllTransferByAccountId(accountId);
        }

        public bool TransferMoney(int accountFromId, string toAccountNumber, decimal amount, string description)
        {
            if (amount < 0)
            {
                return false;
            }

            decimal fee = amount * 0.001m;  //%0.1 fee
            var accountFrom = _accountRepository.GetAccountById(accountFromId);
            var accountTo = _accountRepository.GetAccountByAccountNumber(toAccountNumber);

            if (accountFrom == null || accountTo == null)
            {
                return false;
            }

            if(accountFromId == accountTo.Id)
            {
                return false;
            }

                if (accountFrom.Balance < (amount + fee))
            {
                return false;
            }

            accountFrom.Balance -= amount + fee;
            accountTo.Balance += amount;

            // Transfer kaydı
            var transfer = new Transfer
            {
                AccountFromId = accountFrom.Id,
                AccountToId = accountTo.Id,
                Amount = amount,
                Fee = fee,
                TransferCurrency = accountFrom.AccountCurrency,
                Date = DateTime.Now,
                Description = description,
                AccountFrom = _accountRepository.GetAccountById(accountFromId),
                AccountTo = _accountRepository.GetAccountById(accountTo.Id)
            };

            using (var transaction = _dbContext.Database.BeginTransaction())
            {
                try
                {
                    _accountRepository.UpdateAccount(accountFrom);
                    //throw new Exception(); //test için
                    _accountRepository.UpdateAccount(accountTo);

                    _transferRepository.CreateTransfer(transfer);
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    return false;
                }
            }
            
            return true;
        }

        public List<Transfer> GetAllTransfersByAccountIdWithCustomer(int accountId)
        {
            return _dbContext.Transfers
                .Include(t => t.AccountFrom)
                    .ThenInclude(a => a.Customer)
                .Include(t => t.AccountTo)
                    .ThenInclude(a => a.Customer)
                .Where(t => t.AccountFromId == accountId || t.AccountToId == accountId)
                .ToList();
        }
    }
}
