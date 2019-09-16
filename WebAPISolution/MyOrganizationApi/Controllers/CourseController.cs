using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using Microsoft.AspNetCore.Mvc;
using MyOrganizationApi.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace MyOrganizationApi.Controllers
{
    [Route("api/CourseController")]
    public class CourseController : Controller
    {
        private CourseDB courseDB = new CourseDB();

        // GET: api/<controller>
        [HttpGet]
        public IEnumerable<Course> Get()
        {
            courseDB.ParseXML();
            return courseDB.GetCourses();
        }

        // GET api/<controller>/5
        [HttpGet("{id}")]
        public Course Get(string id)
        {
            courseDB.ParseXML();
            List<Course> courses = courseDB.GetCourses();            
            foreach (Course course in courses)
            {
                if (course.Id.Equals(id))
                {
                    return course;
                }
            }

            return null;
        }
    }
}
