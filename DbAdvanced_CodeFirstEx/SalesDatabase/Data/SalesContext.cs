using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using P03_SalesDatabase.Data.Models;
using System.Linq;

namespace P03_SalesDatabase.Data
{
    public class SalesContext : DbContext
    {
        public DbSet<Product> Products { get; set; }

        public DbSet<Customer> Customers { get; set; }

        public DbSet<Store> Stores { get; set; }

        public DbSet<Sale> Sales { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Server = DESKTOP-FKR965V\SQLEXPRESS; Database = Sales; Trusted_Connection = True;");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            ConfigureProduct(modelBuilder);

            ConfigureCustomer(modelBuilder);

            ConfigureStore(modelBuilder);

            ConfigureSale(modelBuilder);
        }

        private void ConfigureSale(ModelBuilder modelBuilder)
        {
            modelBuilder
            .Entity<Sale>()
            .HasKey(p => p.SaleId);

            modelBuilder
            .Entity<Sale>()
            .Property(p => p.Date)
            .HasDefaultValueSql("getdate()");
        }

        private void ConfigureStore(ModelBuilder modelBuilder)
        {
            modelBuilder
             .Entity<Store>()
             .HasKey(p => p.StoreId);

            modelBuilder
             .Entity<Store>()
             .Property(n => n.Name)
             .HasMaxLength(80)
             .IsUnicode();

            modelBuilder
             .Entity<Store>()
             .HasMany(s => s.Sales)
             .WithOne(s => s.Store)
             .HasForeignKey(s => s.SaleId);
        }

        private void ConfigureCustomer(ModelBuilder modelBuilder)
        {
            modelBuilder
              .Entity<Customer>()
              .HasKey(p => p.CustomerId);

            modelBuilder
          .Entity<Customer>()
          .Property(p => p.Name)
          .HasMaxLength(100)
          .IsUnicode();

            modelBuilder
         .Entity<Customer>()
         .Property(p => p.Email)
         .HasMaxLength(80);

            modelBuilder
             .Entity<Customer>()
             .HasMany(s => s.Sales)
             .WithOne(c => c.Customer)
             .HasForeignKey(s => s.SaleId);
        }

        private void ConfigureProduct(ModelBuilder modelBuilder)
        {
            modelBuilder
                .Entity<Product>()
                .HasKey(p => p.ProductId);

            modelBuilder
              .Entity<Product>()
              .HasMany(s => s.Sales)
              .WithOne(p => p.Product)
              .HasForeignKey(s => s.SaleId);

            modelBuilder
              .Entity<Product>()
              .Property(p => p.Name)
              .HasMaxLength(50)
              .IsUnicode();

            modelBuilder
                .Entity<Product>()
                .Property(p => p.Description)
                .HasMaxLength(250)
                .HasDefaultValue("No description");
        }
    }
}
