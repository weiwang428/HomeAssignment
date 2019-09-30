using System;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


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
        public Guid StudentId { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [JsonIgnore]
        public ICollection<StudentGroup> StudentGroups { get; }
    }
}
