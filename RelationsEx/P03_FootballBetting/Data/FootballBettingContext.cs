using Microsoft.EntityFrameworkCore;
using P03_FootballBetting.Data.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace P03_FootballBetting.Data
{
    public class FootballBettingContext : DbContext
    {
        public DbSet<Bet> Bets { get; set; }

        public DbSet<Color> Colors { get; set; }

        public DbSet<Country> Countries { get; set; }

        public DbSet<Game> Games { get; set; }

        public DbSet<Player> Players { get; set; }

        public DbSet<PlayerStatistic> PlayerStatistics { get; set; }

        public DbSet<Position> Positions { get; set; }

        public DbSet<Team> Teams { get; set; }

        public DbSet<Town> Towns { get; set; }

        public DbSet<User> Users { get; set; }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Server=DESKTOP-FKR965V\SQLEXPRESS;Database=FootballBetting;Trusted_Connection=True;");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            ConfigTeam(modelBuilder);

            ConfigColor(modelBuilder);

            ConfigTown(modelBuilder);

            ConfigCountry(modelBuilder);

            ConfigPlayer(modelBuilder);

            ConfigPosition(modelBuilder);

            ConfigGame(modelBuilder);

            ConfigBet(modelBuilder);

            ConfigUser(modelBuilder);

            ConfigPlayerStatistic(modelBuilder);
        }

        private void ConfigPlayerStatistic(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PlayerStatistic>().HasKey(ck => new { ck.GameId , ck.PlayerId });

            modelBuilder.Entity<PlayerStatistic>().HasOne(p => p.Player).WithMany(g => g.Games).HasForeignKey(p => p.PlayerId);

            modelBuilder.Entity<PlayerStatistic>().HasOne(g => g.Game).WithMany(p => p.Players).HasForeignKey(g => g.GameId);

            modelBuilder.Entity<PlayerStatistic>().Property(p => p.Player).IsRequired();

            modelBuilder.Entity<PlayerStatistic>().Property(p => p.Game).IsRequired();

            modelBuilder.Entity<PlayerStatistic>().HasKey(ck => new { ck.PlayerId, ck.GameId });

        }

        private void ConfigUser(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(p => p.UserId);
                entity.Property(p => p.Username).IsRequired();
                entity.Property(p => p.Password).IsRequired();
                entity.Property(p => p.Balance).IsRequired();
            });
        }

        private void ConfigBet(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Bet>(entity =>
            {
                entity.HasKey(p => p.BetId);
                entity.Property(p => p.Amount).IsRequired();
                entity.Property(p => p.Prediction).IsRequired();
                entity.Property(p => p.Amount).IsRequired();
                entity.Property(p => p.DateTime).IsRequired();
                entity.Property(p => p.UserId).IsRequired();
                entity.Property(p => p.GameId).IsRequired();

                entity.HasOne(b => b.Game).WithMany(g => g.Bets).HasForeignKey(b => b.GameId);

                entity.HasOne(b => b.User).WithMany(u => u.Bets).HasForeignKey(b => b.UserId);
            });
        }

        private void ConfigGame(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Game>(entity =>
            {
                entity.HasKey(p => p.GameId);
                entity.Property(p => p.HomeTeamId).IsRequired();
                entity.Property(p => p.AwayTeamId).IsRequired();
                entity.Property(p => p.DateTime).IsRequired();
                entity.Property(p => p.HomeTeamBetRate).IsRequired();
                entity.Property(p => p.AwayTeamBetRate).IsRequired();
                entity.Property(p => p.DrawBetRate).IsRequired();
                entity.Property(p => p.Result).IsRequired();


                entity.HasOne(g => g.HomeTeam).WithMany(t => t.HomeGames).HasForeignKey(g => g.HomeTeamId).HasForeignKey(g => g.HomeTeamId).OnDelete(DeleteBehavior.Restrict);
                

                entity.HasOne(g => g.AwayTeam).WithMany(t => t.AwayGames).HasForeignKey(g => g.AwayTeamId).HasForeignKey(g => g.AwayTeamId).OnDelete(DeleteBehavior.Restrict);
                
            });
        }

        private void ConfigPosition(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Position>(entity =>
            {
                entity.HasKey(p => p.PositionId);
                entity.Property(p => p.Name).IsRequired();
            });
        }

        private void ConfigPlayer(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Player>(entity =>
            {
                entity.HasKey(p => p.PlayerId);
                entity.Property(p => p.Name).IsRequired();
                entity.Property(p => p.SquadNumber).IsRequired();
                entity.Property(p => p.TeamId).IsRequired();
                entity.Property(p => p.PositionId).IsRequired();

                entity.HasOne(p => p.Team).WithMany(t => t.Players).HasForeignKey(p => p.TeamId);

                entity.HasOne(p => p.Position).WithMany(po => po.Players).HasForeignKey(p => p.PositionId);
            });
        }

        private void ConfigCountry(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Country>(entity =>
            {
                entity.HasKey(p => p.CountryId);
                entity.Property(p => p.Name).IsRequired();

            });
        }

        private void ConfigTown(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Town>(entity =>
            {
                entity.HasKey(p => p.TownId);
                entity.Property(p => p.Name).IsRequired();

                entity.HasOne(t => t.Country).WithMany(c => c.Towns).HasForeignKey(t => t.CountryId);

            });
        }

        private void ConfigColor(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Color>(entity =>
            {
                entity.HasKey(c => c.ColorId);
                entity.Property(p => p.Name).IsRequired();
            });
        }

        private void ConfigTeam(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Team>(entity =>
            {
                entity.HasKey(t => t.TeamId);

                entity.Property(p => p.Name).IsRequired();

                entity.Property(p => p.LogoUrl).IsRequired();

                entity.Property(p => p.Initials).IsRequired().HasColumnType("CHAR(3)");

                entity.Property(p => p.Budget).IsRequired();

                entity.Property(p => p.TownId).IsRequired();

               entity.HasOne(t => t.PrimaryKitColor).WithMany(c => c.PrimaryKitTeams).HasForeignKey(t => t.PrimaryKitColorId).OnDelete(DeleteBehavior.Restrict);
                ;

                entity.HasOne(t => t.SecondaryKitColor).WithMany(c => c.SecondaryKitTeams).HasForeignKey(t => t.SecondaryKitColorId).OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(t => t.Town).WithMany(to => to.Teams).HasForeignKey(t => t.TownId);
            });
        }
    }
}
