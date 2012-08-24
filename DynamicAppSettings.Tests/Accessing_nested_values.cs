using System.Collections.Generic;
using FluentAssertions;
using NUnit.Framework;

namespace DynamicAppSettings.Tests
{
    [TestFixture]
    public class Accessing_nested_values : DynamicAppSettingsContext
    {

        protected override IEnumerable<KeyValuePair<string, string>> GetConfigurationContents()
        {
            yield return new KeyValuePair<string, string>("Server.MaxConnections", "30");
            yield return new KeyValuePair<string, string>("Client.ServerAccess.Address", "http://foo");
            yield return new KeyValuePair<string, string>("Client.ServerAccess.Port", "23");
        }

        [Test]
        public void can_access_existing_value()
        {
            string value = AppSettings.Server.MaxConnections;
            value.Should().NotBeNull();
            value.Should().Be("30");
        }

        [Test]
        public void access_recursive()
        {
            string value = AppSettings.Client.ServerAccess.Address;
            value.Should().Be("http://foo");
        }

        [Test]
        public void complete_nested_found()
        {
            string value = AppSettings.Client.ServerAccess.Port;
            value.Should().Be("23");
        }
    }

    [TestFixture]
    public class Changeable_separator : DynamicAppSettingsContext
    {

        protected override IEnumerable<KeyValuePair<string, string>> GetConfigurationContents()
        {
            DynamicAppSettings.AppSettings.Separator = "/";
            yield return new KeyValuePair<string, string>("Server/MaxConnections", "30");
            yield return new KeyValuePair<string, string>("Client/ServerAccess/Address", "http://foo");
        }

        [Test]
        public void Obtain_level_one()
        {
            int value = AppSettings.Server.MaxConnections;
            value.Should().Be(30);
        }

        [Test]
        public void obtain_level_two()
        {
            string value = AppSettings.Client.ServerAccess.Address;
            value.Should().Be("http://foo");
        }
    }
}