using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.AspNetCore.Mvc;
using MyOrganizationApi.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace MyOrganizationApi.Controllers
{
    [Route("api/TeacherController")]
    public class TeacherController : Controller
    {

        private TeacherDB teacherDB = new TeacherDB();
        

        // GET: api/<controller>
        [HttpGet]
        public IEnumerable<Teacher> Get()
        {
            teacherDB.ParseXML();
            return teacherDB.GetTeachers();
        }

        // GET api/<controller>/5
        [HttpGet("{id}")]
        public Teacher Get(string id)
        {
            teacherDB.ParseXML();
            List<Teacher> teachers = teacherDB.GetTeachers();
            foreach (Teacher teacher in teachers)
            {
                if (teacher.Id.Equals(id))
                {
                    return teacher;
                }
            }
            return null;
        }

    }
}
