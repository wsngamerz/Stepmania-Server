using System;

using Microsoft.EntityFrameworkCore;
using NLog;



namespace StepmaniaServer
{
    // Stepmania Server's Database Context
    public class StepmaniaContext : DbContext
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        
        private static Config config = new Config();

        // Load all of the models
        public DbSet<Ban> Bans { get; set; }
        public DbSet<Game> Games { get; set; }
        public DbSet<Room> Rooms { get; set; }
        public DbSet<Song> Songs { get; set; }
        public DbSet<SongStatistic> SongStatistics { get; set; }
        public DbSet<SongUpdate> SongUpdates { get; set; }
        public DbSet<User> Users { get; set; }


        // conigure the provider
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // get the selected provider from the config
            string databaseProvider = config.Get("/config/database/type", "sqlite");

            switch (databaseProvider)
            {
                case "sqlite":
                    // setup connection based on config
                    logger.Trace("Using SQLite provider");
                    string sqliteFilename = config.Get("/config/database/file", "database.db");
                    string sqliteConnectionString = String.Format("Filename={0}", sqliteFilename);

                    logger.Trace("Database location: {location}", sqliteFilename);
                    logger.Trace("Database connection string: {connString}", sqliteConnectionString);

                    optionsBuilder.UseSqlite(sqliteConnectionString);
                    break;

                case "mysql":
                    // TODO: Implement
                    logger.Trace("Using MySQL provider");
                    break;

                default:
                    logger.Fatal("Unknown Database Provider");
                    break;
            }
        }

        // Apply more information to the models such as setting their
        // Primary keys and relations between models
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Bans
            modelBuilder.Entity<Ban>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasOne(d => d.Room).WithMany(p => p.Bans);
                entity.HasOne(d => d.User).WithMany(p => p.Bans);
            });

            // Games
            modelBuilder.Entity<Game>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasOne(d => d.Room).WithMany(p => p.Games);
                entity.HasOne(d => d.Song).WithMany(p => p.Games);
            });

            // Rooms
            modelBuilder.Entity<Room>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasOne(d => d.ActiveSong).WithMany(p => p.ActiveRooms);
            });

            // Songs
            modelBuilder.Entity<Song>(entity =>
            {
                entity.HasKey(e => e.Id);
            });

            // SongStatistics
            modelBuilder.Entity<SongStatistic>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasOne(d => d.Game).WithMany(p => p.SongStatistics);
                entity.HasOne(d => d.Song).WithMany(p => p.SongStatistics);
                entity.HasOne(d => d.User).WithMany(p => p.SongStatistics);
                entity.HasMany(d => d.Updates).WithOne(p => p.SongStatistic);
            });

            // SongUpdates
            modelBuilder.Entity<SongUpdate>(entity => 
            {
                entity.HasKey(e => e.Id);
            });

            // Users
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasOne(d => d.CurrentRoom).WithMany(p => p.Users);
            });
        }
    }
}
