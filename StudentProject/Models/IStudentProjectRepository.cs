using System;
using System.Collections.Generic;


namespace StudentProject.Models
{
    public interface IStudentProjectRepository
    {
        #region Student API

        bool InitStudent();
        IEnumerable<Student> ListStudents();
        IEnumerable<Object> GetProjects(Guid studentId);

        #endregion

        #region Project and Group API

        //bool InitProject();
        //IEnumerable<Project> ListProjects();
        //bool AddStudentToGroup(Guid groupId, Guid studentId);
        //string CreateGroup(Guid projectId, string groupName);

        #endregion
    }
}
