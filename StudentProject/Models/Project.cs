using System;
using System.Collections.Generic;


namespace StudentProject.Models
{
    public class Project
    {
        public Guid ProjectId { get; set; }
        public string ProjectName { get; set; }
        public ICollection<Group> Groups { get; set; }
    }
}
