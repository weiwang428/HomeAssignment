using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace StudentProject.Models
{
    /// <summary>
    /// Group Model, represents a Group Object.
    /// </summary>
    public class Group
    {
        public Group()
        {
            this.StudentGroups = new HashSet<StudentGroup>();
        }
        [Required]
        public Guid GroupId { get; set; }
        [Required]
        public string GroupName { get; set; }

        // Each group may have multiple students
        public ICollection<StudentGroup> StudentGroups { get;}
    }
}
