using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StudentProject.Models;


namespace StudentProject.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ProjectController : ControllerBase
    {
        readonly IStudentProjectRepository _stuProjRepo;

        // Dependence Injection of the constructor.
        public ProjectController(IStudentProjectRepository _stuProjRepo)
        {
            this._stuProjRepo = _stuProjRepo;
        }

        [HttpGet("Init")]
        public ContentResult Init()
        {
            bool done = _stuProjRepo.InitProject();
            if (done)
                return Content("Done");
            else
                return Content("Problem with initialization");
        }

        // GET /Project/List
        [HttpGet("List")]
        [Produces("application/json")]
        public ActionResult List()
        {
            return Ok(_stuProjRepo.ListProjects());
        }

        // POST /project/AddStudentToGroup?groupId={Group_Id}&studentId={Student_Id}
        [HttpPost("AddStudentToGroup")]
        public ContentResult AddStudentToGroup(string groupId, string studentId)
        {
            if (_stuProjRepo.AddStudentToGroup(new Guid(groupId), new Guid(studentId)))
                return Content("True");
            else
                return Content("False");

        }

        // POST /project/Creategroup?projectid={Project_Id}&groupname={Group_Name}
        [HttpPost("Creategroup")]
        public ContentResult Creategroup(string projectid, string groupname)
        {
            return Content(_stuProjRepo.CreateGroup(new Guid(projectid), groupname));
        }


    }
}