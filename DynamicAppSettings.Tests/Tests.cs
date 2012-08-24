using System.Collections.Generic;
using NUnit.Framework;
using FluentAssertions;

namespace DynamicAppSettings.Tests
{

    [TestFixture]
    public class Accessing_simple_values : DynamicAppSettingsContext
    {

        protected override IEnumerable<KeyValuePair<string, string>> GetConfigurationContents()
        {
            yield return new KeyValuePair<string, string>("foo", "World");
        }

        [Test]
        public void can_access_existing_value()
        {
            string value = AppSettings.foo;
            value.Should().NotBeNull();
            value.Should().Be("World");
        }

        [Test]
        public void unavailable_setting_returns_null()
        {
            string value = AppSettings.Bar;
            value.Should().BeNull();
        }
    }
}