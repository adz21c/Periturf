using System;
using System.Collections.Generic;
using System.Text;

namespace Periturf.Verify.Criterias
{
    class MinCountCriteriaEvaluator : IExpectationCriteriaEvaluator
    {
        private readonly int _minCount;

        private int _count;

        public MinCountCriteriaEvaluator(int minCount)
        {
            _minCount = minCount;
        }

        public bool Completed => true;

        public bool? Met { get; private set; } = false;

        public void Evaluate(ConditionInstance instance)
        {
            _count = _count + 1;

            if (_count >= _minCount)
                Met = true;
        }
    }

    class MinCountCriteriaEvaluatorFactory : IExpectationCriteriaEvaluatorFactory
    {
        private readonly int _minCount;

        public MinCountCriteriaEvaluatorFactory(int minCount)
        {
            _minCount = minCount;
        }

        public IExpectationCriteriaEvaluator CreateInstance()
        {
            return new MinCountCriteriaEvaluator(_minCount);
        }
    }

    public class MinCountCriteriaSpecification : IExpectationCriteriaSpecification
    {
        private readonly int _minCount;

        public MinCountCriteriaSpecification(int minCount, TimeSpan timeout)
        {
            _minCount = minCount;
            Timeout = timeout;
        }

        public TimeSpan? Timeout { get; }

        public string Description => "";

        public IExpectationCriteriaEvaluatorFactory Build()
        {
            return new MinCountCriteriaEvaluatorFactory(_minCount);
        }
    }
}
