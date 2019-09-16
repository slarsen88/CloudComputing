using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using System;

namespace MyOrganizationApi.Models
{
    public class CourseDB
    {
        List<Course> listOfCourses = new List<Course>();
        public void ParseXML()
        {
            
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load("data.xml");
            XmlElement rootElement = xmlDoc.DocumentElement;
            XmlNodeList rootChildren = rootElement.ChildNodes;
            foreach (XmlNode child in rootChildren)
            {
                // PARSES COURSES
                if (child.Name.Equals("courses"))
                {
                    foreach (XmlNode grandChild in child)
                    {
                        Course course = new Course();
                        course.Id = grandChild.Attributes["id"].Value; // ID
                        course.Name = grandChild.ChildNodes[0].InnerText; // Name
                        course.Description = grandChild.ChildNodes[1].InnerText; // Description
                        listOfCourses.Add(course);
                       
                    }
                }
            }
        }

        public List<Course> GetCourses()
        {
            return listOfCourses;
        }
    }
}
