using System;

using Microsoft.EntityFrameworkCore;
using NLog;



namespace StepmaniaServer
{
    public class StepmaniaContext : DbContext
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private static Config config = new Config();

        public DbSet<Ban> Bans { get; set; }
        public DbSet<Game> Games { get; set; }
        public DbSet<Room> Rooms { get; set; }
        public DbSet<Song> Songs { get; set; }
        public DbSet<SongStatistic> SongStatistics { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string databaseProvider = config.Get("/config/database/type", "sqlite");

            switch (databaseProvider)
            {
                case "sqlite":
                    logger.Trace("Using SQLite provider");
                    string sqliteFilename = config.Get("/config/database/file", "database.db");
                    string sqliteConnectionString = String.Format("Filename={0}", sqliteFilename);

                    logger.Trace("Database location: {location}", sqliteFilename);
                    logger.Trace("Database connection string: {connString}", sqliteConnectionString);

                    optionsBuilder.UseSqlite(sqliteConnectionString);
                    break;

                case "mysql":
                    logger.Trace("Using MySQL provider");
                    break;

                default:
                    logger.Fatal("Unknown Database Provider");
                    break;
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Ban>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasOne(d => d.Room).WithMany(p => p.Bans);
                entity.HasOne(d => d.User).WithMany(p => p.Bans);
            });

            modelBuilder.Entity<Game>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasOne(d => d.Room).WithMany(p => p.Games);
                entity.HasOne(d => d.Song).WithMany(p => p.Games);
            });

            modelBuilder.Entity<Room>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasOne(d => d.ActiveSong).WithMany(p => p.ActiveRooms);
            });

            modelBuilder.Entity<Song>(entity =>
            {
                entity.HasKey(e => e.Id);
            });

            modelBuilder.Entity<SongStatistic>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasOne(d => d.Game).WithMany(p => p.SongStatistics);
                entity.HasOne(d => d.Song).WithMany(p => p.SongStatistics);
                entity.HasOne(d => d.User).WithMany(p => p.SongStatistics);
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasOne(d => d.CurrentRoom).WithMany(p => p.Users);
            });
        }
    }
}