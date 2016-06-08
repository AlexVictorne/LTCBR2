using System;

namespace RAXY.Situation
{
    [Serializable]
    public class Relation
    {
        private string _relationType, _relationProperty;
        private string _subject1, _subject2;

        private Relation()
        {
            this._subject1 = string.Empty;
            this._subject2 = string.Empty;
            this._relationType = string.Empty;
            this._relationProperty = string.Empty;
        }

        public Relation(string subject1, string subject2, string relationType, string relationProperty)
        {
            this._subject1 = subject1;
            this._subject2 = subject2;
            this._relationType = relationType.ToLower();
            this._relationProperty = relationProperty.ToLower();
        }

        public string Subject1
        {
            get { return _subject1; }
            set { _subject1 = value.ToLower(); }
        }

        public string Subject2
        {
            get { return _subject2; }
            set { _subject2 = value.ToLower(); }
        }

        public string RelationType
        {
            get { return _relationType; }
            set { _relationType = value.ToLower(); }
        }

        public string RelationProperty
        {
            get { return _relationProperty; }
            set { _relationProperty = value.ToLower(); }
        }

        public static Relation EmptyRelation()
        {
            return new Relation();
        }
    }
}
