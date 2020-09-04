using FakeItEasy;
using Microsoft.AspNetCore.Http;
using NUnit.Framework;
using Periturf.Events;
using Periturf.Web;
using Periturf.Web.Configuration;
using Periturf.Web.Configuration.Requests.Predicates;
using Periturf.Web.Configuration.Requests.Responses;
using Periturf.Web.Setup;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Periturf.Tests.Web
{
    [TestFixture]
    class WebComponentTests
    {
        private WebComponent _sut;
        private Func<IEventContext<IWebRequest>, CancellationToken, Task> _handler;
        private IEventHandlerSpecification<IWebRequest> _handlerSpec;
        private Func<IWebResponse, Task> _responseFactory;
        private Func<IWebRequestEvent, bool> _predicate;
        private IEventHandler<IWebRequest> _eventHandler;
        private IEventHandlerFactory _eventHandlerFactory;

        [SetUp]
        public async Task SetupAsync()
        {
            _eventHandler = A.Fake<IEventHandler<IWebRequest>>();
            _eventHandlerFactory = A.Fake<IEventHandlerFactory>();
            A.CallTo(() => _eventHandlerFactory.Create(A<IEnumerable<IEventHandlerSpecification<IWebRequest>>>._)).Returns(_eventHandler);

            _predicate = A.Fake<Func<IWebRequestEvent, bool>>();
            var predicateSpec = A.Fake<IWebRequestPredicateSpecification>();
            A.CallTo(() => predicateSpec.Build()).Returns(_predicate);

            _responseFactory = A.Fake<Func<IWebResponse, Task>>();
            var responseSpec = A.Fake<IWebRequestResponseSpecification>();
            A.CallTo(() => responseSpec.BuildFactory()).Returns(_responseFactory);

            _handler = A.Dummy<Func<IEventContext<IWebRequest>, CancellationToken, Task>>();
            _handlerSpec = A.Fake<IEventHandlerSpecification<IWebRequest>>();
            A.CallTo(() => _handlerSpec.Build()).Returns(_handler);

            _sut = new WebComponent();
            var configSpec = _sut.CreateConfigurationSpecification<WebComponentSpecification>(_eventHandlerFactory);
            configSpec.OnRequest(r =>
            {
                r.AddPredicateSpecification(predicateSpec);
                r.SetResponseSpecification(responseSpec);
                r.AddHandlerSpecification(_handlerSpec);
            });
            await configSpec.ApplyAsync();
        }

        [Test]
        public async Task Given_ConfiguredRequest_When_Process_Then_HandlersCalledAfterPredicate()
        {
            var context = new DefaultHttpContext();
            A.CallTo(() => _predicate.Invoke(A<IWebRequestEvent>._)).Returns(true);

            await _sut.ProcessAsync(context);

            A.CallTo(() => _eventHandlerFactory.Create(A<IEnumerable<IEventHandlerSpecification<IWebRequest>>>.That.Contains(_handlerSpec))).MustHaveHappened();

            A.CallTo(() => _predicate.Invoke(A<IWebRequestEvent>._)).MustHaveHappened().Then(
                A.CallTo(() => _responseFactory.Invoke(A<IWebResponse>._)).MustHaveHappened());

            A.CallTo(() => _predicate.Invoke(A<IWebRequestEvent>._)).MustHaveHappened().Then(
                A.CallTo(() => _eventHandler.ExecuteHandlersAsync(A<IWebRequest>._, A<CancellationToken>._)).MustHaveHappened());
        }

        [Test]
        public async Task Given_UnusedConfiguredRequest_When_Process_Then_HandlersNotCalledAfterPredicate()
        {
            var context = new DefaultHttpContext();
            A.CallTo(() => _predicate.Invoke(A<IWebRequestEvent>._)).Returns(false);

            await _sut.ProcessAsync(context);

            A.CallTo(() => _eventHandlerFactory.Create(A<IEnumerable<IEventHandlerSpecification<IWebRequest>>>.That.Contains(_handlerSpec))).MustHaveHappened();

            Assert.That(context.Response.StatusCode, Is.EqualTo(404));

            A.CallTo(() => _predicate.Invoke(A<IWebRequestEvent>._)).MustHaveHappened();
            A.CallTo(() => _responseFactory.Invoke(A<IWebResponse>._)).MustNotHaveHappened();
            A.CallTo(() => _eventHandler.ExecuteHandlersAsync(A<IWebRequest>._, A<CancellationToken>._)).MustNotHaveHappened();
        }
    }
}
