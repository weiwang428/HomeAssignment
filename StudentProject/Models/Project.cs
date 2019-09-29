using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


namespace StudentProject.Models
{
    /// <summary>
    /// Project Model, represent a Project.
    /// </summary>
    public class Project
    {
        public Project()
        {
            this.Groups = new HashSet<Group>();
        }
        [Required]
        public Guid ProjectId { get; set; }
        [Required]
        public string ProjectName { get; set; }
        // Each project may contain a number of groups.
        public ICollection<Group> Groups { get; }
    }
}
