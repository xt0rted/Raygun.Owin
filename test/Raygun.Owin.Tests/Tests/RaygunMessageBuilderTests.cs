namespace Raygun.Tests
{
    using System;
    using System.Net;
    using System.Web;

    using NUnit.Framework;

    using Shouldly;

    [TestFixture]
    public class RaygunMessageBuilderTests
    {
        [Test]
        public void Should_set_error_for_exception()
        {
            // Given / When
            var sut = SutFactory().Build();
            var result = sut.Details;

            // Then
            result.Error.ClassName.ShouldBe("System.InvalidOperationException");
            result.Error.Message.ShouldBe("It's broken");
            result.Response.ShouldBe(null);
        }

        [TestCase(313, null)]
        [TestCase(413, "RequestEntityTooLarge")]
        public void Should_set_response_for_http_exception(int statusCode, string statusDescription)
        {
            // Given / When
            var sut = SutFactoryForHttpException(statusCode).Build();
            var result = sut.Details.Response;

            // Then
            result.StatusCode.ShouldBe(statusCode);
            result.StatusDescription.ShouldBe(statusDescription);
        }

        [Test]
        public void Should_set_response_for_web_exception()
        {
            // Given / When
            var sut = SutFactoryForWebException().Build();
            var result = sut.Details.Response;

            // Then
            result.StatusCode.ShouldBe(0);
            result.StatusDescription.ShouldBe("ProtocolError");
        }

        [Test]
        public void Should_set_response_for_web_exception_with_http_response()
        {
            // Given / When
            var sut = SutFactoryForWebRequest().Build();

            // Then
            sut.Details.Response.ShouldNotBe(null);
            sut.Details.Response.StatusCode.ShouldBe(404);
            sut.Details.Response.StatusDescription.ShouldBe("Not Found");
        }

        [Test]
        public void Should_set_response_for_http_exception_instead_of_owin_environment()
        {
            // Given
            var context = new TestOwinContext()
                .WithUri(new Uri("http://raygun.io/clients?language=c%23"))
                .WithResponseStatus(code: 302, phrase: "It's broken");

            var sut = SutFactoryForHttpException(500);

            // When
            var result = sut.SetHttpDetails(context.Environment).Build();

            // Then
            result.Details.Response.StatusCode.ShouldBe(500);
            result.Details.Response.StatusDescription.ShouldBe("InternalServerError");
        }

        [Test]
        public void Should_set_response_for_request_if_not_already_set()
        {
            // Given
            var context = new TestOwinContext()
                .WithUri(new Uri("http://raygun.io/clients?language=c%23"))
                .WithResponseStatus(code: 302, phrase: "It's broken");

            var sut = SutFactory();

            // When
            var result = sut.SetHttpDetails(context.Environment).Build();

            // Then
            result.Details.Response.StatusCode.ShouldBe(302);
            result.Details.Response.StatusDescription.ShouldBe("It's broken");
        }

        private static IRaygunMessageBuilder SutFactory(Exception exception = null)
        {
            if (exception == null)
            {
                exception = new InvalidOperationException("It's broken");
            }

            return RaygunMessageBuilder.New
                                       .SetExceptionDetails(exception);
        }

        private static IRaygunMessageBuilder SutFactoryForHttpException(int statusCode)
        {
            return SutFactory(new HttpException(statusCode, "It's broken"));
        }

        private static IRaygunMessageBuilder SutFactoryForWebException()
        {
            return SutFactory(new WebException("It's broken", WebExceptionStatus.ProtocolError));
        }

        private static IRaygunMessageBuilder SutFactoryForWebRequest()
        {
            Exception exception = null;

            try
            {
                var request = WebRequest.Create("http://bing.com/raygun.io");
                request.GetResponse();
            }
            catch (Exception ex)
            {
                exception = ex;
            }

            exception.ShouldNotBe(null);

            return SutFactory(exception);
        }
    }
}