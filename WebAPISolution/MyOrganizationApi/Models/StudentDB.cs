using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;

namespace MyOrganizationApi.Models
{
    public class StudentDB
    {
        List<Student> listOfStudents = new List<Student>();

        public void ParseXML()
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load("data.xml");
            XmlElement rootElement = xmlDoc.DocumentElement;
            XmlNodeList rootChildren = rootElement.ChildNodes;
            foreach (XmlNode child in rootChildren)
            {
                if (child.Name.Equals("students"))
                {
                    int i = 0;
                    foreach (XmlNode grandChild in child)
                    {
                        Student student = new Student();
                        student.Id = grandChild.Attributes["id"].Value;
                        student.Name = grandChild.InnerText;
                        listOfStudents.Add(student);
                    }
                }
            }
        }

        public List<Student> GetStudents()
        {
            return listOfStudents;
        }
    }
}
