using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StudentProject.Models;

namespace StudentProject.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    public class StudentController : ControllerBase
    {
        // Reference to the Database Context object.
        private readonly IStudentProjectRepository _stuProjRepo;

        // Dependence Injection of the constructor.
        public StudentController(IStudentProjectRepository _stuProjRepo)
        {
            this._stuProjRepo = _stuProjRepo;
        }

        // GET /Student/Init
        [HttpGet]
        public ContentResult Init()
        {
            bool done = _stuProjRepo.InitStudent();
            if (done)
                return Content("Done");
            else
                return Content("Problem with initialization");
        }

        // GET /Student/List
        [HttpGet]
        [Produces("application/json")]
        public ActionResult List()
        {
            return Ok(_stuProjRepo.ListStudents());
        }

        // GET /Student/GetProjects?Id={Student_Id}
        [HttpGet]
        [Produces("application/json")]
        public ActionResult GetProjects(string Id)
        {
            try
            {
                return Ok(_stuProjRepo.GetProjects(new Guid(Id)));
            }
            catch (Exception e)
            {
                return Content(e.Message);
            }
        }
    }
}