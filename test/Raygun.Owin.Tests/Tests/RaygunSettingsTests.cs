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
            ConfigurationManager.AppSettings["raygun:tags"] = null;
        }

        [TestCase("tag1,tag2;tag3|tag4")]
        [TestCase("tag1,,tag2;;tag3||tag4")]
        [TestCase(" tag1 , tag2 ; tag3 | tag4 ")]
        [TestCase(" tag1 , , tag2 ; ; tag3 | | tag4 ")]
        public void Should_handle_app_settings_tags(string tags)
        {
            // Given / When
            var sut = SutFactory(tags);

            // Then
            sut.Tags.ShouldNotBe(null);
            sut.Tags.ShouldNotBeEmpty();
            sut.Tags.ShouldBe(new[] { "tag1", "tag2", "tag3", "tag4" });
        }

        [Test]
        public void Should_handle_app_settings_tags_that_are_empty()
        {
            // Given / When
            var sut = SutFactory(string.Empty);

            // Then
            sut.Tags.ShouldNotBe(null);
            sut.Tags.ShouldBeEmpty();
        }

        [Test]
        public void Should_handle_app_settings_tags_that_are_null()
        {
            // Given / When
            var sut = SutFactory(null);

            // Then
            sut.Tags.ShouldNotBe(null);
            sut.Tags.ShouldBeEmpty();
        }

        private RaygunSettings SutFactory(string appSettingsValue)
        {
            ConfigurationManager.AppSettings["raygun:tags"] = appSettingsValue;

            return new RaygunSettings();
        }
    }
}