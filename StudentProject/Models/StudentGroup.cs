using System;



namespace StudentProject.Models
{
    /// <summary>
    /// Student-group relationship model, represent a many to many relationship.
    /// </summary>
    public class StudentGroup
    {
        public Guid StudentId { get; set; }

        public Student Student { get; set; }

        public Guid GroupID { get; set; }

        public Group Group { get; set; }

    }
}
