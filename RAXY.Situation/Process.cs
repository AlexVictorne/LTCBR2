using System;

namespace RAXY.Situation
{
    [Serializable]
    public class Process
    {
        private string _action, _actionType, _actionDirection;
        private string _subject, _actionTarget;

        private Process() { }
        public Process(string subject, string action,
            string actionType, string actionDirection, string actionTarget)
        {
            this._subject = subject;
            this._action = action.ToLower();
            this._actionType = actionType.ToLower();
            this._actionDirection = actionDirection.ToLower();
            this._actionTarget = actionTarget;
        }

        public string Subject
        {
            get { return _subject; }
            set { _subject = value.ToLower(); }
        }

        public string Action
        {
            get { return _action; }
            set { _action = Situation.CheckAnyType(value.ToLower()) ? Property.FieldTypes.Any.ToString().ToLower() : value.ToLower(); }
        }

        public string ActionType
        {
            get { return _actionType; }
            set { _actionType = Situation.CheckAnyType(value.ToLower()) ? Property.FieldTypes.Any.ToString().ToLower() : value.ToLower(); }
        }

        public string ActionDirection
        {
            get { return _actionDirection; }
            set { _actionDirection = Situation.CheckAnyType(value.ToLower()) ? Property.FieldTypes.Any.ToString().ToLower() : value.ToLower(); }
        }

        /// <summary>
        /// Define action target. TODO: target type can be subject or value changing of some property (f.e. acceleration).
        /// </summary>
        public string ActionTarget
        {
            get { return _actionTarget; }
            set { _actionTarget = value.ToLower(); }
        }
        public bool EqualActions(Process toCompare)
        {
            return (string.Equals(toCompare.Action, this.Action) ||
                    toCompare.Action == Property.FieldTypes.Any.ToString()) &&
                   (string.Equals(toCompare.ActionType, this.ActionType) ||
                    toCompare.ActionType == Property.FieldTypes.Any.ToString()) &&
                   (string.Equals(toCompare.ActionDirection, this.ActionDirection) ||
                    toCompare.ActionDirection == Property.FieldTypes.Any.ToString());
        }

        //public override bool Equals(object obj)
        //{
        //    if (obj.GetType() != typeof(Process)) { return false; }
        //    var res1 = EqualActions((Process) obj);
        //    if (res1)
        //    {
        //        int a = 0;
        //    }
        //    var res2 = ((Process) obj).Subject.EqualsIgnoringName(this.Subject);
        //    var res3 = ((Process) obj).ActionTarget.EqualsIgnoringName(this.ActionTarget);

        //    return EqualActions((Process)obj) && ((Process)obj).Subject.EqualsIgnoringName(this.Subject) && ((Process)obj).ActionTarget.EqualsIgnoringName(this.ActionTarget); 
        //}

        //public bool Fit(Process p)
        //{
        //    var res1 = EqualActions(p);
        //    if (res1)
        //    {
        //        int a = 0;
        //    }
        //    var res2 = this.Subject.Fit(p.Subject);
        //    var res3 = this.ActionTarget.Fit(p.ActionTarget);

        //    return res1 && res2 && res3; 
        //}
    }
}
