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
        public DbSet<Mark> marks { get; set; } = null!;


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
            modelBuilder.Entity<Mark>()
                .Property(e => e.markId)
                .ValueGeneratedOnAdd();


            modelBuilder.Entity<Project>()
                .HasMany(p => p.tasks)             // у проекта много задач
                .WithOne(t => t.project)          // У задачи 1 проект
                .HasForeignKey(p => p.taskId)        // Внешний ключ
                .OnDelete(DeleteBehavior.SetNull); // При удалении задачи в проекте связь с этой задачей станет null

            modelBuilder.Entity<Models.Entity.Task>()
                .HasOne(t => t.project)          // У задачи есть один проект
                .WithMany(p => p.tasks)          // У проекта много задач
                .HasForeignKey(t => t.projectId) // Внешний ключ
                .OnDelete(DeleteBehavior.Cascade); // При удалении проекта удаляются все его задачи

            modelBuilder.Entity<Client>()
                .HasMany(c => c.projects)
                .WithOne(p => p.client)
                .HasForeignKey(p => p.projectId)
                .OnDelete(DeleteBehavior.SetNull); // При удалении проекта соответствующее поле у клиента становится null

            modelBuilder.Entity<Project>()
                .HasOne(p => p.client)
                .WithMany(c => c.projects)
                .HasForeignKey(p => p.clientId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Models.Entity.Task>()
                .HasMany(t => t.marks)
                .WithMany(m => m.tasks)
                .UsingEntity<Dictionary<string, object>>("taskMark",
                    j => j
                    .HasOne<Mark>()
                    .WithMany()
                    .HasForeignKey("markId")
                    .OnDelete(DeleteBehavior.Cascade), // Поведение при удалении Mark
                    j => j
                    .HasOne<Models.Entity.Task>()
                    .WithMany()
                    .HasForeignKey("taskId")
                    .OnDelete(DeleteBehavior.Cascade), // Поведение при удалении Task
                    j => j.ToTable("taskMark"));


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
