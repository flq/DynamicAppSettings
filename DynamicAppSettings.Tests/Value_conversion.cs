using System;
using System.Collections.Generic;
using System.ComponentModel;
using FluentAssertions;
using NUnit.Framework;

namespace DynamicAppSettings.Tests
{
    [TestFixture]
    public class Value_conversion : DynamicAppSettingsContext
    {

        protected override IEnumerable<KeyValuePair<string, string>> GetConfigurationContents()
        {
            yield return new KeyValuePair<string, string>("Foo", "23");
            yield return new KeyValuePair<string, string>("Bar", "True");
        }

        [Test]
        public void can_access_existing_value_int()
        {
            int value = AppSettings.Foo;
            value.Should().Be(23);
        }

        [Test]
        public void can_access_existing_value_bool()
        {
            bool value = AppSettings.Bar;
            value.Should().Be(true);
        }

        [Test]
        public void invalid_conversion_throws_format_exception()
        {
            Assert.Throws<FormatException>(() => { bool bla = AppSettings.Foo; });
        }
    }

    [TestFixture]
    public class Value_conversion_extended : DynamicAppSettingsContext
    {

        protected override IEnumerable<KeyValuePair<string, string>> GetConfigurationContents()
        {
            yield return new KeyValuePair<string, string>("TheMan", "Arthur Brannigan");
        }

        [Test]
        public void can_access_existing_value()
        {
            Man value = AppSettings.TheMan;
            value.FirstName.Should().Be("Arthur");
            value.LastName.Should().Be("Brannigan");
        }
    }

    [TypeConverter(typeof(ManConverter))]
    public class Man
    {
        public string FirstName;
        public string LastName;
    }

    public class ManConverter : QuickConverter<Man>
    {
        public ManConverter()
            : base(s => { 
                var items = s.Split(' ');
                return new Man {FirstName = items[0], LastName = items[1]};
            })
        {
        }
    }
}