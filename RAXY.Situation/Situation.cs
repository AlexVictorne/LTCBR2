using System;
using System.Collections.Generic;
using System.Linq;

namespace RAXY.Situation
{
    [Serializable]
    public class Situation
    {
        private string _name;
        private Subject[] _subjects;
        private Relation[] _relations;
        private Process[] _processes;
        private Condition[] _conditions;
        public Situation(string name)
        {
            this._name = name.ToLower();
        }
        private Situation() { }
        public string Name
        {
            get { return _name; }
            set { _name = value.ToLower(); }
        }
        public Subject[] Subjects
        {
            get { return _subjects; }
            set { _subjects = value; }
        }
        public Relation[] Relations
        {
            get { return _relations; }
            set { _relations = value; }
        } 
        public Process[] Processes
        {
            get { return _processes; }
            set { _processes = value; }
        }

        public Condition[] Conditions
        {
            get { return _conditions; }
            set { _conditions = value; }
        }
        public static bool CheckAnyType(string value)
        {
            value = value.ToLower();
            return value == "любой" || value == "любая" || value == "любое" || value == "any";
        }

        public bool ContainsRelations(IEnumerable<Relation> relationsSet)
        {
            var relations = relationsSet.ToList();
            return false;
        }
    }
}