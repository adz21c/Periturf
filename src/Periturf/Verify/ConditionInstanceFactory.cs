using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Periturf.Verify
{
    class ConditionInstanceFactory : IConditionInstanceFactory
    {
        private readonly ITime _time;
        private DateTime? _start;

        public ConditionInstanceFactory(ITime time)
        {
            _time = time;
        }

        public void Start()
        {
            _start = _time.Now;
        }

        public ConditionInstance Create(string id)
        {
            return Create(id, _time.Now);
        }

        public ConditionInstance Create(string id, DateTime eventDateTime)
        {
            var timespan = _start.HasValue ? eventDateTime - _start.Value : TimeSpan.Zero;
            return new ConditionInstance(timespan, id);
        }
    }
}
