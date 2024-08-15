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
    public class TransferRepository : ITransferRepository
    {
        private readonly ProjectDbContext _projectDbContext;

        public TransferRepository(ProjectDbContext projectDbContext)
        {
            _projectDbContext = projectDbContext;
        }
        public Transfer CreateTransfer(Transfer transfer)
        {
            _projectDbContext.Transfers.Add(transfer);
            _projectDbContext.SaveChanges();
            return transfer;   
        }

        public void DeleteTransfer(int id)
        {
            var deleteTransfer = GetTransferById(id);
            _projectDbContext.Transfers.Remove(deleteTransfer);
            _projectDbContext.SaveChanges();

        }

        public List<Transfer> GetAllTransfer()
        {
            return _projectDbContext.Transfers.ToList();
        }

        public Transfer GetTransferById(int id)
        {
            return _projectDbContext.Transfers.Find(id)!;

        }

        public Transfer UpdateTransfer(Transfer transfer)
        {
            _projectDbContext.Transfers.Update(transfer);
            _projectDbContext.SaveChanges();
            return transfer;

        }

        public List<Transfer> GetAllTransferByAccountId(int accountId)
        {
            return _projectDbContext.Transfers.Where(a => (a.AccountFromId == accountId || a.AccountToId == accountId)).ToList();
        }

        public List<Transfer> GetAllTransfersByAccountIdWithCustomer(int accountId)
        {
            return _projectDbContext.Transfers
                .Include(t => t.AccountFrom)
                    .ThenInclude(a => a.Customer)
                .Include(t => t.AccountTo)
                    .ThenInclude(a => a.Customer)
                .Where(t => t.AccountFromId == accountId || t.AccountToId == accountId)
                .ToList();
        }
    }
}
