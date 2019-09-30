using System;
using System.Linq;
using StudentProject.DBContext;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace StudentProject.Models
{
    public class StudentProjectRepository : IStudentProjectRepository
    {
        private readonly StudentProjectDbContext _dbContext;
        private readonly IConfiguration _configuration;

        // Dependece injection of the DatabaseContext.
        public StudentProjectRepository(StudentProjectDbContext _dbContext, IConfiguration _configuration)
        {
            this._dbContext = _dbContext;
            this._configuration = _configuration;
        }

        /// <summary>
        /// Initialize the Student Data from the dataset in json file, clear the old data first.
        /// </summary>
        /// <returns>True if sucess, otherwise false.</returns>
        public bool InitStudent()
        {
            try
            {
                if (_dbContext.Students.Count() > 0) _dbContext.Students.RemoveRange(_dbContext.Students);
                // Read the initial students from the configuration file.
                var init_student_lst = _configuration.GetSection("InitData:Students").Get<List<Student>>();
                // Create the new student data.
                _dbContext.Students.AddRange(init_student_lst);
                _dbContext.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
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
            var query_result = _dbContext.Projects
                                                  .Where(p =>
                                                            p.Groups.Any(g =>
                                                                            g.StudentGroups.Any(sg => sg.StudentId == studentId)
                                                                        )
                                                        )
                                                  .Include(p => p.Groups)
                                                        .ThenInclude(g => g.StudentGroups)
                                                            .ThenInclude(sg => sg.Student).ToList();
            // Since we only need the group which has the student, so we filter the Groups in each project
            // and remap it to a anonymous class which only the group contains the student information is included.
            var format_result = query_result.Select(p => new
            {
                ProjectId = p.ProjectId,
                ProjectName = p.ProjectName,
                Groups = p.Groups.Where(
                            g => g.StudentGroups.Any(sg => sg.StudentId == studentId)
                         )
            }).ToList();
            return format_result;
        }

        /// <summary>
        /// Initialize the project data from the json file, clear the old groups and projects first.
        /// </summary>
        /// <returns>true if success, otherwise false.</returns>
        public bool InitProject()
        {
            try
            {
                if (_dbContext.Groups.Count() > 0) _dbContext.Groups.RemoveRange(_dbContext.Groups);
                if (_dbContext.Projects.Count() > 0) _dbContext.Projects.RemoveRange(_dbContext.Projects);

                // Read the initial projects from the configuration file.
                var init_project_lst = _configuration.GetSection("InitData:Projects").Get<List<Project>>();
                // Add to initial data to the database.
                _dbContext.Projects.AddRange(init_project_lst);
                _dbContext.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Return the list of the projects which also include the groups in each project, and students in each group.
        /// </summary>
        /// <returns>A collection of projects</returns>
        public IEnumerable<Project> ListProjects()
        {
            return _dbContext.Projects
                                    .Include(p => p.Groups)
                                         .ThenInclude(g => g.StudentGroups)
                                             .ThenInclude(sg => sg.Student)
                                    .ToList();
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
            Student stu = _dbContext.Students
                                            .Include(s => s.StudentGroups)
                                            .Where(s => s.StudentId == studentId)
                                            .FirstOrDefault();
            Group gro = _dbContext.Groups
                                        .Where(g => g.GroupId == groupId)
                                        .FirstOrDefault();
            if (stu == null || gro == null)
            {
                return false;
            }
            // Check if they are already paired.
            var result = stu.StudentGroups
                                        .Any(sg => sg.GroupID == groupId);
            if (result)
                return false;

            // Find if the group is included in a project.
            var proj = _dbContext.Projects
                                        .Include(pp => pp.Groups)
                                            .ThenInclude(gg => gg.StudentGroups)
                                        .FirstOrDefault(p => p.Groups.Contains(gro));

            // If this group is included in a project, then we need to check the other groups in 
            // this project, none of them should have this student.
            if (proj != null)
            {
                // Now find if other group include the student.
                result = proj.Groups
                                .Any(g => 
                                        g.StudentGroups.Any(sg => sg.StudentId == studentId)
                                    );
                if (result)
                    return false;
            }

            // Now we can safely add the student to this group.
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
            Group gro = pro.Groups
                                .Where(g => g.GroupName == groupName)
                                .FirstOrDefault();
            if (gro != null)
                return "Creation Fail, duplicate project name.";
            // Create a new GUID for group.
            Group newGroup = new Group() { GroupId = new Guid(), GroupName = groupName };
            pro.Groups.Add(newGroup);
            _dbContext.SaveChanges();
            // Return the newly created group's GUID.
            return newGroup.GroupId.ToString();
        }

        /// <summary>
        /// Clear the Database Data, for test.
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

        /// <summary>
        /// Initial the Database Data, for test.
        /// </summary>
        private void InitializeDatabaseData()
        {
            var init_student_lst = _configuration.GetSection("InitData:Students").Get<List<Student>>();
            var init_project_lst = _configuration.GetSection("InitData:Projects").Get<List<Project>>();
            var init_group_lst = _configuration.GetSection("InitData:Groups").Get<List<Group>>();

            _dbContext.AddRange(new StudentGroup { Student = init_student_lst[0], Group = init_group_lst[0] });
            _dbContext.AddRange(new StudentGroup { Student = init_student_lst[1], Group = init_group_lst[1] });
            _dbContext.AddRange(new StudentGroup { Student = init_student_lst[2], Group = init_group_lst[2] });
            _dbContext.AddRange(new StudentGroup { Student = init_student_lst[0], Group = init_group_lst[3] });
            _dbContext.AddRange(new StudentGroup { Student = init_student_lst[1], Group = init_group_lst[4] });
            _dbContext.AddRange(new StudentGroup { Student = init_student_lst[3], Group = init_group_lst[5] });

            //// Add the Groups to each Project.
            init_project_lst[0].Groups.Add(init_group_lst[0]);
            init_project_lst[0].Groups.Add(init_group_lst[1]);
            init_project_lst[0].Groups.Add(init_group_lst[2]);
            init_project_lst[1].Groups.Add(init_group_lst[3]);
            init_project_lst[1].Groups.Add(init_group_lst[4]);
            init_project_lst[1].Groups.Add(init_group_lst[5]);

            // Add everything to the database, and then save the changes.
            _dbContext.Projects.AddRange(init_project_lst);
            _dbContext.SaveChangesAsync();
        }

        /// <summary>
        /// Load prepared data for the test purpose. With 2 projects and 6 groups, each project has 3 groups, and each
        /// group has 1 student.
        /// </summary>
        /// <returns>True if sucessful, otherwise return false.</returns>
        public bool LoadPreparedData()
        {
            try
            {
                ClearExistingDatabaseData();
                InitializeDatabaseData();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
