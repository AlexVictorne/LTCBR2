using System;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;

namespace RAXY.Situation
{
    [Serializable()]
    public class Property : ISerializable
    {
        private string _name, _value;
        public enum FieldTypes
        {
            Any
        }
        private Property() { }
        public Property(string name, string value)
        {
            this._name = name.ToLower();
            this._value = value.ToLower();
        }
        public string Name
        {
            get { return _name; }
            set { _name = value.ToLower(); }
        }
        public string Value
        {
            get { return _value; }
            set { _value = RAXY.Situation.Situation.CheckAnyType(value) ? Property.FieldTypes.Any.ToString().ToLower() : value.ToLower(); }
        }
        public void GetObjectData(SerializationInfo info, StreamingContext ctxt)
        {
            info.AddValue("Name", Name);
            info.AddValue("Value", Value);
        }
        public override bool Equals(object obj)
        {
            if (obj.GetType() != typeof(Property)) { return false; }
            var p = (Property) obj;
            if (!string.Equals(this.Name, p.Name)) { return false; }
            if (Regex.IsMatch(p.Value, @"^(>?<?=?(<=)?(>=)?(in)?(<=)?){1}\d+$"))
            {
                var logFunc = SituationToRuleConverter.ParseLogicalFunction(Regex.Replace(p.Value, @"\d+$", ""));
                return SituationToRuleConverter.CheckCondition(this.Value, logFunc, Regex.Match(p.Value, @"\d+$").Value);
            }
            return string.Equals(this.Value, p.Value, StringComparison.CurrentCultureIgnoreCase) || 
                string.Equals(this.Value, FieldTypes.Any.ToString(), StringComparison.CurrentCultureIgnoreCase);
        }
    }
}
