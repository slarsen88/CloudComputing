using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MyOrganizationApi.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace MyOrganizationApi.Controllers
{
    [Route("api/StudentController")]
    public class StudentController : Controller
    {
        private StudentDB studentDB = new StudentDB();


        // GET: api/<controller>
        [HttpGet]
        public IEnumerable<Student> Get()
        {
            studentDB.ParseXML();
            return studentDB.GetStudents();
        }

        // GET api/<controller>/5
        [HttpGet("{id}")]
        public Student Get(string id)
        {
            studentDB.ParseXML();
            List<Student> students = studentDB.GetStudents();
            foreach (var student in students)
            {
                if (student.Id.Equals(id))
                {
                    return student;
                }
            }

            return null;
        }


    }
}
