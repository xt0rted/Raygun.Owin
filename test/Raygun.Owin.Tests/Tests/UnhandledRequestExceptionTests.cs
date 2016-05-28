namespace Raygun.Tests
{
    using System;

    using NUnit.Framework;

    using Shouldly;

    [TestFixture]
    public class UnhandledRequestExceptionTests
    {
        [Test]
        public void Should_set_url()
        {
            // Given
            var url = "/some/page";

            // When
            var result = new UnhandledRequestException(url);

            // Then
            result.RequestUrl.ShouldBe(url);
            result.Message.ShouldBe("The request was unhandled: /some/page");
        }

        [Test]
        public void Should_set_url_and_message()
        {
            // Given
            var url = "/some/page";
            var message = "This is a test.";

            // When
            var result = new UnhandledRequestException(url, message);

            // Then
            result.RequestUrl.ShouldBe(url);
            result.Message.ShouldBe(message);
        }

        [Test]
        public void Should_set_url_message_and_innerexception()
        {
            // Given
            var url = "/some/page";
            var message = "This is a test.";
            var innerException = new InvalidOperationException();

            // When
            var result = new UnhandledRequestException(url, message, innerException);

            // Then
            result.RequestUrl.ShouldBe(url);
            result.Message.ShouldBe(message);
            result.InnerException.ShouldBe(innerException);
        }
    }
}
