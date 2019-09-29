using System;
using System.Collections.Generic;
using StudentProject.DBContext;
using System.Linq;

namespace StudentProject.Models
{
    public class StudentProjectRepository : IStudentProjectRepository
    {
        private StudentProjectDbContext _dbContext;
        public StudentProjectRepository(StudentProjectDbContext _dbContext)
        {
            this._dbContext = _dbContext;
        }

        public bool InitStudent()
        {
            if (ClearExistingDatabaseData())
            {
                InitializeDatabaseData();
                return true;
            }
            else
                return false;            
        }

        public IEnumerable<Student> ListStudents()
        {
            return _dbContext.Students.ToList();
        }

        public IEnumerable<Object> GetProjects(Guid studentId)
        {
            throw new NotFiniteNumberException(); 
        }


        /// <summary>
        /// 
        /// </summary>
        private bool ClearExistingDatabaseData()
        {
            if (_dbContext.Database.EnsureDeleted() && _dbContext.Database.EnsureCreated())
            {
                _dbContext.SaveChanges();
                return true;
            }
            else
                return false;
        }

        private void InitializeDatabaseData()
        {
            // Create the new student data.
            Student s1 = new Student() { StudentId = Guid.NewGuid(), FirstName = "Demo", LastName = "Example" };
            Student s2 = new Student() { StudentId = Guid.NewGuid(), FirstName = "Anders", LastName = "Andersson" };
            Student s3 = new Student() { StudentId = Guid.NewGuid(), FirstName = "Steven", LastName = "Job" };
            Student s4 = new Student() { StudentId = Guid.NewGuid(), FirstName = "Lars", LastName = "Nillson" };

            // Create the new group data (for English Project);
            Group g1 = new Group() { GroupId = Guid.NewGuid(), GroupName = "EnglishIsBetter" };
            Group g2 = new Group() { GroupId = Guid.NewGuid(), GroupName = "EnglishIsAcceptable" };
            Group g3 = new Group() { GroupId = Guid.NewGuid(), GroupName = "EnglishIsCrazy" };

            // Create the new group data (for Math Project);
            Group g4 = new Group() { GroupId = Guid.NewGuid(), GroupName = "MathIsBetter" };
            Group g5 = new Group() { GroupId = Guid.NewGuid(), GroupName = "MathIsAcceptable" };
            Group g6 = new Group() { GroupId = Guid.NewGuid(), GroupName = "MathIsCrazy" };

            _dbContext.AddRange(new StudentGroup { Student = s1, Group = g1 });
            _dbContext.AddRange(new StudentGroup { Student = s2, Group = g2 });
            _dbContext.AddRange(new StudentGroup { Student = s3, Group = g3 });
            _dbContext.AddRange(new StudentGroup { Student = s1, Group = g4 });
            _dbContext.AddRange(new StudentGroup { Student = s3, Group = g5 });
            _dbContext.AddRange(new StudentGroup { Student = s4, Group = g6 });

            // Create the Project Data.
            Project p1 = new Project() { ProjectId = Guid.NewGuid(), ProjectName = "English" };
            Project p2 = new Project() { ProjectId = Guid.NewGuid(), ProjectName = "Math" };

            // Add the Groups to each Project.
            p1.Groups.Add(g1);
            p1.Groups.Add(g2);
            p1.Groups.Add(g3);

            p2.Groups.Add(g4);
            p2.Groups.Add(g5);
            p2.Groups.Add(g6);

            // Add to the database.
            _dbContext.Projects.Add(p1);
            _dbContext.Projects.Add(p2);

            _dbContext.SaveChangesAsync();
        }


    }
}
