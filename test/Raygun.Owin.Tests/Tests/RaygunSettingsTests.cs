namespace Raygun.Tests
{
    using System.Configuration;

    using NUnit.Framework;

    using Shouldly;

    [TestFixture]
    public class RaygunSettingsTests
    {
        [TearDown]
        public void TearDown()
        {
            ConfigurationManager.AppSettings["raygun:applicationVersion"] = null;
            ConfigurationManager.AppSettings["raygun:tags"] = null;
        }

        [TestCase("1.2.3.4-beta2")]
        [TestCase("  1.2.3.4-beta2\t")]
        public void Should_handle_app_settings_application_version(string version)
        {
            // Given / When
            var sut = SutFactoryForVersion(version);

            // Then
            sut.ApplicationVersion.ShouldBe("1.2.3.4-beta2");
        }

        [Test]
        public void Should_handle_app_settings_application_version_that_is_empty()
        {
            // Given / When
            var sut = SutFactoryForVersion(string.Empty);

            // Then
            sut.ApplicationVersion.ShouldBe(null);
        }

        [Test]
        public void Should_handle_app_settings_application_version_that_is_null()
        {
            // Given / When
            var sut = SutFactoryForVersion(null);

            // Then
            sut.ApplicationVersion.ShouldBe(null);
        }

        [TestCase("tag1,tag2;tag3|tag4")]
        [TestCase("tag1,,tag2;;tag3||tag4")]
        [TestCase(" tag1 , tag2 ; tag3 | tag4 ")]
        [TestCase(" tag1 , , tag2 ; ; tag3 | | tag4 ")]
        public void Should_handle_app_settings_tags(string tags)
        {
            // Given / When
            var sut = SutFactoryForTags(tags);

            // Then
            sut.Tags.ShouldNotBe(null);
            sut.Tags.ShouldNotBeEmpty();
            sut.Tags.ShouldBe(new[] { "tag1", "tag2", "tag3", "tag4" });
        }

        [Test]
        public void Should_handle_app_settings_tags_that_are_empty()
        {
            // Given / When
            var sut = SutFactoryForTags(string.Empty);

            // Then
            sut.Tags.ShouldNotBe(null);
            sut.Tags.ShouldBeEmpty();
        }

        [Test]
        public void Should_handle_app_settings_tags_that_are_null()
        {
            // Given / When
            var sut = SutFactoryForTags(null);

            // Then
            sut.Tags.ShouldNotBe(null);
            sut.Tags.ShouldBeEmpty();
        }

        private RaygunSettings SutFactoryForTags(string tags)
        {
            ConfigurationManager.AppSettings["raygun:tags"] = tags;

            return new RaygunSettings();
        }

        private RaygunSettings SutFactoryForVersion(string version)
        {
            ConfigurationManager.AppSettings["raygun:applicationVersion"] = version;

            return new RaygunSettings();
        }
    }
}