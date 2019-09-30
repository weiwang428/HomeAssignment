using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StudentProject.Models;

namespace StudentProject.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    public class ProjectController : ControllerBase
    {
        private readonly IStudentProjectRepository _stuProjRepo;

        // Dependence Injection of the constructor.
        public ProjectController(IStudentProjectRepository _stuProjRepo)
        {
            this._stuProjRepo = _stuProjRepo;
        }

        [HttpGet]
        public ContentResult Init()
        {
            bool done = _stuProjRepo.InitProject();
            if (done)
                return Content("Done");
            else
                return Content("Problem with initialization");
        }

        // GET /Project/List
        [HttpGet]
        [Produces("application/json")]
        public ActionResult List()
        {
            return Ok(_stuProjRepo.ListProjects());
        }

        // POST /project/AddStudentToGroup?groupId={Group_Id}&studentId={Student_Id}
        [HttpPost]
        public ContentResult AddStudentToGroup(string groupId, string studentId)
        {
            try
            {
                if (_stuProjRepo.AddStudentToGroup(new Guid(groupId), new Guid(studentId)))
                    return Content("True");
                else
                    return Content("False");
            }
            catch (Exception e)
            {
                return Content(e.Message);
            }
        }

        // POST /project/Creategroup?projectid={Project_Id}&groupname={Group_Name}
        [HttpPost]
        public ContentResult Creategroup(string projectid, string groupname)
        {
            try
            {
                return Content(_stuProjRepo.CreateGroup(new Guid(projectid), groupname));
            }
            catch (Exception e)
            {
                return Content(e.Message);
            }
        }
    }
}