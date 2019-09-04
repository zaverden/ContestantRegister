using System;

namespace ContestantRegister.Framework.Filter.Attributes
{
    public class FilterConditionAttribute : Attribute
    {
        public FilterConditionAttribute(FilterCondition condition)
        {
            FilterCondition = condition;
        }

        public FilterCondition FilterCondition { get; }
    }
}
