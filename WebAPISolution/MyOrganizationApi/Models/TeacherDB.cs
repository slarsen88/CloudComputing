using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;

namespace MyOrganizationApi.Models
{
    public class TeacherDB
    {
        List<Teacher> listOfTeachers = new List<Teacher>();

        public void ParseXML()
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load("data.xml");
            XmlElement rootElement = xmlDoc.DocumentElement;
            XmlNodeList rootChildren = rootElement.ChildNodes;
            foreach (XmlNode child in rootChildren)
            {

                if (child.Name.Equals("teachers"))
                {
                    foreach (XmlNode grandChild in child)
                    {
                        Teacher teacher = new Teacher();
                        teacher.Id = grandChild.Attributes["id"].Value;
                        teacher.Name = grandChild.ChildNodes[0].InnerText;
                        teacher.Office = grandChild.ChildNodes[1].InnerText;
                        listOfTeachers.Add(teacher);
                    }
                }
            }
        }

        public List<Teacher> GetTeachers()
        {
            return listOfTeachers;
        }
    }
}
