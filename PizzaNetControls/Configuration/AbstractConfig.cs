using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace PizzaNetControls.Configuration
{
    public abstract class AbstractConfig
    {
        public void Save(string filename, Type type)
        {
            using (StreamWriter sw = new StreamWriter(filename))
            {
                XmlSerializer xmls = new XmlSerializer(type);
                xmls.Serialize(sw, this);
            }
        }

        public static object Read(string filename, Type type)
        {
            using (StreamReader sw = new StreamReader(filename))
            {
                XmlSerializer xmls = new XmlSerializer(type);
                return xmls.Deserialize(sw);
            }
        }
    }
}
