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
    public class StudentController : ControllerBase
    {
        // Reference to the Database Context object.
        readonly IStudentProjectRepository _stuProjRepo;

        // Dependence Injection of the constructor.
        public StudentController(IStudentProjectRepository _stuProjRepo)
        {
            this._stuProjRepo = _stuProjRepo;
        }

        // GET /Student/Init
        [HttpGet("Init")]
        public ContentResult Init()
        {
            bool done = _stuProjRepo.InitStudent();
            if (done)
                return Content("Done");
            else
                return Content("Problem with initialization");
        }

        [HttpGet("List")]
        public ActionResult List()
        {
            return Ok(_stuProjRepo.ListStudents());
        }

        [HttpGet("GetProjects")]
        public ActionResult GetProjects(string Id)
        {
            return Ok(_stuProjRepo.GetProjects(new Guid(Id)));
        }




    }
}