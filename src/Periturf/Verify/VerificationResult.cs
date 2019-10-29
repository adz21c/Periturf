using System;
using System.Collections.Generic;
using System.Text;

namespace Periturf.Verify
{
    class VerificationResult : IVerificationResult
    {
        public VerificationResult(bool expectationsMet, IReadOnlyList<IExpectationResult> expectationResults)
        {
            ExpectationsMet = expectationsMet;
            ExpectationResults = expectationResults;
        }

        public bool ExpectationsMet { get; }

        public IReadOnlyList<IExpectationResult> ExpectationResults { get; }
    }
}
