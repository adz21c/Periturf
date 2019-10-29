using FakeItEasy;
using NUnit.Framework;
using Periturf.Components;
using Periturf.Verify;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Periturf.Tests.Verify
{
    [TestFixture]
    class VerifyDefinitionTests
    {
        private IExpectationCriteriaEvaluator _expectationCriteriaEvaluator1;
        private IExpectationCriteriaSpecification _expectationCriteriaSpec1;
        private IExpectationCriteriaEvaluator _expectationCriteriaEvaluator2;
        private IExpectationCriteriaSpecification _expectationCriteriaSpec2;
        private string _componentName;
        private Environment _environment;
        private IComponentConditionSpecification _componentConditionSpecification;

        [SetUp]
        public void SetUp()
        {
            var componentConditionEvaluators = A.CollectionOfDummy<IComponentConditionEvaluator>(2);
         
            _componentConditionSpecification = A.Fake<IComponentConditionSpecification>();
            A.CallTo(() => _componentConditionSpecification.BuildAsync(A<CancellationToken>._))
                .ReturnsNextFromSequence(componentConditionEvaluators.ToArray());

            var componentConditionBuilder = A.Fake<ITestComponentConditionBuilder>();
            A.CallTo(() => componentConditionBuilder.CreateCondition()).Returns(_componentConditionSpecification);

            // Arrange
            var component = A.Fake<IComponent>();
            A.CallTo(() => component.CreateConditionBuilder<ITestComponentConditionBuilder>()).Returns(componentConditionBuilder);
            _componentName = nameof(component);

            var host1 = A.Fake<IHost>();
            A.CallTo(() => host1.Components).Returns(new ReadOnlyDictionary<string, IComponent>(new Dictionary<string, IComponent> { { nameof(component), component } }));

            _expectationCriteriaEvaluator1 = A.Fake<IExpectationCriteriaEvaluator>();
            _expectationCriteriaSpec1 = A.Fake<IExpectationCriteriaSpecification>();
            A.CallTo(() => _expectationCriteriaSpec1.Build()).Returns(_expectationCriteriaEvaluator1);

            _expectationCriteriaEvaluator2 = A.Fake<IExpectationCriteriaEvaluator>();
            _expectationCriteriaSpec2 = A.Fake<IExpectationCriteriaSpecification>();
            A.CallTo(() => _expectationCriteriaSpec2.Build()).Returns(_expectationCriteriaEvaluator2);

            _environment = Environment.Setup(x =>
            {
                x.Host(nameof(host1), host1);
            });
        }

        [Test]
        public async Task Given_SharedCondition_When_Verify_Then_SpecificationBuiltTwice()
        {
            var verifier = await _environment.VerifyAsync(c =>
            {
                var sharedCondition = c.GetComponentConditionBuilder<ITestComponentConditionBuilder>(_componentName).CreateCondition();
                c.Expect(
                    sharedCondition,
                    e => e.Must(_expectationCriteriaSpec1));
                c.Expect(
                    sharedCondition,
                    e => e.Must(_expectationCriteriaSpec2));
            });

            Assert.IsNotNull(verifier);

            A.CallTo(() => _componentConditionSpecification.BuildAsync(A<CancellationToken>._)).MustHaveHappenedTwiceExactly();
        }
    }
}
