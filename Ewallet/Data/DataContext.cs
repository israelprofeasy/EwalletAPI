using Ewallet.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ewallet.Data
{
    public class DataContext : IdentityDbContext<User>
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) { }

        //public DbSet<Role> Roles { get; set; }
        //public DbSet<User> Users { get; set; }
        //public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<Wallet> Wallets { get; set; }
        public DbSet<Currency> Currencies { get; set; }
        public DbSet<WalletCurrency> WalletCurrency { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<Photo> Photos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<User>().Property(u => u.ConcurrencyStamp).IsConcurrencyToken();
            modelBuilder.Entity<Wallet>().Property(p => p.Balance).HasColumnType("decimal(18,4)");
            modelBuilder.Entity<WalletCurrency>().Property(p => p.Balance).HasColumnType("decimal(18,4)");
            modelBuilder.Entity<Transaction>().Property(p => p.Amount).HasColumnType("decimal(18,4)");
        }
    }
}
