using System;
using System.Text.RegularExpressions;

namespace RAXY.Situation
{
    [Serializable]
    public class Condition
    {
        private string _param;
        private string _logFunction;
        private string _expectedValue;

        private Condition(){}
        public Condition(string param, string logFunc, string expectedValue)
        {
            logFunc = Regex.Replace(logFunc, @"\s+", "");
            _param = param.ToLower();
            _expectedValue = expectedValue.ToLower();
            _logFunction = logFunc;
        }

        public string Param
        {
            get { return _param; }
            set { _param = value.ToLower(); }
        }

        public string LogicalFunction
        {
            get { return _logFunction; }
            set { _logFunction = Regex.Replace(value.ToLower(), @"\s+", ""); }
        }

        public string ExpectedValue
        {
            get { return _expectedValue; }
            set { _expectedValue = value.ToLower(); }
        }
        public static Condition EmptyCondition()
        {
            return new Condition();
        }

        public string GetConditionString()
        {
            return string.Format("{0} {1} {2}", _param, _logFunction, _expectedValue);
        }
    }
}
