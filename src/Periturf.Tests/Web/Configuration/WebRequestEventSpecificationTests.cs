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
        [Test]
        public async Task Given_WebRequestSpec_When_Build_Then_ConfigBuilt()
        {
            var eventHandlerFactory = A.Fake<IEventHandlerFactory>();
            
            var predicate = A.Dummy<Func<IWebRequestEvent, bool>>();
            var predicateSpec = A.Fake<IWebRequestPredicateSpecification>();
            A.CallTo(() => predicateSpec.Build()).Returns(predicate);

            var responseFactory = A.Dummy<Func<IWebResponse, Task>>();
            var responseSpec = A.Fake<IWebRequestResponseSpecification>();
            A.CallTo(() => responseSpec.BuildFactory()).Returns(responseFactory);
            
            var handlerSpec = A.Fake<IEventHandlerSpecification<IWebRequest>>();

            var sut = new WebRequestEventSpecification(eventHandlerFactory);
            sut.AddPredicateSpecification(predicateSpec);
            sut.SetResponseSpecification(responseSpec);
            sut.AddHandlerSpecification(handlerSpec);

            var config = sut.Build();

            var request = A.Dummy<IWebRequestEvent>();
            var response = A.Dummy<IWebResponse>();

            config.Matches(request);
            await config.WriteResponse(response);

            A.CallTo(() => predicate.Invoke(A<IWebRequestEvent>._)).MustHaveHappened();
            A.CallTo(() => responseFactory.Invoke(A<IWebResponse>._)).MustHaveHappened();
            A.CallTo(() => eventHandlerFactory.Create(A<IEnumerable<IEventHandlerSpecification<IWebRequest>>>._)).MustHaveHappened();

            Assert.That(config, Is.Not.Null);
        }
    }
}
