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
            ConfigurationManager.AppSettings["raygun:apiEndpoint"] = null;
            ConfigurationManager.AppSettings["raygun:apiKey"] = null;
            ConfigurationManager.AppSettings["raygun:applicationVersion"] = null;
            ConfigurationManager.AppSettings["raygun:tags"] = null;
            ConfigurationManager.AppSettings["raygun:throwOnError"] = null;
            ConfigurationManager.AppSettings["raygun:excludeErrorsFromLocal"] = null;
        }

        [Test]
        public void Should_gracefully_handle_no_custom_settings()
        {
            // Given / When
            var sut = SutFactory();

            // Then
            sut.ApiEndpoint.ShouldBe(Constants.RaygunApiEndpoint);
            sut.ApiKey.ShouldBeNull();
            sut.ApplicationVersion.ShouldBeNull();
            sut.Tags.ShouldBeEmpty();
            sut.ExcludeErrorsFromLocal.ShouldBeFalse();
        }

        [Test]
        public void Should_handle_custom_endpoint()
        {
            // Given / When
            var sut = SutFactory(endpoint: "http:example.com");

            // Then
            sut.ApiEndpoint.ShouldBe("http:example.com");
        }

        [Test]
        public void Should_handle_custom_apikey()
        {
            // Given / When
            var sut = SutFactory(apiKey: "abc123");

            // Then
            sut.ApiKey.ShouldBe("abc123");
        }

        [TestCase("1.2.3.4-beta2")]
        [TestCase("  1.2.3.4-beta2\t")]
        public void Should_handle_app_settings_application_version(string version)
        {
            // Given / When
            var sut = SutFactory(version: version);

            // Then
            sut.ApplicationVersion.ShouldBe("1.2.3.4-beta2");
        }

        [Test]
        public void Should_handle_app_settings_application_version_that_is_empty()
        {
            // Given / When
            var sut = SutFactory(version: string.Empty);

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
            var sut = SutFactory(tags: tags);

            // Then
            sut.Tags.ShouldNotBe(null);
            sut.Tags.ShouldNotBeEmpty();
            sut.Tags.ShouldBe(new[] { "tag1", "tag2", "tag3", "tag4" });
        }

        [Test]
        public void Should_handle_app_settings_tags_that_are_empty()
        {
            // Given / When
            var sut = SutFactory(tags: string.Empty);

            // Then
            sut.Tags.ShouldNotBe(null);
            sut.Tags.ShouldBeEmpty();
        }

        [TestCase(true)]
        [TestCase(false)]
        public void Should_handle_custom_throw_on_error(bool value)
        {
            // Given / When
            var sut = SutFactory(throwOnError: value);

            // Then
            sut.ThrowOnError.ShouldBe(value);
        }

        [TestCase(true)]
        [TestCase(false)]
        public void Should_handle_custom_exclude_local_errors(bool value)
        {
            // Given / When
            var sut = SutFactory(excludeLocalErrors: value);

            // Then
            sut.ExcludeErrorsFromLocal.ShouldBe(value);
        }

        private RaygunSettings SutFactory(
            string endpoint = null,
            string apiKey = null,
            string tags = null,
            string version = null,
            bool? throwOnError = null,
            bool? excludeLocalErrors = null)
        {
            ConfigurationManager.AppSettings["raygun:apiEndpoint"] = endpoint;
            ConfigurationManager.AppSettings["raygun:apiKey"] = apiKey;
            ConfigurationManager.AppSettings["raygun:tags"] = tags;
            ConfigurationManager.AppSettings["raygun:applicationVersion"] = version;
            ConfigurationManager.AppSettings["raygun:throwOnError"] = throwOnError?.ToString();
            ConfigurationManager.AppSettings["raygun:excludeErrorsFromLocal"] = excludeLocalErrors?.ToString();

            return RaygunSettings.LoadFromAppSettings();
        }
    }
}
