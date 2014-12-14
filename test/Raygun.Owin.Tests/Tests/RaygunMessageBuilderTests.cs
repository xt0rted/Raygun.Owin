namespace Raygun.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Web;

    using NUnit.Framework;

    using Raygun.Messages;

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

        [Test]
        public void Should_set_custom_user_data()
        {
            // Given
            var data = new Dictionary<string, object>
            {
                { "number", 13 }
            };
            var sut = SutFactory();

            // When
            var result = sut.SetUserCustomData(data).Build();

            // Then
            result.Details.UserCustomData.ShouldBe(data);
        }

        [Test]
        public void Should_set_custom_user_data_to_null()
        {
            // Given
            var sut = SutFactory();

            // When
            var result = sut.SetUserCustomData(null).Build();

            // Then
            result.Details.UserCustomData.ShouldBe(null);
        }

        [Test]
        public void Should_set_tags()
        {
            // Given
            var tags = new[] { "tag1", "tag2" };
            var sut = SutFactory();

            // When
            var result = sut.SetTags(tags).Build();

            // Then
            result.Details.Tags.ShouldNotBe(null);
            result.Details.Tags.ShouldBe(tags);
        }

        [Test]
        public void Should_not_set_tags_if_empty()
        {
            // Given
            var tags = new string[0];
            var sut = SutFactory();

            // When
            var result = sut.SetTags(tags).Build();

            // Then
            result.Details.Tags.ShouldNotBe(null);
            result.Details.Tags.ShouldBeEmpty();
        }

        [Test]
        public void Should_not_set_tags_if_null()
        {
            // Given
            var sut = SutFactory();

            // When
            var result = sut.SetTags(null).Build();

            // Then
            result.Details.Tags.ShouldNotBe(null);
            result.Details.Tags.ShouldBeEmpty();
        }

        [Test]
        public void Should_set_user_details()
        {
            // Given
            var userDetails = new RaygunIdentifierMessage("testUser");
            var sut = SutFactory();

            // When
            var result = sut.SetUser(userDetails).Build();

            // Then
            result.Details.User.ShouldBe(userDetails);
        }

        [Test]
        public void Should_set_user_details_to_null()
        {
            // Given
            var sut = SutFactory();

            // When
            var result = sut.SetUser(null).Build();

            // Then
            result.Details.User.ShouldBe(null);
        }

        [Test]
        public void Should_set_version_to_custom_value()
        {
            // Given
            const string applicationVersion = "test-version";
            var sut = SutFactory();

            // When
            var result = sut.SetVersion(applicationVersion).Build();

            // Then
            result.Details.Version.ShouldBe(applicationVersion);
        }

        [Test]
        public void Should_set_version_when_no_entry_assembly_found()
        {
            // Given
            var sut = SutFactory();

            // When
            var result = sut.SetVersion().Build();

            // Then
            result.Details.Version.ShouldBe("Not supplied");
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