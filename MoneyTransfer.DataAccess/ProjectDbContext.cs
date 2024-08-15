using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MoneyTransfer.Entities;
using Microsoft.Extensions.Options;
using System.Reflection.Emit;
using System.Security.Principal;

namespace MoneyTransfer.DataAccess
{
    public class ProjectDbContext : DbContext
    {
        public ProjectDbContext(DbContextOptions<ProjectDbContext> options): base(options)
        {
        }

        //DbSet<> -> Class'ların veritabanında bir tabloya karşılık geldiğini belirtir.
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Account> Accounts { get; set; }
        public DbSet<Transfer> Transfers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Customer>();

            modelBuilder.Entity<Account>(entity =>
            {
                entity.HasMany(e => e.TransfersFrom)
                      .WithOne(t => t.AccountFrom)
                      .HasForeignKey(t => t.AccountFromId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasMany(e => e.TransfersTo)
                      .WithOne(t => t.AccountTo)
                      .HasForeignKey(t => t.AccountToId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Transfer>();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.UseSqlServer("Server=DESKTOP-USOAJ0L\\SQLEXPRESS; Database=MoneyTransfer; Integrated Security=true; TrustServerCertificate=true;");
        }
    }
}
