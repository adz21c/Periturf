using FakeItEasy;
using Microsoft.AspNetCore.Http;
using NUnit.Framework;
using Periturf.Events;
using Periturf.Web;
using Periturf.Web.Configuration;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Periturf.Tests.Web
{
    [TestFixture]
    class WebConfigurationTests
    {
        private readonly List<Func<IWebRequestEvent, bool>> _predicates = new List<Func<IWebRequestEvent, bool>>();
        private readonly Func<IWebResponse, Task> _responseFactory = A.Fake<Func<IWebResponse, Task>>();
        private readonly IEventHandler<IWebRequest> _handler = A.Fake<IEventHandler<IWebRequest>>();
        private WebConfiguration _sut;
        private readonly IWebRequestEvent _request = A.Dummy<IWebRequestEvent>();

        [SetUp]
        public void SetUp()
        {
            _predicates.Clear();
            _predicates.Add(x => false);
            Fake.ClearConfiguration(_responseFactory);
            Fake.ClearRecordedCalls(_responseFactory);
            Fake.ClearConfiguration(_handler);
            Fake.ClearRecordedCalls(_handler);

            _sut = new WebConfiguration(
                _predicates,
                _responseFactory,
                _handler);
        }

        [TestCase(true)]
        [TestCase(false)]
        public void Given_OnePredicate_When_Matches_Then_Result(bool match)
        {
            var predicate = A.Fake<Func<IWebRequestEvent, bool>>();
            A.CallTo(() => predicate.Invoke(A<IWebRequestEvent>._)).Returns(match);
            _predicates.Add(predicate);

            var result = _sut.Matches(_request);

            Assert.That(result, Is.EqualTo(match));
            A.CallTo(() => predicate.Invoke(A<IWebRequestEvent>._)).MustHaveHappened();
        }

        [TestCase(false, false, false)]
        [TestCase(false, true, true)]
        [TestCase(true, false, true)]
        [TestCase(true, true, true)]
        public void Given_MultiplePredicates_When_Matches_Then_Result(bool result1, bool result2, bool expectedResult)
        {
            var predicate = A.Fake<Func<IWebRequestEvent, bool>>();
            A.CallTo(() => predicate.Invoke(A<IWebRequestEvent>._)).Returns(result1);
            _predicates.Add(predicate);

            var predicate2 = A.Fake<Func<IWebRequestEvent, bool>>();
            A.CallTo(() => predicate2.Invoke(A<IWebRequestEvent>._)).Returns(result2);
            _predicates.Add(predicate2);

            var result = _sut.Matches(_request);

            Assert.That(result, Is.EqualTo(expectedResult));
        }

        [Test]
        public async Task Given_ResponseFactory_When_WriteResponse_Then_Executed()
        {
            await _sut.WriteResponse(A.Dummy<IWebResponse>());

            A.CallTo(() => _responseFactory.Invoke(A<IWebResponse>._)).MustHaveHappenedOnceExactly();
        }

        [Test]
        public async Task Given_EventHandler_When_ExecuteHandler_Then_Executed()
        {
            await _sut.ExecuteHandlers(_request.Request);

            A.CallTo(() => _handler.ExecuteHandlersAsync(A<IWebRequest>._, A<CancellationToken>._)).MustHaveHappenedOnceExactly();
        }
    }
}
