using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExchangeOffice.Service
{
    // This is the "Brain" that connects to your MDF file
    public class ExchangeDbContext : DbContext
    {
        public ExchangeDbContext() : base("name=ExchangeDbEntities") { }

        public DbSet<User> Users { get; set; }
        public DbSet<Wallet> Wallets { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
    }

    [Table("Users")]
    public class User
    {
        [Key]
        public int Id { get; set; }
        public string Username { get; set; }
    }

    [Table("Wallets")]
    public class Wallet
    {
        [Key]
        public int Id { get; set; }
        public int UserId { get; set; }
        public string CurrencyCode { get; set; }
        public decimal Balance { get; set; }
    }

    [Table("Transactions")]
    public class Transaction
    {
        [Key]
        public int Id { get; set; }
        public int UserId { get; set; }
        public string BoughtCurrency { get; set; }
        public string SoldCurrency { get; set; }
        public decimal ExchangeRate { get; set; }
        public decimal Amount { get; set; }
        public DateTime TransactionDate { get; set; }
    }
}