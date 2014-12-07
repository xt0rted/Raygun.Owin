namespace Raygun.Messages.Tests
{
    using NUnit.Framework;

    using Shouldly;

    [TestFixture]
    public class RaygunClientMessageTests
    {
        [Test]
        public void Should_return_name_from_the_AssemblyTitleAttribute()
        {
            // Given
            var sut = SutFactory();

            // When / Then
            sut.Name.ShouldBe("Raygun.Owin");
        }

        [Test]
        public void Should_return_version_from_the_AssemblyVersionAttribute()
        {
            // Given
            var sut = SutFactory();

            // When / Then
            sut.Version.ShouldBe("0.1.0.0");
        }

        [Test]
        public void Should_return_client_url_for_the_github_repository()
        {
            // Given
            var sut = SutFactory();

            // When / Then
            sut.ClientUrl.ShouldBe("https://github.com/xt0rted/Raygun.Owin");
        }

        private static RaygunClientMessage SutFactory()
        {
            return new RaygunClientMessage();
        }
    }
}