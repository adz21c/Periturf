using NUnit.Framework;
using Periturf.Verify;

namespace Periturf.Tests.Verify
{
    [TestFixture]
    class ExpectationResultTests
    {
        [TestCase(false)]
        [TestCase(true)]
        public void Given_Params_When_Ctor_Then_FieldsMapped(bool met)
        {
            var result = new ExpectationResult(met);

            Assert.AreEqual(met, result.Met);
        }
    }
}
