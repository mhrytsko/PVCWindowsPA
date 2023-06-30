using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using webapi.Models;

namespace webapi.Database
{
    public class AppDbContext : IdentityUserContext<User, Guid>
    {
        private readonly IServiceProvider _serviceProvider;

        //public AppDbContext()
        //{
        //}

        public AppDbContext([NotNullAttribute] DbContextOptions<AppDbContext> options, IServiceProvider serviceProvider) : base(options)
        {
            _serviceProvider = serviceProvider;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                IConfigurationRoot configuration = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json")
                    .Build();

                var connectionString = configuration.GetConnectionString("DefaultConnection") ?? configuration.GetConnectionString("LocalSqlServer");

                if (string.IsNullOrEmpty(connectionString))
                    throw new Exception("Erro - The connection string is missing!");

                optionsBuilder
                    .UseSqlServer(connectionString);
            }
        }

        public UserManager<User>? UserManager => _serviceProvider.GetService<UserManager<User>>();


        public DbSet<Window> Windows { get; set; }
        public DbSet<WindowColor> WindowColors { get; set; }
        public DbSet<WindowProfile> WindowProfiles { get; set; }
        public DbSet<WindowGlassType> WindowGlassTypes { get; set; }
        public DbSet<LeafConfiguration> LeafConfigurations { get; set; }
        public DbSet<PersonalDetail> PersonalDetails { get; set; }
        public DbSet<Brand> Brands { get; set; }
        public DbSet<Budget> Budgets { get; set; }
        public DbSet<BudgetWindow> BudgetWindows { get; set; }
        public DbSet<WindowProfileColor> WindowProfileColors { get; set; }
        public DbSet<Models.Image> Images { get; set; }
        public DbSet<Models.Client> Clients { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // 1º - Relationships configuration

            // Utilizadores
            modelBuilder.Entity<User>(entity =>
            {
                //entity.ToTable("Users");

                // User to Personal data relationship
                entity
                    .HasOne(u => u.PersonalDetail)
                    .WithOne()
                    .HasForeignKey<User>(u => u.PersonalDataId);

                // User to Budget relationship
                entity
                    .HasMany(u => u.Budgets)
                    .WithOne(b => b.User)
                    .HasForeignKey(b => b.UserId);

            });

            // Janelas
            modelBuilder.Entity<Window>(entity =>
            {

                // Window to WindowColor relationship
                entity
                    .HasOne(w => w.IndorColor)
                    .WithMany()
                    .HasForeignKey(w => w.IndorColorId);

                entity
                    .HasOne(w => w.OutdorColor)
                    .WithMany()
                    .HasForeignKey(w => w.OutdorColorId);

                // Window to WindowProfile relationship
                entity
                    .HasOne(w => w.WindowProfile)
                    .WithMany()
                    .HasForeignKey(w => w.WindowProfileId);
            });

            // Orçamentos e Janelas
            modelBuilder.Entity<BudgetWindow>(entity =>
            {
                entity
                    .HasKey(bw => new { bw.BudgetId, bw.WindowId });

                entity
                    .HasOne(bw => bw.Budget)
                    .WithMany(b => b.BudgetWindows)
                    .HasForeignKey(bw => bw.BudgetId);

                entity
                    .HasOne(bw => bw.Window)
                    .WithMany()
                    .HasForeignKey(bw => bw.WindowId);
            });

            // Folhas da janela
            modelBuilder.Entity<LeafConfiguration>(entity =>
            {
                // LeafConfiguration to Window relationship
                entity
                    .HasOne(lc => lc.Window)
                    .WithMany(w => w.LeafConfigurations)
                    .HasForeignKey(lc => lc.WindowId);

                // LeafConfiguration to WindowGlassType relationship
                entity
                    .HasOne(lc => lc.WindowGlassType)
                    .WithMany()
                    .HasForeignKey(lc => lc.WindowGlassTypeId);

            });

            // Cores compativeis com os perfis das janelas
            modelBuilder.Entity<WindowProfileColor>(entity =>
            {
                entity
                    .HasKey(pc => new { pc.ProfileId, pc.ColorId });

                entity
                    .HasOne(pc => pc.Profile)
                    .WithMany(p => p.Colors)
                    .HasForeignKey(pc => pc.ProfileId);
                //.OnDelete(DeleteBehavior.Cascade);

                entity
                    .HasOne(pc => pc.Color)
                    .WithMany()
                    .HasForeignKey(pc => pc.ColorId);
                    //.OnDelete(DeleteBehavior.Cascade);
            });

            // Marcas
            modelBuilder.Entity<Brand>(entity =>
            {
                entity
                    .HasMany<WindowGlassType>()
                    .WithOne(wgt => wgt.Brand)
                    .HasForeignKey(wgt => wgt.BrandId);

                entity
                    .HasMany<WindowProfile>()
                    .WithOne(wp => wp.Brand)
                    .HasForeignKey(wp => wp.BrandId);

                entity
                    .HasMany<WindowColor>()
                    .WithOne(wc => wc.Brand)
                    .HasForeignKey(wc => wc.BrandId);
            });

            // Clientes
            /*modelBuilder.Entity<Client>()
                .Property(p => p.Name)
                .HasComputedColumnSql("[Preco] * [Quantidade]");*/


            base.OnModelCreating(modelBuilder);
        }

        public override int SaveChanges()
        {
            var entries = ChangeTracker
                .Entries()
                .Where(e => e.Entity is Models.ModelBase && (
                        e.State == EntityState.Added
                        || e.State == EntityState.Modified));

            foreach (var entityEntry in entries)
            {
                if (entityEntry.State == EntityState.Added)
                    ((Models.ICrudModel)entityEntry.Entity).CreationDate = DateTime.Now;
                if (entityEntry.State == EntityState.Modified)
                    ((Models.ICrudModel)entityEntry.Entity).ModificationDate = DateTime.Now;
            }

            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var entries = ChangeTracker
                .Entries()
                .Where(e => e.Entity is Models.ModelBase && (
                        e.State == EntityState.Added
                        || e.State == EntityState.Modified));

            foreach (var entityEntry in entries)
            {
                // Timestamp da criação e alteração.
                // Geração dos Id's novos para os que estão vazios.
                if(entityEntry.Entity is Models.ICrudModel entry)
                {
                    if (entityEntry.State == EntityState.Added)
                    {
                        entry.CreationDate = DateTime.Now;
                        if (entry.Id == Guid.Empty)
                            entry.GenerateId();
                    }
                    if (entityEntry.State == EntityState.Modified)
                        entry.ModificationDate = DateTime.Now;
                }
            }

            return base.SaveChangesAsync(cancellationToken);
        }
    }
}
