using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ProjectPlaner.Models.Entity;

namespace ProjectPlaner.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IServiceProvider serviceProvider)
            : base(options)
        {
            //Database.EnsureDeleted();
            //Database.EnsureCreated();
        }

        public DbSet<Models.Entity.Task> tasks { get; set; } = null!;
        public DbSet<Project> projects { get; set; } = null!;
        public DbSet<Client> clients { get; set; } = null!;        

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Models.Entity.Task>()
                .Property(t => t.status)
                .HasConversion<string>();
            modelBuilder.Entity<Models.Entity.Task>()
                .Property(t => t.marker)
                .HasConversion<string>();

            modelBuilder.Entity<Models.Entity.Task>()
                .Property(e => e.taskId)
                .ValueGeneratedOnAdd();
            modelBuilder.Entity<Project>()
                .Property(e => e.projectId)
                .ValueGeneratedOnAdd();
            modelBuilder.Entity<Client>()
                .Property(e => e.clientId)
                .ValueGeneratedOnAdd();

            // Configure the relationship between Task and User
            modelBuilder.Entity<ProjectPlaner.Models.Entity.Task>()
                .HasOne(t => t.user)
                .WithMany()
                .HasForeignKey(t => t.userId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            // Configure the relationship between Project and User
            modelBuilder.Entity<ProjectPlaner.Models.Entity.Project>()
                .HasOne(p => p.user)
                .WithMany() 
                .HasForeignKey(p => p.userId)
                .OnDelete(DeleteBehavior.ClientSetNull); 

            // Configure the relationship between Client and User
            modelBuilder.Entity<ProjectPlaner.Models.Entity.Client>()
                .HasOne(c => c.user)
                .WithMany()
                .HasForeignKey(c => c.userId)
                .OnDelete(DeleteBehavior.ClientSetNull); 

            // Configure cascade delete for Project -> Task (when a Project is deleted, its Tasks are deleted)
            modelBuilder.Entity<ProjectPlaner.Models.Entity.Project>()
                .HasMany(p => p.tasks)
                .WithOne(t => t.project)
                .HasForeignKey(t => t.projectId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure cascade delete for Client -> Project (when a Client is deleted, its Projects are deleted)
            modelBuilder.Entity<ProjectPlaner.Models.Entity.Client>()
                .HasMany(c => c.projects)
                .WithOne(p => p.client)
                .HasForeignKey(p => p.clientId)
                .OnDelete(DeleteBehavior.Cascade);            

            modelBuilder.Entity<Project>()
                .HasOne(p => p.user)
                .WithMany()
                .HasForeignKey(p => p.userId)
                .IsRequired(false);

            modelBuilder.Entity<Client>()
               .HasOne(p => p.user)
               .WithMany() 
               .HasForeignKey(p => p.userId)
               .IsRequired(false);

            var admin = new IdentityRole
            {
                Id = "1",
                Name = "Admin",
                NormalizedName = "ADMIN"
            };

            var user = new IdentityRole
            {
                Id = "2",
                Name = "User",
                NormalizedName = "USER"
            };

            modelBuilder.Entity<IdentityRole>().HasData(admin, user);


        }


        
    }
}
