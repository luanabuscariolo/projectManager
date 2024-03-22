using ListaDeProjetos.DBModels;
using Microsoft.EntityFrameworkCore;
using Task = ListaDeProjetos.DBModels.Task;

namespace ListaDeProjetos.Database
{
    public class ListaDeProjetosContext : DbContext
    {
        public DbSet<Project> Projects { get; set; }
        public DbSet<Task> Tasks { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<UserProject> UserProjects { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=localhost;Database=ListaDeProjetos;User Id=sa;Password=SuperPassword@23;TrustServerCertificate=True");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var userProjectMapping = modelBuilder.Entity<UserProject>();
            userProjectMapping.HasKey(x => new { x.UserId, x.ProjectId });
        }
    }
}
