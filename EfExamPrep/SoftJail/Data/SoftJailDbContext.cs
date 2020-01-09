namespace SoftJail.Data
{
    using Microsoft.EntityFrameworkCore;
    using SoftJail.Data.Models;

    public class SoftJailDbContext : DbContext
    {
        public DbSet<Cell> Cells { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Mail> Mails { get; set; }
        public DbSet<Officer> Officers { get; set; }
        public DbSet<Prisoner> Prisoners { get; set; }
        public DbSet<OfficerPrisoner> OfficersPrisoners { get; set; }
        public SoftJailDbContext()
        {
        }

        public SoftJailDbContext(DbContextOptions options)
            : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder
                    .UseSqlServer(Configuration.ConnectionString);
            }
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            //Each Officer has special position and one or more prisoners to watch

            builder.Entity<OfficerPrisoner>()
                .HasKey(op => new { op.OfficerId, op.PrisonerId });

            builder.Entity<Prisoner>()
                .HasMany(p => p.PrisonerOfficers)
                .WithOne(p => p.Prisoner)
                .HasForeignKey(p => p.PrisonerId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Officer>()
                .HasMany(o => o.OfficerPrisoners)
                .WithOne(o => o.Officer)
                .HasForeignKey(o => o.OfficerId)
                .OnDelete(DeleteBehavior.Restrict);


            //Every Cell and Officer are placed in different Department
            builder.Entity<Department>()
                .HasMany(d => d.Cells)
                .WithOne(d => d.Department)
                .HasForeignKey(d => d.DepartmentId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Department>()
                .HasMany(o => o.Officers)
                .WithOne(d => d.Department)
                .HasForeignKey(d => d.DepartmentId)
                .OnDelete(DeleteBehavior.Restrict);

            //Every Prisoner has a cell and a collection of Mails which he gets during his staying at the prison
            builder.Entity<Prisoner>() //MAILS
                    .HasMany(m => m.Mails)
                    .WithOne(p => p.Prisoner)
                    .HasForeignKey(p => p.PrisonerId)
                    .OnDelete(DeleteBehavior.Restrict);


            builder.Entity<Prisoner>() //CELL
                .HasOne(c => c.Cell)
                .WithMany(p => p.Prisoners)
                .HasForeignKey(c => c.CellId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}