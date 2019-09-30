using System;
using System.Collections.Generic;
using StudentProject.DBContext;
using System.Linq;
using Microsoft.EntityFrameworkCore;


namespace StudentProject.Models
{
    public class StudentProjectRepository : IStudentProjectRepository
    {
        private StudentProjectDbContext _dbContext;
        // Dependece injection of the DatabaseContext.
        public StudentProjectRepository(StudentProjectDbContext _dbContext)
        {
            this._dbContext = _dbContext;
        }

        /// <summary>
        /// Initialize the Student Data, give 4 students as default data, clear the old data first.
        /// </summary>
        /// <returns>True if sucess, otherwise false.</returns>
        public bool InitStudent()
        {
            //if (ClearExistingDatabaseData())
            //{
            //    // Create the new student data.
            //    Student s1 = new Student() { StudentId = Guid.NewGuid(), FirstName = "Demo", LastName = "Example" };
            //    Student s2 = new Student() { StudentId = Guid.NewGuid(), FirstName = "Anders", LastName = "Andersson" };
            //    Student s3 = new Student() { StudentId = Guid.NewGuid(), FirstName = "Steven", LastName = "Job" };
            //    Student s4 = new Student() { StudentId = Guid.NewGuid(), FirstName = "Lars", LastName = "Nillson" };
            //    _dbContext.Students.Add(s1);
            //    _dbContext.Students.Add(s2);
            //    _dbContext.Students.Add(s3);
            //    _dbContext.Students.Add(s4);
            //    _dbContext.SaveChanges();
            //    return true;
            //}
            //else
            //    return false;
            InitializeDatabaseData();
            return true;
        }

        /// <summary>
        /// Return a collection of student objects.
        /// </summary>
        /// <returns>A collection of student objects</returns>
        public IEnumerable<Student> ListStudents()
        {
            return _dbContext.Students.ToList();
        }

        /// <summary>
        /// Giving a studentId, search all the projects, and return the project list which include the student
        /// and the group information, the students in the group is also included.
        /// </summary>
        /// <param name="studentId">Id of the student.</param>
        /// <returns>A project collection which each project has a group include the student</returns>
        public IEnumerable<Object> GetProjects(Guid studentId)
        {
            // Filter the projects by studentId, search the groups in each project, the selected project should 
            // satisfy the matching which means one of the Group in the selected project Groups should contain the 
            // Student.
            var query_result = _dbContext.Projects.Where(p => p.Groups.Any(g => g.StudentGroups.Any(sg => sg.StudentId == studentId)))
                .Include(p => p.Groups).ThenInclude(g => g.StudentGroups).ThenInclude(sg => sg.Student).ToList();
            // Since we only need the group which has the student, so we filter the Groups in each project
            // and remap it to a anonymous class which only the group contains the student information is included.
            var format_result = query_result.Select(p => new
            {
                ProjectId = p.ProjectId,
                ProjectName = p.ProjectName,
                Groups = p.Groups.SingleOrDefault(
                            g => g.StudentGroups.Any(sg => sg.StudentId == studentId)
                         )
            }).ToList();
            return format_result;
        }

        /// <summary>
        /// Initialize the project data, give 2 projects as the default projects.
        /// </summary>
        /// <returns>true if success, otherwise false.</returns>
        public bool InitProject()
        {
            if (ClearExistingDatabaseData())
            {
                // Create the Project Data.
                Project p1 = new Project() { ProjectId = Guid.NewGuid(), ProjectName = "English" };
                Project p2 = new Project() { ProjectId = Guid.NewGuid(), ProjectName = "Math" };
                // Add to the database.
                _dbContext.Projects.Add(p1);
                _dbContext.Projects.Add(p2);
                _dbContext.SaveChanges();
                return true;
            }
            else
                return false;
        }

        /// <summary>
        /// Return the list of the projects which also include the groups in each project, and students in each group.
        /// </summary>
        /// <returns>A collection of projects</returns>
        public IEnumerable<Project> ListProjects()
        {
            //_dbContext.Projects.ForEachAsync(p => p.Groups.Add())
            return _dbContext.Projects.
                Include(p => p.Groups).
                ThenInclude(g => g.StudentGroups).
                ThenInclude(sg => sg.Student).
                ToList();
        }

        /// <summary>
        /// Add the student to the given group.
        /// </summary>
        /// <param name="groupId">Id of the group.</param>
        /// <param name="studentId">Id of the student.</param>
        /// <returns>True if the add sucessful, otherwise false.</returns>
        public bool AddStudentToGroup(Guid groupId, Guid studentId)
        {
            // Make Sure the group and student is valid.
            Student stu = _dbContext.Students.Include(ss => ss.StudentGroups).Where(ss => ss.StudentId == studentId).FirstOrDefault();
            Group gro = _dbContext.Groups.Where(gg => gg.GroupId == groupId).FirstOrDefault();
            if (stu == null || gro == null)
            {
                return false;
            }
            // Check if they are already paired.
            var result = stu.StudentGroups.Where(sg => sg.GroupID == groupId).FirstOrDefault();
            if(result != null)
                return false;

            // Find if the group is included in a project.
            var proj = _dbContext.Projects.Include(pp => pp.Groups).ThenInclude(gg => gg.StudentGroups).FirstOrDefault(p => p.Groups.Contains(gro));

            // This group is not included in any project, then student to it.
            if (proj != null)
            {
                // Now find all the Groups
                var find_grp = proj.Groups.FirstOrDefault(gg => gg.StudentGroups.Any(sg => sg.StudentId == stu.StudentId));
                if (find_grp != null)
                    return false;
            }
            _dbContext.AddRange(new StudentGroup { Student = stu, Group = gro });
            _dbContext.SaveChanges();
            return true;
        }

        /// <summary>
        /// Create a new group by a given group name, and add it to the project which is given by projectId.
        /// </summary>
        /// <param name="projectId">Id of the project.</param>
        /// <param name="groupName">Name of the new create group</param>
        /// <returns>The new created GUID of the group, if the creation fails, it will return "Creation Fail"</returns>
        public string CreateGroup(Guid projectId, string groupName)
        {
            Project pro = _dbContext.Projects
                                    .Include(pp => pp.Groups)
                                    .Where(p => p.ProjectId == projectId)
                                    .FirstOrDefault();
            // Make sure the group exist first.
            if (pro == null)
                return "Creation Fail, no such project.";
            // Make sure the group name is not used in the same project.
            Group gro = pro.Groups.Where(g => g.GroupName == groupName).FirstOrDefault();
            if (gro != null)
                return "Creation Fail, duplicate project name.";
            // Create a new GUID for group.
            Group newGroup = new Group() { GroupId = new Guid(), GroupName = groupName };
            _dbContext.Groups.Add(newGroup);
            _dbContext.SaveChanges();
            return newGroup.GroupId.ToString();
        }

        /// <summary>
        /// Clear the Database Data.
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

        public void InitializeDatabaseData()
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
