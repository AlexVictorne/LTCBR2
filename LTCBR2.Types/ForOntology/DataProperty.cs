using System.Collections.Generic;

namespace LTCBR2.Types.ForOntology
{
    public class DataProperty
    {
        public string Name { get; set; }
        public List<string> PossibleValues = new List<string>();
    }
}