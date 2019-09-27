using Microsoft.EntityFrameworkCore;
using DbContext = Microsoft.EntityFrameworkCore.DbContext;
using StudentProject.Models;

namespace StudentProject.DBContext
{
    public class StudentProjectDbContext : DbContext
    {
        public StudentProjectDbContext() {}
        public StudentProjectDbContext(DbContextOptions<StudentProjectDbContext> options) : base(options) {}
        public DbSet<Student> Students { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<Project> Projects { get; set; }
    }
}
