using System;

namespace Periturf.Verify
{
    public class ConditionInstance
    {
        public ConditionInstance(TimeSpan when, string id)
        {
            When = when;
            ID = id ?? string.Empty;
        }

        public TimeSpan When { get; }

        public string ID { get; }
    }
}
