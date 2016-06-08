using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace RAXY.Situation
{
    public class SituationToRuleConverter
    {
        /// <summary>
        /// Possible logical functions.
        /// </summary>
        public enum LogicalFunctions
        {
            // use ToString("G");
            Unknown,
            Equals,
            NotEquals,
            In,
            Less,
            LessOrEquals,
            Greater,
            GreaterOrEquals
        }
        public static LogicalFunctions ParseLogicalFunction(string toParse)
        {
            if (String.IsNullOrEmpty(toParse)) return LogicalFunctions.Unknown;
            if (toParse == "==" || toParse == "=" || toParse == "equals" || toParse == "equal" || toParse == "равно")
            {
                return LogicalFunctions.Equals;
            } if (toParse == "!=" || toParse == "notequals" || toParse == "notequal" || toParse == "неравно")
            {
                return LogicalFunctions.In;
            }
            if (toParse == "in" || toParse == "в")
            {
                return LogicalFunctions.In;
            }
            if (toParse == "<" || toParse == "less" || toParse == "меньше" || toParse == "&lt;")
            {
                return LogicalFunctions.Less;
            }
            if (toParse == "<=" || toParse == "lessoreqals" || toParse == "lessoreqal" || toParse == "меньшеилиравно" || toParse == "&lt;=")
            {
                return LogicalFunctions.LessOrEquals;
            }
            if (toParse == ">" || toParse == "greater" || toParse == "больше" || toParse == "&gt;")
            {
                return LogicalFunctions.Greater;
            }
            if (toParse == ">=" || toParse == "greateroreqals" || toParse == "greaterlessoreqal" || toParse == "большеилиравно" || toParse == "&gt;=")
            {
                return LogicalFunctions.GreaterOrEquals;
            }

            return LogicalFunctions.Unknown;
        }
        public static bool CanBeConverted(Situation s)
        {
            return s.Conditions.Length >= 1;
        }

        public static bool CheckConditionBySubject(Situation situation, Subject startingSubject, Condition ruleCondition)
        {
            var logFunc = ParseLogicalFunction(ruleCondition.LogicalFunction);
            var query = string.Format("{0}.{1}", startingSubject.Name, ruleCondition.Param.Substring(ruleCondition.Param.IndexOf('.') + 1));
            var foundedValue = GetValueFromSituation(situation, query);
            // if this param doesn't exist, we don't need to compare it.
            if (string.IsNullOrEmpty(foundedValue) || foundedValue.ToLower().Contains("error")) { return false; }

            return CheckCondition(GetValueFromSituation(situation, query), logFunc, ruleCondition.ExpectedValue);
        }

        private static string GetValueFromSituation(Situation situation, string query)
        {
            var queryParams = Regex.Replace(query.ToLower(), @"\s+", "").Split('.');

            // search for subject
            var subject = situation.Subjects.FirstOrDefault(s => s.Name == queryParams[0]);
            if (subject == null)
            {
                return string.Format("Error : there are no subjects with name : [{0}]", queryParams[0]);
            }

            if (queryParams.Length == 3)
            {
                //check second param
                if (queryParams[1] == "процессы" || queryParams[1] == "процесс" || queryParams[1] == "processes" ||
                    queryParams[1] == "process")
                {
                    var processes = situation.Processes.Where(p => p.Subject.Equals(subject)).ToList();
                    if (queryParams[2] == "действие" || queryParams[2] == "action")
                    {
                        // the subject has just one process in the situation
                        return processes[0].Action;
                    }
                    if (queryParams[2] == "типдействия" || queryParams[2] == "actiontype")
                    {
                        // the subject has just one process in the situation
                        return processes[0].ActionType;
                    }
                    if (queryParams[2] == "направлениедействия" || queryParams[2] == "actiondirection")
                    {
                        // the subject has just one process in the situation
                        return processes[0].ActionDirection;
                    }
                    if (queryParams[2] == "цельдействия" || queryParams[2] == "actiontarget")
                    {
                        // the subject has just one process in the situation
                        return processes[0].ActionTarget;
                    }
                    return string.Format("Error : unknown query parameter : [{0}]", queryParams[2]);
                }
            }
            else
            {
                if (queryParams.Length == 2)
                {
                    var property = subject.Properties.FirstOrDefault(prop => prop.Name == queryParams[1]);
                    return property == null
                        ? string.Format("Error : there are no properties with name : [{0}]", queryParams[1])
                        : property.Value;
                }
            }
            return string.Format("Error : can not find value using query : [{0}]", query);
        }

        public static bool CheckCondition(string param, LogicalFunctions func, string value)
        {
            switch (func)
            {
                case LogicalFunctions.Equals:
                    return string.Equals(param, value);
                case LogicalFunctions.NotEquals:
                    return !string.Equals(param, value);
            }

            if (Regex.IsMatch(value, @"^\[\d+\;\d+\]$"))
            {
                var segment = Regex.Matches(value, @"\d+");
                if (func == LogicalFunctions.In && segment.Count == 2)
                {
                    int begin, end, p;
                    if (Int32.TryParse(segment[0].Value, out begin) &&
                        Int32.TryParse(segment[1].Value, out end) && 
                        Int32.TryParse(param, out p))
                    {
                        return p > begin && p < end;
                    }
                }
                return false;
            }
            if (Regex.IsMatch(value, @"^\[(\w+\,)+(\w+)\]$"))
            {
                var set = Regex.Matches(value, @"\w+");
                if (func == LogicalFunctions.In)
                {
                    return set.Cast<string>().Any(o => string.Equals(o, value));
                }
            }
            int out1, out2;
            if (Int32.TryParse(param, out out1) && Int32.TryParse(value, out out2))
            {
                switch (func)
                {
                    case LogicalFunctions.Greater:
                        return out1 > out2;
                    case LogicalFunctions.GreaterOrEquals:
                        return out1 >= out2;
                    case LogicalFunctions.Less:
                        return out1 < out2;
                    case LogicalFunctions.LessOrEquals:
                        return out1 <= out2;
                }
            }
            return false;
        }

        public static Subject IsSubjectParam(Situation situation, string query)
        {
            var queryParams = Regex.Replace(query.ToLower(), @"\s+", "").Split('.');

            // search for subject
            var subject = situation.Subjects.FirstOrDefault(s => s.Name == queryParams[0]);
            return subject;
        }
    }
}
