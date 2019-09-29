using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;


namespace StudentProject.Models
{
    /// <summary>
    /// Student model, represent a student.
    /// </summary>
    public class Student
    {
        public Student()
        {
            this.StudentGroups = new HashSet<StudentGroup>();
        }
        [Required]
        public Guid StudentId{ get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        public ICollection<StudentGroup> StudentGroups { get; }
    }
}
