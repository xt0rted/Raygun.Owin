namespace Raygun.Tests
{
    using System;

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
            var sut = SutFactory();
            var result = sut.Details.Error;

            // Then
            result.ClassName.ShouldBe("System.InvalidOperationException"); 
            result.Message.ShouldBe("InvalidOperationException: It's broken");
        }

        private RaygunMessage SutFactory()
        {
            var exception = new InvalidOperationException("It's broken");

            return RaygunMessageBuilder.New
                                       .SetExceptionDetails(exception)
                                       .Build();
        }
    }
}