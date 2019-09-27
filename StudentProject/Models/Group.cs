using System;
using System.Collections.Generic;

namespace StudentProject.Models
{
    public class Group
    {
        public Guid GroupId { get; set; }
        public string GroupName { get; set; }

        // Each group may have multiple students
        public ICollection<Student> Students { get; set; }
    }
}
