using System;
using Newtonsoft.Json;


namespace StudentProject.Models
{
    /// <summary>
    /// Student-group relationship model, represent a many to many relationship.
    /// </summary>
    public class StudentGroup
    {
        [JsonIgnore]
        public Guid StudentId { get; set; }
        public Student Student { get; set; }
        [JsonIgnore]
        public Guid GroupID { get; set; }
        [JsonIgnore]
        public Group Group { get; set; }

    }
}
