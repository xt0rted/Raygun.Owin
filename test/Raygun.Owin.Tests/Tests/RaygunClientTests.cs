namespace Raygun.Tests
{
    using NUnit.Framework;

    using Shouldly;

    [TestFixture]
    public class RaygunClientTests
    {
        [Test]
        public void Should_return_name_from_the_AssemblyTitleAttribute()
        {
            // Given / When
            var result = RaygunClient.ClientName;

            // Then
            result.ShouldBe("Raygun.Owin");
        }

        [Test]
        public void Should_return_version_from_the_AssemblyVersionAttribute()
        {
            // Given / When
            var expected = typeof (RaygunClient).Assembly.GetName().Version.ToString();
            var result = RaygunClient.ClientVersion;

            // Then
            result.ShouldBe(expected);
        }
    }
}