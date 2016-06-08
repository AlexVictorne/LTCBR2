using System;
using System.Collections.Generic;
using System.Linq;

namespace RAXY.Situation
{
    [Serializable]
    public class Subject
    {
        private List<Property> _properties;
        private string _subjectClass;
        private string _name;
        private string _type;

        public static Subject EmptySubject()
        {
            return new Subject();
        }
        private Subject() { }
        public Subject(string subjectClass, string name, string type)
        {
            this._subjectClass = subjectClass.ToLower();
            this._name = name.ToLower();
            this._type = type.ToLower();
            _properties = new List<Property>();
        }

        public string Name
        {
            get { return _name; }
            set { _name = value.ToLower(); }
        }

        public string SubjectClass
        {
            get { return _subjectClass; }
            set { _subjectClass = RAXY.Situation.Situation.CheckAnyType(value.ToLower()) ? Property.FieldTypes.Any.ToString().ToLower() : value.ToLower(); }
        }

        public string Type
        {
            get { return _type; }
            set { _type = RAXY.Situation.Situation.CheckAnyType(value.ToLower()) ? Property.FieldTypes.Any.ToString().ToLower() : value.ToLower(); }
        }

        public List<Property> Properties
        {
            get { return _properties; }
            set
            {
                _properties = value.OrderBy(o => o.Name).ToList();
            }
        }

        public bool EqualsIgnoringTypeAndName(Subject s)
        {
            if (s == Subject.EmptySubject()) { return true; }
            if (s.Properties.Count != this.Properties.Count) return false;
            if (Properties.Where((t, i) => !t.Equals(s.Properties[i])).Any())
            {
                return false;
            }
            return string.Equals(this.SubjectClass, s.SubjectClass);
        }

        public bool Fit(Subject s)
        {
            if (s.isEmpty()) { return true; }
            var typeEquals = (string.Equals(this.Type, s.Type) || s.Type == Property.FieldTypes.Any.ToString().ToLower());
            var classEquals = string.Equals(this.SubjectClass, s.SubjectClass) ||
                              s.SubjectClass == Property.FieldTypes.Any.ToString().ToLower();
            var buf = s.Properties.Select(prop => this.Properties.FirstOrDefault(o => o.Equals(prop))).ToList();

            return buf.Count == s.Properties.Count && typeEquals && classEquals;
        }
        public bool EqualsIgnoringName(Subject s)
        {
            if (Equals(s, Subject.EmptySubject())) { return true; }
            return this.EqualsIgnoringTypeAndName(s) && (string.Equals(this.Type, s.Type, StringComparison.InvariantCultureIgnoreCase) 
                || string.Equals(s.Type, Property.FieldTypes.Any.ToString(),StringComparison.InvariantCultureIgnoreCase));
        }
        public override bool Equals(object obj)
        {
            if (obj.GetType() != typeof (Subject)) { return false; }
            var s = (Subject) obj;
            if (s.isEmpty()) { return true; }
            return this.EqualsIgnoringName(s) && string.Equals(this.Name, s.Name);
        }

        public bool isEmpty()
        {
            return string.IsNullOrEmpty(Name) && string.IsNullOrEmpty(this.SubjectClass) && string.IsNullOrEmpty(Type);
        }
    }
}
