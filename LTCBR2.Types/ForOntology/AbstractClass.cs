using System.Collections.Generic;

namespace LTCBR2.Types.ForOntology
{
    public class AbstractClass
    {
        public string Name { get; set; }
        public string Parent { get; set; }
        public string Purpose { get; set; }
        public List<DataProperty> Attributes = new List<DataProperty>();
        public List<ObjectProperty> Connections = new List<ObjectProperty>();
    }
}