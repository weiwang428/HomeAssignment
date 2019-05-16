using Microsoft.EntityFrameworkCore;
using DbContext = Microsoft.EntityFrameworkCore.DbContext;

namespace StudentProject.DBContext
{
    public class StudentProjectDbContext : DbContext
    {
        public StudentProjectDbContext() {}
        public StudentProjectDbContext(DbContextOptions<StudentProjectDbContext> options) : base(options) {}
    }
}
