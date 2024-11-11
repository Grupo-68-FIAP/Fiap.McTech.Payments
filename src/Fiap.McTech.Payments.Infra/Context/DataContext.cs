using Fiap.McTech.Payments.CrossCutting.Exceptions;
using Fiap.McTech.Payments.Infra.EntityMapper;
using Microsoft.EntityFrameworkCore;

namespace Fiap.McTech.Payments.Infra.Context
{
    public class DataContext : DbContext
    {
        public DbSet<Fiap.McTech.Payments.Domain.Entities.Payments> Payments { get; set; }

        public DataContext() : base() { }

        public DataContext(DbContextOptions<DataContext> options) : base(options) { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (optionsBuilder.IsConfigured)
                return;
            // configure database to run migrations
            var connectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING");
            if (string.IsNullOrEmpty(connectionString))
                throw new DatabaseException("Environment variable [CONNECTION_STRING] is null.");
            optionsBuilder.UseSqlServer(connectionString);
            Console.WriteLine("Connected: {0}", connectionString);
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new PaymentsMap());
        }
    }
}
