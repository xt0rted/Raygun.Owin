namespace Raygun.Messages.Tests
{
    using NUnit.Framework;

    using Shouldly;

    [TestFixture]
    public class RaygunClientMessageTests
    {
        [Test]
        public void Should_return_name_from_the_client()
        {
            // Given
            var sut = SutFactory();

            // When / Then
            sut.Name.ShouldBe(RaygunClient.ClientName);
        }

        [Test]
        public void Should_return_version_from_the_client()
        {
            // Given
            var sut = SutFactory();

            // When / Then
            sut.Version.ShouldBe(RaygunClient.ClientVersion);
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