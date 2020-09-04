using FakeItEasy;
using NUnit.Framework;
using Periturf.Events;
using Periturf.Web;
using Periturf.Web.Configuration;
using Periturf.Web.Configuration.Requests;
using Periturf.Web.Configuration.Requests.Predicates;
using Periturf.Web.Configuration.Requests.Responses;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Periturf.Tests.Web.Configuration
{
    [TestFixture]
    class WebRequestEventSpecificationTests
    {
        private IEventHandlerFactory _eventHandlerFactory;
        private WebRequestEventSpecification _sut;

        [SetUp]
        public void SetUp()
        {
            _eventHandlerFactory = A.Fake<IEventHandlerFactory>();

            _sut = new WebRequestEventSpecification(_eventHandlerFactory);
        }

        [Test]
        public void Given_Null_When_AddPredicateSpec_Then_Exception()
        {
            Assert.That(() => _sut.AddPredicateSpecification(null), Throws.ArgumentNullException.With.Property("ParamName").EqualTo("spec"));
        }


        [Test]
        public void Given_Null_When_SetResponseSpec_Then_Exception()
        {
            Assert.That(() => _sut.SetResponseSpecification(null), Throws.ArgumentNullException.With.Property("ParamName").EqualTo("spec"));
        }

        [Test]
        public async Task Given_WebRequestSpec_When_Build_Then_ConfigBuilt()
        {
            var predicate = A.Dummy<Func<IWebRequestEvent, bool>>();
            var predicateSpec = A.Fake<IWebRequestPredicateSpecification>();
            A.CallTo(() => predicateSpec.Build()).Returns(predicate);

            var responseFactory = A.Dummy<Func<IWebResponse, Task>>();
            var responseSpec = A.Fake<IWebRequestResponseSpecification>();
            A.CallTo(() => responseSpec.BuildFactory()).Returns(responseFactory);
            
            var handlerSpec = A.Fake<IEventHandlerSpecification<IWebRequest>>();

            _sut.AddPredicateSpecification(predicateSpec);
            _sut.SetResponseSpecification(responseSpec);
            _sut.AddHandlerSpecification(handlerSpec);

            var config = _sut.Build();

            var request = A.Dummy<IWebRequestEvent>();
            var response = A.Dummy<IWebResponse>();

            config.Matches(request);
            await config.WriteResponse(response);

            A.CallTo(() => predicate.Invoke(A<IWebRequestEvent>._)).MustHaveHappened();
            A.CallTo(() => responseFactory.Invoke(A<IWebResponse>._)).MustHaveHappened();
            A.CallTo(() => _eventHandlerFactory.Create(A<IEnumerable<IEventHandlerSpecification<IWebRequest>>>._)).MustHaveHappened();

            Assert.That(config, Is.Not.Null);
        }
    }
}
